using NuciXNA.Primitives;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]
    public sealed class MouseEventArgsTests
    {
        [Test]
        public static void GivenMouseEventArgs_WhenCreated_ThenLocationIsSetCorrectly()
        {
            Point2D location = new(96, 128);
            MouseEventArgs eventArgs = new(location, new Point2D(8, 16));

            Assert.That(eventArgs.Location, Is.EqualTo(location));
        }

        [Test]
        public static void GivenMouseEventArgs_WhenCreated_ThenPreviousLocationIsSetCorrectly()
        {
            Point2D previousLocation = new(32, 48);
            MouseEventArgs eventArgs = new(new Point2D(256, 512), previousLocation);

            Assert.That(eventArgs.PreviousLocation, Is.EqualTo(previousLocation));
        }
    }
}
