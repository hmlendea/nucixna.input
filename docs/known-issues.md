# NuciXNA.Input Known Issues & Limitations

## Current Limitations

### 1. No Analog Gamepad Support
- **Missing:** Left/Right thumbsticks, Left/Right triggers, D-pad as axes
- **Only:** Digital `Buttons` enum (A, B, X, Y, DPad, Shoulders, Sticks-click, Start/Back/Guide)
- **Workaround:** Use `GamePad.GetState().ThumbSticks` directly

### 2. No Touch Input
- **Missing:** Touch panel, gestures, multi-touch
- **Workaround:** Use `TouchPanel.GetState()` directly

### 3. No Text/Character Input
- **Missing:** `TextInput` events, IME support, character composition
- **Workaround:** Subscribe to `Game.Window.TextInput` event

### 4. No Mouse Wheel/Scroll Events
- **Missing:** `MouseWheel` event, scroll delta tracking
- **Available:** `MouseState.ScrollWheelValue` via polling only
- **Workaround:** Poll `ScrollWheelValue` in `Update()`

### 5. No Keyboard Modifiers Helper
- **Missing:** `IsControlDown()`, `IsShiftDown()`, `IsAltDown()` helpers
- **Workaround:** `IsKeyDown(Keys.LeftControl, Keys.RightControl)`

### 6. Single-Threaded Design
- **Assumption:** MonoGame's single-threaded game loop
- **Risk:** Calling `Update()` from multiple threads corrupts state
- **No:** Thread-safe polling during `Update()`

---

## Known Bugs / Workarounds

### 1. Linux Multi-Monitor Y-Axis Fix Incomplete
**File:** `InputManager.cs`, lines 65-75

```csharp
if (rawX < 0)
{
    currentMouseState = new MouseState(
        rawX + window.ClientBounds.X,
        rawY + window.ClientBounds.Y,  // Y corrected unconditionally
        ...
    );
}
```

**Issue:** Only triggers when `X < 0`. If window on monitor **above** primary (negative Y, positive X), Y coordinate remains in virtual desktop space.

**Impact:** Mouse events report wrong Y on top-mounted secondary monitors.

**Fix needed:** Check both X and Y against `ClientBounds`.

### 2. Workaround Fields Marked TODO
**File:** `InputManager.cs`, lines 430-434

```csharp
public Point2D MouseLocation => new(currentMouseState.Position.X, currentMouseState.Position.Y);
public bool MouseButtonInputHandled { get; set; }

public bool IsLeftMouseButtonClicked()
    => GetMouseButtonState(MouseButton.Left).Equals(ButtonState.Pressed);
```

**Status:** Marked `// TODO: Everything below this is required by a workaround and should be removed as soon as it is properly fixed`

**Impact:** Public API surface includes temporary workaround.

### 3. HeldDown Events Fire Every Frame
**Behavior:** `KeyboardKeyHeldDown`, `MouseButtonHeldDown`, `GamepadButtonHeldDown` fire **every frame** while input held.

**Impact:** High event frequency (60+/sec). Handlers must be lightweight.

**Mitigation:** Unsubscribe from `*HeldDown` events if not needed.

### 4. Gamepad Button Iteration Overhead
**Code:** `CheckGamepadButtonStates()` iterates all 4 players × all `Buttons` enum values (~17) = 68 checks/frame.

**Impact:** Minor but measurable on low-end devices.

**Optimization:** Could track only connected gamepads, only pressed buttons.

### 5. No Gamepad Connection State Events
**Missing:** `GamepadConnected`, `GamepadDisconnected` events.

**Workaround:** Poll `GamePad.GetState(playerIndex).IsConnected` in `Update()`.

### 6. Mouse Position in Events Uses Corrected Coordinates
**Behavior:** After Linux fix, `MouseButtonEventArgs.Location` and `MouseEventArgs.Location` use corrected coordinates.

**Consistency:** Good — all mouse events use same coordinate space.

**Caveat:** If fix doesn't trigger (X >= 0), coordinates are raw (may be virtual desktop).

---

## Design Decisions (Not Bugs)

### 1. Singleton Pattern
**Rationale:** Simple global access for game-wide input.
**Trade-off:** Harder to test, not DI-friendly without wrapper.

### 2. EventArgs Allocation Per Event
**Rationale:** Simplicity, immutability.
**Trade-off:** GC pressure at 60fps with many held inputs.

### 3. Enum-Like Classes (ButtonState, MouseButton)
**Rationale:** Type safety, extensibility, implicit conversions.
**Trade-off:** More verbose than `enum`, no switch exhaustiveness.

### 4. Point2D from NuciXNA.Primitives
**Rationale:** Shared primitive across NuciXNA libraries.
**Trade-off:** External dependency for basic struct.

### 5. No Input Buffering / Action Mapping
**Rationale:** Low-level input only; higher layers handle actions.
**Trade-off:** Users must build action maps themselves.

---

## Platform-Specific Issues

### Windows
- Raw Input for keyboard/mouse (low latency)
- XInput for gamepads (Xbox-compatible)
- No known issues

### Linux (DesktopGL/SDL2)
- Multi-monitor coordinate fix (partial)
- Requires `ttf-mscorefonts-installer` for default SpriteFont
- Wayland: May have input grab issues in fullscreen

### macOS (DesktopGL/SDL2)
- Same as Linux
- Metal backend (MonoGame 3.8+)
- No known issues

---

## Performance Profile

| Operation | Cost | Frequency |
|-----------|------|-----------|
| `Keyboard.GetState()` | Low | 1/frame |
| `Mouse.GetState()` | Low | 1/frame |
| `GamePad.GetState()` × 4 | Medium | 4/frame |
| Keyboard key iteration | Low (pressed keys only) | 1/frame |
| Gamepad button iteration | Medium (68 checks) | 1/frame |
| Mouse button checks | Very low (5) | 1/frame |
| EventArgs allocation | Medium | ~80/frame (worst case) |
| Event invocation | Low | ~80/frame |

**Typical frame cost:** < 0.5ms on modern hardware.

---

## Migration Risks

### Upgrading MonoGame
- **3.8.x → 4.x:** May change `GameWindow`, `MouseState`, `GamePadState` APIs
- **DesktopGL → other runtimes:** Different coordinate behavior possible

### Upgrading .NET
- **net10.0 → net11.0:** `Lock` type (used in singleton) is .NET 9+; should remain compatible
- **C# version:** Uses modern features (collection expressions, primary constructors)

---

## Future Improvement Ideas

1. **Add analog gamepad support** — thumbsticks, triggers as events/polling
2. **Add touch input** — `TouchPanel` integration
3. **Add text input events** — wrap `Window.TextInput`
4. **Add mouse wheel events** — scroll delta tracking
5. **Fix Linux Y-axis coordinate** — check both axes
6. **Remove workaround fields** — clean up public API
7. **Add gamepad connection events** — `IsConnected` change detection
8. **Optimize gamepad iteration** — only connected, only changed
9. **Add input buffering** — frame history for combo detection
10. **Extract interface** — `IInputManager` for DI/testing
11. **Add async event queue option** — decouple from game thread
12. **Support netstandard2.0** — wider compatibility