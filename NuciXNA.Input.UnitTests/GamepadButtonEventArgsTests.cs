using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]
    public sealed class GamepadButtonEventArgsTests
    {
        [Test]
        public static void GivenGamepadButtonEventArgs_WhenCreated_ThenButtonIsSetCorrectly()
        {
            GamepadButtonEventArgs eventArgs = new(
                Buttons.A,
                ButtonState.Pressed,
                PlayerIndex.One);

            Assert.That(eventArgs.Button, Is.EqualTo(Buttons.A));
        }

        [Test]
        public static void GivenGamepadButtonEventArgs_WhenCreated_ThenButtonStateIsCorrect()
        {
            GamepadButtonEventArgs eventArgs = new(
                Buttons.B,
                ButtonState.HeldDown,
                PlayerIndex.One);

            Assert.That(eventArgs.ButtonState, Is.EqualTo(ButtonState.HeldDown));
        }

        [Test]
        public static void GivenGamepadButtonEventArgs_WhenCreated_ThenPlayerIndexIsCorrect()
        {
            GamepadButtonEventArgs eventArgs = new(
                Buttons.X,
                ButtonState.Released,
                PlayerIndex.Two);

            Assert.That(eventArgs.PlayerIndex, Is.EqualTo(PlayerIndex.Two));
        }
    }
}
