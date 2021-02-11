using System;
using System.Collections.Generic;
using PriorityQueueLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PriorityQueueTests
{
    [TestClass]
    public class PriorityQueueTests
    {
        private static void CheckQueue(PriorityQueue<int> queue, int count)
        {
            Assert.AreEqual(queue.Count, count);
            Assert.AreEqual(queue.Empty, count == 0);
        }

        private static void CheckHeap(PriorityQueue<int> queue)
        {
            var values = new List<int>();

            foreach (var value in queue)
            {
                values.Add(value);
            }

            int sign = queue.QueueType == PriorityQueue<int>.Type.Min ? 1 : -1;

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

        [TestMethod]
        public void DefaultCtorShouldCreateEmptyMinHeap()
        {
            var queue = new PriorityQueue<int>();

            Assert.AreEqual(queue.QueueType, PriorityQueue<int>.Type.Min);
            PriorityQueueTests.CheckQueue(queue, 0);
        }

        [TestMethod]
        public void CtorWithTypeShouldCreateEmptyHeapOfTheSpecifiedType()
        {
            var queue = new PriorityQueue<int>(PriorityQueue<int>.Type.Max);

            Assert.AreEqual(queue.QueueType, PriorityQueue<int>.Type.Max);
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
                queue.Top();
            };

            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public void AddElementShouldSetEmptyToFalseAndCountToOne()
        {
            var queue = new PriorityQueue<int>();
            queue.Add(1);

            PriorityQueueTests.CheckQueue(queue, 1);
        }

        [TestMethod]
        public void AddValuesToMinHeapShouldComformMinHeapPredicate()
        {
            var queue = new PriorityQueue<int>();

            var rand = new Random();
            var values = new byte[10];
            rand.NextBytes(values);

            foreach (var value in values)
            {
                queue.Add(value);
            }

            PriorityQueueTests.CheckQueue(queue, values.Length);
            PriorityQueueTests.CheckHeap(queue);
        }
    }
}
