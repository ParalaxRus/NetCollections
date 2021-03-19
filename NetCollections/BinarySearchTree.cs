using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("NetCollectionsTests")]

namespace NetCollections
{
    /// <summary>Balanced binary search tree abstract class.</summary>
    public abstract class BinarySearchTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        #region Details

        protected Node<T> root = null;

        /// <summary>Subtree type of heaviness.</summary>
        protected enum HeavinessType
        {
            LeftLeft,
            LeftRight,
            RightLeft,
            RightRight
        }

        /// <summary>Recursively checks wether tree is balanced or not.</summary>
        protected static bool IsBalanced(Node<T> node, ref int height)
        {
            if (node == null)
            {
                height = 0;
                return true;
            }

            int left = 0;
            if (!BinarySearchTree<T>.IsBalanced(node.Left, ref left))
            {
                return false;
            }

            int right = 0;
            if (!BinarySearchTree<T>.IsBalanced(node.Right, ref right))
            {
                return false;
            }

            if (Math.Abs(left - right) > 1)
            {
                return false;
            }

            height = left > right ? left + 1: right + 1;

            return true;
        }

        /// <summary>Recursively checks wether tree is a valid binary search tree or not.</summary>
        /// <remarks>Algorithm is based on the fact that each previous value in 
        /// in-order taraversal should be less than current.</remarks>
        protected static bool IsValid(Node<T> n, ref bool found, ref T min)
        {
            if (n == null)
            {   
                return true;
            }

            if (!BinarySearchTree<T>.IsValid(n.Left, ref found, ref min))
            {
                return false;
            }

            if ( (n.Left == null) && !found )
            {
                // Min node found
                found = true;
                min = n.Value;
            }
            else
            {
                // Previous value (min) in in-order traversal should be less than current value
                int compare = n.Value.CompareTo(min);
                if (compare <= 0)
                {
                    return false;
                }
            }

            // Next previous value (min)
            min = n.Value;
            return BinarySearchTree<T>.IsValid(n.Right, ref found, ref min);
        }

        protected static int GetNodesCount(Node<T> node)
        {
            if (node == null)
            {
                return 0;
            }

            return BinarySearchTree<T>.GetNodesCount(node.Left) + 
                   BinarySearchTree<T>.GetNodesCount(node.Right) + 1;
        }

        /// <summary>Iterative implementation of the in-order traversal.</summary>
        protected IEnumerable<T> InOrder()
        {
            if (this.root == null)
            {
                yield break;
            }

            var stack = new Stack<Node<T>>();

            var node = this.root;
            while (node != null)
            {
                stack.Push(node);
                node = node.Left;
            }

            while (stack.Count != 0)
            {
                node = stack.Pop();

                // Duplicates support
                for (int i = 0; i < node.Count; ++i)
                {
                    yield return node.Value;
                }
                
                if (node.Right == null)
                {
                    continue;
                }

                node = node.Right;
                while (node != null)
                {
                    stack.Push(node);
                    node = node.Left;
                }
            }
        }

        /// <summary>Searches for a node with the given value.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        /// <returns>Node if successfull or null otherwise.</returns>
        protected Node<T> Find(T value)
        {
            var node = this.root;

            while (node != null)
            {
                int compare = value.CompareTo(node.Value);
                if (compare == 0)
                {
                    break;
                }

                if (compare > 0)
                {
                    node = node.Right;
                }
                else
                {
                    node = node.Left;
                }
            }

            return node;
        }

        /// <summary>Gets height of the specified node.</summary>
        protected static int GetHeight(Node<T> node)
        {
            return node != null ? node.Height : 0;
        }

        /// <summary>Updates height of the specified node.</summary>
        protected static void UpdateHeight(Node<T> node)
        {
            if (node.Children == 0)
            {
                node.Height = 0;
                return;
            }

            int left = BinarySearchTree<T>.GetHeight(node.Left);
            int right = BinarySearchTree<T>.GetHeight(node.Right);

            node.Height = Math.Max(left, right) + 1;
        }

        /// <summary>Gets in-order successor.</summary>
        protected static Node<T> GetInorderSuccessor(Node<T> node)
        {
            if (node.Right == null)
            {
                throw new ArgumentNullException();
            }

            node = node.Right;
            while (node.Left != null)
            {
                node = node.Left;
            }

            return node;
        }

        /// <summary>Gets heaviness type of the subtree rooted with the specified node and balance factor.</summary>
        /// <remarks>Should be called for unbalanced subtrees so factor does not belong to [-1, 1].</remarks>
        protected static HeavinessType GetHeaviness(Node<T> node, int factor)
        {
            if (factor > 1)
            {
                // Left heavy
                factor = BinarySearchTree<T>.GetFactor(node.Left);

                return factor > 0 ? HeavinessType.LeftLeft : HeavinessType.LeftRight;
            }
            else
            {
                // Right heavy
                factor = BinarySearchTree<T>.GetFactor(node.Right);

                return factor < 0 ? HeavinessType.RightRight : HeavinessType.RightLeft;
            }
        }

        /// <summary>Calculates specified node's balance factor.</summary>
        protected static int GetFactor(Node<T> node)
        {
            int left = node.Left != null ? BinarySearchTree<T>.GetHeight(node.Left) + 1 : 0;
            int right = node.Right != null ? BinarySearchTree<T>.GetHeight(node.Right) + 1 : 0;

            return (left - right);
        }

        /// <summary>Removes value from the tree.</summary>
        /// <remarks>Returns node be removed if successfull or null if value does not exist.</remarks>
        protected Node<T> RemoveNode(T value)
        {
            var node = this.Find(value);
            if (node == null)
            {
                // Value does not exist
                return null;
            }

            --node.Count;
            --this.Count;
            
            return node;
        }

        #endregion

        #region Internals for unit tests mostly

        /// <summary>Recursively checks wether tree is balanced or not.</summary>
        internal bool IsBalanced()
        {
            int height = 0;
            return BinarySearchTree<T>.IsBalanced(this.root, ref height);
        }

        /// <summary>Recursively checks wether tree is a valid binary search tree or not.</summary>
        internal bool IsValid()
        {
            bool found = false;
            var min = default(T);
            return BinarySearchTree<T>.IsValid(this.root, ref found, ref min);
        }

        /// <summary>Recursively gets nodes count.</summary>
        internal int GetNodesCount()
        {
            return BinarySearchTree<T>.GetNodesCount(this.root);
        }

        #endregion

        /// <summary>Gets tree size.</summary>
        /// <remarks>Includes duplicates count.</remarks>
        public int Count { get; private set; }

        /// <summary>Checks whether tree is empty or not.</summary>
        public bool Empty 
        {
             get { return (this.Count == 0); }
        }

        /// <summary>Gets tree height.</summary>
        public int Height 
        { 
            get { return this.root == null ? 0 : this.root.Height; } 
        }

        /// <summary>Default constructor.</summary>
        public BinarySearchTree()
        {
        }

        /// <summary>Constructor with the specified input values.</summary>
        /// <timecomplexity>O(NlogN).</timecomplexity>
        public BinarySearchTree(IEnumerable<T> values)
        {
            this.AddRange(values);
        }

        /// <summary>Adds range of values to the tree.</summary>
        /// <timecomplexity>O(KlogN).</timecomplexity>
        public void AddRange(IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var v in values)
            {
                this.Add(v);
            }
        }

        /// <summary>Adds value to the tree.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        public virtual void Add(T value)
        {
            ++this.Count;
        }

        /// <summary>Removes value from the tree.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        public abstract bool Remove(T value);

        /// <summary>Checks wether tree contains specified value or not.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        /// <returns>Pair of values: existence flag and duplicates count.</returns>
        public Tuple<bool, int> Contains(T value)
        {
            var node = this.Find(value);

            return node != null ? new Tuple<bool, int>(true, node.Count) : 
                                  new Tuple<bool, int>(false, 0);
        }
        
        #region IEnumerable

        /// <summary>Enumerates values as per in-order traversal.</summary>
        public IEnumerator<T> GetEnumerator()
        {
            return this.InOrder().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
