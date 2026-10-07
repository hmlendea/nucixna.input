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
    /// <param name="location">Mouse location.</param>
    public sealed class MouseEventArgs(
        Point2D location,
        Point2D previousLocation)
    {
        /// <summary>
        /// Gets current location of the mouse.
        /// </summary>
        /// <value>The current mouse location.</value>
        public Point2D Location { get; } = location;

        /// <summary>
        /// Gets previous location of the mouse.
        /// </summary>
        /// <value>The previous mouse location.</value>
        public Point2D PreviousLocation { get; } = previousLocation;
    }
}
