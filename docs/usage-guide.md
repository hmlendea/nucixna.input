# NuciXNA.Input Usage Guide

## Installation

```bash
dotnet add package NuciXNA.Input
```

Requires: `.NET 10.0`, `MonoGame.Framework.DesktopGL 3.8.4+`

---

## Basic Setup

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NuciXNA.Input;

public class Game1 : Game
{
    protected override void Initialize()
    {
        // Subscribe to events BEFORE first Update
        InputManager.Instance.KeyboardKeyPressed += OnKeyPressed;
        InputManager.Instance.MouseButtonPressed += OnMousePressed;
        InputManager.Instance.MouseMoved += OnMouseMoved;
        InputManager.Instance.GamepadButtonPressed += OnGamepadPressed;

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        // MUST call once per frame with the GameWindow
        InputManager.Instance.Update(Window);

        // Polling examples
        if (InputManager.Instance.IsKeyDown(Keys.LeftControl, Keys.S))
        {
            SaveGame();
        }

        if (InputManager.Instance.IsAnyMouseButtonDown())
        {
            // Drag operation, etc.
        }

        if (InputManager.Instance.IsGamepadButtonDown(PlayerIndex.One, Buttons.A, Buttons.B))
        {
            // Both A and B held
        }

        base.Update(gameTime);
    }

    private void OnKeyPressed(object sender, KeyboardKeyEventArgs e)
    {
        if (e.Key == Keys.Escape)
            Exit();

        if (e.KeyState == ButtonState.Pressed && e.Key == Keys.F11)
            ToggleFullscreen();
    }

    private void OnMousePressed(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == MouseButton.Left && e.ButtonState == ButtonState.Pressed)
        {
            // Click at e.Location
        }
    }

    private void OnMouseMoved(object sender, MouseEventArgs e)
    {
        // e.Location, e.PreviousLocation
        UpdateHoverUI(e.Location);
    }

    private void OnGamepadPressed(object sender, GamepadButtonEventArgs e)
    {
        if (e.PlayerIndex == PlayerIndex.One && e.Button == Buttons.Start)
            PauseGame();
    }
}
```

---

## Event vs Polling

| Approach | Use When |
|----------|----------|
| **Events** | Discrete actions (jump, shoot, menu open), one-time triggers |
| **Polling** | Continuous state (movement, held buttons), frame logic |

**Events fire during `Update()`** — handlers run synchronously on the game thread.

---

## ButtonState Semantics

| State | `IsDown` | Transition |
|-------|----------|------------|
| `Idle` | false | Up → Up |
| `Pressed` | true | Up → Down (this frame only) |
| `HeldDown` | true | Down → Down |
| `Released` | false | Down → Up (this frame only) |

**Check press:** `e.ButtonState == ButtonState.Pressed`
**Check hold:** `e.ButtonState == ButtonState.HeldDown || e.ButtonState == ButtonState.Pressed`
**Check release:** `e.ButtonState == ButtonState.Released`
**Check currently down:** `e.ButtonState.IsDown` (Pressed or HeldDown)

---

## Keyboard

### Events
- `KeyboardKeyPressed` — key went down this frame
- `KeyboardKeyReleased` — key went up this frame
- `KeyboardKeyHeldDown` — key down in both frames

### Polling
```csharp
// All keys down
InputManager.Instance.IsKeyDown(Keys.LeftShift, Keys.W);

// Any key down
InputManager.Instance.IsAnyKeyDown(Keys.W, Keys.A, Keys.S, Keys.D);

// Any key at all
InputManager.Instance.IsAnyKeyDown();
```

---

## Mouse

### Buttons (5)
`Left`, `Right`, `Middle`, `Back` (XButton1), `Forward` (XButton2)

### Events
- `MouseButtonPressed` — button down this frame
- `MouseButtonReleased` — button up this frame
- `MouseButtonHeldDown` — button down both frames
- `MouseMoved` — position changed (includes `PreviousLocation`)

### Polling
```csharp
InputManager.Instance.IsMouseButtonDown(MouseButton.Left, MouseButton.Right);
InputManager.Instance.IsAnyMouseButtonDown();
InputManager.Instance.IsAnyMouseButtonDown(MouseButton.Left, MouseButton.Middle);
```

### Position
```csharp
Point2D pos = InputManager.Instance.MouseLocation;
// or from event:
e.Location;        // current
e.PreviousLocation; // previous frame
```

---

## Gamepad

### Support
- Up to 4 controllers (`PlayerIndex.One` through `Four`)
- All `Buttons` enum values (digital buttons only)
- **No analog stick/trigger support**

### Events (per player)
- `GamepadButtonPressed`
- `GamepadButtonReleased`
- `GamepadButtonHeldDown`

### Polling
```csharp
// Specific player, specific buttons
InputManager.Instance.IsGamepadButtonDown(PlayerIndex.One, Buttons.A, Buttons.DPadUp);

// Any button for player
InputManager.Instance.IsAnyGamepadButtonDown(PlayerIndex.Two);

// Any of specified buttons for player
InputManager.Instance.IsAnyGamepadButtonDown(PlayerIndex.One, Buttons.A, Buttons.B, Buttons.X, Buttons.Y);
```

---

## Common Patterns

### Combo Detection (Polling)
```csharp
if (InputManager.Instance.IsKeyDown(Keys.LeftControl, Keys.LeftShift, Keys.S))
{
    // Ctrl+Shift+S
}
```

### Click Detection (Event)
```csharp
void OnMousePressed(object sender, MouseButtonEventArgs e)
{
    if (e.Button == MouseButton.Left && e.ButtonState == ButtonState.Pressed)
    {
        // Single click
    }
}
```

### Drag Detection (Event + Polling)
```csharp
Point2D dragStart;

void OnMousePressed(object sender, MouseButtonEventArgs e)
{
    if (e.Button == MouseButton.Left && e.ButtonState == ButtonState.Pressed)
        dragStart = e.Location;
}

void OnMouseMoved(object sender, MouseEventArgs e)
{
    if (InputManager.Instance.IsMouseButtonDown(MouseButton.Left))
    {
        var delta = e.Location - dragStart;
        // Drag operation
    }
}
```

### Gamepad Menu Navigation
```csharp
void OnGamepadPressed(object sender, GamepadButtonEventArgs e)
{
    if (e.ButtonState != ButtonState.Pressed) return;

    switch (e.Button)
    {
        case Buttons.DPadUp:    MoveSelection(-1); break;
        case Buttons.DPadDown:  MoveSelection(+1); break;
        case Buttons.A:         ConfirmSelection(); break;
        case Buttons.B:         CancelSelection(); break;
    }
}
```

---

## Focus Loss / Reset

```csharp
protected override void OnDeactivated(object sender, EventArgs e)
{
    InputManager.Instance.ResetInputStates();
    base.OnDeactivated(sender, e);
}
```

Prevents "stuck" held states when window loses focus.

---

## Linux Multi-Monitor Note

On Linux/SDL2, if the game window is on a monitor **left of the primary**, `Mouse.GetState()` returns virtual-desktop coordinates (negative X). `InputManager.Update(Window)` automatically corrects this using `Window.ClientBounds`.

**No action required** — handled internally.

---

## Thread Safety

- `InputManager.Instance` — thread-safe (double-checked locking)
- `Update()` — **must** be called from game thread
- Event handlers — execute on game thread during `Update()`
- Polling methods — safe to call from game thread after `Update()`

---

## Version Compatibility

| NuciXNA.Input | .NET | MonoGame |
|---------------|------|----------|
| 2.1.x | net10.0 | 3.8.x |

Check NuGet for latest.