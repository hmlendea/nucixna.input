# NuciXNA.Input Documentation Index

## Overview
NuciXNA.Input is a lightweight input management library for MonoGame/XNA providing event-driven and polling-based APIs for keyboard, mouse, and gamepad input.

## Documentation Files

| File | Description |
|------|-------------|
| [architecture.md](architecture.md) | System architecture, components, dependencies, data flow |
| [api-reference.md](api-reference.md) | Complete public API reference with signatures |
| [usage-guide.md](usage-guide.md) | Getting started, common patterns, best practices |
| [event-system.md](event-system.md) | Event flow, subscription patterns, argument details |
| [state-management.md](state-management.md) | Frame-based state model, transition logic, polling implementation |
| [integration.md](integration.md) | MonoGame integration, screen management, DI, multiplayer |
| [testing.md](testing.md) | Test structure, coverage, gaps, recommended additions |
| [known-issues.md](known-issues.md) | Limitations, bugs, workarounds, platform notes |

## Quick Links

### For New Users
1. [Installation & Quick Start](usage-guide.md#installation)
2. [Basic Setup](usage-guide.md#basic-setup)
3. [Event vs Polling](usage-guide.md#event-vs-polling)

### For API Reference
- [InputManager](api-reference.md#inputmanager)
- [ButtonState](api-reference.md#buttonstate)
- [MouseButton](api-reference.md#mousebutton)
- [Event Args](api-reference.md#keyboardkeyeventargs)

### For Architecture Understanding
- [Core Components](architecture.md#core-components)
- [Update Flow](architecture.md#update-flow)
- [State Transitions](state-management.md#transition-derivation)

### For Integration
- [MonoGame Game Class](integration.md#monogame-game-class-integration)
- [Screen Management](integration.md#screenstate-management-integration)
- [Dependency Injection](integration.md#dependency-injection-advanced)

### For Testing
- [Current Coverage](testing.md#current-coverage)
- [Missing Tests](testing.md#missing-test-coverage-gaps)
- [Recommended Additions](testing.md#recommended-test-additions)

### For Troubleshooting
- [Known Limitations](known-issues.md#current-limitations)
- [Known Bugs](known-issues.md#known-bugs--workarounds)
- [Platform Notes](known-issues.md#platform-specific-issues)

## Repository Structure
```
NuciXNA.Input.sln
├── NuciXNA.Input/              # Main library
│   ├── InputManager.cs         # Singleton input manager
│   ├── ButtonState.cs          # Frame-state enum
│   ├── KeyboardKeyEvent.cs     # Keyboard events
│   ├── MouseButton.cs          # Mouse button enum
│   ├── MouseButtonEvent.cs     # Mouse button events
│   ├── MouseEvent.cs           # Mouse move events
│   ├── GamepadButtonEvent.cs   # Gamepad events
│   └── NuciXNA.Input.csproj
├── NuciXNA.Input.UnitTests/    # NUnit tests
│   ├── *Tests.cs
│   └── NuciXNA.Input.UnitTests.csproj
└── .github/workflows/dotnet.yml # CI
```

## Key Types

| Type | Purpose |
|------|---------|
| `InputManager` | Singleton, frame update, events, polling |
| `ButtonState` | Idle/Pressed/Released/HeldDown |
| `MouseButton` | Left/Right/Middle/Back/Forward |
| `KeyboardKeyEventArgs` | Key + ButtonState |
| `MouseButtonEventArgs` | Button + ButtonState + Location |
| `MouseEventArgs` | Location + PreviousLocation |
| `GamepadButtonEventArgs` | Button + ButtonState + PlayerIndex |

## Requirements
- .NET 10.0
- MonoGame.Framework.DesktopGL 3.8.4+
- NuciXNA.Primitives 2.1.7+ (for Point2D)

## License
GPL-3.0-or-later