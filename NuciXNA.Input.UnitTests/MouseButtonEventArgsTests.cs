using NuciXNA.Primitives;
using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    public class MouseButtonEventArgsTests
    {
        [Test]
        public void GivenMouseButtonEventArgs_WhenCreated_ThenButtonIsSetCorrectly()
        {
            MouseButtonEventArgs args = new(MouseButton.Left, ButtonState.Pressed, new Point2D(10, 20));

            Assert.That(args.Button, Is.EqualTo(MouseButton.Left));
        }

        [Test]
        public void GivenMouseButtonEventArgs_WhenCreated_ThenButtonStateIsSetCorrectly()
        {
            MouseButtonEventArgs args = new(MouseButton.Right, ButtonState.Released, new Point2D(0, 0));

            Assert.That(args.ButtonState, Is.EqualTo(ButtonState.Released));
        }

        [Test]
        public void GivenMouseButtonEventArgs_WhenCreated_ThenLocationIsSetCorrectly()
        {
            Point2D location = new(42, 7);
            MouseButtonEventArgs args = new(MouseButton.Middle, ButtonState.HeldDown, location);

            Assert.That(args.Location, Is.EqualTo(location));
        }
    }
}
