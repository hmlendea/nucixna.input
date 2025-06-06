using System;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    public class ButtonStateTests
    {
        [Test]
        public void IsDown_ValueIsCorrect()
        {
            Assert.That(ButtonState.Idle.IsDown, Is.False);
            Assert.That(ButtonState.Pressed.IsDown);
            Assert.That(ButtonState.Released.IsDown, Is.False);
            Assert.That(ButtonState.HeldDown.IsDown);
        }

        [Test]
        public void FromId_CalledWithExistingId_ReturnsCorrectButtonState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That(ButtonState.FromId(state.Id), Is.EqualTo(state));
            }
        }

        [Test]
        public void FromId_CalledWithInexistentId_ThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => ButtonState.FromId(873));

        [Test]
        public void FromName_CalledWithExistingName_ReturnsCorrectButtonState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That(ButtonState.FromName(state.Name), Is.EqualTo(state));
            }
        }

        [Test]
        public void FromName_CalledWithInexistentName_ThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => ButtonState.FromName("Hori"));

        [Test]
        public void ToString_ReturnsCorrectValue()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That(state.ToString(), Is.EqualTo(state.Name));
            }
        }

        [Test]
        public void GetHashCode_ReturnsCorrectValue()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That(state.GetHashCode(), Is.EqualTo(state.Id.GetHashCode()));
            }
        }

        [Test]
        public void Equals_CalledWithSameButtonState_ReturnsTrue()
            => Assert.That(ButtonState.Idle.Equals(ButtonState.Idle));

        [Test]
        public void Equals_CalledWithOtherButtonState_ReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals(ButtonState.HeldDown), Is.False);

        [Test]
        public void Equals_CalledWithSameButtonStateAsObject_ReturnsTrue()
            => Assert.That(ButtonState.Idle.Equals((object)ButtonState.Idle));

        [Test]
        public void Equals_CalledWithOtherButtonStateAsObject_ReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals((object)ButtonState.HeldDown), Is.False);

        [Test]
        public void Equals_CalledWithOtherType_ReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals(DateTime.Now), Is.False);

        [Test]
        public void Equals_CalledWithNull_ReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals(null), Is.False);

        [Test]
        public void EqualsOperator_OtherIsSameButtonState_ReturnsTrue()
            => Assert.That(ButtonState.Idle == ButtonState.Idle);

        [Test]
        public void EqualsOperator_CurrentIsNull_ReturnsFalse()
            => Assert.That(null == ButtonState.Idle, Is.False);

        [Test]
        public void EqualsOperator_OtherIsNull_ReturnsFalse()
            => Assert.That(ButtonState.Idle == null, Is.False);

        [Test]
        public void CastAsInt_ReturnsCorrectValue()
        {
            ButtonState state = ButtonState.Idle;

            Assert.That((int)state, Is.EqualTo(state.Id));
        }

        [Test]
        public void CastAsString_ReturnsCorrectValue()
        {
            ButtonState state = ButtonState.Idle;

            Assert.That((string)state, Is.EqualTo(state.Name));
        }

        [Test]
        public void AssignInteger_AssignedExistingId_ReturnsCorrectButtonState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That((ButtonState)state.Id, Is.EqualTo(state));
            }
        }

        [Test]
        public void AssignInteger_AssignedInexistentId_ThrowsArgumentException()
        {
            ButtonState state;

            Assert.Throws<ArgumentException>(() => state = 873);
        }

        [Test]
        public void AssignString_AssignedExistingName_ReturnsCorrectButtonState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That((ButtonState)state.Name, Is.EqualTo(state));
            }
        }

        [Test]
        public void AssignString_AssignedInexistentName_ThrowsArgumentException()
        {
            ButtonState state;

            Assert.Throws<ArgumentException>(() => state = "Hori");
        }
    }
}
