using NuciXNA.Primitives;
using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    public class MouseButtonEventArgsTests
    {
        [Test]
        public void GivenMouseButtonEventArgs_WhenCreated_ThenButtonIsSetCorrectly()
        {
            var args = new MouseButtonEventArgs(MouseButton.Left, ButtonState.Pressed, new Point2D(10, 20));

            Assert.That(args.Button, Is.EqualTo(MouseButton.Left));
        }

        [Test]
        public void GivenMouseButtonEventArgs_WhenCreated_ThenButtonStateIsSetCorrectly()
        {
            var args = new MouseButtonEventArgs(MouseButton.Right, ButtonState.Released, new Point2D(0, 0));

            Assert.That(args.ButtonState, Is.EqualTo(ButtonState.Released));
        }

        [Test]
        public void GivenMouseButtonEventArgs_WhenCreated_ThenLocationIsSetCorrectly()
        {
            var args = new MouseButtonEventArgs(MouseButton.Middle, ButtonState.HeldDown, new Point2D(42, 7));

            Assert.That(args.Location.X, Is.EqualTo(42));
            Assert.That(args.Location.Y, Is.EqualTo(7));
        }
    }
}
