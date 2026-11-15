using System;
using System.Collections.Generic;

namespace NuciXNA.Input
{
    /// <summary>
    /// Button state.
    /// </summary>
    public sealed class ButtonState : IEquatable<ButtonState>
    {
        /// <summary>
        /// The button is up.
        /// </summary>
        public static readonly ButtonState Idle = new(0, nameof(Idle), false);

        /// <summary>
        /// The button was just pressed.
        /// </summary>
        public static readonly ButtonState Pressed = new(1, nameof(Pressed), true);

        /// <summary>
        /// The button was just released.
        /// </summary>
        public static readonly ButtonState Released = new(2, nameof(Released), false);

        /// <summary>
        /// The button is held down.
        /// </summary>
        public static readonly ButtonState HeldDown = new(3, nameof(HeldDown), true);

        private static readonly Dictionary<int, ButtonState> entries = new()
        {
            { Idle.Id, Idle },
            { Pressed.Id, Pressed },
            { Released.Id, Released },
            { HeldDown.Id, HeldDown }
        };

        private static readonly ButtonState[] cachedValues =
            [Idle, Pressed, Released, HeldDown];

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the button is down.
        /// </summary>
        /// <value>Whether the button is down.</value>
        public bool IsDown { get; }

        private ButtonState(int id, string name, bool isDown)
        {
            Id = id;
            Name = name;
            IsDown = isDown;
        }

        /// <summary>
        /// Gets the state with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The matching state.</returns>
        public static ButtonState FromId(int id)
        {
            if (!entries.TryGetValue(id, out ButtonState? matchingState))
            {
                throw new ArgumentException(
                    $"A {nameof(ButtonState)} with the identifier \"{id}\" " +
                    "does not exist");
            }

            return matchingState;
        }

        /// <summary>
        /// Gets the state with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The matching state.</returns>
        public static ButtonState FromName(string name)
        {
            foreach (ButtonState buttonState in cachedValues)
            {
                if (string.Equals(buttonState.Name, name))
                {
                    return buttonState;
                }
            }

            throw new ArgumentException(
                $"A {nameof(ButtonState)} with the name \"{name}\" " +
                "does not exist");
        }

        /// <summary>
        /// Gets all the states.
        /// </summary>
        /// <returns>All the states.</returns>
        public static IEnumerable<ButtonState> GetValues() => cachedValues;

        /// <summary>
        /// Returns the name of the state.
        /// </summary>
        /// <returns>The name of the state.</returns>
        public override string ToString() => Name;

        /// <summary>
        /// Serves as a hash function for a <see cref="ButtonState"/> object.
        /// </summary>
        /// <returns>A hash code for the current state.</returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Determines whether the specified state is equal to the current state.
        /// </summary>
        /// <param name="other">The state to compare with the current state.</param>
        /// <returns>Whether the states are equal.</returns>
        public bool Equals(ButtonState? other)
        {
            if (other is null)
            {
                return false;
            }

            return int.Equals(other.Id, Id);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current state.
        /// </summary>
        /// <param name="obj">The object to compare with the current state.</param>
        /// <returns>Whether the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            return Equals(obj as ButtonState);
        }

        /// <summary>
        /// Determines whether two states are equal.
        /// </summary>
        /// <param name="me">The first state.</param>
        /// <param name="other">The second state.</param>
        /// <returns>Whether the states are equal.</returns>
        public static bool operator ==(ButtonState? me, ButtonState? other)
        {
            if (me is null)
            {
                return other is null;
            }

            return me.Equals(other);
        }

        /// <summary>
        /// Determines whether two states are different.
        /// </summary>
        /// <param name="me">The first state.</param>
        /// <param name="other">The second state.</param>
        /// <returns>Whether the states are different.</returns>
        public static bool operator !=(ButtonState? me, ButtonState? other)
            => !(me == other);

        /// <summary>
        /// Converts a state to its identifier.
        /// </summary>
        /// <param name="me">The state.</param>
        /// <returns>The identifier.</returns>
        public static implicit operator int(ButtonState me) => me.Id;

        /// <summary>
        /// Converts a state to its name.
        /// </summary>
        /// <param name="me">The state.</param>
        /// <returns>The name.</returns>
        public static implicit operator string(ButtonState me) => me.ToString();

        /// <summary>
        /// Converts an identifier to a state.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The state.</returns>
        public static implicit operator ButtonState(int id) => FromId(id);

        /// <summary>
        /// Converts a name to a state.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The state.</returns>
        public static implicit operator ButtonState(string name) => FromName(name);
    }
}
