using Microsoft.Xna.Framework.Input;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]
    public sealed class KeyboardKeyEventArgsTests
    {
        [Test]
        public static void GivenKeyboardKeyEventArgs_WhenCreated_ThenKeyIsSetCorrectly()
        {
            KeyboardKeyEventArgs eventArgs = new(Keys.Space, ButtonState.Pressed);

            Assert.That(eventArgs.Key, Is.EqualTo(Keys.Space));
        }

        [Test]
        public static void GivenKeyboardKeyEventArgs_WhenCreated_ThenKeyStateIsSetCorrectly()
        {
            KeyboardKeyEventArgs eventArgs = new(Keys.Enter, ButtonState.HeldDown);

            Assert.That(eventArgs.KeyState, Is.EqualTo(ButtonState.HeldDown));
        }
    }
}
