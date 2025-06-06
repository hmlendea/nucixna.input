using System;

using NUnit.Framework;

namespace NuciXNA.Input.UnitTests
{
    public class MouseButtonTests
    {
        [Test]
        public void FromId_ReturnsCorrectMouseButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(MouseButton.FromId(state.Id), Is.EqualTo(state));
            }
        }

        [Test]
        public void FromId_CalledWithInexistentId_ThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => MouseButton.FromId(873));

        [Test]
        public void FromName_ReturnsCorrectMouseButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(MouseButton.FromName(state.Name), Is.EqualTo(state));
            }
        }

        [Test]
        public void FromName_CalledWithInexistentName_ThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => MouseButton.FromName("Hori"));

        [Test]
        public void ToString_ReturnsCorrectValue()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(state.ToString(), Is.EqualTo(state.Name));
            }
        }

        [Test]
        public void GetHashCode_ReturnsCorrectValue()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(state.GetHashCode(), Is.EqualTo(state.Id.GetHashCode()));
            }
        }

        [Test]
        public void Equals_CalledWithSameMouseButton_ReturnsTrue()
            => Assert.That(MouseButton.Left.Equals(MouseButton.Left));

        [Test]
        public void Equals_CalledWithOtherMouseButton_ReturnsFalse()
            => Assert.That(MouseButton.Left.Equals(MouseButton.Right), Is.False);

        [Test]
        public void Equals_CalledWithSameMouseButtonAsObject_ReturnsTrue()
            => Assert.That(MouseButton.Left.Equals((object)MouseButton.Left));

        [Test]
        public void Equals_CalledWithOtherMouseButtonAsObject_ReturnsFalse()
            => Assert.That(MouseButton.Left.Equals((object)MouseButton.Right), Is.False);

        [Test]
        public void Equals_CalledWithOtherType_ReturnsFalse()
            => Assert.That(MouseButton.Left.Equals(DateTime.Now), Is.False);

        [Test]
        public void Equals_CalledWithNull_ReturnsFalse()
            => Assert.That(MouseButton.Left.Equals(null), Is.False);

        [Test]
        public void EqualsOperator_OtherIsSameMouseButton_ReturnsTrue()
            => Assert.That(MouseButton.Left == MouseButton.Left);

        [Test]
        public void EqualsOperator_CurrentIsNull_ReturnsFalse()
        {
            Assert.That(null == MouseButton.Left, Is.False);
        }

        [Test]
        public void EqualsOperator_OtherIsNull_ReturnsFalse()
            => Assert.That(MouseButton.Left == null, Is.False);

        [Test]
        public void CastAsInt_ReturnsCorrectValue()
        {
            MouseButton state = MouseButton.Left;

            Assert.That((int)state, Is.EqualTo(state.Id));
        }

        [Test]
        public void CastAsString_ReturnsCorrectValue()
        {
            MouseButton state = MouseButton.Left;

            Assert.That((string)state, Is.EqualTo(state.Name));
        }

        [Test]
        public void AssignInteger_AssignedExistingId_ReturnsCorrectMouseButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(state.Id, Is.EqualTo(state));
            }
        }

        [Test]
        public void AssignInteger_AssignedInexistentId_ThrowsArgumentException()
        {
            MouseButton state;

            Assert.Throws<ArgumentException>(() => state = 873);
        }

        [Test]
        public void AssignString_AssignedExistingName_ReturnsCorrectMouseButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.That(state.Name, Is.EqualTo(state));
            }
        }

        [Test]
        public void AssignString_AssignedInexistentName_ThrowsArgumentException()
        {
            MouseButton state;

            Assert.Throws<ArgumentException>(() => state = "Hori");
        }
    }
}
