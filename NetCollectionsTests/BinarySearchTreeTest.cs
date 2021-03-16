using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCollections;

namespace NetCollectionsTests
{
    [TestClass]
    public class BinarySearchTreeTests
    {
        private static void CheckTree<T>(BinarySearchTree<T> tree, int count, int height) where T : IComparable<T>
        {
            Assert.AreEqual(tree.Count, count);
            Assert.AreEqual(tree.Height, height);
            Assert.AreEqual(tree.Empty, count == 0);

            Assert.IsTrue(tree.IsBalanced());
            Assert.IsTrue(tree.IsValid());
        }

        [TestMethod]
        public void DefaultCtorShouldCreateEmptyTree()
        {
            var tree = new BinarySearchTree<int>();

            BinarySearchTreeTests.CheckTree(tree, 0, 0);
        }

        [TestMethod]
        public void AddTwoNodesShouldSetCountToTwoAndHeightToOne()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);
            tree.Add(2);

            BinarySearchTreeTests.CheckTree(tree, 2, 1);
        }

        [TestMethod]
        public void LeftLeftHeavyCheck()
        {
            var values = new int[] { 99, 155, 92, 80, 60 };

            var tree = new BinarySearchTree<int>();
            
            foreach (var val in values)
            {
                tree.Add(val);
            }

            BinarySearchTreeTests.CheckTree(tree, 5, 2);
        }

        [TestMethod]
        public void LeftRightHeavyCheck()
        {
            var values = new int[] { 99, 155, 92, 80, 90 };

            var tree = new BinarySearchTree<int>();
            
            foreach (var val in values)
            {
                tree.Add(val);
            }

            BinarySearchTreeTests.CheckTree(tree, 5, 2);
        }

        [TestMethod]
        public void RightLeftHeavyCheck()
        {
            var values = new int[] { 99, 155, 92, 234, 189 };

            var tree = new BinarySearchTree<int>();
            
            foreach (var val in values)
            {
                tree.Add(val);
            }

            BinarySearchTreeTests.CheckTree(tree, 5, 2);
        }

        [TestMethod]
        public void RightRightHeavyCheck()
        {
            var values = new int[] { 99, 155, 92, 234, 250 };

            var tree = new BinarySearchTree<int>();
            
            foreach (var val in values)
            {
                tree.Add(val);
            }

            BinarySearchTreeTests.CheckTree(tree, 5, 2);
        }

        [TestMethod]
        public void AddRandomValuesShouldCreateBalancedBst()
        {
            var tree = new BinarySearchTree<int>();

            var values = TestHelpers.CreateRandomValues();
            foreach (var value in values)
            {
                tree.Add(value);
            }

            Assert.IsTrue(tree.IsBalanced());
            Assert.IsTrue(tree.IsValid());
        }
    }
}
