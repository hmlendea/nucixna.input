using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XNAButtonState = Microsoft.Xna.Framework.Input.ButtonState;

using NuciXNA.Primitives;
using NuciXNA.Primitives.Mapping;

namespace NuciXNA.Input
{
    /// <summary>
    /// Input manager.
    /// </summary>
    public sealed class InputManager : IInputManager
    {
        private static readonly Lock instanceCreationLock = new();
        private static readonly Keys[] allKeys = Enum.GetValues<Keys>();
        private static readonly Buttons[] allButtons = Enum.GetValues<Buttons>();
        private static readonly PlayerIndex[] allPlayerIndices = Enum.GetValues<PlayerIndex>();

        private static volatile InputManager? instance;

        private GamePadState[] currentGamepadStates =
            new GamePadState[allPlayerIndices.Length];
        private GamePadState[] previousGamepadStates =
            new GamePadState[allPlayerIndices.Length];
        private KeyboardState currentKeyState;
        private KeyboardState previousKeyState;
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static InputManager Instance => GetInstance();

        /// <summary>
        /// Gets the current mouse location.
        /// </summary>
        /// <value>The current mouse location.</value>
        public Point2D MouseLocation
            => new(currentMouseState.Position.X, currentMouseState.Position.Y);

        /// <summary>
        /// Gets or sets whether mouse button input was handled.
        /// </summary>
        /// <value>Whether mouse button input was handled.</value>
        public bool MouseButtonInputHandled { get; set; }

        /// <summary>
        /// Occurs when a mouse button was pressed.
        /// </summary>
        public event MouseButtonEventHandler MouseButtonPressed;

        /// <summary>
        /// Occurs when a mouse button was released.
        /// </summary>
        public event MouseButtonEventHandler MouseButtonReleased;

        /// <summary>
        /// Occurs when a mouse button is down.
        /// </summary>
        public event MouseButtonEventHandler MouseButtonHeldDown;

        /// <summary>
        /// Occurs when the mouse moves.
        /// </summary>
        public event MouseEventHandler MouseMoved;

        /// <summary>
        /// Occurs when a gamepad button was pressed.
        /// </summary>
        public event GamepadButtonEventHandler GamepadButtonPressed;

        /// <summary>
        /// Occurs when a gamepad button was released.
        /// </summary>
        public event GamepadButtonEventHandler GamepadButtonReleased;

        /// <summary>
        /// Occurs when a gamepad button is down.
        /// </summary>
        public event GamepadButtonEventHandler GamepadButtonHeldDown;

        /// <summary>
        /// Occurs when a keyboard key was pressed.
        /// </summary>
        public event KeyboardKeyEventHandler KeyboardKeyPressed;

        /// <summary>
        /// Occurs when a keyboard key was released.
        /// </summary>
        public event KeyboardKeyEventHandler KeyboardKeyReleased;

        /// <summary>
        /// Occurs when a keyboard key is down.
        /// </summary>
        public event KeyboardKeyEventHandler KeyboardKeyHeldDown;

        /// <summary>
        /// Updates the input state.
        /// </summary>
        /// <param name="window">Game window.</param>
        public void Update(GameWindow window)
        {
            SwapStateBuffers();

            currentKeyState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            foreach (PlayerIndex playerIndex in allPlayerIndices)
            {
                currentGamepadStates[(int)playerIndex] = GamePad.GetState(playerIndex);
            }

            CorrectMouseCoordinates(window);

            CheckGamepadButtonStates();
            CheckKeyboardKeyStates();
            CheckMouseButtonStates();
            CheckMouseMoved();
        }

        /// <summary>
        /// Resets the input states.
        /// </summary>
        public void ResetInputStates()
        {
            SwapStateBuffers();

            currentKeyState = new KeyboardState();
            currentMouseState = new MouseState();
            Array.Clear(currentGamepadStates);
        }

        /// <summary>
        /// Checks if a gamepad button is down.
        /// </summary>
        /// <param name="playerIndex">Player index.</param>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if all buttons are down.</returns>
        public bool IsGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons)
            => buttons.All(GetGamepadState(playerIndex).IsButtonDown);

        /// <summary>
        /// Checks if any gamepad button is down.
        /// </summary>
        /// <param name="playerIndex">Player index.</param>
        /// <returns>True if any button is down.</returns>
        public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex)
            => IsAnyGamepadButtonDown(playerIndex, allButtons);

        /// <summary>
        /// Checks if any gamepad button is down.
        /// </summary>
        /// <param name="playerIndex">Player index.</param>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if any button is down.</returns>
        public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons)
            => IsAnyGamepadButtonDown(playerIndex, (IEnumerable<Buttons>)buttons);

        /// <summary>
        /// Checks if any gamepad button is down.
        /// </summary>
        /// <param name="playerIndex">Player index.</param>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if any button is down.</returns>
        public bool IsAnyGamepadButtonDown(
            PlayerIndex playerIndex,
            IEnumerable<Buttons> buttons)
            => buttons.Any(GetGamepadState(playerIndex).IsButtonDown);

        /// <summary>
        /// Checks if a key is down.
        /// </summary>
        /// <param name="keys">Keys to check.</param>
        /// <returns>True if all keys are down.</returns>
        public bool IsKeyDown(params Keys[] keys)
            => keys.All(currentKeyState.IsKeyDown);

        /// <summary>
        /// Checks if any key is down.
        /// </summary>
        /// <returns>True if any key is down.</returns>
        public bool IsAnyKeyDown()
            => IsAnyKeyDown(allKeys);

        /// <summary>
        /// Checks if any key is down.
        /// </summary>
        /// <param name="keys">Keys to check.</param>
        /// <returns>True if any key is down.</returns>
        public bool IsAnyKeyDown(params Keys[] keys)
            => IsAnyKeyDown((IEnumerable<Keys>)keys);

        /// <summary>
        /// Checks if any key is down.
        /// </summary>
        /// <param name="keys">Keys to check.</param>
        /// <returns>True if any key is down.</returns>
        public bool IsAnyKeyDown(IEnumerable<Keys> keys)
            => keys.Any(currentKeyState.IsKeyDown);

        /// <summary>
        /// Checks if a mouse button is down.
        /// </summary>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if all buttons are down.</returns>
        public bool IsMouseButtonDown(params MouseButton[] buttons)
            => buttons.All(button => GetMouseButtonState(button).IsDown);

        /// <summary>
        /// Checks if any mouse button is down.
        /// </summary>
        /// <returns>True if any button is down.</returns>
        public bool IsAnyMouseButtonDown()
            => IsAnyMouseButtonDown(MouseButton.GetValues());

        /// <summary>
        /// Checks if any mouse button is down.
        /// </summary>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if any button is down.</returns>
        public bool IsAnyMouseButtonDown(params MouseButton[] buttons)
            => IsAnyMouseButtonDown((IEnumerable<MouseButton>)buttons);

        /// <summary>
        /// Checks if any mouse button is down.
        /// </summary>
        /// <param name="buttons">Buttons to check.</param>
        /// <returns>True if any button is down.</returns>
        public bool IsAnyMouseButtonDown(IEnumerable<MouseButton> buttons)
            => buttons.Any(button => GetMouseButtonState(button).IsDown);

        /// <summary>
        /// Checks if the left mouse button was clicked.
        /// </summary>
        /// <returns>True if the left mouse button was clicked.</returns>
        public bool IsLeftMouseButtonClicked()
            => object.Equals(GetMouseButtonState(MouseButton.Left), ButtonState.Pressed);

        private static InputManager GetInstance()
        {
            InputManager? currentInstance = instance;

            if (currentInstance is not null)
            {
                return currentInstance;
            }

            lock (instanceCreationLock)
            {
                instance ??= new InputManager();

                return instance;
            }
        }

        private void SwapStateBuffers()
        {
            previousKeyState = currentKeyState;
            previousMouseState = currentMouseState;

            GamePadState[] swapBuffer = previousGamepadStates;
            previousGamepadStates = currentGamepadStates;
            currentGamepadStates = swapBuffer;
        }

        private void CorrectMouseCoordinates(GameWindow window)
        {
            int virtualDesktopX = currentMouseState.X;

            // On Linux/SDL2 with the window on a monitor to the LEFT of the primary,
            // Mouse.GetState() returns both X and Y in virtual-desktop coordinates
            // (negative when the left monitor sits at a negative virtual-desktop offset).
            // ClientBounds holds the client-area origin in the same coordinate space,
            // so adding it converts to client-relative coords for both axes.
            if (virtualDesktopX < 0)
            {
                currentMouseState = new MouseState(
                    virtualDesktopX + window.ClientBounds.X,
                    currentMouseState.Y + window.ClientBounds.Y,
                    currentMouseState.ScrollWheelValue,
                    currentMouseState.LeftButton,
                    currentMouseState.MiddleButton,
                    currentMouseState.RightButton,
                    currentMouseState.XButton1,
                    currentMouseState.XButton2);
            }
        }

        private GamePadState GetGamepadState(PlayerIndex playerIndex)
            => currentGamepadStates[(int)playerIndex];

        private void CheckGamepadButtonStates()
        {
            foreach (PlayerIndex playerIndex in allPlayerIndices)
            {
                foreach (Buttons button in allButtons)
                {
                    CheckGamepadButtonState(playerIndex, button);
                }
            }
        }

        private void CheckGamepadButtonState(PlayerIndex playerIndex, Buttons button)
        {
            GamePadState currentGamePadState = currentGamepadStates[(int)playerIndex];
            GamePadState previousGamePadState = previousGamepadStates[(int)playerIndex];
            bool isCurrentlyDown = currentGamePadState.IsButtonDown(button);
            bool wasPreviouslyDown = previousGamePadState.IsButtonDown(button);

            if (isCurrentlyDown)
            {
                NotifyGamepadButtonDown(playerIndex, button, wasPreviouslyDown);
            }
            else if (wasPreviouslyDown)
            {
                OnGamepadButtonReleased(
                    this,
                    new GamepadButtonEventArgs(
                        button,
                        ButtonState.Released,
                        playerIndex));
            }
        }

        private void NotifyGamepadButtonDown(
            PlayerIndex playerIndex,
            Buttons button,
            bool wasPreviouslyDown)
        {
            if (wasPreviouslyDown)
            {
                OnGamepadButtonHeldDown(
                    this,
                    new GamepadButtonEventArgs(
                        button,
                        ButtonState.HeldDown,
                        playerIndex));
            }
            else
            {
                OnGamepadButtonPressed(
                    this,
                    new GamepadButtonEventArgs(
                        button,
                        ButtonState.Pressed,
                        playerIndex));
            }
        }

        private void CheckKeyboardKeyStates()
        {
            Keys[] currentPressedKeys = currentKeyState.GetPressedKeys();
            Keys[] previousPressedKeys = previousKeyState.GetPressedKeys();

            foreach (Keys key in currentPressedKeys)
            {
                if (previousKeyState.IsKeyDown(key))
                {
                    OnKeyboardKeyHeldDown(
                        this,
                        new KeyboardKeyEventArgs(key, ButtonState.HeldDown));
                }
                else
                {
                    OnKeyboardKeyPressed(
                        this,
                        new KeyboardKeyEventArgs(key, ButtonState.Pressed));
                }
            }

            foreach (Keys key in previousPressedKeys)
            {
                if (!currentKeyState.IsKeyDown(key))
                {
                    OnKeyboardKeyReleased(
                        this,
                        new KeyboardKeyEventArgs(key, ButtonState.Released));
                }
            }
        }

        private void CheckMouseButtonStates()
        {
            Point2D cursorLocation = currentMouseState.Position.ToPoint2D();

            foreach (MouseButton button in MouseButton.GetValues())
            {
                ButtonState buttonState = GetMouseButtonState(button);

                MouseButtonEventArgs eventArgs = new(
                    button,
                    buttonState,
                    cursorLocation);

                if (object.Equals(buttonState, ButtonState.Pressed))
                {
                    OnMouseButtonPressed(this, eventArgs);
                }
                else if (object.Equals(buttonState, ButtonState.Released))
                {
                    OnMouseButtonReleased(this, eventArgs);
                }
                else if (object.Equals(buttonState, ButtonState.HeldDown))
                {
                    OnMouseButtonHeldDown(this, eventArgs);
                }
            }
        }

        private void CheckMouseMoved()
        {
            Point position = currentMouseState.Position;
            Point previousPosition = previousMouseState.Position;

            if (object.Equals(position, previousPosition))
            {
                return;
            }

            OnMouseMoved(
                this,
                new(
                    position.ToPoint2D(),
                    previousPosition.ToPoint2D()));
        }

        private ButtonState GetMouseButtonState(MouseButton button)
        {
            XNAButtonState currentButtonState = GetButtonState(button, currentMouseState);
            XNAButtonState previousButtonState = GetButtonState(button, previousMouseState);

            if (object.Equals(currentButtonState, XNAButtonState.Pressed))
            {
                if (object.Equals(previousButtonState, XNAButtonState.Pressed))
                {
                    return ButtonState.HeldDown;
                }

                return ButtonState.Pressed;
            }

            if (object.Equals(previousButtonState, XNAButtonState.Pressed))
            {
                return ButtonState.Released;
            }

            return ButtonState.Idle;
        }

        private static XNAButtonState GetButtonState(MouseButton button, MouseState mouseState)
        {
            if (object.Equals(button, MouseButton.Left))
            {
                return mouseState.LeftButton;
            }

            if (object.Equals(button, MouseButton.Right))
            {
                return mouseState.RightButton;
            }

            if (object.Equals(button, MouseButton.Middle))
            {
                return mouseState.MiddleButton;
            }

            if (object.Equals(button, MouseButton.Back))
            {
                return mouseState.XButton1;
            }

            if (object.Equals(button, MouseButton.Forward))
            {
                return mouseState.XButton2;
            }

            return XNAButtonState.Released;
        }

        /// <summary>
        /// Fires when a gamepad button was pressed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnGamepadButtonPressed(object sender, GamepadButtonEventArgs e)
            => GamepadButtonPressed?.Invoke(sender, e);

        /// <summary>
        /// Fires when a gamepad button was released.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnGamepadButtonReleased(object sender, GamepadButtonEventArgs e)
            => GamepadButtonReleased?.Invoke(sender, e);

        /// <summary>
        /// Fires when a gamepad button is held down.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnGamepadButtonHeldDown(object sender, GamepadButtonEventArgs e)
            => GamepadButtonHeldDown?.Invoke(sender, e);

        /// <summary>
        /// Fires when a keyboard key was pressed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnKeyboardKeyPressed(object sender, KeyboardKeyEventArgs e)
            => KeyboardKeyPressed?.Invoke(sender, e);

        /// <summary>
        /// Fires when a keyboard key was released.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnKeyboardKeyReleased(object sender, KeyboardKeyEventArgs e)
            => KeyboardKeyReleased?.Invoke(sender, e);

        /// <summary>
        /// Fires when a keyboard key is down.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnKeyboardKeyHeldDown(object sender, KeyboardKeyEventArgs e)
            => KeyboardKeyHeldDown?.Invoke(sender, e);

        /// <summary>
        /// Fires when a mouse button was pressed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
            => MouseButtonPressed?.Invoke(sender, e);

        /// <summary>
        /// Fires when a mouse button was released.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
            => MouseButtonReleased?.Invoke(sender, e);

        /// <summary>
        /// Fires when a mouse button is held down.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMouseButtonHeldDown(object sender, MouseButtonEventArgs e)
            => MouseButtonHeldDown?.Invoke(sender, e);

        /// <summary>
        /// Fires when the mouse was moved.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMouseMoved(object sender, MouseEventArgs e)
            => MouseMoved?.Invoke(sender, e);
    }
}
