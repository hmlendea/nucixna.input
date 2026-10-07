# NuciXNA.Input Architecture

## Overview

NuciXNA.Input is a lightweight input management library for MonoGame/XNA applications. It provides both **event-driven** and **polling-based** APIs for keyboard, mouse, and gamepad input, with frame-based state tracking.

## Repository Structure

```
NuciXNA.Input.sln
├── NuciXNA.Input/                 # Main library project
│   ├── InputManager.cs            # Core singleton input manager
│   ├── ButtonState.cs             # Frame-based button state enum-like class
│   ├── KeyboardKeyEvent.cs        # Keyboard event args + delegate
│   ├── MouseButton.cs             # Mouse button enum-like class
│   ├── MouseButtonEvent.cs        # Mouse button event args + delegate
│   ├── MouseEvent.cs              # Mouse movement event args + delegate
│   ├── GamepadButtonEvent.cs      # Gamepad button event args + delegate
│   └── NuciXNA.Input.csproj
├── NuciXNA.Input.UnitTests/       # Unit test project (NUnit)
│   ├── ButtonStateTests.cs
│   ├── KeyboardKeyEventArgsTests.cs
│   ├── MouseButtonTests.cs
│   ├── MouseButtonEventArgsTests.cs
│   ├── MouseEventArgsTests.cs
│   ├── GamepadButtonEventArgsTests.cs
│   └── NuciXNA.Input.UnitTests.csproj
└── .github/workflows/dotnet.yml   # CI pipeline
```

## Core Components

### 1. InputManager (Singleton)

**Location:** `NuciXNA.Input/InputManager.cs`

**Purpose:** Central input state manager. Tracks current and previous frame states for keyboard, mouse, and up to 4 gamepads. Raises events on state transitions.

**Key Responsibilities:**
- Frame-based state capture via `Update(GameWindow)`
- Event dispatch for press/release/hold transitions
- Polling API for current state queries
- Linux/SDL2 multi-monitor coordinate fix

**Public API:**

| Member | Type | Description |
|--------|------|-------------|
| `Instance` | static property | Thread-safe singleton accessor |
| `Update(GameWindow)` | method | Capture current frame state, fire events |
| `ResetInputStates()` | method | Clear all state (e.g., on focus loss) |
| `IsKeyDown(Keys[])` | method | All specified keys currently down |
| `IsAnyKeyDown(Keys[])` | method | Any specified key currently down |
| `IsMouseButtonDown(MouseButton[])` | method | All specified mouse buttons down |
| `IsAnyMouseButtonDown(MouseButton[])` | method | Any specified mouse button down |
| `IsGamepadButtonDown(PlayerIndex, Buttons[])` | method | All specified gamepad buttons down |
| `IsAnyGamepadButtonDown(PlayerIndex, Buttons[])` | method | Any specified gamepad button down |
| `MouseLocation` | property | Current mouse position (Point2D) |
| `MouseButtonInputHandled` | property | Workaround flag (TODO: remove) |
| `IsLeftMouseButtonClicked()` | method | Left button pressed this frame |

**Events:**

| Event | Delegate | Args | Fired When |
|-------|----------|------|------------|
| `KeyboardKeyPressed` | `KeyboardKeyEventHandler` | `KeyboardKeyEventArgs` | Key transitioned up→down |
| `KeyboardKeyReleased` | `KeyboardKeyEventHandler` | `KeyboardKeyEventArgs` | Key transitioned down→up |
| `KeyboardKeyHeldDown` | `KeyboardKeyEventHandler` | `KeyboardKeyEventArgs` | Key down in both frames |
| `MouseButtonPressed` | `MouseButtonEventHandler` | `MouseButtonEventArgs` | Mouse button up→down |
| `MouseButtonReleased` | `MouseButtonEventHandler` | `MouseButtonEventArgs` | Mouse button down→up |
| `MouseButtonHeldDown` | `MouseButtonEventHandler` | `MouseButtonEventArgs` | Mouse button down both frames |
| `MouseMoved` | `MouseEventHandler` | `MouseEventArgs` | Mouse position changed |
| `GamepadButtonPressed` | `GamepadButtonEventHandler` | `GamepadButtonEventArgs` | Gamepad button up→down |
| `GamepadButtonReleased` | `GamepadButtonEventHandler` | `GamepadButtonEventArgs` | Gamepad button down→up |
| `GamepadButtonHeldDown` | `GamepadButtonEventHandler` | `GamepadButtonEventArgs` | Gamepad button down both frames |

**Internal State:**
- `currentKeyState` / `previousKeyState` — `KeyboardState`
- `currentMouseState` / `previousMouseState` — `MouseState`
- `currentGamepadStates[4]` / `previousGamepadStates[4]` — `GamePadState[]`
- Static caches: `allKeys`, `allButtons`, `allPlayerIndices`

**Update Flow:**
```
Update(window)
├── Swap previous/current states (keyboard, mouse, gamepads)
├── Keyboard.GetState() → currentKeyState
├── Mouse.GetState() → currentMouseState
├── GamePad.GetState() for each PlayerIndex → currentGamepadStates
├── Linux coordinate fix (if rawX < 0)
├── CheckGamepadButtonStates()
├── CheckKeyboardKeyStates()
├── CheckMouseButtonStates()
└── CheckMouseMoved()
```

**State Transition Logic (per button/key):**

| Current | Previous | Event Fired | ButtonState |
|---------|----------|-------------|-------------|
| Down | Down | HeldDown | HeldDown |
| Down | Up | Pressed | Pressed |
| Up | Down | Released | Released |
| Up | Up | (none) | Idle |

### 2. ButtonState (Frame-State Enum)

**Location:** `NuciXNA.Input/ButtonState.cs`

**Purpose:** Represents the frame-relative state of any button/key. Immutable singleton instances.

**Values:**

| Instance | Id | IsDown | Meaning |
|----------|-----|--------|---------|
| `Idle` | 0 | false | Not pressed this frame or previous |
| `Pressed` | 1 | true | Transitioned up→down this frame |
| `Released` | 2 | false | Transitioned down→up this frame |
| `HeldDown` | 3 | true | Down in both current and previous frame |

**Features:**
- Implicit conversion to/from `int` and `string`
- `FromId(int)`, `FromName(string)` factory methods
- `GetValues()` returns all four states
- Equality by `Id`

### 3. MouseButton (Mouse Button Enum)

**Location:** `NuciXNA.Input/MouseButton.cs`

**Purpose:** Abstraction over XNA's mouse button fields. Maps to `MouseState` properties.

**Values:**

| Instance | Id | XNA Mapping |
|----------|-----|-------------|
| `Left` | 1 | `MouseState.LeftButton` |
| `Right` | 2 | `MouseState.RightButton` |
| `Middle` | 3 | `MouseState.MiddleButton` |
| `Back` | 4 | `MouseState.XButton1` |
| `Forward` | 5 | `MouseState.XButton2` |

**Features:** Same pattern as `ButtonState` (implicit conversions, factories, `GetValues()`).

### 4. Event Argument Classes

| Class | Location | Fields |
|-------|----------|--------|
| `KeyboardKeyEventArgs` | `KeyboardKeyEvent.cs` | `Keys Key`, `ButtonState KeyState` |
| `MouseButtonEventArgs` | `MouseButtonEvent.cs` | `MouseButton Button`, `ButtonState ButtonState`, `Point2D Location` |
| `MouseEventArgs` | `MouseEvent.cs` | `Point2D Location`, `Point2D PreviousLocation` |
| `GamepadButtonEventArgs` | `GamepadButtonEvent.cs` | `Buttons Button`, `ButtonState ButtonState`, `PlayerIndex PlayerIndex` |

All use `NuciXNA.Primitives.Point2D` for coordinates.

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `MonoGame.Framework.DesktopGL` | 3.8.4 | XNA API implementation (Keyboard, Mouse, GamePad, GameWindow) |
| `NuciXNA.Primitives` | 2.1.7 | `Point2D` struct, mapping utilities |

## Target Framework

- **net10.0** (both projects)

## Thread Safety

- `Instance` property uses double-checked locking with `Lock` (C# 13+)
- `Update()` is **not thread-safe** — must be called from game thread
- Event invocation is synchronous on the calling thread
- No internal synchronization for state reads during `Update()`

## Linux/SDL2 Multi-Monitor Workaround

In `Update()`, when `Mouse.GetState().X < 0`, the code assumes the window is on a monitor left of the primary in virtual desktop coordinates. It adds `window.ClientBounds.X/Y` to convert to client-relative coordinates.

**Limitation:** Only corrects X < 0 case; Y correction applied unconditionally in same block.

## Known Issues / TODOs

1. **Workaround fields** (lines 430-434 in InputManager.cs):
   - `MouseButtonInputHandled` — external flag to suppress processing
   - `IsLeftMouseButtonClicked()` — duplicate of `GetMouseButtonState(Left) == Pressed`
   - Marked `TODO: remove as soon as properly fixed`

2. **No gamepad axis/trigger support** — only digital buttons

3. **No touch input** — mouse/keyboard/gamepad only

4. **Single-threaded design** — assumes MonoGame's single-threaded game loop

## Testing

- **Framework:** NUnit 4.6.1 + NUnit3TestAdapter 6.2.0
- **Coverage:** Event args construction, ButtonState/MouseButton conversions, equality, factory methods
- **Missing:** Integration tests for `InputManager.Update()` event firing, state transitions, Linux coordinate fix

## Build & CI

- **Build:** `dotnet build`
- **Test:** `dotnet test`
- **CI:** GitHub Actions (ubuntu-latest, .NET 10.0.x)
- **Font setup:** Installs `ttf-mscorefonts-installer` for MonoGame

## Versioning

- Current: **2.1.1** (in NuciXNA.Input.csproj)
- Scheme: Semantic versioning
- License: GPL-3.0-or-later