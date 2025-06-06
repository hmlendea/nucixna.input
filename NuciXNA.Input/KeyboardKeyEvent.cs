using Microsoft.Xna.Framework.Input;

namespace NuciXNA.Input
{
    /// <summary>
    /// Keyboard key event handler.
    /// </summary>
    public delegate void KeyboardKeyEventHandler(object sender, KeyboardKeyEventArgs e);

    /// <summary>
    /// Keyboard key event arguments.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="keyState">Key state.</param>
    public class KeyboardKeyEventArgs(Keys key, ButtonState keyState)
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public Keys Key { get; private set; } = key;

        /// <summary>
        /// Gets the state of the key.
        /// </summary>
        /// <value>The state of the key.</value>
        public ButtonState KeyState { get; private set; } = keyState;
    }
}
