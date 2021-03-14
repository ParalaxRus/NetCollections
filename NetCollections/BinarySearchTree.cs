using System;
using System.Collections;
using System.Collections.Generic;

namespace NetCollections
{
    /// <summary>Self balancing binary search tree.</summary>
    /// <remarks>Allows duplicates.<remarks>
    public class BinarySearchTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        #region Details

        Node root = null;

        /// <summary>Binary tree node class.</summary>
        private class Node
        {
            public T Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
            /// <summary>Parent is needed because most of the methods are iterative.</summary>
            public Node Parent { get; set; }
            public int Height { get; set; }
            public int Count { get; set; }

            /// <summary>Gets children count.</summary>
            public int Children
            {
                get
                {
                    int left = this.Left != null ? 1 : 0;
                    int right = this.Right != null ? 1 : 0;

                    return left + right;
                }
            }

            public Node(T value)
            {
                this.Value = value;
                this.Left = null;
                this.Right = null;
                this.Parent = null;
                this.Height = 0;
                this.Count = 1;
            }

            public Node(T value, Node parent) : this(value)
            {
                this.Parent = parent;
            }

            
        }

        /// <summary>Recursively checks wether tree is balanced or not.</summary>
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

        /// <summary>Iterative implementation of the in-order traversal.</summary>
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

        /// <summary>Recursively checks wether tree is balanced or not.</summary>
        private bool IsBalanced()
        {
            int height = 0;
            return BinarySearchTree<T>.IsBalanced(this.root, ref height);
        }

        /// <summary>Searches for a node with the given value.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        /// <returns>Node if successfull or null otherwise.</returns>
        private Node Find(T value)
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

        /// <summary>Adds node iteratively.</summary>
        private Node AddNode(T value)
        {
            if (this.root == null)
            {
                this.root = new Node(value);
                return this.root;
            }

            int compare = 0;
            Node parent = null;
            var current = this.root;
            while (current != null)
            {
                compare = value.CompareTo(current.Value);
                if (compare == 0)
                {
                    ++current.Count;
                    return current;
                }

                parent = current;
                if (compare > 0)
                {
                    current = current.Right;
                }
                else
                {
                    current = current.Left;
                }
            }

            var node = new Node(value, parent);

            if (compare > 0)
            {
                parent.Right = node;
            }
            else
            {
                parent.Left = node;
            }
            
            return node;
        }

        #region Remove

        /// <summary>Removes leaf node.</summary>
        private void RemoveLeaf(Node node)
        {
            if (node == this.root)
            {
                this.root = null;
                return;
            }

            if (node.Parent.Left == node)
            {
                node.Parent.Left = null;
            }
            else
            {
                node.Parent.Right = null;
            }
        }

        /// <summary>Removes node with the single child.</summary>
        private void RemoveOneChild(Node node)
        {
            var child = node.Left != null ? node.Left : node.Right;

            if (node == this.root)
            {
                this.root = child;
                return;
            }

            if (node.Parent.Left == node)
            {
                node.Parent.Left = child;
            }
            else
            {
                node.Parent.Right = child;
            }
        }

        /// <summary>Removes node with both children.</summary>
        private void RemoveTwoChildren(Node node)
        {
            
        }

        /// <summary>Removes node iteratively.</summary>
        private void Remove(Node node)
        {
            int children = node.Children;
            if (children == 0)
            {
                this.RemoveLeaf(node);
            }
            else if (children == 1)
            {
                this.RemoveOneChild(node);
            }
            else
            {
                this.RemoveTwoChildren(node);
            }
        }

        /// <summary>Removes node iteratively.</summary>
        private Node RemoveNode(T value)
        {
            var node = this.Find(value);
            if (node == null)
            {
                return null;
            }

            --node.Count;
            if (node.Count != 0)
            {
                return node;
            }

            this.Remove(node);

            return node;
        }

        #endregion

        /// <summary>Performs bottom up tree balancing starting with the specified node.</summary>
        private void Balance(Node node)
        {
            
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
        public void Add(T value)
        {
            var node = this.AddNode(value);

            this.Balance(node);

            ++this.Count;
        }

        /// <summary>Removes value from the tree.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        public bool Remove(T value)
        {
            var node = this.RemoveNode(value);
            if (node == null)
            {
                // Value does not exist
                return false;
            }

            this.Balance(node);

            --this.Count;

            return true;
        }

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
