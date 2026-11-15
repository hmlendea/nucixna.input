using Microsoft.Xna.Framework.Input;
using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]

    public sealed class KeyboardKeyEventArgsTests
    {
        [Test]
        public void GivenKeyboardKeyEventArgs_WhenCreated_ThenKeyIsSetCorrectly()
        {
            KeyboardKeyEventArgs args = new(Keys.Space, ButtonState.Pressed);

            Assert.That(args.Key, Is.EqualTo(Keys.Space));
        }

        [Test]
        public void GivenKeyboardKeyEventArgs_WhenCreated_ThenKeyStateIsSetCorrectly()
        {
            KeyboardKeyEventArgs args = new(Keys.Enter, ButtonState.HeldDown);

            Assert.That(args.KeyState, Is.EqualTo(ButtonState.HeldDown));
        }
    }
}
