﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace PriorityQueueLib
{

    /// <summary>Binary heap base implementation of the priority queue with the fast look up.</summary>
    public class PriorityLookupQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        /// <summary>Array of values as per binary heap design.</summary>
        private List<T> values = new List<T>();

        /// <summary>Min queue by default.</summary>
        private PriorityQueueType type = PriorityQueueType.Min;

        /// <summary>Lookup table.</summary>
        private Dictionary<T, int> lookup = new Dictionary<T, int>();

        /// <summary>Gets comparison sign.</summary>
        /// <remarks>All the comparisons are done for the min heap by default.
        /// Thus for the max heap its needed to reverse all the comparisons.</remarks>
        private int ComparisonSign
        {
            get
            {
                return this.QueueType == PriorityQueueType.Min ? 1 : -1;
            }
        }

        /// <summary>Bubbles down value at the specified index 
        /// all the way down until possible.</summary>
        private void BubbleDown(int index)
        {
            int next = index;
            do
            {
                index = next;
                next = this.MoveDown(index);
            }               
            while (next != index);
        }

        private void Heapify()
        {
            for (int i = this.Count / 2; i >= 0; --i)
            {
                this.BubbleDown(i);
            }
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

        /// <summary>Moves value at the specified index one step down if possible.</summary>
        /// <returns>New index of the current element if success or same index otherwise.</returns>
        private int MoveDown(int i)
        {
            var current = this.values[i];

            var l = this.GetLeft(i);
            l = l < this.Count ? l : i;
            var left = this.values[l];

            var r = this.GetRight(i);
            r = r < this.Count ? r : i;
            var right = this.values[r];

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

        /// <summary>Adds value to the lookup table.</summary>       
        /// <returns>True if priority queue already has the same value or false otherwise.</returns>
        private bool LookupAdd(T value)
        {
            bool contained = true;

            if (!this.lookup.ContainsKey(value))
            {
                this.lookup.Add(value, 0);
                contained = false;
            }

            ++this.lookup[value];

            return contained;
        }

        /// <summary>Removes value from the lookup table.</summary>       
        /// <returns>True if last element with the same value has been removed or false otherwise.</returns>
        private bool LookupRemove(T value)
        {
            bool removed = false;

            if (!this.lookup.ContainsKey(value))
            {
                throw new ArgumentException(string.Format("Value {0} is not present", value));
            }

            --this.lookup[value];
            if (this.lookup[value] == 0)
            {
                this.lookup.Remove(value);
                removed = true;
            }

            return removed;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public PriorityLookupQueue(PriorityQueueType type = PriorityQueueType.Min)
        {    
            this.QueueType = type;
        }

        public PriorityLookupQueue(IEnumerable<T> values, PriorityQueueType type) : this(type)
        {
            this.values.AddRange(values);
            
            foreach (var val in values)
            {
                this.LookupAdd(val);
            }

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

            if (!this.Empty)
            {
                this.BubbleDown(0);
            }

            this.lookup.Remove(value);
            
            return value;
        }

        /// <summary>Adds value to the queue.</summary>
        public void Add(T value)
        {
            this.values.Add(value);
            this.LookupAdd(value);

            int current = this.GetLast();
            while (current != 0)
            {
                int parent = this.GetParent(current);

                if ( (this.values[parent].CompareTo(this.values[current]) * this.ComparisonSign) <= 0)
                {
                    // Parent is smaller and its a min heap
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
        public PriorityQueueType QueueType { get; private set; }
    }
}
