using System;
using System.Collections;
using System.Collections.Generic;

namespace PriorityQueueLib
{
    /// <summary>Binary heap base implementation of the priority queue with the fast look up.</summary>
    /// <summary>Does not support duplicate values.</summary>
    public class PriorityLookupQueue<K, V> : PriorityQueueBase, IEnumerable<KeyValuePair<K,V>> where K : IComparable<K>
    {
        #region Details

        /// <summary>Array of values as per binary heap design (priority, value).</summary>
        private List<KeyValuePair<K, V>> array = new List<KeyValuePair<K, V>>();

        /// <summary>Lookup table (value, array index).</summary>
        private Dictionary<V, int> lookup = new Dictionary<V, int>();

        /// <summary>Adds priority/value pair to the end of array.</summary>
        private void AddLast(K priority, V value)
        {
            if (this.lookup.ContainsKey(value))
            {
                throw new InvalidOperationException(string.Format("Value {0} already exists", value));
            }

            // Adding to array first so lookup index should exist
            this.array.Add(new KeyValuePair<K, V>(priority, value));
            this.lookup.Add(value, this.GetLast());
        }

        /// <summary>Removes last element.</summary>
        private void RemoveLast()
        {
            if (this.Empty)
            {
                throw new ArgumentException("Queue is empty");
            }

            int index = this.GetLast();

            var value = this.array[index].Value;
            if (!this.lookup.ContainsKey(value))
            {
                throw new InvalidOperationException(string.Format("Value {0} does not exist", value));
            }

            this.array.RemoveAt(index);
            this.lookup.Remove(value);
        }

        /// <summary>Swaps two elements in the queue.</summary>
        private void Swap(int i, int j)
        {
            if (i == j)
            {
                return;
            }

            // Swapping lookup indices
            var first = this.array[i].Value;
            var second = this.array[j].Value;
            this.lookup[first] = j;
            this.lookup[second] = i;
            
            // Swaping heap elements
            var kvp = this.array[i];
            this.array[i] = this.array[j];
            this.array[j] = kvp;
        }

        /// <summary>Moves value at the specified index one step down if possible.</summary>
        /// <returns>New index of the current element if success or same index otherwise.</returns>
        protected override int MoveDown(int i)
        {
            var l = this.GetLeft(i);
            l = l < this.Count ? l : i;
            var left = this.array[l].Key;

            var r = this.GetRight(i);
            r = r < this.Count ? r : i;
            var right = this.array[r].Key;

            // MinHeap/MaxHeap: looking for a smaller/bigger between two children
            int n = 0;
            var next = default(K);
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

            var current = this.array[i].Key;

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

        #endregion

        /// <summary>Gets queue size.</summary>
        public override int Count { get { return this.array.Count; }}

        /// <summary>Checks whether queue is empty or not.</summary>
        public override bool Empty { get { return (this.Count == 0); }}

        /// <summary>Default constructor.</summary>
        public PriorityLookupQueue(PriorityQueueType type = PriorityQueueType.Min) : base(type)
        {    
        }

        /// <summary>Constructor with the specified input values.</summary>
        /// <timecomplexity>Better than O(NlogN) see https://en.wikipedia.org/wiki/Binary_heap.</timecomplexity>
        /// <remarks>Should not call base class PriorityQueue(IEnumerable) ctor 
        /// because lookup indices will be out of sync.</remarks>
        public PriorityLookupQueue(IEnumerable<K> keys, IEnumerable<V> values, PriorityQueueType type) : this(type)
        {
            if ((keys == null) || (values == null))
            {
                throw new ArgumentNullException();
            }

            var keyEnumerator = keys.GetEnumerator();
            var valueEnumerator = values.GetEnumerator();
            
            while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
            {
                var key = keyEnumerator.Current;
                var value = valueEnumerator.Current;

                this.AddLast(key, value);
            }

            if ( keyEnumerator.MoveNext() || valueEnumerator.MoveNext() )
            {
                throw new ArgumentException("Keys and values should be of the same length");
            }

            this.Heapify();
        }

        /// <summary>Peeks first element.</summary>
        /// <timecomplexity>O(1)</timecomplexity>
        public KeyValuePair<K,V> Peek()
        {
            if (this.Empty)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return this.array[0];
        }

        /// <summary>Removes element from the top of the queue.</summary>
        /// <timecomplexity>O(h) in the worst case where h is height of the binary tree.</timecomplexity>
        public KeyValuePair<K,V> Dequeue()
        {
            // Storing top
            var value = this.Peek();

            // Moving last to the top
            int last = this.GetLast();
            this.Swap(0, last);

            this.RemoveLast();

            if (!this.Empty)
            {
                this.BubbleDown(0);
            }
            
            return value;
        }

        /// <summary>Adds priority and value to the queue.</summary>
        /// <timecomplexity>O(h) in the worst case where h is height of the binary tree.</timecomplexity> 
        public void Enqueue(K priority, V value)
        {
            this.AddLast(priority, value);

            int current = this.GetLast();
            while (current != 0)
            {
                int parent = this.GetParent(current);

                var parentPriority = this.array[parent].Key;
                var currentPriority = this.array[current].Key;

                if ( (parentPriority.CompareTo(currentPriority) * this.ComparisonSign) <= 0)
                {
                    // parent is smaller and its a min heap or 
                    // parent is bigger ant its a max heap
                    break;
                }

                this.Swap(parent, current);

                current = parent;
            }
        }

        /// <summary>Sets new priority for the specified value.</summary>
        /// <remarks>Equivalent to increase/decrease key operator defined for a binary heap.</remarks>
        /// <timecomplexity>O(h) where h is height of the binary tree.</timecomplexity>
        /// <returns>Old priority.</returns>
        public K SetPriority(V value, K priority)
        {
            if (!this.ContainsValue(value))
            {
                throw new ArgumentException(string.Format("Value {0} does not exist", value));
            }

            int index = this.lookup[value];
            K oldPriority = this.array[index].Key;

            if (priority.CompareTo(oldPriority) == 0)
            {
                return oldPriority;
            }

            // To change priority without BubbleUp() implementation:
            // 1) Replace current element with the last
            // 2) Remove last
            // 3) Bubble down current
            // 4) Enqueue new element
            // In the worst case its O(2h) complexity

            this.Swap(index, this.GetLast());
            this.RemoveLast();
            if (index < this.Count)
            {
                this.BubbleDown(index);
            }
            this.Enqueue(priority, value);

            return oldPriority;
        }

        /// <summary>Gets specified value priority.</summary>
        /// <timecomplexity>O(1)</timecomplexity>
        public K GetPriority(V value)
        {
            if (!this.ContainsValue(value))
            {
                throw new ArgumentException(string.Format("Value {0} does not exist", value));
            }

            int index = this.lookup[value];
            return this.array[index].Key;
        }

        /// <summary>Checks whether queue contains specified value or not.</summary>
        /// <timecomplexity>O(1)</timecomplexity>
        public bool ContainsValue(V value)
        {
            return this.lookup.ContainsKey(value);
        }

        #region IEnumerable

        public IEnumerator<KeyValuePair<K,V>> GetEnumerator()
        {
            return this.array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.array.GetEnumerator();
        }

        #endregion
    }
}
