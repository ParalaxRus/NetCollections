using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCollections;

namespace NetCollectionsTests
{
    [TestClass]
    public class AvlTreeTests
    {
        #region Private methods

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

        #endregion

        [TestMethod]
        public void DefaultCtorShouldCreateEmptyTree()
        {
            var tree = new AvlTree<int>();

            AvlTreeTests.CheckTree(tree, 0, 0, new List<int>());
        }

        [TestMethod]
        public void CtorWithValuesShouldCreateBalancedTree()
        {
            var values = TestHelpers.CreateRandomValues(true);
            var tree = new AvlTree<byte>(values);

            AvlTreeTests.CheckTree(tree, values.Count(), tree.Height, AvlTreeTests.GetSorted(values));
        }

        #region Add tests

        [TestMethod]
        public void AddTwoNodesShouldSetCountToTwoAndHeightToOne()
        {
            var tree = new AvlTree<int>();
            tree.Add(1);
            tree.Add(2);

            AvlTreeTests.CheckTree(tree, 2, 1, new int[] { 1, 2 });
        }

        [TestMethod]
        public void AddDuplicateValueShouldIncreaseValueCountButNotChangeNodesCount()
        {
            var tree = new AvlTree<int>();
            tree.Add(1);
            tree.Add(1);

            tree.Add(2);
            tree.Add(2);

            var values = new int[] { 1, 1, 2, 2 };
            AvlTreeTests.CheckTree(tree, 4, 1, values);

            Assert.AreEqual(tree.GetNodesCount(), 2);
        }

        [TestMethod]
        public void LeftLeftHeavyCheck()
        {
            var values = new int[] { 99, 155, 92, 80, 60 };

            var tree = new AvlTree<int>();
            
            foreach (var val in values)
            {
                tree.Add(val);
            }

            AvlTreeTests.CheckTree(tree, 5, 2, new int[] { 60, 80, 92, 99, 155 });
        }

        [TestMethod]
        public void LeftRightHeavyCheck()
        {
            var values = new int[] { 99, 155, 92, 80, 90 };

            var tree = new AvlTree<int>();
            
            foreach (var val in values)
            {
                tree.Add(val);
            }

            AvlTreeTests.CheckTree(tree, 5, 2, new int[] { 80, 90, 92, 99, 155 });
        }

        [TestMethod]
        public void RightLeftHeavyCheck()
        {
            var values = new int[] { 99, 155, 92, 234, 189 };

            var tree = new AvlTree<int>();
            
            foreach (var val in values)
            {
                tree.Add(val);
            }

            AvlTreeTests.CheckTree(tree, 5, 2, new int[] { 92, 99, 155, 189, 234 });
        }

        [TestMethod]
        public void RightRightHeavyCheck()
        {
            var values = new int[] { 99, 155, 92, 234, 250 };

            var tree = new AvlTree<int>();
            
            foreach (var val in values)
            {
                tree.Add(val);
            }

            AvlTreeTests.CheckTree(tree, 5, 2, new int[] { 92, 99, 155, 234, 250 });
        }

        [TestMethod]
        public void AddRandomValuesShouldCreateBalancedBst()
        {
            var tree = new AvlTree<byte>();

            var values = TestHelpers.CreateRandomValues(true);
            foreach (var value in values)
            {
                tree.Add(value);
            }

            AvlTreeTests.CheckTree(tree, values.Count(), tree.Height, AvlTreeTests.GetSorted(values));
        }

        #endregion

        #region Remove tests

        [TestMethod]
        public void RemoveForEmptyTreeShouldReturnFalse()
        {
            var tree = new AvlTree<int>();

            Assert.IsFalse(tree.Remove(0));
        }

        [TestMethod]
        public void RemoveNodeWithNoChildren()
        {
            var tree = new AvlTree<int>();
            tree.Add(1);

            Assert.IsTrue(tree.Remove(1));
            
            AvlTreeTests.CheckTree(tree, 0, 0, new int[] {  });
        }

        [TestMethod]
        public void RemoveNodeWithOneChild()
        {
            var tree = new AvlTree<int>();
            tree.Add(1);
            tree.Add(2);

            Assert.IsTrue(tree.Remove(1));
            
            AvlTreeTests.CheckTree(tree, 1, 0, new int[] { 2 });
        }

        [TestMethod]
        public void RemoveNodeWithBothChildren()
        {
            var tree = new AvlTree<int>();
            tree.Add(1);
            tree.Add(2);
            tree.Add(-1);

            Assert.IsTrue(tree.Remove(1));
            
            AvlTreeTests.CheckTree(tree, 2, 1, new int[] { -1, 2 });
        }

        [TestMethod]
        public void RemoveNonExistingValueShouldNotModifyTreeAndReturnFalse()
        {
            var tree = new AvlTree<int>();
            tree.Add(1);

            Assert.IsFalse(tree.Remove(2));
            
            AvlTreeTests.CheckTree(tree, 1, 0, new int[] { 1 });
        }

        [TestMethod]
        public void RemoveDuplicateValueShouldReturnTrueUpdateValuesCountAndNotChangeNodesCount()
        {
            var tree = new AvlTree<int>();
            tree.Add(1);
            tree.Add(1);
            tree.Add(2);
            
            Assert.IsTrue(tree.Remove(1));

            var values = new int[] { 1, 2 };
            AvlTreeTests.CheckTree(tree, 2, 1, values);

            Assert.AreEqual(tree.GetNodesCount(), 2);
        }

        [TestMethod]
        public void RemoveRootWithRebalanceShouldProduceBalancedTree()
        {
            var values = new byte[] { 165, 77, 222, 1, 251, 184, 169 };
            var tree = new AvlTree<byte>(values);

            Assert.IsTrue(tree.Remove(165));

            AvlTreeTests.CheckTree(tree, 6, 2, new byte[] { 1, 77, 169, 184, 222, 251 });
        }

        [TestMethod]
        public void RemoveNodeWithBothChildrenAndInorderSuccessorContainingDuplicatesShouldProduceBalancedTree()
        {           
            var values = new byte[] { 34, 17, 86, 8, 32, 50, 50, 87 };
            var tree = new AvlTree<byte>(values);

            Assert.IsTrue(tree.Remove(34));

            Assert.IsTrue(tree.IsValid());
            Assert.IsTrue(tree.IsBalanced());
        }

        [TestMethod]
        public void RemovingNodeInTheRightSubtreeLeadingToRebalancingWholeTreeShouldProduceBalancedTree()
        {
            var values = new byte[] { 116, 248, 195, 231, 42, 60, 54, 18, 192, 116, 214, 211 };
            var tree = new AvlTree<byte>(values);
            
            var toRemove = new byte[] { 231, 211 };
            foreach (var v in toRemove)
            { 
                Assert.IsTrue(tree.Remove(v));

                Assert.IsTrue(tree.IsValid());
                Assert.IsTrue(tree.IsBalanced());
            }
        }

        [TestMethod]
        public void RemoveRandomValuesShouldKeepTreeBalanced()
        {
            var values = TestHelpers.CreateRandomValues(true);
            var tree = new AvlTree<byte>(values);
            
            var valuesList = values.ToList();

            var rand = new Random();
            while (valuesList.Count != 0)
            {
                int index = rand.Next(0, valuesList.Count);
                var value = valuesList[index];

                Console.Write("{0}, ", value);

                Assert.IsTrue(tree.Remove(value));
                valuesList.RemoveAt(index);
                
                AvlTreeTests.CheckTree(tree, valuesList.Count, tree.Height, AvlTreeTests.GetSorted(valuesList));
            }
        }

        #endregion

        #region Contains tests

        [TestMethod]
        public void ContainsForEmptyTreeShouldReturnFalse()
        {
            var tree = new AvlTree<int>();

            Assert.IsFalse(tree.Contains(0).Item1);
        }

        [TestMethod]
        public void ContainsNonExistingValueShouldReturnFalse()
        {
            var tree = new AvlTree<int>();
            tree.Add(1);

            Assert.IsFalse(tree.Contains(0).Item1);
        }

        [TestMethod]
        public void ContainsExistingValueShouldReturnTrueAndDuplicatesCount()
        {
            var tree = new AvlTree<int>();
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
            var tree = new AvlTree<byte>(values);
            
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

                AvlTreeTests.CheckTree(tree, valuesList.Count, tree.Height, AvlTreeTests.GetSorted(valuesList));

                tree.Remove(value);
                valuesList.RemoveAt(index);
            }
        }

        #endregion

        [TestMethod]
        public void RandomAddRemoveValuesShouldKeepTreeBalanced()
        {
            var tree = new AvlTree<byte>();

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
                    Console.Write("{0}, ", addValues[j]);
                    tree.Remove(addValues[j]);
                }

                Console.WriteLine();

                Assert.IsTrue(tree.IsValid());
                Assert.IsTrue(tree.IsBalanced());
            }
        }

        [TestMethod]
        public void RandomAddRemoveCustomValuesShouldKeepTreeBalanced()
        {
            var tree = new AvlTree<WeightedUri>();

            var random = new Random();
            for (int i = 0; i < 3; ++i)
            {
                var toAddvalues = TestHelpers.CreateRandomCustomValues(true);

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

        [TestMethod]
        public void AvlTreeExample()
        {
            var elements = new List<WeightedUri>()
            {
                new WeightedUri(new Uri("https://www.test2.com"), 2),
                new WeightedUri(new Uri("https://www.test1.com"), 1),
                new WeightedUri(new Uri("https://www.test3.com"), 3),
                new WeightedUri(new Uri("https://www.test4.com"), 4),
            };

            // Create with original seed of elements
            var tree = new AvlTree<WeightedUri>(elements);
            Console.WriteLine("Count={0} Empty={1} Height={2}", tree.Count, tree.Empty, tree.Height);

            var existing = new WeightedUri(new Uri("https://www.test1.com"), 1);
            Console.WriteLine("Contains={0}", tree.Contains(existing));

            // IEnumerable example
            Console.WriteLine("Enumerating:");
            foreach (var element in tree)
            {
                Console.WriteLine(element);
            }

            // Add duplicate element
            var duplicate = new WeightedUri(new Uri("https://www.test2.com"), 2);
            tree.Add(duplicate);
            Console.WriteLine("Count={0} Empty={1} Height={2}", tree.Count, tree.Empty, tree.Height);

            // Remove existing
            existing = new WeightedUri(new Uri("https://www.test1.com"), 1);
            tree.Remove(existing);
            Console.WriteLine("Contains={0}", tree.Contains(existing));

            // Removing all elements
            var left = new List<WeightedUri>();
            foreach (var element in tree)
            {
                left.Add(element);
            }
            foreach (var element in left)
            {
                tree.Remove(element);
            }
            Console.WriteLine("Count={0} Empty={1} Height={2}", tree.Count, tree.Empty, tree.Height);
        }
    }
}
