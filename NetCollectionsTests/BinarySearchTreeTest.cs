using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCollections;

namespace NetCollectionsTests
{
    [TestClass]
    public class BinarySearchTreeTests
    {
        private static void CheckTree<T>(BinarySearchTree<T> tree, 
                                         int                 count, 
                                         int                 height, 
                                         IEnumerable<T>      expectedValues) where T : IComparable<T>
        {
            Assert.AreEqual(tree.Count, count);
            Assert.AreEqual(tree.Height, height);
            Assert.AreEqual(tree.Empty, count == 0);

            var values = new List<T>();
            foreach (var v in tree)
            {
                values.Add(v);
            }

            expectedValues.SequenceEqual(values);

            Assert.IsTrue(tree.IsValid());
            Assert.IsTrue(tree.IsBalanced());
        }

        private static IEnumerable<T> GetSorted<T>(IEnumerable<T> values)
        {
            return values.OrderBy(v => v);
        }

        [TestMethod]
        public void DefaultCtorShouldCreateEmptyTree()
        {
            var tree = new BinarySearchTree<int>();

            BinarySearchTreeTests.CheckTree(tree, 0, 0, new List<int>());
        }

        [TestMethod]
        public void CtorWithValuesShouldCreateBalancedTree()
        {
            var values = TestHelpers.CreateRandomValues(true);
            var tree = new BinarySearchTree<byte>(values);

            BinarySearchTreeTests.CheckTree(tree, values.Count(), tree.Height, BinarySearchTreeTests.GetSorted(values));
        }

        #region Add tests

        [TestMethod]
        public void AddTwoNodesShouldSetCountToTwoAndHeightToOne()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);
            tree.Add(2);

            BinarySearchTreeTests.CheckTree(tree, 2, 1, new int[] { 1, 2 });
        }

        [TestMethod]
        public void AddDuplicateValueShouldIncreaseValueCountButNotChangeNodesCount()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);
            tree.Add(1);

            tree.Add(2);
            tree.Add(2);

            var values = new int[] { 1, 1, 2, 2 };
            BinarySearchTreeTests.CheckTree(tree, 4, 1, values);

            Assert.AreEqual(tree.GetNodesCount(), 2);
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

            BinarySearchTreeTests.CheckTree(tree, 5, 2, new int[] { 60, 80, 92, 99, 155 });
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

            BinarySearchTreeTests.CheckTree(tree, 5, 2, new int[] { 80, 90, 92, 99, 155 });
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

            BinarySearchTreeTests.CheckTree(tree, 5, 2, new int[] { 92, 99, 155, 189, 234 });
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

            BinarySearchTreeTests.CheckTree(tree, 5, 2, new int[] { 92, 99, 155, 234, 250 });
        }

        [TestMethod]
        public void AddRandomValuesShouldCreateBalancedBst()
        {
            var tree = new BinarySearchTree<byte>();

            var values = TestHelpers.CreateRandomValues(true);
            foreach (var value in values)
            {
                tree.Add(value);
            }

            BinarySearchTreeTests.CheckTree(tree, values.Count(), tree.Height, BinarySearchTreeTests.GetSorted(values));
        }

        #endregion

        #region Remove tests

        [TestMethod]
        public void RemoveForEmptyTreeShouldReturnFalse()
        {
            var tree = new BinarySearchTree<int>();

            Assert.IsFalse(tree.Remove(0));
        }

        [TestMethod]
        public void RemoveNodeWithNoChildren()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);

            Assert.IsTrue(tree.Remove(1));
            
            BinarySearchTreeTests.CheckTree(tree, 0, 0, new int[] {  });
        }

        [TestMethod]
        public void RemoveNodeWithOneChild()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);
            tree.Add(2);

            Assert.IsTrue(tree.Remove(1));
            
            BinarySearchTreeTests.CheckTree(tree, 1, 0, new int[] { 2 });
        }

        [TestMethod]
        public void RemoveNodeWithBothChildren()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);
            tree.Add(2);
            tree.Add(-1);

            // Remove should return removed parent to fix this bug ...
            Assert.IsTrue(tree.Remove(1));
            
            BinarySearchTreeTests.CheckTree(tree, 2, 1, new int[] { -1, 2 });
        }

        [TestMethod]
        public void RemoveNonExistingValueShouldNotModifyTreeAndReturnFalse()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);

            Assert.IsFalse(tree.Remove(2));
            
            BinarySearchTreeTests.CheckTree(tree, 1, 0, new int[] { 1 });
        }

        [TestMethod]
        public void RemoveDuplicateValueShouldReturnTrueUpdateValuesCountAndNotChangeNodesCount()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);
            tree.Add(1);
            tree.Add(2);
            
            Assert.IsTrue(tree.Remove(1));

            var values = new int[] { 1, 2 };
            BinarySearchTreeTests.CheckTree(tree, 2, 1, values);

            Assert.AreEqual(tree.GetNodesCount(), 2);
        }

        [TestMethod]
        public void RemoveRandomValuesShouldKeepTreeBalanced()
        {
            var values = TestHelpers.CreateRandomValues(true);
            var tree = new BinarySearchTree<byte>(values);
            
            var valuesList = values.ToList();

            var rand = new Random();
            while (valuesList.Count != 0)
            {
                int index = rand.Next(0, valuesList.Count);
                var value = valuesList[index];

                Console.Write("{0}, ", value);

                Assert.IsTrue(tree.Remove(value));
                valuesList.RemoveAt(index);
                
                BinarySearchTreeTests.CheckTree(tree, valuesList.Count, tree.Height, BinarySearchTreeTests.GetSorted(valuesList));
            }
        }

        #endregion

        #region Contains tests

        [TestMethod]
        public void ContainsForEmptyTreeShouldReturnFalse()
        {
            var tree = new BinarySearchTree<int>();

            Assert.IsFalse(tree.Contains(0).Item1);
        }

        [TestMethod]
        public void ContainsNonExistingValueShouldReturnFalse()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);

            Assert.IsFalse(tree.Contains(0).Item1);
        }

        [TestMethod]
        public void ContainsExistingValueShouldReturnTrueAndDuplicatesCount()
        {
            var tree = new BinarySearchTree<int>();
            tree.Add(1);
            tree.Add(2);
            tree.Add(3);
            tree.Add(2);
            tree.Add(2);

            var res = tree.Contains(2);
            Assert.IsTrue(res.Item1);
            Assert.AreEqual(res.Item2, 3);
        }

        [TestMethod]
        public void ContainsExistingValueAmongRandomGeneratesValuesShouldReturnTrueAndShouldNotChangeTree()
        {
            var values = TestHelpers.CreateRandomValues(true);
            var tree = new BinarySearchTree<byte>(values);
            
            var valuesList = values.ToList();

            var rand = new Random();
            while (valuesList.Count != 0)
            {
                int index = rand.Next(0, valuesList.Count);
                byte value = valuesList[index];

                Console.Write("{0}, ", value);

                var res = tree.Contains(value);
                var duplicatesCount = valuesList.Where(v => v == value).Count();

                Assert.IsTrue(res.Item1);
                Assert.AreEqual(res.Item2, duplicatesCount);

                BinarySearchTreeTests.CheckTree(tree, valuesList.Count, tree.Height, BinarySearchTreeTests.GetSorted(valuesList));

                tree.Remove(value);
                valuesList.RemoveAt(index);
            }
        }

        #endregion

        [TestMethod]
        public void RandomAddRemoveValuesShouldKeepTreeBalanced()
        {
            var tree = new BinarySearchTree<byte>();

            var random = new Random();
            for (int i = 0; i < 3; ++i)
            {
                var toAddvalues = TestHelpers.CreateRandomValues(true);

                tree.AddRange(toAddvalues);

                var addValues = toAddvalues.ToList();

                // First count of values
                int removeCount = random.Next(1, addValues.Count);
                for (int j = 0; j < removeCount; ++j)
                {
                    tree.Remove(addValues[j]);
                }

                Assert.IsTrue(tree.IsValid());
                Assert.IsTrue(tree.IsBalanced());
            }
        }
    }
}
