using NuciXNA.Primitives;
using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    public class MouseEventArgsTests
    {
        [Test]
        public void GivenMouseEventArgs_WhenCreated_ThenLocationIsSetCorrectly()
        {
            var args = new MouseEventArgs(new Point2D(15, 30), new Point2D(0, 0));

            Assert.That(args.Location.X, Is.EqualTo(15));
            Assert.That(args.Location.Y, Is.EqualTo(30));
        }

        [Test]
        public void GivenMouseEventArgs_WhenCreated_ThenPreviousLocationIsSetCorrectly()
        {
            var args = new MouseEventArgs(new Point2D(15, 30), new Point2D(5, 10));

            Assert.That(args.PreviousLocation.X, Is.EqualTo(5));
            Assert.That(args.PreviousLocation.Y, Is.EqualTo(10));
        }
    }
}
