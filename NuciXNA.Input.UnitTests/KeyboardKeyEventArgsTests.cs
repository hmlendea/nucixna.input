using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    public class KeyboardKeyEventArgsTests
    {
        [Test]
        public void GivenKeyboardKeyEventArgs_WhenCreated_ThenKeyIsSetCorrectly()
        {
            var args = new KeyboardKeyEventArgs(Keys.Space, ButtonState.Pressed);

            Assert.That(args.Key, Is.EqualTo(Keys.Space));
        }

        [Test]
        public void GivenKeyboardKeyEventArgs_WhenCreated_ThenKeyStateIsSetCorrectly()
        {
            var args = new KeyboardKeyEventArgs(Keys.Enter, ButtonState.HeldDown);

            Assert.That(args.KeyState, Is.EqualTo(ButtonState.HeldDown));
        }
    }
}
