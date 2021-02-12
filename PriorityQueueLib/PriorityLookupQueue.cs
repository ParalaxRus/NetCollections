using System;
using System.Collections.Generic;

namespace PriorityQueueLib
{
    /// <summary>Binary heap base implementation of the priority queue with the fast look up.</summary>
    public class PriorityLookupQueue<T> : PriorityQueue<T> where T : IComparable<T>
    {
        /// <summary>Lookup table.</summary>
        private Dictionary<T, int> lookup = new Dictionary<T, int>();

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

        public PriorityLookupQueue(PriorityQueueType type = PriorityQueueType.Min) : base(type)
        {    
        }

        public PriorityLookupQueue(IEnumerable<T> values, PriorityQueueType type) : this(type)
        {
            foreach (var val in values)
            {
                this.LookupAdd(val);
            }
        }

        /// <summary>Removes element from the top of the queue.</summary>
        /// <timecomplexity>O(h) in the worst case where h is height of the binary tree.</timecomplexity>
        public override T Top()
        {
            var value = base.Top();

            this.LookupRemove(value);

            return value;
        }

        /// <summary>Adds value to the queue.</summary>
        /// <timecomplexity>O(h) in the worst case where h is height of the binary tree.</timecomplexity>
        public override void Add(T value)
        {
            base.Add(value);
            
            this.LookupAdd(value);
        }

        /// <summary>Checks whether queue contains specified value or not.</summary>
        /// <timecomplexity>O(1)</timecomplexity>
        public bool Contains(T value)
        {
            return this.lookup.ContainsKey(value);
        }
    }
}
