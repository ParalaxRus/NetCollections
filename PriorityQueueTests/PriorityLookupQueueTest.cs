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

        private class WeightedUri : IComparable<WeightedUri>
        {
            public Uri Host { get; private set; }
            public int Weight { get; private set; }

            public WeightedUri(Uri host, int weight)
            {
                this.Host = host;
                this.Weight = weight;
            }

            public int CompareTo(WeightedUri other)
            {
                if (other == null) 
                {
                    return 1;
                }

                return this.Weight.CompareTo(other.Weight);
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
    }
}
