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
    public class InputManager
    {
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

        GamePadState[] currentGamepadStates = new GamePadState[4];
        GamePadState[] previousGamepadStates = new GamePadState[4];
        KeyboardState currentKeyState, previousKeyState;
        MouseState currentMouseState, previousMouseState;

        static volatile InputManager instance;
        static readonly Lock syncRoot = new();
        static readonly Keys[] allKeys = Enum.GetValues<Keys>();
        static readonly Buttons[] allButtons = Enum.GetValues<Buttons>();
        static readonly PlayerIndex[] allPlayerIndices = Enum.GetValues<PlayerIndex>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance ??= new InputManager();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Updates the content.
        /// </summary>
        public void Update(GameWindow window)
        {
            previousKeyState = currentKeyState;
            previousMouseState = currentMouseState;

            GamePadState[] swapTemp = previousGamepadStates;
            previousGamepadStates = currentGamepadStates;
            currentGamepadStates = swapTemp;

            currentKeyState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            for (int i = 0; i < allPlayerIndices.Length; i++)
            {
                currentGamepadStates[i] = GamePad.GetState(allPlayerIndices[i]);
            }

            int rawX = currentMouseState.X;
            int rawY = currentMouseState.Y;

            // On Linux/SDL2 with the window on a monitor to the LEFT of the primary,
            // Mouse.GetState() returns both X and Y in virtual-desktop coordinates
            // (negative when the left monitor sits at a negative virtual-desktop offset).
            // ClientBounds holds the client-area origin in the same coordinate space,
            // so adding it converts to client-relative coords for both axes.
            if (rawX < 0)
            {
                currentMouseState = new MouseState(
                    rawX + window.ClientBounds.X,
                    rawY + window.ClientBounds.Y,
                    currentMouseState.ScrollWheelValue,
                    currentMouseState.LeftButton,
                    currentMouseState.MiddleButton,
                    currentMouseState.RightButton,
                    currentMouseState.XButton1,
                    currentMouseState.XButton2);
            }

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
            previousKeyState = currentKeyState;
            previousMouseState = currentMouseState;

            GamePadState[] swapTemp = previousGamepadStates;
            previousGamepadStates = currentGamepadStates;
            currentGamepadStates = swapTemp;

            currentKeyState = new KeyboardState();
            currentMouseState = new MouseState();
            Array.Clear(currentGamepadStates, 0, 4);
        }

        public bool IsGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons)
            => buttons.All(b => currentGamepadStates[(int)playerIndex].IsButtonDown(b));

        public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex)
            => IsAnyGamepadButtonDown(playerIndex, allButtons);

        public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, params Buttons[] buttons)
            => IsAnyGamepadButtonDown(playerIndex, buttons as IEnumerable<Buttons>);

        public bool IsAnyGamepadButtonDown(PlayerIndex playerIndex, IEnumerable<Buttons> buttons)
            => buttons.Any(b => currentGamepadStates[(int)playerIndex].IsButtonDown(b));

        public bool IsKeyDown(params Keys[] keys)
        {
            return keys.All(currentKeyState.IsKeyDown);
        }

        public bool IsAnyKeyDown()
            => IsAnyKeyDown(allKeys);

        public bool IsAnyKeyDown(params Keys[] keys)
            => IsAnyKeyDown(keys as IEnumerable<Keys>);

        public bool IsAnyKeyDown(IEnumerable<Keys> keys)
            => keys.Any(currentKeyState.IsKeyDown);

        public bool IsMouseButtonDown(params MouseButton[] buttons)
            => buttons
                .Select(GetMouseButtonState)
                .All(x => x.IsDown);

        public bool IsAnyMouseButtonDown()
            => IsAnyMouseButtonDown(MouseButton.GetValues());

        public bool IsAnyMouseButtonDown(params MouseButton[] buttons)
            => IsAnyMouseButtonDown(buttons as IEnumerable<MouseButton>);

        public bool IsAnyMouseButtonDown(IEnumerable<MouseButton> buttons)
            => buttons
                .Select(GetMouseButtonState)
                .Any(x => x.IsDown);

        void CheckGamepadButtonStates()
        {
            foreach (PlayerIndex playerIndex in allPlayerIndices)
            {
                foreach (Buttons button in allButtons)
                {
                    bool isCurrentlyDown = currentGamepadStates[(int)playerIndex].IsButtonDown(button);
                    bool wasPreviouslyDown = previousGamepadStates[(int)playerIndex].IsButtonDown(button);

                    if (isCurrentlyDown)
                    {
                        if (wasPreviouslyDown)
                        {
                            OnGamepadButtonHeldDown(this, new GamepadButtonEventArgs(button, ButtonState.HeldDown, playerIndex));
                        }
                        else
                        {
                            OnGamepadButtonPressed(this, new GamepadButtonEventArgs(button, ButtonState.Pressed, playerIndex));
                        }
                    }
                    else if (!isCurrentlyDown && wasPreviouslyDown)
                    {
                        OnGamepadButtonReleased(this, new GamepadButtonEventArgs(button, ButtonState.Released, playerIndex));
                    }
                }
            }
        }

        void CheckKeyboardKeyStates()
        {
            Keys[] currentPressedKeys = currentKeyState.GetPressedKeys();
            Keys[] previousPressedKeys = previousKeyState.GetPressedKeys();

            foreach (Keys key in currentPressedKeys)
            {
                if (previousKeyState.IsKeyDown(key))
                {
                    OnKeyboardKeyHeldDown(this, new KeyboardKeyEventArgs(key, ButtonState.HeldDown));
                }
                else
                {
                    OnKeyboardKeyPressed(this, new KeyboardKeyEventArgs(key, ButtonState.Pressed));
                }
            }

            foreach (Keys key in previousPressedKeys)
            {
                if (!currentKeyState.IsKeyDown(key))
                {
                    OnKeyboardKeyReleased(this, new KeyboardKeyEventArgs(key, ButtonState.Released));
                }
            }
        }

        void CheckMouseButtonStates()
        {
            Point2D cursorLocation = currentMouseState.Position.ToPoint2D();

            foreach (MouseButton button in MouseButton.GetValues())
            {
                ButtonState state = GetMouseButtonState(button);

                MouseButtonEventArgs eventArgs = new(
                    button,
                    state,
                    cursorLocation);

                if (state == ButtonState.Pressed)
                {
                    OnMouseButtonPressed(this, eventArgs);
                }
                else if (state == ButtonState.Released)
                {
                    OnMouseButtonReleased(this, eventArgs);
                }
                else if (state == ButtonState.HeldDown)
                {
                    OnMouseButtonHeldDown(this, eventArgs);
                }
            }
        }

        void CheckMouseMoved()
        {
            if (currentMouseState.Position.Equals(previousMouseState.Position))
            {
                return;
            }

            OnMouseMoved(this, new(
                currentMouseState.Position.ToPoint2D(),
                previousMouseState.Position.ToPoint2D()));
        }

        ButtonState GetMouseButtonState(MouseButton button)
        {
            XNAButtonState currentState = XNAButtonState.Released;
            XNAButtonState previousState = XNAButtonState.Released;

            if (button == MouseButton.Left)
            {
                currentState = currentMouseState.LeftButton;
                previousState = previousMouseState.LeftButton;
            }
            else if (button == MouseButton.Right)
            {
                currentState = currentMouseState.RightButton;
                previousState = previousMouseState.RightButton;
            }
            else if (button == MouseButton.Middle)
            {
                currentState = currentMouseState.MiddleButton;
                previousState = previousMouseState.MiddleButton;
            }
            else if (button == MouseButton.Back)
            {
                currentState = currentMouseState.XButton1;
                previousState = previousMouseState.XButton1;
            }
            else if (button == MouseButton.Forward)
            {
                currentState = currentMouseState.XButton2;
                previousState = previousMouseState.XButton2;
            }

            if (currentState == XNAButtonState.Pressed)
            {
                if (previousState == XNAButtonState.Pressed)
                {
                    return ButtonState.HeldDown;
                }

                return ButtonState.Pressed;
            }

            if (previousState == XNAButtonState.Pressed)
            {
                return ButtonState.Released;
            }

            return ButtonState.Idle;
        }

        /// <summary>
        /// Fires when a gamepad button was pressed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnGamepadButtonPressed(object sender, GamepadButtonEventArgs e)
            => GamepadButtonPressed?.Invoke(sender, e);

        /// <summary>
        /// Fires when a gamepad button was released.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnGamepadButtonReleased(object sender, GamepadButtonEventArgs e)
            => GamepadButtonReleased?.Invoke(sender, e);

        /// <summary>
        /// Fires when a gamepad button is held down.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnGamepadButtonHeldDown(object sender, GamepadButtonEventArgs e)
            => GamepadButtonHeldDown?.Invoke(sender, e);

        /// <summary>
        /// Fires when a keyboard key was pressed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnKeyboardKeyPressed(object sender, KeyboardKeyEventArgs e)
            => KeyboardKeyPressed?.Invoke(sender, e);

        /// <summary>
        /// Fires when a keyboard key was released.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnKeyboardKeyReleased(object sender, KeyboardKeyEventArgs e)
            => KeyboardKeyReleased?.Invoke(sender, e);

        /// <summary>
        /// Fires when a keyboard key is down.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnKeyboardKeyHeldDown(object sender, KeyboardKeyEventArgs e)
            => KeyboardKeyHeldDown?.Invoke(sender, e);

        /// <summary>
        /// Fires when a mouse button was pressed.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
            => MouseButtonPressed?.Invoke(sender, e);

        /// <summary>
        /// Fires when a mouse button was released.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
            => MouseButtonReleased?.Invoke(sender, e);

        /// <summary>
        /// Fires when a mouse button is held down.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnMouseButtonHeldDown(object sender, MouseButtonEventArgs e)
            => MouseButtonHeldDown?.Invoke(sender, e);

        /// <summary>
        /// Fires when the mouse was moved.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        void OnMouseMoved(object sender, MouseEventArgs e)
            => MouseMoved?.Invoke(sender, e);

        // TODO: Everything below this is required by a workaround to a problem and should be removed as soon as it is properly fixed

        public Point2D MouseLocation => new(currentMouseState.Position.X, currentMouseState.Position.Y);
        public bool MouseButtonInputHandled { get; set; }

        public bool IsLeftMouseButtonClicked()
            => GetMouseButtonState(MouseButton.Left).Equals(ButtonState.Pressed);
    }
}
