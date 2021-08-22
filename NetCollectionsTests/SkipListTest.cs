using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCollections;

namespace NetCollectionsTests
{
    [TestClass]
    public class SkipListTests
    {
        private static void Check<T>(SkipList<T> list, ICollection<T> expectedValues) where T : struct, IComparable<T>
        {
            var valuesAsStr = string.Join(",", expectedValues.Select(x => x.ToString()).ToArray());

            Assert.AreEqual(list.Count, expectedValues.Count, valuesAsStr);

            foreach (var val in expectedValues)
            {
                Assert.IsTrue(list.Contains(val), valuesAsStr);
            }
        }

        private static ICollection<int> CreateRandom(int size = 50, int min = int.MinValue, int max = int.MaxValue)
        {
            var rand = new Random();

            var values = new int[size];
            for (int i = 0; i < size; ++i)
            {
                values[i] = rand.Next(min, max);
            }
            
            return values;
        }

        [TestMethod]
        public void DefaultCtorShouldCreateEmptyList()
        {
            var list = new SkipList<int>();

            Assert.AreEqual(list.Count, 0);
        }

        [TestMethod]
        public void AddingElementShouldIncreaseCountByOne()
        {
            var list = new SkipList<int>();
            list.Add(1);

            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public void AddingDuplicateShouldIncreaseCountByOne()
        {
            var list = new SkipList<int>();
            list.Add(1);
            list.Add(1);

            Assert.AreEqual(list.Count, 2);
        }

        [TestMethod]
        public void RemovingElementShouldDecreaseCountByOne()
        {
            var list = new SkipList<int>();
            list.Add(1);
            list.Remove(1);

            Assert.AreEqual(list.Count, 0);
        }

        [TestMethod]
        public void RemovingDuplicateShouldIncreaseCountByOne()
        {
            var list = new SkipList<int>();
            list.Add(1);
            list.Add(1);
            list.Remove(1);

            Assert.AreEqual(list.Count, 1);
        }

        [TestMethod]
        public void RemoveFromEmptyListShouldNotChangeCount()
        {
            var list = new SkipList<int>();
            list.Remove(1);

            Assert.AreEqual(list.Count, 0);
        }

        [TestMethod]
        public void CallContainsForEmptyListShouldReturnFalse()
        {
            var list = new SkipList<int>();
            Assert.IsFalse(list.Contains(0));
        }

        [TestMethod]
        public void CallContainsForNonExistentValueShouldReturnFalse()
        {
            var list = new SkipList<int>();
            list.Add(1);
            Assert.IsFalse(list.Contains(0));
        }

        [TestMethod]
        public void CallContainsForExistentValueShouldReturnFalse()
        {
            var list = new SkipList<int>();
            list.Add(1);
            list.Add(-5);
            Assert.IsTrue(list.Contains(-5));
        }

        [TestMethod]
        public void AddValuesIncludingWithDefaultValueShouldCreateCorrectList()
        {
            var list = new SkipList<int>();

            var values = new int[] {1, 0, -1}; // zero is default(int)
            foreach (var val in values)
            {
                list.Add(val);
            }

            SkipListTests.Check(list, values);
        }

        [TestMethod]
        public void AddDuplicateShouldAddItAndReturnTrue()
        {
            var list = new SkipList<int>();
            var values = new int[] {0, 0, 0};
            foreach (var val in values)
            {
                list.Add(val);
            }

            SkipListTests.Check(list, values);
        }

        [TestMethod]
        public void RemoveFromEmptyListShouldReturnFalse()
        {
            var list = new SkipList<int>();
            Assert.IsFalse(list.Remove(0));
        }

        [TestMethod]
        public void RemoveNonExistingValueShouldNotModifyListAndReturnFalse()
        {
            var list = new SkipList<int>();

            var values = new int[] {3, 0, 0, -2};
            foreach (var val in values)
            {
                list.Add(val);
            }

            Assert.IsFalse(list.Remove(-1));
            SkipListTests.Check(list, values);
        }

        [TestMethod]
        public void RemoveExistingValueShouldModifyListAndReturnTrue()
        {
            var list = new SkipList<int>();

            var values = new int[] {3, 0, 1, -2};
            foreach (var val in values)
            {
                list.Add(val);
            }

            Assert.IsTrue(list.Remove(0));
            SkipListTests.Check(list, new int[] {3, 1, -2});
        }

        [TestMethod]
        public void RemoveDuplicateShouldRemoveItAndReturnTrue()
        {
            var list = new SkipList<int>();

            var values = new int[] {3, 0, 1, 0, 2, 0};
            foreach (var val in values)
            {
                list.Add(val);
            }

            Assert.IsTrue(list.Remove(0));
            Assert.IsTrue(list.Remove(0));
            SkipListTests.Check(list, new int[] {3, 1, 2, 0});
        }

        [TestMethod]
        public void CreateListFromRandomValuesShouldCreateValidList()
        {
            var list = new SkipList<int>();

            var values = SkipListTests.CreateRandom(100);
            foreach (var val in values)
            {
                list.Add(val);
            }

            SkipListTests.Check(list, values);
        }

        [TestMethod]
        public void AddRemoveAndAddAgainShouldProduceValidResult()
        {
            var list = new SkipList<int>();

            // Original values
            var values = SkipListTests.CreateRandom(100).ToList();
            foreach (var val in values)
            {
                list.Add(val);
            }

            // Random indices to remove
            var removeIdx = SkipListTests.CreateRandom(20, 0, values.Count);

            // Removing from the list
            foreach (var i in removeIdx)
            {
                list.Remove(values[i]);
            }

            // Additional random values
            var newValues = SkipListTests.CreateRandom(50).ToList();

            // Adding to the list
            foreach (var val in newValues)
            {
                list.Add(val);
            }

            // Concatinating expected values (original - removed + additional)
            for (int i = 0; i < values.Count; ++i)
            {
                if (!removeIdx.Contains(i))
                {
                    newValues.Add(values[i]);
                }
            }

            SkipListTests.Check(list, newValues);
        }
    }
}
