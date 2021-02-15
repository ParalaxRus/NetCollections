using System;
using System.Collections.Generic;
using PriorityQueueLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PriorityQueueTests
{
    [TestClass]
    public class PriorityLookupQueueTests
    {
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

        /*private static IEnumerable<KeyValuePair<K,V>> CreateValues<K,V>()
        {
            var rand = new Random();
            int size = rand.Next(2, 20);

            var buffer = new byte[size];
            rand.NextBytes(buffer);

            var values = new List<WeightedUri>();
            foreach (var val in buffer)
            {
                values.Add(new WeightedUri(new Uri("https://www.test.com"), val));
            }

            return values;
        }*/

        /*private static void AddValuesShouldConformHeapProperty<T>(IEnumerable<T> values, PriorityQueueType type) where T : IComparable<T>
        {
            var queue = new PriorityQueue<T>(type);

            foreach (var value in values)
            {
                queue.Enqueue(value);
            }

            PriorityQueueTests.CheckQueue(queue, values.Count());
            PriorityQueueTests.CheckHeap(queue);
        }

        private static void CreateFromCollectionShouldHeapifyCorrecly<T>(IEnumerable<T> values, PriorityQueueType type) where T : IComparable<T>
        {
            var queue = new PriorityQueue<T>(values, type);

            PriorityQueueTests.CheckQueue(queue, values.Count());
            PriorityQueueTests.CheckHeap(queue);
        }

        private static void PeekShouldReturnTheSameValuesAsTop<T>(IEnumerable<T> values, PriorityQueueType type) where T : IComparable<T>
        {
            var queue = new PriorityQueue<T>(values, type);

            while (!queue.Empty)
            {
                var val1 = queue.Peek();
                var val2 = queue.Dequeue();

                Assert.AreEqual(val1, val2);
            }

            PriorityQueueTests.CheckQueue(queue, 0);
        }

        private static void AddRemoveAndAddIntsShouldProduceValidQueue(PriorityQueueType type)
        {
            var values = PriorityQueueTests.CreateValues();
            
            var queue = new PriorityQueue<int>(type);
            foreach (var val in values)
            {
                queue.Enqueue(val);
            }

            var rand = new Random();
            int toRemove = rand.Next(0, values.Count() - 1);
            for (int i = 0; i < toRemove; ++i)
            {
                queue.Dequeue();
            }

            values = PriorityQueueTests.CreateValues();
            foreach (var val in values)
            {
                queue.Enqueue(val);
            }

            PriorityQueueTests.CheckHeap(queue);
        }

        private static void AddRemoveAndAddCustomShouldProduceValidQueue(PriorityQueueType type)
        {
            var values = PriorityQueueTests.CreateCustomValues();
            
            var queue = new PriorityQueue<WeightedUri>(type);
            foreach (var val in values)
            {
                queue.Enqueue(val);
            }

            var rand = new Random();
            int toRemove = rand.Next(0, values.Count() - 1);
            for (int i = 0; i < toRemove; ++i)
            {
                queue.Dequeue();
            }

            values = PriorityQueueTests.CreateCustomValues();
            foreach (var val in values)
            {
                queue.Enqueue(val);
            }

            PriorityQueueTests.CheckHeap(queue);
        }*/        

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
        public void AddElementShouldSetEmptyToFalseAndCountToOne()
        {
            var queue = new PriorityLookupQueue<int, Uri>();
            queue.Enqueue(1, new Uri("https://www.test.com"));

            PriorityQueueTests.CheckQueue(queue, 1);
        }

        /*[TestMethod]
        public void AddValuesToMinHeapShouldConformMinHeapProperty()
        {
            PriorityQueueTests.AddValuesShouldConformHeapProperty(
                PriorityQueueTests.CreateValues(),  PriorityQueueType.Min);
        }

        [TestMethod]
        public void AddValuesToMaxHeapShouldConformMaxHeapProperty()
        {
            PriorityQueueTests.AddValuesShouldConformHeapProperty(
                PriorityQueueTests.CreateValues(),  PriorityQueueType.Max);
        }

        [TestMethod]
        public void CreateMinHeapFromCollectionShouldCorrectlyHeapify()
        {
            PriorityQueueTests.CreateFromCollectionShouldHeapifyCorrecly(
                PriorityQueueTests.CreateValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void CreateMaxHeapFromCollectionShouldCorrectlyHeapify()
        {
            PriorityQueueTests.CreateFromCollectionShouldHeapifyCorrecly(
                PriorityQueueTests.CreateValues(), PriorityQueueType.Max);
        }

        [TestMethod]
        public void PeekMinQueueShouldReturnTheSameValuesAsTop()
        {
            PriorityQueueTests.PeekShouldReturnTheSameValuesAsTop(
                PriorityQueueTests.CreateValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void PeekMaxQueueShouldReturnTheSameValuesAsTop()
        {
            PriorityQueueTests.PeekShouldReturnTheSameValuesAsTop(
                PriorityQueueTests.CreateValues(), PriorityQueueType.Max);
        }

        [TestMethod]
        public void AddTopAddMultipleValuesShouldProduceValidMinQueue()
        {
            PriorityQueueTests.AddRemoveAndAddIntsShouldProduceValidQueue(PriorityQueueType.Min);
        }

        [TestMethod]
        public void AddTopAddMultipleValuesShouldProduceValidMaxQueue()
        {
            PriorityQueueTests.AddRemoveAndAddIntsShouldProduceValidQueue(PriorityQueueType.Max);
        }*/
    }
}
