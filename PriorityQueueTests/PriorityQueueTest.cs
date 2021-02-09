using System;
using PriorityQueueLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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

        private static bool IsHeap<T>(PriorityQueue<int> queue)
        {
            foreach (var value in queue)
            {
                // HERE !!!
            }

            return true;
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
    }
}
