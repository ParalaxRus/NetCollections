using PriorityQueueLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PriorityQueueTests
{
    [TestClass]
    public class PriorityLookupQueueTests
    {
        /*[TestMethod]
        public void EmptyQueueShouldNotContainValue()
        {
            var queue = new PriorityLookupQueue<int>();

            Assert.IsFalse(queue.Contains(0));
        }

        [TestMethod]
        public void CallContainsForNonExistingValueShouldReturnFalse()
        {
            var queue = new PriorityLookupQueue<int>();
            queue.Add(5);

            Assert.IsFalse(queue.Contains(0));
        }

        [TestMethod]
        public void CallContainsForExistingValueShouldReturnTrue()
        {
            var queue = new PriorityLookupQueue<int>();
            queue.Add(5);

            Assert.IsTrue(queue.Contains(5));
        }

        [TestMethod]
        public void AddingSameValueMultipleTwiceShouldReturnContainsTrueUntilAllDuplicatesInTheQueue()
        {
            var queue = new PriorityLookupQueue<int>();

            const int dups = 4;

            for (int i = 0; i < dups; ++i)
            {
                queue.Add(7);
            }

            for (int i = 0; i < dups; ++i)
            {
                Assert.IsTrue(queue.Contains(7));

                queue.Top();
            }

            Assert.IsFalse(queue.Contains(7));
        }*/
    }
}
