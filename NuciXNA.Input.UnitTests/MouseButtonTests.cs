using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    [TestFixture]
    public sealed class MouseButtonTests
    {
        [Test]
        public static void GivenExistingIdentifier_WhenCallingFromId_ThenReturnsTheButton()
        {
            foreach (MouseButton mouseButton in MouseButton.GetValues())
            {
                Assert.That(MouseButton.FromId(mouseButton.Id), Is.EqualTo(mouseButton));
            }
        }

        [Test]
        public static void GivenInexistentIdentifier_WhenCallingFromId_ThenThrows()
            => Assert.Throws<ArgumentException>(() => MouseButton.FromId(873));

        [Test]
        public static void GivenExistingName_WhenCallingFromName_ThenReturnsTheMatchingButton()
        {
            foreach (MouseButton mouseButton in MouseButton.GetValues())
            {
                Assert.That(MouseButton.FromName(mouseButton.Name), Is.EqualTo(mouseButton));
            }
        }

        [Test]
        public static void GivenInexistentName_WhenCallingFromName_ThenThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => MouseButton.FromName("Testy McTestface"));

        [Test]
        public static void GivenButton_WhenCallingToString_ThenReturnsItsName()
        {
            foreach (MouseButton mouseButton in MouseButton.GetValues())
            {
                Assert.That(mouseButton.ToString(), Is.EqualTo(mouseButton.Name));
            }
        }

        [Test]
        public static void GivenButton_WhenCallingGetHashCode_ThenReturnsIdentifierHash()
        {
            foreach (MouseButton mouseButton in MouseButton.GetValues())
            {
                Assert.That(
                    mouseButton.GetHashCode(),
                    Is.EqualTo(mouseButton.Id.GetHashCode()));
            }
        }

        [Test]
        public static void GivenSameButton_WhenCheckingEquality_ThenReturnsTrue()
            => Assert.That(MouseButton.Left.Equals(MouseButton.Left));

        [Test]
        public static void GivenDifferentButton_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(MouseButton.Left.Equals(MouseButton.Right), Is.False);

        [Test]
        public static void GivenSameButtonAsObject_WhenCheckingEquality_ThenReturnsTrue()
            => Assert.That(MouseButton.Left.Equals((object)MouseButton.Left));

        [Test]
        public static void GivenDifferentButtonAsObject_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(MouseButton.Left.Equals((object)MouseButton.Right), Is.False);

        [Test]
        public static void GivenUnrelatedType_WhenCheckingEquality_ThenReturnsFalse()
        {
            object unrelatedValue = new();

            Assert.That(MouseButton.Left.Equals(unrelatedValue), Is.False);
        }

        [Test]
        public static void GivenNull_WhenCheckingEquality_ThenReturnsFalse()
            => Assert.That(MouseButton.Left.Equals(null), Is.False);

        [Test]
        public static void GivenSameButton_WhenUsingEqualityOperator_ThenReturnsTrue()
            => Assert.That(MouseButton.Left == MouseButton.Left);

        [Test]
        public static void GivenNullLeftOperand_WhenUsingEqualityOperator_ThenReturnsFalse()
        {
            Assert.That(null == MouseButton.Left, Is.False);
        }

        [Test]
        public static void GivenNullRightOperand_WhenUsingEqualityOperator_ThenReturnsFalse()
            => Assert.That(MouseButton.Left == null, Is.False);

        [Test]
        public static void GivenButton_WhenCastingToInteger_ThenReturnsItsIdentifier()
        {
            MouseButton mouseButton = MouseButton.Left;

            Assert.That((int)mouseButton, Is.EqualTo(mouseButton.Id));
        }

        [Test]
        public static void GivenButton_WhenCastingToString_ThenReturnsItsName()
        {
            MouseButton mouseButton = MouseButton.Left;

            Assert.That((string)mouseButton, Is.EqualTo(mouseButton.Name));
        }

        [Test]
        public static void GivenExistingIdentifier_WhenAssigningToButton_ThenReturnsTheButton()
        {
            foreach (MouseButton mouseButton in MouseButton.GetValues())
            {
                Assert.That((MouseButton)mouseButton.Id, Is.EqualTo(mouseButton));
            }
        }

        [Test]
        public static void GivenInexistentIdentifier_WhenAssigningToButton_ThenThrows()
        {
            MouseButton mouseButton;

            Assert.Throws<ArgumentException>(() => mouseButton = 873);
        }

        [Test]
        public static void GivenExistingName_WhenAssigningToButton_ThenReturnsTheButton()
        {
            foreach (MouseButton mouseButton in MouseButton.GetValues())
            {
                Assert.That((MouseButton)mouseButton.Name, Is.EqualTo(mouseButton));
            }
        }

        [Test]
        public static void GivenInexistentName_WhenAssigningToButton_ThenThrows()
        {
            MouseButton mouseButton;

            Assert.Throws<ArgumentException>(() => mouseButton = "Testy McTestface");
        }

        [Test]
        public static void GivenTwoNullButtons_WhenUsingEqualityOperator_ThenReturnsTrue()
            => Assert.That((MouseButton)null == (MouseButton)null);

        [Test]
        public static void GivenSameButton_WhenUsingNotEqualsOperator_ThenReturnsFalse()
            => Assert.That(
                MouseButton.Left != MouseButton.Left,
                Is.False);

        [Test]
        public static void GivenDifferentButtons_WhenUsingNotEqualsOperator_ThenReturnsTrue()
            => Assert.That(MouseButton.Left != MouseButton.Right);

        [Test]
        public static void GivenNullAndButton_WhenUsingNotEqualsOperator_ThenReturnsTrue()
            => Assert.That(null != MouseButton.Left);

        [Test]
        public static void GivenButton_WhenGetValuesIsCalled_ThenReturnsFiveItems()
            => Assert.That(MouseButton.GetValues(), Has.Exactly(5).Items);

        [Test]
        public static void GivenButton_WhenGetValuesIsCalled_ThenContainsAllButtons()
        {
            IEnumerable<MouseButton> mouseButtons = MouseButton.GetValues();

            Assert.That(mouseButtons, Contains.Item(MouseButton.Left));
            Assert.That(mouseButtons, Contains.Item(MouseButton.Right));
            Assert.That(mouseButtons, Contains.Item(MouseButton.Middle));
            Assert.That(mouseButtons, Contains.Item(MouseButton.Back));
            Assert.That(mouseButtons, Contains.Item(MouseButton.Forward));
        }
    }
}
