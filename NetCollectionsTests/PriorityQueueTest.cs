using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCollections;

namespace NetCollectionsTests
{
    [TestClass]
    public class PriorityQueueTests
    {
        public static void CheckQueue(PriorityQueueBase queue, int count)
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
            var values = TestHelpers.CreateRandomValues();
            
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

            values = TestHelpers.CreateRandomValues();
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

        private static void CheckContains<T>(IEnumerable<T> values, T value, PriorityQueueType type, bool contains) where T : IComparable<T>
        {
            var queue = new PriorityQueue<T>(values, type);

            Assert.AreEqual(queue.Contains(value), contains);
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
                TestHelpers.CreateRandomValues(),  PriorityQueueType.Min);
        }

        [TestMethod]
        public void AddValuesToMaxHeapShouldConformMaxHeapProperty()
        {
            PriorityQueueTests.AddValuesShouldConformHeapProperty(
                TestHelpers.CreateRandomValues(),  PriorityQueueType.Max);
        }

        [TestMethod]
        public void CreateMinHeapFromCollectionShouldCorrectlyHeapify()
        {
            PriorityQueueTests.CreateFromCollectionShouldHeapifyCorrecly(
                TestHelpers.CreateRandomValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void CreateMaxHeapFromCollectionShouldCorrectlyHeapify()
        {
            PriorityQueueTests.CreateFromCollectionShouldHeapifyCorrecly(
                TestHelpers.CreateRandomValues(), PriorityQueueType.Max);
        }

        [TestMethod]
        public void PeekMinQueueShouldReturnTheSameValuesAsTop()
        {
            PriorityQueueTests.PeekShouldReturnTheSameValuesAsTop(
                TestHelpers.CreateRandomValues(), PriorityQueueType.Min);
        }

        [TestMethod]
        public void PeekMaxQueueShouldReturnTheSameValuesAsTop()
        {
            PriorityQueueTests.PeekShouldReturnTheSameValuesAsTop(
                TestHelpers.CreateRandomValues(), PriorityQueueType.Max);
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

        [TestMethod]
        public void ContainsForNonExistingVlaueInMinQueueShouldReturnFalse()
        {           
            PriorityQueueTests.CheckContains<int>(new int[] { 7, -1, 10 }, 5, PriorityQueueType.Min, false);
        }

        [TestMethod]
        public void ContainsForNonExistingVlaueInMaxQueueShouldReturnFalse()
        {
            PriorityQueueTests.CheckContains<int>(new int[] { 7, -1, 10 }, 5, PriorityQueueType.Max, false);
        }

        [TestMethod]
        public void ContainsForExistingVlaueInMinQueueShouldReturnTrue()
        {
            PriorityQueueTests.CheckContains<int>(new int[] { 7, -1, 10 }, 7, PriorityQueueType.Min, true);
        }

        [TestMethod]
        public void ContainsForExistingVlaueInMaxQueueShouldReturnTrue()
        {
            PriorityQueueTests.CheckContains<int>(new int[] { 7, -1, 10 }, 7, PriorityQueueType.Max, true);
        }

        #endregion

        #region Custom type tests

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

        [TestMethod]
        public void ContainsForNonExistingVlaueInMinQueueShouldReturnFalseCustom()
        {
            var values = PriorityQueueTests.CreateCustomValues().ToList();
            var value = new WeightedUri(values[values.Count / 2].Host, -1);
            PriorityQueueTests.CheckContains<WeightedUri>(values, value, PriorityQueueType.Min, false);
        }

        [TestMethod]
        public void ContainsForNonExistingVlaueInMaxQueueShouldReturnFalseCustom()
        {
            var values = PriorityQueueTests.CreateCustomValues().ToList();
            var value = new WeightedUri(values[values.Count / 2].Host, -1);
            PriorityQueueTests.CheckContains<WeightedUri>(values, value, PriorityQueueType.Max, false);
        }

        [TestMethod]
        public void ContainsForExistingVlaueInMinQueueShouldReturnTrueCustom()
        {
            var values = PriorityQueueTests.CreateCustomValues().ToList();
            var value = new WeightedUri(new Uri("https://www.nonexistingvalue.com"), values[values.Count / 2].Weight);
            PriorityQueueTests.CheckContains<WeightedUri>(values, value, PriorityQueueType.Min, true);
        }

        [TestMethod]
        public void ContainsForExistingVlaueInMaxQueueShouldReturnTrueCustom()
        {
            var values = PriorityQueueTests.CreateCustomValues().ToList();
            var value = new WeightedUri(new Uri("https://www.nonexistingvalue.com"), values[values.Count / 2].Weight);
            PriorityQueueTests.CheckContains<WeightedUri>(values, value, PriorityQueueType.Max, true);
        }
        
        #endregion

        [TestMethod]
        public void PriorityQueueExample()
        {
            var elements = new List<WeightedUri>()
            {
                new WeightedUri(new Uri("https://www.test2.com"), 2),
                new WeightedUri(new Uri("https://www.test1.com"), 1),
                new WeightedUri(new Uri("https://www.test3.com"), 3),
            };

            // Create with original seed of elements
            var queue = new PriorityQueue<WeightedUri>(elements, PriorityQueueType.Max);

            var existing = new WeightedUri(new Uri("https://www.test1.com"), 1);
            Console.WriteLine("Contains=" + queue.Contains(existing));

            // IEnumerable example
            Console.WriteLine("Enumerating:");
            foreach (var element in queue)
            {
                Console.WriteLine(element);
            }

            // Enqueue element with the duplicate max weight
            queue.Enqueue(new WeightedUri(new Uri("https://www.testduplicate.com"), 3));

            Console.WriteLine("Count=" + queue.Count + " Empty=" + queue.Empty);

            // Peek top of the queue
            Console.WriteLine("Peek: " + queue.Peek());

            // Draining queue
            while (!queue.Empty)
            {
                Console.WriteLine("Dequeue: " + queue.Dequeue());
            }

            Console.WriteLine("Count=" + queue.Count + " Empty=" + queue.Empty);
        }
    }
}
