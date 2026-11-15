using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]
    public sealed class MouseButtonTests
    {
        [Test]
        public void GivenExistingIdentifier_WhenCallingFromId_ThenReturnsTheMatchingButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(MouseButton.FromId(state.Id), Is.EqualTo(state));
            }
        }

        [Test]
        public void GivenInexistentIdentifier_WhenCallingFromId_ThenThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => MouseButton.FromId(873));

        [Test]
        public void GivenExistingName_WhenCallingFromName_ThenReturnsTheMatchingButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(MouseButton.FromName(state.Name), Is.EqualTo(state));
            }
        }

        [Test]
        public void GivenInexistentName_WhenCallingFromName_ThenThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => MouseButton.FromName("Hori"));

        [Test]
        public void GivenButton_WhenCallingToString_ThenReturnsItsName()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(state.ToString(), Is.EqualTo(state.Name));
            }
        }

        [Test]
        public void GivenButton_WhenCallingGetHashCode_ThenReturnsIdentifierHash()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(state.GetHashCode(), Is.EqualTo(state.Id.GetHashCode()));
            }
        }

        [Test]
        public void GivenSameButton_WhenCheckingEquality_ThenReturnsTrue()
            => Assert.That(MouseButton.Left.Equals(MouseButton.Left));

        [Test]
        public void GivenDifferentButton_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(MouseButton.Left.Equals(MouseButton.Right), Is.False);

        [Test]
        public void GivenSameButtonAsObject_WhenCheckingEquality_ThenReturnsTrue()
            => Assert.That(MouseButton.Left.Equals((object)MouseButton.Left));

        [Test]
        public void GivenDifferentButtonAsObject_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(MouseButton.Left.Equals((object)MouseButton.Right), Is.False);

        [Test]
        public void GivenUnrelatedType_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(MouseButton.Left.Equals(DateTime.Now), Is.False);

        [Test]
        public void GivenNull_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(MouseButton.Left.Equals(null), Is.False);

        [Test]
        public void GivenSameButton_WhenUsingEqualityOperator_ThenReturnsTrue()
            => Assert.That(MouseButton.Left == MouseButton.Left);

        [Test]
        public void GivenNullLeftOperand_WhenUsingEqualityOperator_ThenReturnsFalse()
        {
            Assert.That(null == MouseButton.Left, Is.False);
        }

        [Test]
        public void GivenNullRightOperand_WhenUsingEqualityOperator_ThenReturnsFalse()
            => Assert.That(MouseButton.Left == null, Is.False);

        [Test]
        public void GivenButton_WhenCastingToInteger_ThenReturnsItsIdentifier()
        {
            MouseButton state = MouseButton.Left;

            Assert.That((int)state, Is.EqualTo(state.Id));
        }

        [Test]
        public void GivenButton_WhenCastingToString_ThenReturnsItsName()
        {
            MouseButton state = MouseButton.Left;

            Assert.That((string)state, Is.EqualTo(state.Name));
        }

        [Test]
        public void GivenExistingIdentifier_WhenAssigningToButton_ThenReturnsTheButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That((MouseButton)state.Id, Is.EqualTo(state));
            }
        }

        [Test]
        public void GivenInexistentIdentifier_WhenAssigningToButton_ThenThrows()
        {
            MouseButton state;

            Assert.Throws<ArgumentException>(() => state = 873);
        }

        [Test]
        public void GivenExistingName_WhenAssigningToButton_ThenReturnsTheButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That((MouseButton)state.Name, Is.EqualTo(state));
            }
        }

        [Test]
        public void GivenInexistentName_WhenAssigningToButton_ThenThrows()
        {
            MouseButton state;

            Assert.Throws<ArgumentException>(() => state = "Hori");
        }

        [Test]
        public void GivenTwoNullButtons_WhenUsingEqualityOperator_ThenReturnsTrue()
            => Assert.That((MouseButton)null == (MouseButton)null);

        [Test]
        public void GivenSameButton_WhenUsingNotEqualsOperator_ThenReturnsFalse()
            => Assert.That(
                MouseButton.Left != MouseButton.Left,
                Is.False);

        [Test]
        public void GivenDifferentButtons_WhenUsingNotEqualsOperator_ThenReturnsTrue()
            => Assert.That(MouseButton.Left != MouseButton.Right);

        [Test]
        public void GivenNullAndButton_WhenUsingNotEqualsOperator_ThenReturnsTrue()
            => Assert.That(null != MouseButton.Left);

        [Test]
        public void GivenButton_WhenGetValuesIsCalled_ThenReturnsFiveItems()
            => Assert.That(MouseButton.GetValues(), Has.Exactly(5).Items);

        [Test]
        public void GivenButton_WhenGetValuesIsCalled_ThenContainsAllButtons()
        {
            IEnumerable<MouseButton> values = MouseButton.GetValues();

            Assert.That(values, Contains.Item(MouseButton.Left));
            Assert.That(values, Contains.Item(MouseButton.Right));
            Assert.That(values, Contains.Item(MouseButton.Middle));
            Assert.That(values, Contains.Item(MouseButton.Back));
            Assert.That(values, Contains.Item(MouseButton.Forward));
        }
    }
}
