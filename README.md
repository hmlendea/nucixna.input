[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucixna.input)](https://github.com/hmlendea/nucixna.input/releases/latest)
[![Build Status](https://github.com/hmlendea/nucixna.input/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucixna.input/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/hmlendea/nucixna.input)](https://github.com/hmlendea/nucixna.input/blob/master/LICENSE)

# NuciXNA.Input

Input management for NuciXNA (MonoGame/XNA), with event-driven and polling-based APIs for keyboard and mouse input.

## 📑 Table of Contents

- [Capabilities](#-capabilities)
- [Usage](#-usage)
- [Known Limitations](#️-known-limitations)
- [System Requirements](#️-system-requirements)
- [Compatibility](#-compatibility)
- [Installation](#-installation)
- [Development](#️-development)
- [Architecture](#️-architecture)
- [Documentation](#-documentation)
- [API Reference](#-api-reference)
- [Privacy and Data](#️-privacy-and-data)
- [Security](#-security)
- [Contributing](#-contributing)
- [Project Engagement](#-project-engagement)
- [License](#-license)

## ✨ Capabilities

- Keyboard input events: pressed, released, held-down
- Mouse input events: button pressed/released/held-down and movement
- Polling helpers for "all" and "any" key/button checks
- Frame-based state tracking (`Pressed`, `Released`, `HeldDown`, `Idle`)
- Lightweight singleton API (`InputManager.Instance`)

## 🚀 Usage

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

## ⚠️ Known Limitations

See [Known Issues](docs/known-issues.md) for limitations, bugs, workarounds, and platform notes.

## 🖥️ System Requirements

| Component | Minimum | Recommended |
|-----------|---------|-------------|
| .NET | `net10.0` | `net10.0` |
| MonoGame | DesktopGL 3.8.4 | DesktopGL 3.8.4 |

## 🧩 Compatibility

| Component | Supported Versions | Notes |
|-----------|--------------------|-------|
| MonoGame | DesktopGL 3.8.4 | Referenced via `MonoGame.Framework.DesktopGL` NuGet package |
| .NET | 10.0 | Target framework `net10.0` |

## 📦 Installation

[![Obtain it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciXNA.Input)

### Package Manager Installation

```bash
dotnet add package NuciXNA.Input
```

Or, via the `Package Manager Console`:
```powershell
Install-Package NuciXNA.Input
```

## 🛠️ Development

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Setup

```bash
git clone https://github.com/hmlendea/nucixna.input.git
cd nucixna.input
dotnet restore
```

### Build

```bash
dotnet build NuciXNA.Input.csproj
```

### Test

```bash
dotnet test NuciXNA.Input.sln
```

## 🏗️ Architecture

See the [architecture documentation](ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 📚 Documentation

| Resource | Description |
|----------|-------------|
| [Architecture](ARCHITECTURE.md) | System architecture, components, data flow, and design decisions |
| [API Reference](docs/api-reference.md) | Complete public API signatures |
| [Usage Guide](docs/usage-guide.md) | Installation, setup, patterns, best practices |
| [Event System](docs/event-system.md) | Event flow, subscription patterns, argument details |
| [State Management](docs/state-management.md) | Frame-based state model, transition logic, polling |
| [Integration](docs/integration.md) | MonoGame integration, screen management, DI, multiplayer |
| [Testing](docs/testing.md) | Test structure, coverage, gaps, recommended additions |
| [Known Issues](docs/known-issues.md) | Limitations, bugs, workarounds, platform notes |

## 🌐 API Reference

See [docs/api-reference.md](docs/api-reference.md) for the complete public API signatures.

## 🛡️ Privacy and Data

For the detailed description of how the application handles privacy and personal data, see [PRIVACY.md](./PRIVACY.md).

## 🔒 Security

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Add tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nucixna.input/issues) for problems or suggestions

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nucixna.input/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3.0 or later`.
See [LICENSE](./LICENSE) for further information.
