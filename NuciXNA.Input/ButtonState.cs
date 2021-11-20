using System;
using System.Collections.Generic;
using System.Linq;

namespace NuciXNA.Input
{
    /// <summary>
    /// Button state.
    /// </summary>
    public sealed class ButtonState : IEquatable<ButtonState>
    {
        static IDictionary<int, ButtonState> entries =
            new Dictionary<int, ButtonState>
            {
                { Idle.Id, Idle },
                { Pressed.Id, Pressed },
                { Released.Id, Released },
                { HeldDown.Id, HeldDown }
            };

        /// <summary>
        /// The mouse button is up.
        /// </summary>
        public static ButtonState Idle => new ButtonState(0, nameof(Idle), false);

        /// <summary>
        /// The mouse button was just pressed.
        /// </summary>
        public static ButtonState Pressed => new ButtonState(1, nameof(Pressed), true);

        /// <summary>
        /// They mouse button was just released.
        /// </summary>
        public static ButtonState Released => new ButtonState(2, nameof(Released), false);

        /// <summary>
        /// The mouse button is down.
        /// </summary>
        public static ButtonState HeldDown => new ButtonState(3, nameof(HeldDown), true);

        public int Id { get; }

        public string Name { get; }

        public bool IsDown { get; }

        private ButtonState(int id, string name, bool isDown)
        {
            Id = id;
            Name = name;
            IsDown = isDown;
        }

        public static ButtonState FromId(int id)
        {
            if (!entries.ContainsKey(id))
            {
                throw new ArgumentException($"A {nameof(ButtonState)} with the identifier \"{id}\" does not exist");
            }

            return entries[id];
        }

        public static ButtonState FromName(string name)
        {
            ButtonState button = entries.Values.FirstOrDefault(x => x.Name == name);

            if (button == null)
            {
                throw new ArgumentException($"A {nameof(ButtonState)} with the name \"{name}\" does not exist");
            }

            return button;
        }

        public override string ToString()
            => Name;

        public override int GetHashCode()
            => Id.GetHashCode();

        public static IEnumerable<ButtonState> GetValues()
            => entries.Values.ToList();

        public bool Equals(ButtonState other)
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
            
            return Equals(obj as ButtonState);
        }

        public static bool operator ==(ButtonState me, ButtonState other)
        {
            if (me is null)
            {
                return other is null;
            }

            return me.Equals(other);
        }

        public static bool operator !=(ButtonState me, ButtonState other)
            => !(me == other);

        public static implicit operator int(ButtonState me)
            => me.Id;

        public static implicit operator string(ButtonState me)
            => me.ToString();
        
        public static implicit operator ButtonState(int id)
            => ButtonState.FromId(id);

        public static implicit operator ButtonState(string name)
            => ButtonState.FromName(name);
    }
}
