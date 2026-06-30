using NuciXNA.Primitives;
using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    public class MouseEventArgsTests
    {
        [Test]
        public void GivenMouseEventArgs_WhenCreated_ThenLocationIsSetCorrectly()
        {
            var location = new Point2D(15, 30);
            var args = new MouseEventArgs(location, new Point2D(0, 0));

            Assert.That(args.Location, Is.EqualTo(location));
        }

        [Test]
        public void GivenMouseEventArgs_WhenCreated_ThenPreviousLocationIsSetCorrectly()
        {
            var previousLocation = new Point2D(5, 10);
            var args = new MouseEventArgs(new Point2D(15, 30), previousLocation);

            Assert.That(args.PreviousLocation, Is.EqualTo(previousLocation));
        }
    }
}
