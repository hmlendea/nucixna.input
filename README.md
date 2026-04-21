[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html) [![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucixna.input)](https://github.com/hmlendea/nucixna.input/releases/latest) [![Build Status](https://github.com/hmlendea/nucixna.input/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucixna.input/actions/workflows/dotnet.yml) [![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciXNA.Input

Input management for NuciXNA (MonoGame/XNA), with event-driven and polling-based APIs for keyboard and mouse input.

## Features

- Keyboard input events: pressed, released, held-down
- Mouse input events: button pressed/released/held-down and movement
- Polling helpers for "all" and "any" key/button checks
- Frame-based state tracking (`Pressed`, `Released`, `HeldDown`, `Idle`)
- Lightweight singleton API (`InputManager.Instance`)

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciXNA.Input)

### .NET CLI

```bash
dotnet add package NuciXNA.Input
```

### Package Manager

```powershell
Install-Package NuciXNA.Input
```

## Requirements

- .NET: `net10.0`
- MonoGame DesktopGL (or compatible MonoGame runtime)

## Quick Start

Call `Update` once per frame (typically from your `Game.Update`) and subscribe to events.

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NuciXNA.Input;

public class Game1 : Game
{
	protected override void Initialize()
	{
		InputManager.Instance.KeyboardKeyPressed += OnKeyboardKeyPressed;
		InputManager.Instance.MouseButtonPressed += OnMouseButtonPressed;
		InputManager.Instance.MouseMoved += OnMouseMoved;

		base.Initialize();
	}

	protected override void Update(GameTime gameTime)
	{
		InputManager.Instance.Update(Window);

		if (InputManager.Instance.IsKeyDown(Keys.LeftControl, Keys.S))
		{
			// Handle Ctrl+S
		}

		if (InputManager.Instance.IsAnyMouseButtonDown())
		{
			// At least one mouse button is currently down
		}

		base.Update(gameTime);
	}

	private static void OnKeyboardKeyPressed(object sender, KeyboardKeyEventArgs e)
	{
		if (e.Key == Keys.Escape)
		{
			// Handle Escape
		}
	}

	private static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
	{
		// e.Button, e.ButtonState, e.Location
	}

	private static void OnMouseMoved(object sender, MouseEventArgs e)
	{
		// e.Location, e.PreviousLocation
	}
}
```

## Event API

`InputManager` exposes the following events:

- `KeyboardKeyPressed`
- `KeyboardKeyReleased`
- `KeyboardKeyHeldDown`
- `MouseButtonPressed`
- `MouseButtonReleased`
- `MouseButtonHeldDown`
- `MouseMoved`

Event args include rich state:

- `KeyboardKeyEventArgs`: `Key`, `KeyState`
- `MouseButtonEventArgs`: `Button`, `ButtonState`, `Location`
- `MouseEventArgs`: `Location`, `PreviousLocation`

## Polling API

Common polling methods:

- `IsKeyDown(params Keys[] keys)`
- `IsAnyKeyDown()`
- `IsAnyKeyDown(params Keys[] keys)`
- `IsMouseButtonDown(params MouseButton[] buttons)`
- `IsAnyMouseButtonDown()`
- `IsAnyMouseButtonDown(params MouseButton[] buttons)`

Button state model:

- `ButtonState.Idle`
- `ButtonState.Pressed`
- `ButtonState.Released`
- `ButtonState.HeldDown`

Mouse buttons:

- `MouseButton.Left`
- `MouseButton.Right`
- `MouseButton.Middle`
- `MouseButton.Back`
- `MouseButton.Forward`

## Development

### Build

```bash
dotnet build NuciXNA.Input.csproj
```

### Run

```bash
dotnet run --project NuciXNA.Input.csproj
```

### Test

```bash
dotnet test NuciXNA.Input.sln
```

## Contributing

Contributions are welcome.

When contributing:

- keep the project cross-platform
- preserve the existing public API unless a breaking change is intentional
- keep the changes focused and consistent with the current coding style
- update the documentation when the behavior changes
- include tests for any new behavior

## License

Licensed under the GNU General Public License v3.0 or later.
See [LICENSE](LICENSE) for details.
