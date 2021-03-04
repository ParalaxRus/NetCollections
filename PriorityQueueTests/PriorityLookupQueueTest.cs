using System;
using System.Collections.Generic;
using PriorityQueueLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace PriorityQueueTests
{
    [TestClass]
    public class PriorityLookupQueueTests
    {
        #region Private methods

        private static void CheckHeap<K, V>(PriorityLookupQueue<K, V> queue) where K : IComparable<K>
        {
            var values = new List<KeyValuePair<K, V>>();

            foreach (var value in queue)
            {
                values.Add(value);
            }

            int sign = queue.QueueType == PriorityQueueType.Min ? 1 : -1;

            for (int i = 0; i < values.Count; ++i)
            {               
                int l = 2 * i + 1;
                int r = 2 * i + 2;

                var current = values[i].Key;

                if (l < values.Count)
                {
                    var left = values[l].Key;
                    Assert.IsTrue(sign * current.CompareTo(left) <= 0);
                }

                if (r < values.Count)
                {
                    var right = values[r].Key;
                    Assert.IsTrue(sign * current.CompareTo(right) <= 0);
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, Uri>> CreateValues()
        {
            var rand = new Random();
            int size = rand.Next(2, 20);

            var values = new List<KeyValuePair<string, Uri>>();
            for (int i = 0; i < size; ++i)
            {
                var key = Guid.NewGuid().ToString();
                var kvp = new KeyValuePair<string, Uri>(key, new Uri(string.Format("https://www.{0}.com", key)));
                values.Add(kvp);
            }

            return values;
        }

        private static void AddValuesShouldConformHeapProperty<K,V>(IEnumerable<KeyValuePair<K,V>> values, PriorityQueueType type) where K : IComparable<K>
        {
            var queue = new PriorityLookupQueue<K,V>(type);

            foreach (var value in values)
            {
                queue.Enqueue(value.Key, value.Value);
            }

            PriorityQueueTests.CheckQueue(queue, values.Count());
            PriorityLookupQueueTests.CheckHeap(queue);
        }

        private static void CreateFromCollectionShouldHeapifyCorrecly<K,V>(IEnumerable<KeyValuePair<K,V>> kvps, PriorityQueueType type) where K : IComparable<K>
        {
            var keys = kvps.Select(kvp => kvp.Key);
            var values = kvps.Select(kvp => kvp.Value);

            var queue = new PriorityLookupQueue<K,V>(keys, values, type);

            PriorityQueueTests.CheckQueue(queue, values.Count());
            PriorityLookupQueueTests.CheckHeap(queue);
        }

        private static void PeekShouldReturnTheSameValuesAsTop<K,V>(IEnumerable<KeyValuePair<K,V>> kvps, PriorityQueueType type) where K : IComparable<K>
        {
            var keys = kvps.Select(kvp => kvp.Key);
            var values = kvps.Select(kvp => kvp.Value);

            var queue = new PriorityLookupQueue<K,V>(keys, values, type);

            while (!queue.Empty)
            {
                var val1 = queue.Peek();
                var val2 = queue.Dequeue();

                Assert.AreEqual(val1, val2);
            }

            PriorityQueueTests.CheckQueue(queue, 0);
        }

        private static void AddRemoveAndAddShouldProduceValidQueue(PriorityQueueType type)
        {
            var kvps = PriorityLookupQueueTests.CreateValues();

            var queue = new PriorityLookupQueue<string,Uri>(type);
            foreach (var kvp in kvps)
            {
                queue.Enqueue(kvp.Key, kvp.Value);
            }

            var rand = new Random();
            int toRemove = rand.Next(0, kvps.Count() - 1);
            for (int i = 0; i < toRemove; ++i)
            {
                queue.Dequeue();
            }

            kvps = PriorityLookupQueueTests.CreateValues();
            foreach (var kvp in kvps)
            {
                queue.Enqueue(kvp.Key, kvp.Value);
            }

            PriorityLookupQueueTests.CheckHeap(queue);
        }

        private static void GetPriorityExistingValueShouldReturnCorrectKey(PriorityQueueType type)
        {
            var kvps = PriorityLookupQueueTests.CreateValues();

            var queue = new PriorityLookupQueue<string, Uri>(type);
            foreach (var val in kvps)
            {
                queue.Enqueue(val.Key, val.Value);
            }

            var rand = new Random();
            int index = rand.Next(0, queue.Count);
            var kvp = kvps.ElementAt(index);

            Assert.AreEqual(queue.GetPriority(kvp.Value), kvp.Key);
        }

        private static void SetPriorityForExistingValueShouldUpdateIt(PriorityQueueType type)
        {
            var kvps = PriorityLookupQueueTests.CreateValues();

            var queue = new PriorityLookupQueue<string, Uri>(type);
            foreach (var val in kvps)
            {
                queue.Enqueue(val.Key, val.Value);
            }

            var rand = new Random();
            int index = rand.Next(0, queue.Count);
            var kvp = kvps.ElementAt(index);

            var priority = Guid.NewGuid().ToString();
            var oldPriority = queue.SetPriority(kvp.Value, priority);

            Assert.AreEqual(oldPriority, kvp.Key);
            Assert.AreEqual(queue.GetPriority(kvp.Value), priority);
            Assert.AreEqual(queue.Count, kvps.Count());

            PriorityLookupQueueTests.CheckHeap(queue);
        }

        #endregion

        [TestMethod]
        public void DefaultCtorShouldCreateEmptyMinHeap()
        {
            var queue = new PriorityLookupQueue<int, Uri>();

            Assert.AreEqual(queue.QueueType, PriorityQueueType.Min);
            PriorityQueueTests.CheckQueue(queue, 0);
        }

        [TestMethod]
        public void CtorWithTypeShouldCreateEmptyHeapOfTheSpecifiedType()
        {
            var queue = new PriorityLookupQueue<int, Uri>(PriorityQueueType.Max);

            Assert.AreEqual(queue.QueueType, PriorityQueueType.Max);
            PriorityQueueTests.CheckQueue(queue, 0);
        }

        [TestMethod]
        public void PeekOnEmptyQueueShouldThrowInvalidOperationException()
        {
            Action action = () => 
            {
                var queue = new PriorityLookupQueue<int, Uri>();
                queue.Peek();
            };

            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public void TopOnEmptyQueueShouldThrowInvalidOperationException()
        {
            Action action = () => 
            {
                var queue = new PriorityLookupQueue<int, Uri>();
                queue.Dequeue();
            };

            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public void AddDuplicateValueShouldThrowInvalidOperationException()
        {
            var queue = new PriorityLookupQueue<int, Uri>();
            queue.Enqueue(1, new Uri("https://www.test.com"));

            Action action = () => 
            {
                queue.Enqueue(2, new Uri("https://www.test.com"));
            };

            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public void AddDuplicateKeyIsValidAndShouldAddItAfterAlreadyExistingKey()
        {
            var queue = new PriorityLookupQueue<int, Uri>();
            queue.Enqueue(1, new Uri("https://www.test1.com"));
            queue.Enqueue(1, new Uri("https://www.test2.com"));

            var kvp1 = queue.Dequeue();
            Assert.AreEqual(kvp1.Key, 1);
            Assert.AreEqual(kvp1.Value, new Uri("https://www.test1.com"));

            var kvp2 = queue.Dequeue();
            Assert.AreEqual(kvp2.Key, 1);
            Assert.AreEqual(kvp2.Value, new Uri("https://www.test2.com"));
        }

        [TestMethod]
        public void AddElementShouldSetEmptyToFalseAndCountToOne()
        {
            var queue = new PriorityLookupQueue<int, Uri>();
            queue.Enqueue(1, new Uri("https://www.test.com"));

            PriorityQueueTests.CheckQueue(queue, 1);
        }

        [TestMethod]
        public void AddValuesToMinHeapShouldConformMinHeapProperty()
        {
            PriorityLookupQueueTests.AddValuesShouldConformHeapProperty(
                PriorityLookupQueueTests.CreateValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void AddValuesToMaxHeapShouldConformMaxHeapProperty()
        {
            PriorityLookupQueueTests.AddValuesShouldConformHeapProperty(
                PriorityLookupQueueTests.CreateValues(), PriorityQueueType.Max);
        }

        [TestMethod]
        public void CreateMinHeapFromCollectionShouldCorrectlyHeapify()
        {
            PriorityLookupQueueTests.CreateFromCollectionShouldHeapifyCorrecly(
                PriorityLookupQueueTests.CreateValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void CreateMaxHeapFromCollectionShouldCorrectlyHeapify()
        {
            PriorityLookupQueueTests.CreateFromCollectionShouldHeapifyCorrecly(
                PriorityLookupQueueTests.CreateValues(), PriorityQueueType.Max);
        }

        [TestMethod]
        public void PeekMinQueueShouldReturnTheSameValuesAsTop()
        {
            PriorityLookupQueueTests.PeekShouldReturnTheSameValuesAsTop(
                PriorityLookupQueueTests.CreateValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void PeekMaxQueueShouldReturnTheSameValuesAsTop()
        {
            PriorityLookupQueueTests.PeekShouldReturnTheSameValuesAsTop(
                PriorityLookupQueueTests.CreateValues(), PriorityQueueType.Max);
        }

        [TestMethod]
        public void AddTopAddMultipleValuesShouldProduceValidMinQueue()
        {
            PriorityLookupQueueTests.AddRemoveAndAddShouldProduceValidQueue(PriorityQueueType.Min);
        }

        [TestMethod]
        public void AddTopAddMultipleValuesShouldProduceValidMaxQueue()
        {
            PriorityLookupQueueTests.AddRemoveAndAddShouldProduceValidQueue(PriorityQueueType.Max);
        }

        #region GetPriority tests

        [TestMethod]
        public void GetPriorityForNonExistingValueShouldThrowArgumentException()
        {
            var queue = new PriorityLookupQueue<int, string>();

            Action action = () => 
            {
                queue.GetPriority("test");
            };

            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void GetPriorityExistingValueShouldReturnCorrectKeyForMinQueue()
        {
            PriorityLookupQueueTests.GetPriorityExistingValueShouldReturnCorrectKey(PriorityQueueType.Min);
        }

        [TestMethod]
        public void GetPriorityExistingValueShouldReturnCorrectKeyForMaxQueue()
        {
            PriorityLookupQueueTests.GetPriorityExistingValueShouldReturnCorrectKey(PriorityQueueType.Max);
        }

        #endregion

        #region SetPriority tests

        [TestMethod]
        public void SetPriorityForNonExistingValueShouldThrowArgumentException()
        {
            var queue = new PriorityLookupQueue<int, string>();
            queue.Enqueue(1, "one");

            Action action = () => 
            {
                queue.SetPriority("invalid", 2);
            };

            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void SetPriorityForExistingValueShouldUpdateItMinQueue()
        {
            PriorityLookupQueueTests.SetPriorityForExistingValueShouldUpdateIt(PriorityQueueType.Min);
        }

        [TestMethod]
        public void SetPriorityForExistingValueShouldUpdateItMaxQueue()
        {
            PriorityLookupQueueTests.SetPriorityForExistingValueShouldUpdateIt(PriorityQueueType.Max);
        }

        #endregion

        #region Contains tests

        [TestMethod]
        public void CallContainsValueForEmptyQueueShouldReturnFalse()
        {
            var queue = new PriorityLookupQueue<string, string>();
            
            Assert.IsFalse(queue.ContainsValue("1"));
        }

        [TestMethod]
        public void CallContainsValueForNonExistingValueShouldReturnFalse()
        {
            var queue = new PriorityLookupQueue<string, string>();
            queue.Enqueue("1", "2");
            
            Assert.IsFalse(queue.ContainsValue("1"));
        }

        [TestMethod]
        public void CallContainsValueForExistingValueShouldReturnTrue()
        {
            var queue = new PriorityLookupQueue<string, string>();
            queue.Enqueue("2", "1");
            
            Assert.IsTrue(queue.ContainsValue("1"));
        }

        [TestMethod]
        public void CallContainsValueForExistingValueWithDuplicateKeysShouldReturnTrue()
        {
            var queue = new PriorityLookupQueue<string, string>();
            queue.Enqueue("2", "1");
            queue.Enqueue("2", "2");
            
            Assert.IsTrue(queue.ContainsValue("1"));
        }

        [TestMethod]
        public void CallContainsValueShouldReturnTrueUntilElementIsInQueue()
        {
            var queue = new PriorityLookupQueue<int, string>();
            queue.Enqueue(-1, Guid.NewGuid().ToString());
            queue.Enqueue(0, Guid.NewGuid().ToString());
            queue.Enqueue(1, "value");
            queue.Enqueue(2, Guid.NewGuid().ToString());
            queue.Enqueue(3, Guid.NewGuid().ToString());
            
            for (int i = 0; i < 3; ++i)
            {
                Assert.IsTrue(queue.ContainsValue("value"));
                queue.Dequeue();
            }

            Assert.IsFalse(queue.ContainsValue("value"));
        }

        #endregion
    }
}
