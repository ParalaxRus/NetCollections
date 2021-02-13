using System;
using System.Collections.Generic;

namespace PriorityQueueLib
{
    /// <summary>Priority heap type.</summary>
    public enum PriorityQueueType 
    {
        Min,
        Max
    }
    /// <summary>Priority queue interface.</summary>
    public interface IPriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
    {
        #region Properties

        /// <summary>Gets queue size.</summary>
        public int Count { get; }

        /// <summary>Checks whether queue is empty or not.</summary>
        public bool Empty { get; }

        /// <summary>Gets queue type.</summary>
        public PriorityQueueType QueueType { get; }

        #endregion

        #region Methods

        /// <summary>Peeks first element.</summary>
        public T Peek();

        /// <summary>Removes element from the top of the queue.</summary>
        public T Top();

        /// <summary>Adds value to the queue.</summary>
        public void Add(T value);

        /// <summary>Checks whether queue contains specified value or not.</summary>
        public bool Contains(T value);

        /// <summary>Changes value of the existing element while preserving heap property.</summary>
        /// <remarks>Equivalent to increase/decrease key operator defined for a binary heap.</remarks>
        /// <returns>Returns true if the value has been updated otherwise false.</returns>
        public bool Update(T oldValue, T newValue);

        #endregion
    }
}
