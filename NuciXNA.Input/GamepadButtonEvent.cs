using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NuciXNA.Input
{
    /// <summary>
    /// Gamepad button event handler.
    /// </summary>
    public delegate void GamepadButtonEventHandler(object sender, GamepadButtonEventArgs e);

    /// <summary>
    /// Gamepad button event arguments.
    /// </summary>
    /// <param name="button">Button.</param>
    /// <param name="buttonState">Button state.</param>
    /// <param name="playerIndex">Player index.</param>
    public class GamepadButtonEventArgs(Buttons button, ButtonState buttonState, PlayerIndex playerIndex)
    {
        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>The button.</value>
        public Buttons Button { get; private set; } = button;

        /// <summary>
        /// Gets the state of the button.
        /// </summary>
        /// <value>The state of the button.</value>
        public ButtonState ButtonState { get; private set; } = buttonState;

        /// <summary>
        /// Gets the player index.
        /// </summary>
        /// <value>The player index.</value>
        public PlayerIndex PlayerIndex { get; private set; } = playerIndex;
    }
}
