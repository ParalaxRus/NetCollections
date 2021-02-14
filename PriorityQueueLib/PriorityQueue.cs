using System;
using System.Collections;
using System.Collections.Generic;

namespace PriorityQueueLib
{
    /// <summary>Binary heap base implementation of the priority queue.</summary>
    public class PriorityQueue<T> : PriorityQueueBase, IEnumerable<T> where T : IComparable<T>
    {
        #region Details

        /// <summary>Array of values as per binary heap design.</summary>
        private List<T> array = new List<T>();

        /// <summary>Moves value at the specified index one step down if possible.</summary>
        /// <returns>New index of the current element if success or same index otherwise.</returns>
        protected override int MoveDown(int i)
        {
            var l = this.GetLeft(i);
            l = l < this.Count ? l : i;
            var left = this.array[l];

            var r = this.GetRight(i);
            r = r < this.Count ? r : i;
            var right = this.array[r];

            // MinHeap/MaxHeap: looking for a smaller/bigger between two children
            int n = 0;
            var next = default(T);
            if ( (left.CompareTo(right) * this.ComparisonSign) < 0 )
            {
                n = l;
                next = left;
            }
            else
            {
                n = r;
                next = right;
            }

            var current = this.array[i];

            // MinHeap: swapping with the smallest if its bigger than current
            // MaxHeap: swapping with the bigger if its smaller than current
            if ( (current.CompareTo(next) * this.ComparisonSign) > 0 )
            {
                this.Swap(i, n);
            }
            else
            {
                // Next is current because no swap happened and downheap should be stopped
                n = i;
            }

            return n;
        }

        /// <summary>Finds specified value in the queue if any.</summary>
        private int Find(T value)
        {
            for (int i = 0; i < this.array.Count; ++i)
            {
                if (value.CompareTo(this.array[i]) == 0)
                {
                    return i;
                }
            }

            return PriorityQueue<T>.NotFound;
        }

        /// <summary>Swaps two elements in the queue.</summary>
        private void Swap(int i, int j)
        {
            if (i == j)
            {
                return;
            }

            T tmp = this.array[i];
            this.array[i] = this.array[j];
            this.array[j] = tmp;
        }

        #endregion

        /// <summary>Gets queue size.</summary>
        public override int Count { get { return this.array.Count; }}

        /// <summary>Checks whether queue is empty or not.</summary>
        public override bool Empty { get {return (this.Count == 0); }}

        public PriorityQueue(PriorityQueueType type = PriorityQueueType.Min) : base(type)
        {    
        }

        public PriorityQueue(IEnumerable<T> values, PriorityQueueType type) : base(type)
        {
            this.array.AddRange(values);

            this.Heapify();
        }

        /// <summary>Peeks first element.</summary>
        /// <timecomplexity>O(1)</timecomplexity>
        public T Peek()
        {
            if (this.Empty)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return this.array[0];
        }

        /// <summary>Removes element from the top of the queue.</summary>
        /// <timecomplexity>O(h) in the worst case where h is height of the binary tree.</timecomplexity>
        public T Dequeue()
        {
            // Storing top
            var value = this.Peek();

            // Moving last to the top
            int last = this.GetLast();
            this.Swap(0, last);

            // Removing last
            this.array.RemoveAt(this.Count - 1);

            if (!this.Empty)
            {
                this.BubbleDown(0);
            }
            
            return value;
        }

        /// <summary>Adds value to the queue.</summary>
        /// <timecomplexity>O(h) in the worst case where h is height of the binary tree.</timecomplexity> 
        public void Enqueue(T value)
        {
            this.array.Add(value);

            int current = this.GetLast();
            while (current != 0)
            {
                int parent = this.GetParent(current);

                if ( (this.array[parent].CompareTo(this.array[current]) * this.ComparisonSign) <= 0)
                {
                    // Parent is smaller and its a min heap
                    break;
                }

                this.Swap(parent, current);

                current = parent;
            }
        }

        /// <summary>Checks whether queue contains specified value or not.</summary>
        /// <timecomplexity>O(n) in the worst case where n is length of the queue.</timecomplexity>
        public bool Contains(T value)
        {
            return (this.Find(value) != PriorityQueue<T>.NotFound);
        }

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return this.array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
