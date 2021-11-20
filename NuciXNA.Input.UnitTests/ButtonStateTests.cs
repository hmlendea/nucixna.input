using System;

using NUnit.Framework;

using NuciXNA.Input;

namespace NuciXNA.UnitTests.Input
{
    public class ButtonStateTests
    {
        [Test]
        public void IsDown_ValueIsCorrect()
        {
            Assert.AreEqual(false, ButtonState.Idle.IsDown);
            Assert.AreEqual(true, ButtonState.Pressed.IsDown);
            Assert.AreEqual(false, ButtonState.Released.IsDown);
            Assert.AreEqual(true, ButtonState.HeldDown.IsDown);
        }

        [Test]
        public void FromId_CalledWithExistingId_ReturnsCorrectButtonState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.AreEqual(state, ButtonState.FromId(state.Id));
            }
        }

        [Test]
        public void FromId_CalledWithInexistentId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ButtonState.FromId(873));
        }

        [Test]
        public void FromName_CalledWithExistingName_ReturnsCorrectButtonState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.AreEqual(state, ButtonState.FromName(state.Name));
            }
        }

        [Test]
        public void FromName_CalledWithInexistentName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ButtonState.FromName("Hori"));
        }

        [Test]
        public void ToString_ReturnsCorrectValue()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.AreEqual(state.Name, state.ToString());
            }
        }

        [Test]
        public void GetHashCode_ReturnsCorrectValue()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.AreEqual(state.Id.GetHashCode(), state.GetHashCode());
            }
        }

        [Test]
        public void Equals_CalledWithSameButtonState_ReturnsTrue()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsTrue(state.Equals(ButtonState.Idle));
        }

        [Test]
        public void Equals_CalledWithOtherButtonState_ReturnsFalse()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsFalse(state.Equals(ButtonState.HeldDown));
        }

        [Test]
        public void Equals_CalledWithSameButtonStateAsObject_ReturnsTrue()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsTrue(state.Equals((object)ButtonState.Idle));
        }

        [Test]
        public void Equals_CalledWithOtherButtonStateAsObject_ReturnsFalse()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsFalse(state.Equals((object)ButtonState.HeldDown));
        }

        [Test]
        public void Equals_CalledWithOtherType_ReturnsFalse()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsFalse(state.Equals(DateTime.Now));
        }

        [Test]
        public void Equals_CalledWithNull_ReturnsFalse()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsFalse(state.Equals(null));
        }

        [Test]
        public void EqualsOperator_OtherIsSameButtonState_ReturnsTrue()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsTrue(state == ButtonState.Idle);
        }

        [Test]
        public void EqualsOperator_CurrentIsNull_ReturnsFalse()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsFalse(null == ButtonState.Idle);
        }

        [Test]
        public void EqualsOperator_OtherIsNull_ReturnsFalse()
        {
            ButtonState state = ButtonState.Idle;

            Assert.IsFalse(state == null);
        }

        [Test]
        public void CastAsInt_ReturnsCorrectValue()
        {
            ButtonState state = ButtonState.Idle;

            Assert.AreEqual(state.Id, (int)state);
        }

        [Test]
        public void CastAsString_ReturnsCorrectValue()
        {
            ButtonState state = ButtonState.Idle;

            Assert.AreEqual(state.Name, (string)state);
        }

        [Test]
        public void AssignInteger_AssignedExistingId_ReturnsCorrectButtonState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                ButtonState state2 = state.Id;
            
                Assert.AreEqual(state, state2);
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
                ButtonState state2 = state.Name;
            
                Assert.AreEqual(state, state2);
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
