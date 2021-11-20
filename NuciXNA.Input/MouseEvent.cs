using NuciXNA.Primitives;

namespace NuciXNA.Input
{
    /// <summary>
    /// Mouse event handler.
    /// </summary>
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    /// <summary>
    /// Mouse event arguments.
    /// </summary>
    public class MouseEventArgs
    {
        /// <summary>
        /// Gets current location of the mouse.
        /// </summary>
        /// <value>The current mouse location.</value>
        public Point2D Location { get; private set; }

        /// <summary>
        /// Gets previous location of the mouse.
        /// </summary>
        /// <value>The previous mouse location.</value>
        public Point2D PreviousLocation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseEventArgs"/> class.
        /// </summary>
        /// <param name="location">Mouse location.</param>
        public MouseEventArgs(Point2D location, Point2D previousLocation)
        {
            Location = location;
            PreviousLocation = previousLocation;
        }
    }
}
