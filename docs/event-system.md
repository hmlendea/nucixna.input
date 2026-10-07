# NuciXNA.Input Event System

## Overview

The event system is the primary **event-driven** API. Events fire synchronously during `InputManager.Update(GameWindow)` on the game thread.

---

## Event Flow

```
Game.Update()
    └─ InputManager.Update(window)
         ├─ Capture current frame state
         ├─ CheckGamepadButtonStates()
         │    └─ For each player, each button:
         │         ├─ Down + PrevDown → GamepadButtonHeldDown
         │         ├─ Down + PrevUp   → GamepadButtonPressed
         │         └─ Up + PrevDown   → GamepadButtonReleased
         ├─ CheckKeyboardKeyStates()
         │    ├─ For each currently pressed key:
         │    │    ├─ PrevDown → KeyboardKeyHeldDown
         │    │    └─ PrevUp   → KeyboardKeyPressed
         │    └─ For each previously pressed key now up:
         │         └─ KeyboardKeyReleased
         ├─ CheckMouseButtonStates()
         │    └─ For each MouseButton:
         │         ├─ Pressed   → MouseButtonPressed
         │         ├─ Released  → MouseButtonReleased
         │         └─ HeldDown  → MouseButtonHeldDown
         └─ CheckMouseMoved()
              └─ Position changed → MouseMoved
```

---

## Event Details

### Keyboard Events

| Event | Fired When | Args |
|-------|------------|------|
| `KeyboardKeyPressed` | Key transitioned Up → Down | `KeyboardKeyEventArgs { Key, KeyState=Pressed }` |
| `KeyboardKeyReleased` | Key transitioned Down → Up | `KeyboardKeyEventArgs { Key, KeyState=Released }` |
| `KeyboardKeyHeldDown` | Key Down in both frames | `KeyboardKeyEventArgs { Key, KeyState=HeldDown }` |

**Note:** `KeyboardKeyHeldDown` fires **every frame** while key is held. `KeyboardKeyPressed` fires **only on the transition frame**.

### Mouse Button Events

| Event | Fired When | Args |
|-------|------------|------|
| `MouseButtonPressed` | Button Up → Down | `MouseButtonEventArgs { Button, ButtonState=Pressed, Location }` |
| `MouseButtonReleased` | Button Down → Up | `MouseButtonEventArgs { Button, ButtonState=Released, Location }` |
| `MouseButtonHeldDown` | Button Down both frames | `MouseButtonEventArgs { Button, ButtonState=HeldDown, Location }` |

**Location:** Current mouse position (client-relative, post Linux fix).

### Mouse Movement Event

| Event | Fired When | Args |
|-------|------------|------|
| `MouseMoved` | `currentMouseState.Position != previousMouseState.Position` | `MouseEventArgs { Location, PreviousLocation }` |

**Fires at most once per frame** — even if mouse moved multiple pixels.

### Gamepad Events (Per Player)

| Event | Fired When | Args |
|-------|------------|------|
| `GamepadButtonPressed` | Button Up → Down | `GamepadButtonEventArgs { Button, ButtonState=Pressed, PlayerIndex }` |
| `GamepadButtonReleased` | Button Down → Up | `GamepadButtonEventArgs { Button, ButtonState=Released, PlayerIndex }` |
| `GamepadButtonHeldDown` | Button Down both frames | `GamepadButtonEventArgs { Button, ButtonState=HeldDown, PlayerIndex }` |

**Iterates all 4 PlayerIndex values** and all `Buttons` enum values each frame.

---

## Subscription Patterns

### Initialize Once
```csharp
protected override void Initialize()
{
    InputManager.Instance.KeyboardKeyPressed += OnKeyPressed;
    // ...
    base.Initialize();
}
```

### Lambda (Short-lived)
```csharp
InputManager.Instance.MouseMoved += (s, e) =>
    tooltip.Position = e.Location;
```

### Unsubscribe (Cleanup)
```csharp
protected override void UnloadContent()
{
    InputManager.Instance.KeyboardKeyPressed -= OnKeyPressed;
    base.UnloadContent();
}
```

---

## Event Argument Details

### KeyboardKeyEventArgs
```csharp
public Keys Key { get; }           // XNA Keys enum value
public ButtonState KeyState { get; } // Pressed | Released | HeldDown
```

### MouseButtonEventArgs
```csharp
public MouseButton Button { get; }      // Left | Right | Middle | Back | Forward
public ButtonState ButtonState { get; } // Pressed | Released | HeldDown
public Point2D Location { get; }        // Client-relative position
```

### MouseEventArgs
```csharp
public Point2D Location { get; }           // Current position
public Point2D PreviousLocation { get; }   // Previous frame position
```

### GamepadButtonEventArgs
```csharp
public Buttons Button { get; }           // XNA Buttons enum
public ButtonState ButtonState { get; }  // Pressed | Released | HeldDown
public PlayerIndex PlayerIndex { get; }  // One | Two | Three | Four
```

---

## Common Event Handling Patterns

### Single Press Detection
```csharp
void OnKeyPressed(object sender, KeyboardKeyEventArgs e)
{
    if (e.KeyState == ButtonState.Pressed && e.Key == Keys.Space)
        Jump();
}
```

### Hold Detection (Continuous)
```csharp
void OnKeyHeldDown(object sender, KeyboardKeyEventArgs e)
{
    if (e.Key == Keys.W)
        MoveForward();
}
```

### Release Detection
```csharp
void OnKeyReleased(object sender, KeyboardKeyEventArgs e)
{
    if (e.Key == Keys.LeftShift)
        StopSprinting();
}
```

### Mouse Click with Position
```csharp
void OnMousePressed(object sender, MouseButtonEventArgs e)
{
    if (e.Button == MouseButton.Left && e.ButtonState == ButtonState.Pressed)
        TrySelectUnitAt(e.Location);
}
```

### Drag with Delta
```csharp
void OnMouseMoved(object sender, MouseEventArgs e)
{
    if (InputManager.Instance.IsMouseButtonDown(MouseButton.Left))
    {
        var delta = e.Location - e.PreviousLocation;
        camera.Position += delta;
    }
}
```

### Gamepad per Player
```csharp
void OnGamepadPressed(object sender, GamepadButtonEventArgs e)
{
    if (e.PlayerIndex != PlayerIndex.One) return;
    if (e.ButtonState != ButtonState.Pressed) return;

    switch (e.Button)
    {
        case Buttons.A: Confirm(); break;
        case Buttons.B: Cancel(); break;
    }
}
```

---

## Performance Considerations

- **All events fire every frame** for held states (HeldDown events)
- **Gamepad:** 4 players × ~17 buttons = 68 checks/frame
- **Keyboard:** Only iterates currently pressed keys + previously pressed keys
- **Mouse:** 5 buttons checked every frame
- **Event allocation:** New `EventArgs` instance per event per frame

**Optimization:** Unsubscribe from `*HeldDown` events if not needed.

---

## Thread Safety

- Events invoked **synchronously** on game thread during `Update()`
- Handlers **must not** call `Update()` recursively
- Handlers **should be fast** — blocking delays frame
- No built-in async/event queue — direct invocation

---

## Ordering Guarantees

Within a single `Update()` call:

1. Gamepad events (all players, all buttons)
2. Keyboard events (pressed keys, then released keys)
3. Mouse button events (all 5 buttons)
4. Mouse move event (if moved)

**No guarantee** between different event types for same input (e.g., `KeyboardKeyPressed` vs `KeyboardKeyHeldDown` for same key — only one fires per frame per key).

---

## Linux Coordinate Fix Impact

Mouse events (`MouseButtonEventArgs.Location`, `MouseEventArgs.Location/PreviousLocation`) use **corrected client-relative coordinates** when `rawX < 0`.

The fix runs **before** event firing, so all mouse events receive corrected positions.