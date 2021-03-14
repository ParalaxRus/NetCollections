﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace NetCollections
{
    /// <summary>Self balancing binary search tree.</summary>
    /// <remarks>Allows duplicates.<remarks>
    public partial class BinarySearchTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        #region Details

        /// <summary>Subtree type of heaviness.</summary>
        private enum HeavinessType
        {
            LeftLeft,
            LeftRight,
            RightLeft,
            RightRight
        }

        Node root = null;

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

        /// <summary>Get height of the specified node.</summary>
        private static int GetHeight(Node node)
        {
            return node != null ? node.Height : 0;
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
        private void RemoveWithOneChild(Node node)
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

        /// <summary>Gets in-order successor.</summary>
        /// <remarks>Node should have right subtree.</remarks>
        private Node GetInorderSuccessor(Node node)
        {
            node = node.Right;
            while (node.Left != null)
            {
                node = node.Left;
            }

            return node;
        }

        /// <summary>Removes node with both children.</summary>
        private void RemoveWithTwoChildren(Node node)
        {
            var min = this.GetInorderSuccessor(node);
            
            node.Value = min.Value;

            this.RemoveWithOneChild(min);
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
                this.RemoveWithOneChild(node);
            }
            else
            {
                this.RemoveWithTwoChildren(node);
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

            // Making sure that no duplicates left
            --node.Count;
            if (node.Count != 0)
            {
                return node;
            }

            this.Remove(node);

            return node;
        }

        #endregion

        #region Balance

        /// <summary>Gets heaviness type of the subtree rooted with the specified node and balance factor.</summary>
        /// <remarks>Should be called for unbalanced subtrees so factor does not belong to [-1, 1].</remarks>
        private static HeavinessType GetHeaviness(Node node, int factor)
        {
            if (factor > 1)
            {
                // Left heavy
                factor = BinarySearchTree<T>.GetHeight(node.Left.Left) - 
                         BinarySearchTree<T>.GetHeight(node.Left.Right);

                return factor > 0 ? HeavinessType.LeftLeft : HeavinessType.LeftRight;
            }
            else
            {
                // Right heavy
                factor = BinarySearchTree<T>.GetHeight(node.Right.Left) - 
                         BinarySearchTree<T>.GetHeight(node.Right.Right);

                return factor < 0 ? HeavinessType.RightRight : HeavinessType.RightLeft;
            }
        }

        private Node RotateLeft(Node node)
        {
            
        }

        private Node RotateRight(Node node)
        {
            
        }

        private Node BalanceSubtree(Node node)
        {
            int factor = BinarySearchTree<T>.GetHeight(node.Right) - BinarySearchTree<T>.GetHeight(node.Left);

            if ((factor >= -1) && (factor <= 1))
            {
                return node;
            }

            var heavy = BinarySearchTree<T>.GetHeaviness(node, factor);
            if (heavy == HeavinessType.LeftLeft)
            {
                return this.RotateRight(node);
            }
            else if (heavy == HeavinessType.LeftRight)
            {
                node.Left = this.RotateLeft(node.Left);
                return this.RotateRight(node);
            }
            else if (heavy == HeavinessType.RightLeft)
            {
                node.Right = this.RotateRight(node.Left);
                return this.RotateLeft(node);
            }
            else
            {
                return this.RotateLeft(node);
            }
        }

        /// <summary>Performs bottom up tree balancing starting with the specified node.</summary>
        private void Balance(Node node)
        {
            while (node != null)
            {
                var parent = node.Parent;
                var type = node.GetNodeType();

                node = this.BalanceSubtree(node);

                parent.Set(node, type);
                node = parent;
            }
        }

        #endregion

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
