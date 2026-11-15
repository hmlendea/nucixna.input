using System;
using System.Collections.Generic;

namespace NuciXNA.Input
{
    /// <summary>
    /// Mouse button.
    /// </summary>
    public sealed class MouseButton : IEquatable<MouseButton>
    {
        /// <summary>
        /// The left mouse button.
        /// </summary>
        public static readonly MouseButton Left = new(1, nameof(Left));

        /// <summary>
        /// The right mouse button.
        /// </summary>
        public static readonly MouseButton Right = new(2, nameof(Right));

        /// <summary>
        /// The middle mouse button.
        /// </summary>
        public static readonly MouseButton Middle = new(3, nameof(Middle));

        /// <summary>
        /// The back mouse button.
        /// </summary>
        public static readonly MouseButton Back = new(4, nameof(Back));

        /// <summary>
        /// The forward mouse button.
        /// </summary>
        public static readonly MouseButton Forward = new(5, nameof(Forward));

        private static readonly Dictionary<int, MouseButton> entries = new()
        {
            { Left.Id, Left },
            { Right.Id, Right },
            { Middle.Id, Middle },
            { Back.Id, Back },
            { Forward.Id, Forward }
        };

        private static readonly MouseButton[] cachedValues =
            [Left, Right, Middle, Back, Forward];

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

        private MouseButton(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets the mouse button with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The matching mouse button.</returns>
        public static MouseButton FromId(int id)
        {
            if (!entries.TryGetValue(id, out MouseButton? matchingButton))
            {
                throw new ArgumentException(
                    $"A {nameof(MouseButton)} with the identifier \"{id}\" " +
                    "does not exist");
            }

            return matchingButton;
        }

        /// <summary>
        /// Gets the mouse button with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The matching mouse button.</returns>
        public static MouseButton FromName(string name)
        {
            foreach (MouseButton mouseButton in cachedValues)
            {
                if (string.Equals(mouseButton.Name, name))
                {
                    return mouseButton;
                }
            }

            throw new ArgumentException(
                $"A {nameof(MouseButton)} with the name \"{name}\" " +
                "does not exist");
        }

        /// <summary>
        /// Gets all the mouse buttons.
        /// </summary>
        /// <returns>All the mouse buttons.</returns>
        public static IEnumerable<MouseButton> GetValues() => cachedValues;

        /// <summary>
        /// Returns the name of the mouse button.
        /// </summary>
        /// <returns>The name of the mouse button.</returns>
        public override string ToString() => Name;

        /// <summary>
        /// Serves as a hash function for a <see cref="MouseButton"/> object.
        /// </summary>
        /// <returns>A hash code for the current mouse button.</returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Determines whether the specified button is equal to the current button.
        /// </summary>
        /// <param name="other">The button to compare with the current button.</param>
        /// <returns>Whether the buttons are equal.</returns>
        public bool Equals(MouseButton? other)
        {
            if (other is null)
            {
                return false;
            }

            return int.Equals(other.Id, Id);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current button.
        /// </summary>
        /// <param name="obj">The object to compare with the current button.</param>
        /// <returns>Whether the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            return Equals(obj as MouseButton);
        }

        /// <summary>
        /// Determines whether two mouse buttons are equal.
        /// </summary>
        /// <param name="me">The first mouse button.</param>
        /// <param name="other">The second mouse button.</param>
        /// <returns>Whether the mouse buttons are equal.</returns>
        public static bool operator ==(MouseButton? me, MouseButton? other)
        {
            if (me is null)
            {
                return other is null;
            }

            return me.Equals(other);
        }

        /// <summary>
        /// Determines whether two mouse buttons are different.
        /// </summary>
        /// <param name="me">The first mouse button.</param>
        /// <param name="other">The second mouse button.</param>
        /// <returns>Whether the mouse buttons are different.</returns>
        public static bool operator !=(MouseButton? me, MouseButton? other)
            => !(me == other);

        /// <summary>
        /// Converts a mouse button to its identifier.
        /// </summary>
        /// <param name="me">The mouse button.</param>
        /// <returns>The identifier.</returns>
        public static implicit operator int(MouseButton me) => me.Id;

        /// <summary>
        /// Converts a mouse button to its name.
        /// </summary>
        /// <param name="me">The mouse button.</param>
        /// <returns>The name.</returns>
        public static implicit operator string(MouseButton me) => me.ToString();

        /// <summary>
        /// Converts an identifier to a mouse button.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The mouse button.</returns>
        public static implicit operator MouseButton(int id) => FromId(id);

        /// <summary>
        /// Converts a name to a mouse button.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The mouse button.</returns>
        public static implicit operator MouseButton(string name) => FromName(name);
    }
}
