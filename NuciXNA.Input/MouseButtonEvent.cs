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
    public class MouseButtonEventArgs
    {
        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public MouseButton Button { get; private set; }

        /// <summary>
        /// Gets the state of the button.
        /// </summary>
        /// <value>The state of the button.</value>
        public ButtonState ButtonState { get; private set; }

        /// <summary>
        /// Gets location of the mouse.
        /// </summary>
        /// <value>The mouse location.</value>
        public Point2D Location { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseButtonEventArgs"/> class.
        /// </summary>
        /// <param name="button">Button.</param>
        /// <param name="buttonState">Button state.</param>
        /// <param name="location">Mouse location.</param>
        public MouseButtonEventArgs(MouseButton button, ButtonState buttonState, Point2D location)
        {
            Button = button;
            ButtonState = buttonState;
            Location = location;
        }
    }
}
