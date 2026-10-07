using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NuciXNA.Input
{
    /// <summary>
    /// Input manager interface.
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        /// Occurs when a mouse button was pressed.
        /// </summary>
        event MouseButtonEventHandler MouseButtonPressed;

        /// <summary>
        /// Occurs when a mouse button was released.
        /// </summary>
        event MouseButtonEventHandler MouseButtonReleased;

        /// <summary>
        /// Occurs when a mouse button is down.
        /// </summary>
        event MouseButtonEventHandler MouseButtonHeldDown;

        /// <summary>
        /// Occurs when the mouse moves.
        /// </summary>
        event MouseEventHandler MouseMoved;

        /// <summary>
        /// Occurs when a gamepad button was pressed.
        /// </summary>
        event GamepadButtonEventHandler GamepadButtonPressed;

        /// <summary>
        /// Occurs when a gamepad button was released.
        /// </summary>
        event GamepadButtonEventHandler GamepadButtonReleased;

        /// <summary>
        /// Occurs when a gamepad button is down.
        /// </summary>
        event GamepadButtonEventHandler GamepadButtonHeldDown;

        /// <summary>
        /// Occurs when a keyboard key was pressed.
        /// </summary>
        event KeyboardKeyEventHandler KeyboardKeyPressed;

        /// <summary>
        /// Occurs when a keyboard key was released.
        /// </summary>
        event KeyboardKeyEventHandler KeyboardKeyReleased;

        /// <summary>
        /// Occurs when a keyboard key is down.
        /// </summary>
        event KeyboardKeyEventHandler KeyboardKeyHeldDown;

        /// <summary>
        /// Updates the input state.
        /// </summary>
        /// <param name="window">Game window.</param>
        void Update(GameWindow window);

        /// <summary>
        /// Resets the input states.
        /// </summary>
        void ResetInputStates();

        /// <summary>
        /// Checks if a gamepad button is down.
        /// </summary>
        /// <param name="playerIndex">Player index.</param>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if all buttons are down.</returns>
        bool IsGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons);

        /// <summary>
        /// Checks if any gamepad button is down.
        /// </summary>
        /// <param name="playerIndex">Player index.</param>
        /// <returns>True if any button is down.</returns>
        bool IsAnyGamepadButtonDown(PlayerIndex playerIndex);

        /// <summary>
        /// Checks if any gamepad button is down.
        /// </summary>
        /// <param name="playerIndex">Player index.</param>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if any button is down.</returns>
        bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons);

        /// <summary>
        /// Checks if any gamepad button is down.
        /// </summary>
        /// <param name="playerIndex">Player index.</param>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if any button is down.</returns>
        bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, IEnumerable<Buttons> buttons);

        /// <summary>
        /// Checks if a key is down.
        /// </summary>
        /// <param name="keys">Keys to check.</param>
        /// <returns>True if all keys are down.</returns>
        bool IsKeyDown(params Keys[] keys);

        /// <summary>
        /// Checks if any key is down.
        /// </summary>
        /// <returns>True if any key is down.</returns>
        bool IsAnyKeyDown();

        /// <summary>
        /// Checks if any key is down.
        /// </summary>
        /// <param name="keys">Keys to check.</param>
        /// <returns>True if any key is down.</returns>
        bool IsAnyKeyDown(params Keys[] keys);

        /// <summary>
        /// Checks if any key is down.
        /// </summary>
        /// <param name="keys">Keys to check.</param>
        /// <returns>True if any key is down.</returns>
        bool IsAnyKeyDown(IEnumerable<Keys> keys);

        /// <summary>
        /// Checks if a mouse button is down.
        /// </summary>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if all buttons are down.</returns>
        bool IsMouseButtonDown(params MouseButton[] buttons);

        /// <summary>
        /// Checks if any mouse button is down.
        /// </summary>
        /// <returns>True if any button is down.</returns>
        bool IsAnyMouseButtonDown();

        /// <summary>
        /// Checks if any mouse button is down.
        /// </summary>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if any button is down.</returns>
        bool IsAnyMouseButtonDown(params MouseButton[] buttons);

        /// <summary>
        /// Checks if any mouse button is down.
        /// </summary>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if any button is down.</returns>
        bool IsAnyMouseButtonDown(IEnumerable<MouseButton> buttons);
    }
}