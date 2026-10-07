# NuciXNA.Input State Management

## Frame-Based State Model

NuciXNA.Input uses a **dual-buffer frame state** model: current frame + previous frame. State transitions are derived by comparing the two.

---

## State Storage

### Keyboard
```csharp
KeyboardState currentKeyState;
KeyboardState previousKeyState;
```

### Mouse
```csharp
MouseState currentMouseState;
MouseState previousMouseState;
```

### Gamepad (4 players)
```csharp
GamePadState[] currentGamepadStates = new GamePadState[4];
GamePadState[] previousGamepadStates = new GamePadState[4];
```

---

## Update Cycle

```csharp
public void Update(GameWindow window)
{
    // 1. Swap previous ↔ current (no allocation)
    previousKeyState = currentKeyState;
    previousMouseState = currentMouseState;
    var swapTemp = previousGamepadStates;
    previousGamepadStates = currentGamepadStates;
    currentGamepadStates = swapTemp;

    // 2. Capture new current state
    currentKeyState = Keyboard.GetState();
    currentMouseState = Mouse.GetState();
    for (int i = 0; i < 4; i++)
        currentGamepadStates[i] = GamePad.GetState((PlayerIndex)i);

    // 3. Linux coordinate fix
    if (currentMouseState.X < 0)
        currentMouseState = new MouseState(
            currentMouseState.X + window.ClientBounds.X,
            currentMouseState.Y + window.ClientBounds.Y,
            ...);

    // 4. Derive transitions & fire events
    CheckGamepadButtonStates();
    CheckKeyboardKeyStates();
    CheckMouseButtonStates();
    CheckMouseMoved();
}
```

**Key insight:** Swapping arrays avoids allocation. `previousGamepadStates` becomes the new `currentGamepadStates` buffer for next frame.

---

## Transition Derivation

### Keyboard (CheckKeyboardKeyStates)

```csharp
Keys[] currentPressedKeys = currentKeyState.GetPressedKeys();
Keys[] previousPressedKeys = previousKeyState.GetPressedKeys();

// Currently pressed keys
foreach (Keys key in currentPressedKeys)
{
    if (previousKeyState.IsKeyDown(key))
        OnKeyboardKeyHeldDown(key, ButtonState.HeldDown);
    else
        OnKeyboardKeyPressed(key, ButtonState.Pressed);
}

// Keys released this frame
foreach (Keys key in previousPressedKeys)
{
    if (!currentKeyState.IsKeyDown(key))
        OnKeyboardKeyReleased(key, ButtonState.Released);
}
```

**Efficiency:** Only iterates pressed keys, not all `Keys` enum values.

### Gamepad (CheckGamepadButtonStates)

```csharp
foreach (PlayerIndex playerIndex in allPlayerIndices)
{
    foreach (Buttons button in allButtons)
    {
        bool isDown = currentGamepadStates[(int)playerIndex].IsButtonDown(button);
        bool wasDown = previousGamepadStates[(int)playerIndex].IsButtonDown(button);

        if (isDown)
        {
            if (wasDown) OnHeldDown(...);
            else OnPressed(...);
        }
        else if (wasDown)
        {
            OnReleased(...);
        }
    }
}
```

**Exhaustive:** Checks every button for every player every frame (~68 checks).

### Mouse Buttons (CheckMouseButtonStates)

```csharp
Point2D cursorLocation = currentMouseState.Position.ToPoint2D();

foreach (MouseButton button in MouseButton.GetValues())
{
    ButtonState state = GetMouseButtonState(button);
    var args = new MouseButtonEventArgs(button, state, cursorLocation);

    if (state == ButtonState.Pressed) OnMouseButtonPressed(args);
    else if (state == ButtonState.Released) OnMouseButtonReleased(args);
    else if (state == ButtonState.HeldDown) OnMouseButtonHeldDown(args);
}
```

**GetMouseButtonState** maps `MouseButton` enum to `MouseState` fields:

| MouseButton | MouseState Field |
|-------------|------------------|
| Left | LeftButton |
| Right | RightButton |
| Middle | MiddleButton |
| Back | XButton1 |
| Forward | XButton2 |

Then compares current vs previous `XNAButtonState` (Pressed/Released).

### Mouse Move (CheckMouseMoved)

```csharp
if (!currentMouseState.Position.Equals(previousMouseState.Position))
    OnMouseMoved(new MouseEventArgs(
        currentMouseState.Position.ToPoint2D(),
        previousMouseState.Position.ToPoint2D()));
```

---

## ButtonState Mapping

| Current XNA | Previous XNA | NuciXNA ButtonState | Event |
|-------------|--------------|---------------------|-------|
| Pressed | Pressed | HeldDown | *HeldDown |
| Pressed | Released | Pressed | *Pressed |
| Released | Pressed | Released | *Released |
| Released | Released | Idle | (none) |

---

## ResetInputStates()

```csharp
public void ResetInputStates()
{
    previousKeyState = currentKeyState;
    previousMouseState = currentMouseState;
    var swapTemp = previousGamepadStates;
    previousGamepadStates = currentGamepadStates;
    currentGamepadStates = swapTemp;

    currentKeyState = new KeyboardState();      // All keys up
    currentMouseState = new MouseState();       // All buttons up, pos (0,0)
    Array.Clear(currentGamepadStates, 0, 4);    // All buttons up
}
```

**Purpose:** Call on focus loss to prevent "stuck" held states. After reset:
- Next `Update()` sees all inputs as "just released" (Released events fire)
- Subsequent frames see Idle

---

## Polling Implementation

### Keyboard
```csharp
public bool IsKeyDown(params Keys[] keys)
    => keys.All(currentKeyState.IsKeyDown);

public bool IsAnyKeyDown(IEnumerable<Keys> keys)
    => keys.Any(currentKeyState.IsKeyDown);
```

### Mouse
```csharp
public bool IsMouseButtonDown(params MouseButton[] buttons)
    => buttons.Select(GetMouseButtonState).All(x => x.IsDown);

public bool IsAnyMouseButtonDown(IEnumerable<MouseButton> buttons)
    => buttons.Select(GetMouseButtonState).Any(x => x.IsDown);
```

**Uses `ButtonState.IsDown`** — true for Pressed and HeldDown.

### Gamepad
```csharp
public bool IsGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons)
    => buttons.All(b => currentGamepadStates[(int)playerIndex].IsButtonDown(b));

public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, IEnumerable<Buttons> buttons)
    => buttons.Any(b => currentGamepadStates[(int)playerIndex].IsButtonDown(b));
```

---

## Static Caches

```csharp
static readonly Keys[] allKeys = Enum.GetValues<Keys>();
static readonly Buttons[] allButtons = Enum.GetValues<Buttons>();
static readonly PlayerIndex[] allPlayerIndices = Enum.GetValues<PlayerIndex>();
```

**Initialized once** — avoids per-frame allocation.

---

## Linux/SDL2 Coordinate Fix

### Problem
On Linux with SDL2, when window is on a monitor **left of primary**:
- `Mouse.GetState().X` returns **virtual desktop coordinate** (negative)
- `Mouse.GetState().Y` also in virtual desktop space
- `Window.ClientBounds` is in same coordinate space

### Fix
```csharp
int rawX = currentMouseState.X;
int rawY = currentMouseState.Y;

if (rawX < 0)
{
    currentMouseState = new MouseState(
        rawX + window.ClientBounds.X,
        rawY + window.ClientBounds.Y,
        currentMouseState.ScrollWheelValue,
        currentMouseState.LeftButton,
        currentMouseState.MiddleButton,
        currentMouseState.RightButton,
        currentMouseState.XButton1,
        currentMouseState.XButton2);
}
```

**Assumption:** Negative X implies left-of-primary monitor. Adds ClientBounds origin to convert to client-relative.

**Limitation:** Only triggers on `X < 0`. If window on monitor above primary (negative Y only), not corrected.

---

## Memory Characteristics

| Aspect | Detail |
|--------|--------|
| Allocations/frame | ~5 EventArgs objects (keyboard) + 5 (mouse) + 68 (gamepad) + 1 (mouse move) |
| Array allocations | None (swapped) |
| Enum arrays | Cached static |
| GC pressure | Moderate — many short-lived EventArgs |

---

## Invariants

1. **After `Update()`:** `previous*State` == state at start of frame; `current*State` == state at end of frame
2. **Events fire exactly once** per transition per frame
3. **HeldDown fires every frame** while input held
4. **Idle never fires** an event
5. **ResetInputStates()** makes next frame see all inputs as Released
6. **Mouse position** in events is always client-relative (post-fix)