# NuciXNA.Input API Reference

## Namespace: `NuciXNA.Input`

---

### InputManager

**Singleton input state manager.** Call `Update(GameWindow)` once per frame.

```csharp
public sealed class InputManager
{
    // Singleton
    public static InputManager Instance { get; }

    // Frame update
    public void Update(GameWindow window);
    public void ResetInputStates();

    // Keyboard polling
    public bool IsKeyDown(params Keys[] keys);
    public bool IsAnyKeyDown();
    public bool IsAnyKeyDown(params Keys[] keys);
    public bool IsAnyKeyDown(IEnumerable<Keys> keys);

    // Mouse polling
    public bool IsMouseButtonDown(params MouseButton[] buttons);
    public bool IsAnyMouseButtonDown();
    public bool IsAnyMouseButtonDown(params MouseButton[] buttons);
    public bool IsAnyMouseButtonDown(IEnumerable<MouseButton> buttons);

    // Gamepad polling
    public bool IsGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons);
    public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex);
    public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons);
    public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, IEnumerable<Buttons> buttons);

    // Properties
    public Point2D MouseLocation { get; }
    public bool MouseButtonInputHandled { get; set; } // TODO: remove

    // Helpers
    public bool IsLeftMouseButtonClicked();

    // Events
    public event KeyboardKeyEventHandler KeyboardKeyPressed;
    public event KeyboardKeyEventHandler KeyboardKeyReleased;
    public event KeyboardKeyEventHandler KeyboardKeyHeldDown;

    public event MouseButtonEventHandler MouseButtonPressed;
    public event MouseButtonEventHandler MouseButtonReleased;
    public event MouseButtonEventHandler MouseButtonHeldDown;

    public event MouseEventHandler MouseMoved;

    public event GamepadButtonEventHandler GamepadButtonPressed;
    public event GamepadButtonEventHandler GamepadButtonReleased;
    public event GamepadButtonEventHandler GamepadButtonHeldDown;
}
```

---

### ButtonState

**Frame-relative button state.** Immutable singleton instances.

```csharp
public sealed class ButtonState : IEquatable<ButtonState>
{
    // Instances
    public static readonly ButtonState Idle;       // Id=0, IsDown=false
    public static readonly ButtonState Pressed;    // Id=1, IsDown=true
    public static readonly ButtonState Released;   // Id=2, IsDown=false
    public static readonly ButtonState HeldDown;   // Id=3, IsDown=true

    // Properties
    public int Id { get; }
    public string Name { get; }
    public bool IsDown { get; }

    // Factories
    public static ButtonState FromId(int id);
    public static ButtonState FromName(string name);
    public static IEnumerable<ButtonState> GetValues();

    // Conversions
    public static implicit operator int(ButtonState state);
    public static implicit operator string(ButtonState state);
    public static explicit operator ButtonState(int id);
    public static explicit operator ButtonState(string name);

    // Equality
    public bool Equals(ButtonState other);
    public override bool Equals(object obj);
    public override int GetHashCode();
    public static bool operator ==(ButtonState left, ButtonState right);
    public static bool operator !=(ButtonState left, ButtonState right);
    public override string ToString(); // returns Name
}
```

---

### MouseButton

**Mouse button abstraction.** Maps to `MouseState` properties.

```csharp
public sealed class MouseButton : IEquatable<MouseButton>
{
    // Instances
    public static readonly MouseButton Left;      // Id=1 → LeftButton
    public static readonly MouseButton Right;     // Id=2 → RightButton
    public static readonly MouseButton Middle;    // Id=3 → MiddleButton
    public static readonly MouseButton Back;      // Id=4 → XButton1
    public static readonly MouseButton Forward;   // Id=5 → XButton2

    // Properties
    public int Id { get; }
    public string Name { get; }

    // Factories
    public static MouseButton FromId(int id);
    public static MouseButton FromName(string name);
    public static IEnumerable<MouseButton> GetValues();

    // Conversions
    public static implicit operator int(MouseButton button);
    public static implicit operator string(MouseButton button);
    public static explicit operator MouseButton(int id);
    public static explicit operator MouseButton(string name);

    // Equality
    public bool Equals(MouseButton other);
    public override bool Equals(object obj);
    public override int GetHashCode();
    public static bool operator ==(MouseButton left, MouseButton right);
    public static bool operator !=(MouseButton left, MouseButton right);
    public override string ToString(); // returns Name
}
```

---

### KeyboardKeyEventArgs

```csharp
public class KeyboardKeyEventArgs
{
    public KeyboardKeyEventArgs(Keys key, ButtonState keyState);

    public Keys Key { get; }
    public ButtonState KeyState { get; }
}
```

**Delegate:** `public delegate void KeyboardKeyEventHandler(object sender, KeyboardKeyEventArgs e);`

---

### MouseButtonEventArgs

```csharp
public class MouseButtonEventArgs
{
    public MouseButtonEventArgs(MouseButton button, ButtonState buttonState, Point2D location);

    public MouseButton Button { get; }
    public ButtonState ButtonState { get; }
    public Point2D Location { get; }
}
```

**Delegate:** `public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);`

---

### MouseEventArgs

```csharp
public class MouseEventArgs
{
    public MouseEventArgs(Point2D location, Point2D previousLocation);

    public Point2D Location { get; }
    public Point2D PreviousLocation { get; }
}
```

**Delegate:** `public delegate void MouseEventHandler(object sender, MouseEventArgs e);`

---

### GamepadButtonEventArgs

```csharp
public class GamepadButtonEventArgs
{
    public GamepadButtonEventArgs(Buttons button, ButtonState buttonState, PlayerIndex playerIndex);

    public Buttons Button { get; }
    public ButtonState ButtonState { get; }
    public PlayerIndex PlayerIndex { get; }
}
```

**Delegate:** `public delegate void GamepadButtonEventHandler(object sender, GamepadButtonEventArgs e);`

---

## Namespace: `NuciXNA.Primitives` (Dependency)

### Point2D

```csharp
public readonly struct Point2D : IEquatable<Point2D>
{
    public Point2D(int x, int y);
    public int X { get; }
    public int Y { get; }

    // Vector operations
    public static Point2D operator +(Point2D a, Point2D b);
    public static Point2D operator -(Point2D a, Point2D b);
    public static Point2D operator *(Point2D a, int scalar);
    public static Point2D operator /(Point2D a, int scalar);

    // Equality
    public bool Equals(Point2D other);
    public override bool Equals(object obj);
    public override int GetHashCode();
    public static bool operator ==(Point2D left, Point2D right);
    public static bool operator !=(Point2D left, Point2D right);
}
```

**Extension:** `MouseState.Position.ToPoint2D()` (from `NuciXNA.Primitives.Mapping`)