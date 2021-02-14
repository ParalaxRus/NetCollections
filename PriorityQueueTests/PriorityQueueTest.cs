using System;
using System.Linq;
using System.Collections.Generic;
using PriorityQueueLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PriorityQueueTests
{
    [TestClass]
    public class PriorityQueueTests
    {
        private static void CheckQueue(PriorityQueueBase queue, int count)
        {
            Assert.AreEqual(queue.Count, count);
            Assert.AreEqual(queue.Empty, count == 0);
        }

        private static void CheckHeap<T>(PriorityQueue<T> queue) where T : IComparable<T>
        {
            var values = new List<T>();

            foreach (var value in queue)
            {
                values.Add(value);
            }

            int sign = queue.QueueType == PriorityQueueType.Min ? 1 : -1;

            for (int i = 0; i < values.Count; ++i)
            {               
                int l = 2 * i + 1;
                int r = 2 * i + 2;

                if (l < values.Count)
                {
                    Assert.IsTrue(sign * values[i].CompareTo(values[l]) <= 0);
                }

                if (r < values.Count)
                {
                    Assert.IsTrue(sign * values[i].CompareTo(values[r]) <= 0);
                }
            }
        }

        private static IEnumerable<byte> CreateValues()
        {
            var rand = new Random();
            int size = rand.Next(2, 20);

            var values = new byte[size];
            rand.NextBytes(values);

            return values;
        }

        private static IEnumerable<WeightedUri> CreateCustomValues()
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
        }

        private static void AddValuesShouldConformHeapProperty<T>(IEnumerable<T> values, PriorityQueueType type) where T : IComparable<T>
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
        }

        #region Value type tests

        [TestMethod]
        public void DefaultCtorShouldCreateEmptyMinHeap()
        {
            var queue = new PriorityQueue<int>();

            Assert.AreEqual(queue.QueueType, PriorityQueueType.Min);
            PriorityQueueTests.CheckQueue(queue, 0);
        }

        [TestMethod]
        public void CtorWithTypeShouldCreateEmptyHeapOfTheSpecifiedType()
        {
            var queue = new PriorityQueue<int>(PriorityQueueType.Max);

            Assert.AreEqual(queue.QueueType, PriorityQueueType.Max);
            PriorityQueueTests.CheckQueue(queue, 0);
        }

        [TestMethod]
        public void PeekOnEmptyQueueShouldThrowInvalidOperationException()
        {
            Action action = () => 
            {
                var queue = new PriorityQueue<int>();
                queue.Peek();
            };

            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public void TopOnEmptyQueueShouldThrowInvalidOperationException()
        {
            Action action = () => 
            {
                var queue = new PriorityQueue<int>();
                queue.Dequeue();
            };

            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public void AddElementShouldSetEmptyToFalseAndCountToOne()
        {
            var queue = new PriorityQueue<int>();
            queue.Enqueue(1);

            PriorityQueueTests.CheckQueue(queue, 1);
        }

        [TestMethod]
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
        }

        #endregion

        #region Custom type tests

        public class WeightedUri : IComparable<WeightedUri>
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

        [TestMethod]
        public void DefaultCtorShouldCreateEmptyMinHeapCustom()
        {
            var queue = new PriorityQueue<WeightedUri>();

            Assert.AreEqual(queue.QueueType, PriorityQueueType.Min);
            PriorityQueueTests.CheckQueue(queue, 0);
        }

        [TestMethod]
        public void CtorWithTypeShouldCreateEmptyHeapOfTheSpecifiedTypeCustom()
        {
            var queue = new PriorityQueue<WeightedUri>(PriorityQueueType.Max);

            Assert.AreEqual(queue.QueueType, PriorityQueueType.Max);
            PriorityQueueTests.CheckQueue(queue, 0);
        }

        [TestMethod]
        public void PeekOnEmptyQueueShouldThrowInvalidOperationExceptionCustom()
        {
            Action action = () => 
            {
                var queue = new PriorityQueue<WeightedUri>();
                queue.Peek();
            };

            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public void TopOnEmptyQueueShouldThrowInvalidOperationExceptionCustom()
        {
            Action action = () => 
            {
                var queue = new PriorityQueue<WeightedUri>();
                queue.Dequeue();
            };

            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public void AddElementShouldSetEmptyToFalseAndCountToOneCustom()
        {
            var queue = new PriorityQueue<WeightedUri>();
            queue.Enqueue(new WeightedUri(new Uri("https://www.test.com"), 1));

            PriorityQueueTests.CheckQueue(queue, 1);
        }

        [TestMethod]
        public void AddValuesToMinHeapShouldConformMinHeapPropertyCustom()
        {
            PriorityQueueTests.AddValuesShouldConformHeapProperty(
                PriorityQueueTests.CreateCustomValues(),  PriorityQueueType.Min);
        }

        [TestMethod]
        public void AddValuesToMaxHeapShouldConformMaxHeapPropertyCustom()
        {
            PriorityQueueTests.AddValuesShouldConformHeapProperty(
                PriorityQueueTests.CreateCustomValues(),  PriorityQueueType.Max);
        }

        [TestMethod]
        public void CreateMinHeapFromCollectionShouldCorrectlyHeapifyCustom()
        {
            PriorityQueueTests.CreateFromCollectionShouldHeapifyCorrecly(
                PriorityQueueTests.CreateCustomValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void CreateMaxHeapFromCollectionShouldCorrectlyHeapifyCustom()
        {
            PriorityQueueTests.CreateFromCollectionShouldHeapifyCorrecly(
                PriorityQueueTests.CreateCustomValues(), PriorityQueueType.Max);
        }

        [TestMethod]
        public void PeekMinQueueShouldReturnTheSameValuesAsTopCustom()
        {
            PriorityQueueTests.PeekShouldReturnTheSameValuesAsTop(
                PriorityQueueTests.CreateCustomValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void PeekMaxQueueShouldReturnTheSameValuesAsTopCustom()
        {
            PriorityQueueTests.PeekShouldReturnTheSameValuesAsTop(
                PriorityQueueTests.CreateCustomValues(), PriorityQueueType.Max);
        }

        [TestMethod]
        public void AddTopAddMultipleValuesShouldProduceValidMinQueueCustom()
        {
            PriorityQueueTests.AddRemoveAndAddCustomShouldProduceValidQueue(PriorityQueueType.Min);
        }

        [TestMethod]
        public void AddTopAddMultipleValuesShouldProduceValidMaxQueueCustom()
        {
            PriorityQueueTests.AddRemoveAndAddCustomShouldProduceValidQueue(PriorityQueueType.Max);
        }
        
        #endregion
    }
}
