using NuciXNA.Primitives;

namespace NuciXNA.Input
{
    /// <summary>
    /// Mouse button event handler.
    /// </summary>
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    /// <summary>
    /// Mouse button event arguments.
    /// </summary>
    /// <param name="button">Button.</param>
    /// <param name="buttonState">Button state.</param>
    /// <param name="location">Mouse location.</param>
    public class MouseButtonEventArgs(MouseButton button, ButtonState buttonState, Point2D location)
    {
        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public MouseButton Button { get; private set; } = button;

        /// <summary>
        /// Gets the state of the button.
        /// </summary>
        /// <value>The state of the button.</value>
        public ButtonState ButtonState { get; private set; } = buttonState;

        /// <summary>
        /// Gets location of the mouse.
        /// </summary>
        /// <value>The mouse location.</value>
        public Point2D Location { get; private set; } = location;
    }
}
