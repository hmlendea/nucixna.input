using NuciXNA.Primitives;
using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]

    public sealed class MouseEventArgsTests
    {
        [Test]
        public void GivenMouseEventArgs_WhenCreated_ThenLocationIsSetCorrectly()
        {
            Point2D location = new(15, 30);
            MouseEventArgs args = new(location, new Point2D(0, 0));

            Assert.That(args.Location, Is.EqualTo(location));
        }

        [Test]
        public void GivenMouseEventArgs_WhenCreated_ThenPreviousLocationIsSetCorrectly()
        {
            Point2D previousLocation = new(5, 10);
            MouseEventArgs args = new(new Point2D(15, 30), previousLocation);

            Assert.That(args.PreviousLocation, Is.EqualTo(previousLocation));
        }
    }
}
