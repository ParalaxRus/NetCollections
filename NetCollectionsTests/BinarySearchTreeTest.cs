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
    }
}
