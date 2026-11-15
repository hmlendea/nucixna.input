using NuciXNA.Primitives;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]
    public sealed class MouseButtonEventArgsTests
    {
        [Test]
        public static void GivenMouseButtonEventArgs_WhenCreated_ThenButtonIsSetCorrectly()
        {
            MouseButtonEventArgs eventArgs = new(
                MouseButton.Left,
                ButtonState.Pressed,
                new Point2D(16, 32));

            Assert.That(eventArgs.Button, Is.EqualTo(MouseButton.Left));
        }

        [Test]
        public static void GivenMouseButtonEventArgs_WhenCreated_ThenButtonStateIsSetCorrectly()
        {
            MouseButtonEventArgs eventArgs = new(
                MouseButton.Right,
                ButtonState.Released,
                new Point2D(4, 8));

            Assert.That(eventArgs.ButtonState, Is.EqualTo(ButtonState.Released));
        }

        [Test]
        public static void GivenMouseButtonEventArgs_WhenCreated_ThenLocationIsSetCorrectly()
        {
            Point2D location = new(42, 64);
            MouseButtonEventArgs eventArgs = new(
                MouseButton.Middle,
                ButtonState.HeldDown,
                location);

            Assert.That(eventArgs.Location, Is.EqualTo(location));
        }
    }
}
