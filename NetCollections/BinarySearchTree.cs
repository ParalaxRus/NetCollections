using System;
using System.Collections;
using System.Collections.Generic;

namespace NetCollections
{
    /// <summary>Self balancing binary serach tree.</summary>
    public class BinarySearchTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        #region Details

        Node root = null;

        private class Node
        {
            public T Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }

            public Node(T value)
            {
                this.Value = value;
            }
        }

        private static bool IsBalanced(Node node, ref int height)
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

        /// <summary>Iterative implementation of the inorder traversal.</summary>
        private IEnumerable<T> InOrder()
        {
            if (this.root == null)
            {
                yield break;
            }

            var stack = new Stack<Node>();

            var node = this.root;
            while (node != null)
            {
                stack.Push(node);
                node = node.Left;
            }

            while (stack.Count != 0)
            {
                node = stack.Pop();

                yield return node.Value;
                
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

        private bool IsBalanced()
        {
            int height = 0;
            return BinarySearchTree<T>.IsBalanced(this.root, ref height);
        }

        #endregion

        /// <summary>Gets tree size.</summary>
        public int Count { get; private set; }

        /// <summary>Checks whether tree is empty or not.</summary>
        public bool Empty { get {return (this.Count == 0); }}

        /// <summary>Default constructor.</summary>
        public BinarySearchTree()
        {
        }

        /// <summary>Constructor with the specified input values.</summary>
        /// <timecomplexity>O(NlogN).</timecomplexity>
        public BinarySearchTree(IEnumerable<T> values)
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

        public void Add(T value)
        {
            ++this.Count;
        }

        public void Remove(T value)
        {
            --this.Count;

            if (this.Empty)
            {
                this.root = null;
            }
        }

        /// <summary>Checks wether tree contains specified value or not.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        public bool Contains(T value)
        {
            var node = this.root;

            if (node != null)
            {
                int compare = value.CompareTo(node.Value);
                if (compare == 0)
                {
                    return true;
                }
                else if (compare > 0)
                {
                    node = node.Right;
                }
                else
                {
                    node = node.Left;
                }
            }

            return false;
        }
        
        #region IEnumerable

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
