using System;
using System.Collections.Generic;

namespace NuciXNA.Input
{
    /// <summary>
    /// Button state.
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

        static readonly Dictionary<int, MouseButton> entries = new()
        {
            { Left.Id, Left },
            { Right.Id, Right },
            { Middle.Id, Middle },
            { Back.Id, Back },
            { Forward.Id, Forward }
        };

        static readonly MouseButton[] cachedValues = [Left, Right, Middle, Back, Forward];

        public int Id { get; }

        public string Name { get; }

        private MouseButton(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static MouseButton FromId(int id)
        {
            if (!entries.TryGetValue(id, out MouseButton value))
            {
                throw new ArgumentException($"A {nameof(MouseButton)} with the identifier \"{id}\" does not exist");
            }

            return value;
        }

        public static MouseButton FromName(string name)
        {
            foreach (MouseButton button in cachedValues)
            {
                if (button.Name.Equals(name))
                {
                    return button;
                }
            }

            throw new ArgumentException($"A {nameof(MouseButton)} with the name \"{name}\" does not exist");
        }

        public override string ToString() => Name;

        public override int GetHashCode() => Id.GetHashCode();

        public bool Equals(MouseButton other)
        {
            if (other is null)
            {
                return false;
            }

            if (!other.Id.Equals(Id))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return Equals(obj as MouseButton);
        }

        public static bool operator ==(MouseButton me, MouseButton other)
        {
            if (me is null)
            {
                return other is null;
            }

            return me.Equals(other);
        }

        public static bool operator !=(MouseButton me, MouseButton other) => !(me == other);

        public static IEnumerable<MouseButton> GetValues() => cachedValues;

        public static implicit operator int(MouseButton me) => me.Id;

        public static implicit operator string(MouseButton me) => me.ToString();

        public static implicit operator MouseButton(int id) => FromId(id);

        public static implicit operator MouseButton(string name) => FromName(name);
    }
}
