# NuciXNA.Input Integration Guide

## MonoGame Game Class Integration

### Minimal Setup
```csharp
public class Game1 : Game
{
    protected override void Initialize()
    {
        InputManager.Instance.KeyboardKeyPressed += OnKeyPressed;
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Instance.Update(Window);
        base.Update(gameTime);
    }

    private void OnKeyPressed(object sender, KeyboardKeyEventArgs e)
    {
        if (e.Key == Keys.Escape) Exit();
    }
}
```

### With Focus Handling
```csharp
protected override void OnActivated(object sender, EventArgs e)
{
    // Window regained focus - optional reset
    base.OnActivated(sender, e);
}

protected override void OnDeactivated(object sender, EventArgs e)
{
    // Window lost focus - prevent stuck keys
    InputManager.Instance.ResetInputStates();
    base.OnDeactivated(sender, e);
}
```

---

## Screen/State Management Integration

### Per-Screen Input Handling
```csharp
public abstract class GameScreen
{
    protected InputManager Input => InputManager.Instance;
    protected bool IsActive { get; private set; }

    public virtual void Activate()
    {
        IsActive = true;
        SubscribeEvents();
    }

    public virtual void Deactivate()
    {
        IsActive = false;
        UnsubscribeEvents();
        Input.ResetInputStates(); // Clean slate
    }

    protected virtual void SubscribeEvents()
    {
        Input.KeyboardKeyPressed += OnKeyPressed;
        Input.MouseButtonPressed += OnMousePressed;
    }

    protected virtual void UnsubscribeEvents()
    {
        Input.KeyboardKeyPressed -= OnKeyPressed;
        Input.MouseButtonPressed -= OnMousePressed;
    }

    protected virtual void OnKeyPressed(object sender, KeyboardKeyEventArgs e) { }
    protected virtual void OnMousePressed(object sender, MouseButtonEventArgs e) { }

    public virtual void Update(GameTime gameTime) { }
}
```

### Screen Manager
```csharp
public class ScreenManager
{
    private GameScreen _currentScreen;

    public void ChangeScreen(GameScreen newScreen)
    {
        _currentScreen?.Deactivate();
        _currentScreen = newScreen;
        _currentScreen?.Activate();
    }

    public void Update(GameTime gameTime)
    {
        _currentScreen?.Update(gameTime);
    }
}
```

---

## UI System Integration

### Button Click Handling
```csharp
public class UIButton
{
    public Rectangle Bounds { get; set; }
    public event Action Clicked;

    public void Update()
    {
        var input = InputManager.Instance;
        var mousePos = input.MouseLocation;

        if (Bounds.Contains(mousePos.X, mousePos.Y))
        {
            if (input.IsMouseButtonDown(MouseButton.Left))
                _isHovered = true;

            if (_wasHovered && input.IsMouseButtonDown(MouseButton.Left) == false)
                Clicked?.Invoke(); // Click released over button
        }
        _wasHovered = _isHovered;
    }
}
```

### Text Input (Not Built-in)
NuciXNA.Input does **not** provide text input (character events). Use MonoGame's `TextInput` event:

```csharp
Window.TextInput += (s, e) =>
{
    char c = e.Character;
    // Handle character input
};
```

---

## Multiplayer / Split-Screen

### Per-Player Input Context
```csharp
public class PlayerInput
{
    public PlayerIndex Index { get; }
    private InputManager Input => InputManager.Instance;

    public PlayerInput(PlayerIndex index) => Index = index;

    public bool IsButtonDown(Buttons button)
        => Input.IsGamepadButtonDown(Index, button);

    public bool IsAnyButtonDown(params Buttons[] buttons)
        => Input.IsAnyGamepadButtonDown(Index, buttons);

    public event GamepadButtonEventHandler ButtonPressed
    {
        add => Input.GamepadButtonPressed += Filter(value);
        remove => Input.GamepadButtonPressed -= Filter(value);
    }

    private GamepadButtonEventHandler Filter(GamepadButtonEventHandler handler)
        => (s, e) => { if (e.PlayerIndex == Index) handler(s, e); };
}
```

---

## Dependency Injection (Advanced)

### Interface Extraction
```csharp
public interface IInputManager
{
    void Update(GameWindow window);
    void ResetInputStates();
    bool IsKeyDown(params Keys[] keys);
    bool IsAnyKeyDown(params Keys[] keys);
    bool IsMouseButtonDown(params MouseButton[] buttons);
    bool IsAnyMouseButtonDown(params MouseButton[] buttons);
    bool IsGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons);
    bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons);
    Point2D MouseLocation { get; }

    event KeyboardKeyEventHandler KeyboardKeyPressed;
    event KeyboardKeyEventHandler KeyboardKeyReleased;
    event KeyboardKeyEventHandler KeyboardKeyHeldDown;
    event MouseButtonEventHandler MouseButtonPressed;
    event MouseButtonEventHandler MouseButtonReleased;
    event MouseButtonEventHandler MouseButtonHeldDown;
    event MouseEventHandler MouseMoved;
    event GamepadButtonEventHandler GamepadButtonPressed;
    event GamepadButtonEventHandler GamepadButtonReleased;
    event GamepadButtonEventHandler GamepadButtonHeldDown;
}

// Wrapper for DI
public class InputManagerWrapper : IInputManager
{
    private readonly InputManager _inner = InputManager.Instance;

    // Forward all members...
}
```

### Registration (Microsoft.Extensions.DependencyInjection)
```csharp
services.AddSingleton<IInputManager>(_ => new InputManagerWrapper());
```

---

## Headless / Server Usage

**Not supported** — requires `GameWindow` and MonoGame initialization. For server-side input simulation, create a mock `IInputManager`.

---

## Platform-Specific Notes

### Windows
- Works out of the box
- Raw Input / XInput for gamepads

### Linux (DesktopGL)
- SDL2 backend
- **Multi-monitor coordinate fix** applied automatically
- Requires `ttf-mscorefonts-installer` for default font

### macOS (DesktopGL)
- SDL2 backend
- Same coordinate behavior as Linux

### Console / Mobile
- Not tested
- May require different MonoGame runtime (not DesktopGL)

---

## Version Migration

### 1.x → 2.x
- Target framework: net6.0 → net10.0
- MonoGame: 3.8.x (same)
- API: Compatible (no breaking changes in 2.1.x)

### Future: 3.x (Planned)
- May add: Touch input, gamepad axes, text input
- May remove: Workaround fields (`MouseButtonInputHandled`, `IsLeftMouseButtonClicked`)