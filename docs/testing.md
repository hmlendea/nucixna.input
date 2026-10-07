# NuciXNA.Input Testing Guide

## Test Project Structure

```
NuciXNA.Input.UnitTests/
├── ButtonStateTests.cs           # ButtonState conversions, equality, factories
├── KeyboardKeyEventArgsTests.cs  # KeyboardKeyEventArgs construction
├── MouseButtonTests.cs           # MouseButton conversions, equality, factories
├── MouseButtonEventArgsTests.cs  # MouseButtonEventArgs construction
├── MouseEventArgsTests.cs        # MouseEventArgs construction
├── GamepadButtonEventArgsTests.cs # GamepadButtonEventArgs construction
└── NuciXNA.Input.UnitTests.csproj
```

## Running Tests

```bash
# From solution root
dotnet test

# From test project
cd NuciXNA.Input.UnitTests
dotnet test

# With verbosity
dotnet test --verbosity normal

# Filter by class
dotnet test --filter "FullyQualifiedName~ButtonStateTests"

# Filter by method
dotnet test --filter "FullyQualifiedName~IsDown_ValueIsCorrect"
```

## Current Coverage

### ButtonStateTests (19 tests)
| Test | Verifies |
|------|----------|
| `IsDown_ValueIsCorrect` | `IsDown` property for all 4 states |
| `FromId_CalledWithExistingId_ReturnsCorrectButtonState` | `FromId` round-trip |
| `FromId_CalledWithInexistentId_ThrowsArgumentException` | Invalid ID throws |
| `FromName_CalledWithExistingName_ReturnsCorrectButtonState` | `FromName` round-trip |
| `FromName_CalledWithInexistentName_ThrowsArgumentException` | Invalid name throws |
| `ToString_ReturnsCorrectValue` | `ToString()` returns `Name` |
| `GetHashCode_ReturnsCorrectValue` | Hash code = `Id.GetHashCode()` |
| `Equals_CalledWithSameButtonState_ReturnsTrue` | Same instance equality |
| `Equals_CalledWithOtherButtonState_ReturnsFalse` | Different instance inequality |
| `Equals_CalledWithSameButtonStateAsObject_ReturnsTrue` | Object equality |
| `Equals_CalledWithOtherButtonStateAsObject_ReturnsFalse` | Object inequality |
| `Equals_CalledWithOtherType_ReturnsFalse` | Cross-type returns false |
| `Equals_CalledWithNull_ReturnsFalse` | Null returns false |
| `EqualsOperator_OtherIsSameButtonState_ReturnsTrue` | `==` operator |
| `EqualsOperator_CurrentIsNull_ReturnsFalse` | `null == state` |
| `EqualsOperator_OtherIsNull_ReturnsFalse` | `state == null` |
| `CastAsInt_ReturnsCorrectValue` | Implicit `int` conversion |
| `CastAsString_ReturnsCorrectValue` | Implicit `string` conversion |
| `AssignInteger_AssignedExistingId_ReturnsCorrectButtonState` | Explicit `int` → `ButtonState` |
| `AssignInteger_AssignedInexistentId_ThrowsArgumentException` | Invalid explicit `int` throws |
| `AssignString_AssignedExistingName_ReturnsCorrectButtonState` | Explicit `string` → `ButtonState` |
| `AssignString_AssignedInexistentName_ThrowsArgumentException` | Invalid explicit `string` throws |
| `GivenTwoNullButtonStates_WhenComparedWithEqualsOperator_ThenReturnsTrue` | `null == null` |

### MouseButtonTests (22 tests)
Same pattern as ButtonStateTests for MouseButton enum (5 values).

### KeyboardKeyEventArgsTests (2 tests)
| Test | Verifies |
|------|----------|
| `GivenKeyboardKeyEventArgs_WhenCreated_ThenKeyIsSetCorrectly` | `Key` property |
| `GivenKeyboardKeyEventArgs_WhenCreated_ThenKeyStateIsSetCorrectly` | `KeyState` property |

### MouseButtonEventArgsTests (3 tests)
| Test | Verifies |
|------|----------|
| `GivenMouseButtonEventArgs_WhenCreated_ThenButtonIsSetCorrectly` | `Button` property |
| `GivenMouseButtonEventArgs_WhenCreated_ThenButtonStateIsSetCorrectly` | `ButtonState` property |
| `GivenMouseButtonEventArgs_WhenCreated_ThenLocationIsSetCorrectly` | `Location` property |

### MouseEventArgsTests (2 tests)
| Test | Verifies |
|------|----------|
| `GivenMouseEventArgs_WhenCreated_ThenLocationIsSetCorrectly` | `Location` property |
| `GivenMouseEventArgs_WhenCreated_ThenPreviousLocationIsSetCorrectly` | `PreviousLocation` property |

### GamepadButtonEventArgsTests (3 tests)
| Test | Verifies |
|------|----------|
| `GivenGamepadButtonEventArgs_WhenCreated_ThenButtonIsSetCorrectly` | `Button` property |
| `GivenGamepadButtonEventArgs_WhenCreated_ThenButtonStateIsSetCorrectly` | `ButtonState` property |
| `GivenGamepadButtonEventArgs_WhenCreated_ThenPlayerIndexIsSetCorrectly` | `PlayerIndex` property |

**Total: ~51 tests**

---

## Missing Test Coverage (Gaps)

### InputManager — Critical Gaps
| Area | Missing Tests |
|------|---------------|
| `Update()` event firing | No tests verify events fire on correct transitions |
| `Update()` state transitions | No tests for Pressed/Released/HeldDown/Idle logic |
| `Update()` Linux coordinate fix | No test for negative X correction |
| `ResetInputStates()` | No test verifies state clearing |
| Polling methods | No tests for `IsKeyDown`, `IsAnyKeyDown`, `IsMouseButtonDown`, etc. |
| Gamepad polling | No tests for `IsGamepadButtonDown`, `IsAnyGamepadButtonDown` |
| Singleton thread safety | No concurrent access tests |
| `MouseButtonInputHandled` flag | No tests for workaround behavior |
| `IsLeftMouseButtonClicked()` | No tests |

### Integration Scenarios
| Scenario | Missing |
|----------|---------|
| Multi-frame sequence | Press → Hold → Release across frames |
| Simultaneous multi-key | Chord detection (Ctrl+Shift+S) |
| Multi-gamepad | 4 controllers simultaneously |
| Focus loss/reset | `ResetInputStates()` after deactivation |
| Mouse move + button | Drag sequences |

---

## Recommended Test Additions

### InputManagerTests.cs (New File)
```csharp
[TestFixture]
public class InputManagerTests
{
    [SetUp]
    public void Setup() => InputManager.Instance.ResetInputStates();

    [Test]
    public void Update_FirstFrame_NoPreviousState_NoEventsFired() { ... }

    [Test]
    public void Update_KeyPressed_FiresKeyboardKeyPressed() { ... }

    [Test]
    public void Update_KeyHeld_FiresKeyboardKeyHeldDown() { ... }

    [Test]
    public void Update_KeyReleased_FiresKeyboardKeyReleased() { ... }

    [Test]
    public void Update_MouseButtonPressed_FiresMouseButtonPressed() { ... }

    [Test]
    public void Update_MouseMoved_FiresMouseMoved() { ... }

    [Test]
    public void Update_GamepadButtonPressed_FiresGamepadButtonPressed() { ... }

    [Test]
    public void IsKeyDown_AllKeysDown_ReturnsTrue() { ... }

    [Test]
    public void IsAnyKeyDown_AnyKeyDown_ReturnsTrue() { ... }

    [Test]
    public void IsMouseButtonDown_ButtonDown_ReturnsTrue() { ... }

    [Test]
    public void IsAnyMouseButtonDown_AnyButtonDown_ReturnsTrue() { ... }

    [Test]
    public void IsGamepadButtonDown_ButtonDown_ReturnsTrue() { ... }

    [Test]
    public void IsAnyGamepadButtonDown_AnyButtonDown_ReturnsTrue() { ... }

    [Test]
    public void ResetInputStates_ClearsAllState() { ... }

    [Test]
    public void Update_LinuxNegativeX_CorrectsToClientCoordinates() { ... }
}
```

### Mocking Strategy
- `Keyboard.GetState()`, `Mouse.GetState()`, `GamePad.GetState()` are static → use **Microsoft Fakes** or wrap in interface
- Alternative: Use real MonoGame in headless mode (requires display)
- Consider extracting `IInputStateProvider` for testability

---

## CI Integration

```yaml
# .github/workflows/dotnet.yml
- name: Test
  run: dotnet test --no-build --verbosity normal
```

Runs on `ubuntu-latest` with .NET 10.0.x. Installs `ttf-mscorefonts-installer` for MonoGame font rendering.

---

## Test Design Principles

1. **Unit tests only** — no integration tests currently
2. **Constructor/property verification** — event args only
3. **Enum-like class contracts** — conversions, equality, factories
4. **No behavior tests** — `InputManager.Update()` logic untested
5. **Deterministic** — no timing, no external dependencies

---

## Adding Tests

1. Create test class in `NuciXNA.Input.UnitTests/`
2. Follow naming: `{ClassName}Tests.cs`
3. Use NUnit 4 attributes: `[TestFixture]`, `[Test]`, `[SetUp]`, `[TearDown]`
4. Use `Assert.That()` syntax (NUnit 4 constraint model)
5. Run `dotnet test` to verify