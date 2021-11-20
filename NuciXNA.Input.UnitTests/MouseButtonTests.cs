using System;

using NUnit.Framework;

using NuciXNA.Input;

namespace NuciXNA.UnitTests.Input
{
    public class MouseButtonTests
    {
        [Test]
        public void FromId_ReturnsCorrectMouseButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.AreEqual(state, MouseButton.FromId(state.Id));
            }
        }

        [Test]
        public void FromId_CalledWithInexistentId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => MouseButton.FromId(873));
        }

        [Test]
        public void FromName_ReturnsCorrectMouseButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.AreEqual(state, MouseButton.FromName(state.Name));
            }
        }

        [Test]
        public void FromName_CalledWithInexistentName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => MouseButton.FromName("Hori"));
        }

        [Test]
        public void ToString_ReturnsCorrectValue()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.AreEqual(state.Name, state.ToString());
            }
        }

        [Test]
        public void GetHashCode_ReturnsCorrectValue()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                Assert.AreEqual(state.Id.GetHashCode(), state.GetHashCode());
            }
        }

        [Test]
        public void Equals_CalledWithSameMouseButton_ReturnsTrue()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsTrue(state.Equals(MouseButton.Left));
        }

        [Test]
        public void Equals_CalledWithOtherMouseButton_ReturnsFalse()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsFalse(state.Equals(MouseButton.Right));
        }

        [Test]
        public void Equals_CalledWithSameMouseButtonAsObject_ReturnsTrue()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsTrue(state.Equals((object)MouseButton.Left));
        }

        [Test]
        public void Equals_CalledWithOtherMouseButtonAsObject_ReturnsFalse()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsFalse(state.Equals((object)MouseButton.Right));
        }

        [Test]
        public void Equals_CalledWithOtherType_ReturnsFalse()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsFalse(state.Equals(DateTime.Now));
        }

        [Test]
        public void Equals_CalledWithNull_ReturnsFalse()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsFalse(state.Equals(null));
        }

        [Test]
        public void EqualsOperator_OtherIsSameMouseButton_ReturnsTrue()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsTrue(state == MouseButton.Left);
        }

        [Test]
        public void EqualsOperator_CurrentIsNull_ReturnsFalse()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsFalse(null == MouseButton.Left);
        }

        [Test]
        public void EqualsOperator_OtherIsNull_ReturnsFalse()
        {
            MouseButton state = MouseButton.Left;

            Assert.IsFalse(state == null);
        }

        [Test]
        public void CastAsInt_ReturnsCorrectValue()
        {
            MouseButton state = MouseButton.Left;

            Assert.AreEqual(state.Id, (int)state);
        }

        [Test]
        public void CastAsString_ReturnsCorrectValue()
        {
            MouseButton state = MouseButton.Left;

            Assert.AreEqual(state.Name, (string)state);
        }

        [Test]
        public void AssignInteger_AssignedExistingId_ReturnsCorrectMouseButton()
        {
            foreach (MouseButton state in MouseButton.GetValues())
            {
                MouseButton state2 = state.Id;
            
                Assert.AreEqual(state, state2);
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
                MouseButton state2 = state.Name;
            
                Assert.AreEqual(state, state2);
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
