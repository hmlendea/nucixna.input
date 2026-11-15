using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]
    public sealed class ButtonStateTests
    {
        [Test]
        public static void GivenStateValues_WhenCheckingIsDown_ThenOnlyTwoAreDown()
        {
            Assert.That(ButtonState.Idle.IsDown, Is.False);
            Assert.That(ButtonState.Pressed.IsDown);
            Assert.That(ButtonState.Released.IsDown, Is.False);
            Assert.That(ButtonState.HeldDown.IsDown);
        }

        [Test]
        public static void GivenExistingIdentifier_WhenCallingFromId_ThenReturnsTheState()
        {
            foreach (ButtonState buttonState in ButtonState.GetValues())
            {
                Assert.That(ButtonState.FromId(buttonState.Id), Is.EqualTo(buttonState));
            }
        }

        [Test]
        public static void GivenInexistentIdentifier_WhenCallingFromId_ThenThrows()
            => Assert.Throws<ArgumentException>(() => ButtonState.FromId(873));

        [Test]
        public static void GivenExistingName_WhenCallingFromName_ThenReturnsTheMatchingState()
        {
            foreach (ButtonState buttonState in ButtonState.GetValues())
            {
                Assert.That(ButtonState.FromName(buttonState.Name), Is.EqualTo(buttonState));
            }
        }

        [Test]
        public static void GivenInexistentName_WhenCallingFromName_ThenThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => ButtonState.FromName("Testy McTestface"));

        [Test]
        public static void GivenState_WhenCallingToString_ThenReturnsItsName()
        {
            foreach (ButtonState buttonState in ButtonState.GetValues())
            {
                Assert.That(buttonState.ToString(), Is.EqualTo(buttonState.Name));
            }
        }

        [Test]
        public static void GivenState_WhenCallingGetHashCode_ThenReturnsIdentifierHash()
        {
            foreach (ButtonState buttonState in ButtonState.GetValues())
            {
                Assert.That(
                    buttonState.GetHashCode(),
                    Is.EqualTo(buttonState.Id.GetHashCode()));
            }
        }

        [Test]
        public static void GivenSameState_WhenCheckingEquality_ThenReturnsTrue()
            => Assert.That(ButtonState.Idle.Equals(ButtonState.Idle));

        [Test]
        public static void GivenDifferentState_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals(ButtonState.HeldDown), Is.False);

        [Test]
        public static void GivenSameStateAsObject_WhenCheckingEquality_ThenReturnsTrue()
            => Assert.That(ButtonState.Idle.Equals((object)ButtonState.Idle));

        [Test]
        public static void GivenDifferentStateAsObject_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals((object)ButtonState.HeldDown), Is.False);

        [Test]
        public static void GivenUnrelatedType_WhenCheckingEquality_ThenReturnsFalse()
        {
            object unrelatedValue = new();

            Assert.That(ButtonState.Idle.Equals(unrelatedValue), Is.False);
        }

        [Test]
        public static void GivenNull_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle.Equals(null), Is.False);

        [Test]
        public static void GivenSameState_WhenUsingEqualityOperator_ThenReturnsTrue()
            => Assert.That(ButtonState.Idle == ButtonState.Idle);

        [Test]
        public static void GivenNullLeftOperand_WhenUsingEqualityOperator_ThenReturnsFalse()
            => Assert.That(null == ButtonState.Idle, Is.False);

        [Test]
        public static void GivenNullRightOperand_WhenUsingEqualityOperator_ThenReturnsFalse()
            => Assert.That(ButtonState.Idle == null, Is.False);

        [Test]
        public static void GivenState_WhenCastingToInteger_ThenReturnsItsIdentifier()
        {
            ButtonState buttonState = ButtonState.Idle;

            Assert.That((int)buttonState, Is.EqualTo(buttonState.Id));
        }

        [Test]
        public static void GivenState_WhenCastingToString_ThenReturnsItsName()
        {
            ButtonState buttonState = ButtonState.Idle;

            Assert.That((string)buttonState, Is.EqualTo(buttonState.Name));
        }

        [Test]
        public static void GivenExistingIdentifier_WhenAssigningToState_ThenReturnsTheState()
        {
            foreach (ButtonState buttonState in ButtonState.GetValues())
            {
                Assert.That((ButtonState)buttonState.Id, Is.EqualTo(buttonState));
            }
        }

        [Test]
        public static void GivenInexistentIdentifier_WhenAssigningToState_ThenThrows()
        {
            ButtonState buttonState;

            Assert.Throws<ArgumentException>(() => buttonState = 873);
        }

        [Test]
        public static void GivenExistingName_WhenAssigningToState_ThenReturnsTheState()
        {
            foreach (ButtonState buttonState in ButtonState.GetValues())
            {
                Assert.That((ButtonState)buttonState.Name, Is.EqualTo(buttonState));
            }
        }

        [Test]
        public static void GivenInexistentName_WhenAssigningToState_ThenThrows()
        {
            ButtonState buttonState;

            Assert.Throws<ArgumentException>(() => buttonState = "Testy McTestface");
        }

        [Test]
        public static void GivenTwoNullStates_WhenUsingEqualityOperator_ThenReturnsTrue()
            => Assert.That((ButtonState)null == (ButtonState)null);

        [Test]
        public static void GivenSameState_WhenUsingNotEqualsOperator_ThenReturnsFalse()
            => Assert.That(
                ButtonState.Idle != ButtonState.Idle,
                Is.False);

        [Test]
        public static void GivenDifferentStates_WhenUsingNotEqualsOperator_ThenReturnsTrue()
            => Assert.That(ButtonState.Idle != ButtonState.HeldDown);

        [Test]
        public static void GivenNullAndState_WhenUsingNotEqualsOperator_ThenReturnsTrue()
            => Assert.That(null != ButtonState.Idle);

        [Test]
        public static void GivenButtonState_WhenGetValuesIsCalled_ThenReturnsFourItems()
            => Assert.That(ButtonState.GetValues(), Has.Exactly(4).Items);

        [Test]
        public static void GivenButtonState_WhenGetValuesIsCalled_ThenContainsAllStates()
        {
            IEnumerable<ButtonState> buttonStates = ButtonState.GetValues();

            Assert.That(buttonStates, Contains.Item(ButtonState.Idle));
            Assert.That(buttonStates, Contains.Item(ButtonState.Pressed));
            Assert.That(buttonStates, Contains.Item(ButtonState.Released));
            Assert.That(buttonStates, Contains.Item(ButtonState.HeldDown));
        }
    }
}
