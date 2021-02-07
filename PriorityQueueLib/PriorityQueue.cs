using System;
using System.Collections.Generic;

namespace PriorityQueueLib
{
    /// <summary>Binary heap base implementation of the priority queue.</summary>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> values = new List<T>();

        /// <summary>Min queue by default.</summary>
        private Type type = Type.Min;

        private void Heapify()
        {

        }

        /// <summary>Gets index of the parent element.</summary>
        private int GetParent(int index)
        {
            return (index - 1) / 2;
        }

        /// <summary>Gets index of the left child.</summary>
        private int GetLeft(int index)
        {
            return (index * 2) + 1;
        }

        /// <summary>Gets index of the right child.</summary>
        private int GetRight(int index)
        {
            return (index * 2) + 2;
        }

        /// <summary>Gets index of the last element in the queue.</summary>
        private int GetLast()
        {
            return this.Count - 1;
        }

        /// <summary>Swaps two elements in the queue.</summary>
        private void Swap(int i, int j)
        {
            if (i == j)
            {
                return;
            }

            T tmp = this.values[i];
            this.values[i] = this.values[j];
            this.values[j] = tmp;
        }

        /// <summary>Downheap current element once if possible.</summary>
        /// <returns>New index of the current element if success or same index otherwise.</returns>
        private int DownHeap(int i)
        {
            var current = this.values[i];

            var l = this.GetLeft(i);
            l = l < this.Count ? l : i;
            var left = this.values[l];

            var r = this.GetRight(i);
            r = r < this.Count ? r : i;
            var right = this.values[r];

            // MinHeap: looking for a smaller between two children
            int n = 0;
            var next = default(T);
            if (left.CompareTo(right) < 0)
            {
                n = l;
                next = left;
            }
            else
            {
                n = r;
                next = right;
            }

            // Min heap: swapping with the smallest if bigger or with itself
            if (current.CompareTo(next) > 0)
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

        public enum Type 
        {
            Min = 0,
            Max
        }

        public PriorityQueue(Type type = Type.Min)
        {    
            this.QueueType = type;
        }

        public PriorityQueue(IEnumerable<T> values) : this()
        {
            this.values.AddRange(values);

            this.Heapify();
        }

        /// <summary>Peeks first element.</summary>
        public T Peek()
        {
            if (this.Empty)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return this.values[0];
        }

        /// <summary>Removes element from the top of the queue.</summary>
        public T Top()
        {
            // Storing top
            var value = this.Peek();

            // Moving last to the top
            int last = this.GetLast();
            this.values[0] = this.values[last];

            // Removing last
            this.values.RemoveAt(this.Count - 1);

            // Performing down heap until possible
            int current = 0; 
            int next = 0;
            do
            {
                next = this.DownHeap(current);
            }
            while (next != current);
            
            return value;
        }

        /// <summary>Adds value to the queue.</summary>
        public void Add(T value)
        {
            this.values.Add(value);

            int current = this.GetLast();
            int parent = current;
            while (parent != current)
            {
                parent = this.GetParent(current);

                if (this.values[parent].CompareTo(this.values[current]) >= 0)
                {
                    // Stopping up head because parent is smaller and its a min heap
                    break;
                }

                this.Swap(parent, current);
                current = parent;
            }
        }

        /// <summary>Gets queue size.</summary>
        public int Count { get { return this.values.Count; }}

        /// <summary>Checks whether queue is empty or not.</summary>
        public bool Empty { get {return (this.Count == 0); }}

        /// <summary>Gets queue type.</summary>
        public Type QueueType { get; private set; }
    }
}
