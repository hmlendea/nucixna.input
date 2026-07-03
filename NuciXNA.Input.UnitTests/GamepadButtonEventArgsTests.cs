using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    public class GamepadButtonEventArgsTests
    {
        [Test]
        public void GivenGamepadButtonEventArgs_WhenCreated_ThenButtonIsSetCorrectly()
        {
            GamepadButtonEventArgs args = new(Buttons.A, ButtonState.Pressed, PlayerIndex.One);

            Assert.That(args.Button, Is.EqualTo(Buttons.A));
        }

        [Test]
        public void GivenGamepadButtonEventArgs_WhenCreated_ThenButtonStateIsSetCorrectly()
        {
            GamepadButtonEventArgs args = new(Buttons.B, ButtonState.HeldDown, PlayerIndex.One);

            Assert.That(args.ButtonState, Is.EqualTo(ButtonState.HeldDown));
        }

        [Test]
        public void GivenGamepadButtonEventArgs_WhenCreated_ThenPlayerIndexIsSetCorrectly()
        {
            GamepadButtonEventArgs args = new(Buttons.X, ButtonState.Released, PlayerIndex.Two);

            Assert.That(args.PlayerIndex, Is.EqualTo(PlayerIndex.Two));
        }
    }
}
