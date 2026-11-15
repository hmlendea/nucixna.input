using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]
    public sealed class ButtonStateTests
    {
        [Test]
        public void GivenStateValues_WhenCheckingIsDown_ThenOnlyPressedAndHeldDownAreDown()
        {
            Assert.That(ButtonState.Idle.IsDown, Is.False);
            Assert.That(ButtonState.Pressed.IsDown);
            Assert.That(ButtonState.Released.IsDown, Is.False);
            Assert.That(ButtonState.HeldDown.IsDown);
        }

        [Test]
        public void GivenExistingIdentifier_WhenCallingFromId_ThenReturnsTheMatchingState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That(ButtonState.FromId(state.Id), Is.EqualTo(state));
            }
        }

        [Test]
        public void GivenInexistentIdentifier_WhenCallingFromId_ThenThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => ButtonState.FromId(873));

        [Test]
        public void GivenExistingName_WhenCallingFromName_ThenReturnsTheMatchingState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That(ButtonState.FromName(state.Name), Is.EqualTo(state));
            }
        }

        [Test]
        public void GivenInexistentName_WhenCallingFromName_ThenThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => ButtonState.FromName("Hori"));

        [Test]
        public void GivenState_WhenCallingToString_ThenReturnsItsName()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That(state.ToString(), Is.EqualTo(state.Name));
            }
        }

        [Test]
        public void GivenState_WhenCallingGetHashCode_ThenReturnsIdentifierHash()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That(state.GetHashCode(), Is.EqualTo(state.Id.GetHashCode()));
            }
        }

        [Test]
        public void GivenSameState_WhenCheckingEquality_ThenReturnsTrue()
            => Assert.That(ButtonState.Idle.Equals(ButtonState.Idle));

        [Test]
        public void GivenDifferentState_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals(ButtonState.HeldDown), Is.False);

        [Test]
        public void GivenSameStateAsObject_WhenCheckingEquality_ThenReturnsTrue()
            => Assert.That(ButtonState.Idle.Equals((object)ButtonState.Idle));

        [Test]
        public void GivenDifferentStateAsObject_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals((object)ButtonState.HeldDown), Is.False);

        [Test]
        public void GivenUnrelatedType_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals(DateTime.Now), Is.False);

        [Test]
        public void GivenNull_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals(null), Is.False);

        [Test]
        public void GivenSameState_WhenUsingEqualityOperator_ThenReturnsTrue()
            => Assert.That(ButtonState.Idle == ButtonState.Idle);

        [Test]
        public void GivenNullLeftOperand_WhenUsingEqualityOperator_ThenReturnsFalse()
            => Assert.That(null == ButtonState.Idle, Is.False);

        [Test]
        public void GivenNullRightOperand_WhenUsingEqualityOperator_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle == null, Is.False);

        [Test]
        public void GivenState_WhenCastingToInteger_ThenReturnsItsIdentifier()
        {
            ButtonState state = ButtonState.Idle;

            Assert.That((int)state, Is.EqualTo(state.Id));
        }

        [Test]
        public void GivenState_WhenCastingToString_ThenReturnsItsName()
        {
            ButtonState state = ButtonState.Idle;

            Assert.That((string)state, Is.EqualTo(state.Name));
        }

        [Test]
        public void GivenExistingIdentifier_WhenAssigningToState_ThenReturnsTheState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That((ButtonState)state.Id, Is.EqualTo(state));
            }
        }

        [Test]
        public void GivenInexistentIdentifier_WhenAssigningToState_ThenThrows()
        {
            ButtonState state;

            Assert.Throws<ArgumentException>(() => state = 873);
        }

        [Test]
        public void GivenExistingName_WhenAssigningToState_ThenReturnsTheState()
        {
            foreach (ButtonState state in ButtonState.GetValues())
            {
                Assert.That((ButtonState)state.Name, Is.EqualTo(state));
            }
        }

        [Test]
        public void GivenInexistentName_WhenAssigningToState_ThenThrows()
        {
            ButtonState state;

            Assert.Throws<ArgumentException>(() => state = "Hori");
        }

        [Test]
        public void GivenTwoNullStates_WhenUsingEqualityOperator_ThenReturnsTrue()
            => Assert.That((ButtonState)null == (ButtonState)null);

        [Test]
        public void GivenSameState_WhenUsingNotEqualsOperator_ThenReturnsFalse()
            => Assert.That(
                ButtonState.Idle != ButtonState.Idle,
                Is.False);

        [Test]
        public void GivenDifferentButtonStates_WhenUsingNotEqualsOperator_ThenReturnsTrue()
            => Assert.That(ButtonState.Idle != ButtonState.HeldDown);

        [Test]
        public void GivenNullAndState_WhenUsingNotEqualsOperator_ThenReturnsTrue()
            => Assert.That(null != ButtonState.Idle);

        [Test]
        public void GivenButtonState_WhenGetValuesIsCalled_ThenReturnsFourItems()
            => Assert.That(ButtonState.GetValues(), Has.Exactly(4).Items);

        [Test]
        public void GivenButtonState_WhenGetValuesIsCalled_ThenContainsAllStates()
        {
            IEnumerable<ButtonState> values = ButtonState.GetValues();

            Assert.That(values, Contains.Item(ButtonState.Idle));
            Assert.That(values, Contains.Item(ButtonState.Pressed));
            Assert.That(values, Contains.Item(ButtonState.Released));
            Assert.That(values, Contains.Item(ButtonState.HeldDown));
        }
    }
}
