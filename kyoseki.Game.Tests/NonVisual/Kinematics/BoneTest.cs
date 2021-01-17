using System;
using kyoseki.Game.Kinematics;
using NUnit.Framework;

namespace kyoseki.Game.Tests.NonVisual.Kinematics
{
    public class BoneTest
    {
        /// <summary>
        /// This test ensures the correct behavior of the
        /// <see cref="Bone.Child"/> property.
        /// </summary>
        [Test]
        public void TestChildBehavior()
        {
            var parent = createBone("Parent");

            var child = createBone("Child");

            var child2 = createBone("Child2");

            parent.Children = new[] { child, child2 };

            // If the parent has more than one child, accessing its Child property should throw an error.
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                var _ = parent.Child;
            });

            parent.Children = new[] { child };

            // If the parent has a singluar child, its Child property should be equal to that child.
            Assert.AreEqual(parent.Child, child);

            parent.Child = null;

            // If the parent has no childre, its Child property should be null.
            Assert.IsNull(parent.Child);
        }

        /// <summary>
        /// Ensures the correct behavior of the parent
        /// setting and unsetting the <see cref="Bone.Parent"/> property on its children.
        /// </summary>
        [Test]
        public void TestParentBehavior()
        {
            var parent = createBone("Parent");

            var parent2 = createBone("Parent2");

            var child = createBone("Child");

            parent.Child = child;

            // Adding a bone with a parent to a different parent should throw an error.
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                parent2.Child = child;
            });

            // Removing the child from the previous parent and adding it to a new one should work.
            parent.Child = null;
            parent2.Child = child;

            Assert.AreEqual(parent2.Child, child);
        }

        private Bone createBone(string name)
        {
            return new()
            {
                Name = name
            };
        }
    }
}
