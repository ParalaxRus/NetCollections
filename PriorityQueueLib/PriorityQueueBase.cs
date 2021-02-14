using System.Collections;

namespace PriorityQueueLib
{
    public enum PriorityQueueType 
    {
        Min,
        Max
    }

    /// <summary>Priority queue base class.</summary>
    public abstract class PriorityQueueBase
    {
        /// <summary>A value indicating that element is not present in the heap.</summary>
        protected const int NotFound = -1;

        /// <summary>Gets comparison sign.</summary>
        /// <remarks>All the comparisons are done for the min heap by default.
        /// Thus for the max heap its needed to reverse all the comparisons.</remarks>
        protected int ComparisonSign
        {
            get
            {
                return this.QueueType == PriorityQueueType.Min ? 1 : -1;
            }
        }

        /// <summary>Gets index of the parent element.</summary>
        protected int GetParent(int index)
        {
            return (index - 1) / 2;
        }

        /// <summary>Gets index of the left child.</summary>
        protected int GetLeft(int index)
        {
            return (index * 2) + 1;
        }

        /// <summary>Gets index of the right child.</summary>
        protected int GetRight(int index)
        {
            return (index * 2) + 2;
        }

        /// <summary>Gets index of the last element in the queue.</summary>
        protected int GetLast()
        {
            return this.Count - 1;
        }

        /// <summary>Bubbles down value at the specified index 
        /// all the way down until possible.</summary>
        protected void BubbleDown(int index)
        {
            int next = index;
            do
            {
                index = next;
                next = this.MoveDown(index);
            }               
            while (next != index);
        }

        /// <summary>Builds binary heap in a bottom up manner.</summary>
        protected void Heapify()
        {
            for (int i = this.Count / 2; i >= 0; --i)
            {
                this.BubbleDown(i);
            }
        }

        /// <summary>Moves value at the specified index one step down if possible.</summary>
        /// <returns>New index of the current element if success or same index otherwise.</returns>
        protected abstract int MoveDown(int i);

        protected PriorityQueueBase(PriorityQueueType type)
        {    
            this.QueueType = type;
        }

        /// <summary>Gets queue size.</summary>
        public abstract int Count { get; }

        /// <summary>Checks whether queue is empty or not.</summary>
        public abstract bool Empty { get; }

        /// <summary>Gets queue type.</summary>
        public PriorityQueueType QueueType { get; }
    }
}
