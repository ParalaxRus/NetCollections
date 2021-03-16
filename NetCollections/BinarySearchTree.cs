using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("NetCollectionsTests")]

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

        /// <summary>Recursively checks wether tree is a valid binary search tree or not.</summary>
        /// <remarks>Algorithm is based on the fact that each previous value in 
        /// in-order taraversal should be less than current.</remarks>
        private static bool IsValid(Node n, ref bool found, ref T min)
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

        private static int GetNodesCount(Node node)
        {
            if (node == null)
            {
                return 0;
            }

            return BinarySearchTree<T>.GetNodesCount(node.Left) + 
                   BinarySearchTree<T>.GetNodesCount(node.Right) + 1;
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

        /// <summary>Updates height of the specified node.</summary>
        private static void UpdateHeight(Node node)
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

        /// <summary>Updates heights from the specified node and up to the root.</summary>
        private static void UpdateHeightBottomUp(Node node)
        {
            while (node != null)
            {
                BinarySearchTree<T>.UpdateHeight(node);
                node = node.Parent;
            }
        }

        /// <summary>Adds node iteratively.</summary>
        /// <returns>Newly added node.</returns>
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

        /// <summary>Removes leaf node.</summary>
        /// <returns>Removed node's parent.</returns>
        private Node RemoveLeaf(Node node)
        {
            if (node == this.root)
            {
                this.root = null;
                return null;
            }

            if (node.Parent.Left == node)
            {
                node.Parent.Left = null;
            }
            else
            {
                node.Parent.Right = null;
            }

            return node.Parent;
        }

        /// <summary>Removes node with the single child.</summary>
        /// <returns>Removed node's parent.</returns>
        private Node RemoveWithOneChild(Node node)
        {
            var child = node.Left != null ? node.Left : node.Right;

            if (node == this.root)
            {
                this.root = child;
                return this.root;
            }

            if (node.Parent.Left == node)
            {
                node.Parent.Left = child;
            }
            else
            {
                node.Parent.Right = child;
            }

            return node.Parent;
        }

        /// <summary>Removes node with both children.</summary>
        /// <returns>Removed node's parent.</returns>
        private Node RemoveWithTwoChildren(Node node)
        {
            var successor = this.GetInorderSuccessor(node);
            
            node.Value = successor.Value;

            return this.RemoveWithOneChild(successor);
        }

        /// <summary>Removes node iteratively.</summary>
        private void Remove(Node node)
        {
            // Removed node's parent
            Node parent = null;

            int children = node.Children;
            if (children == 0)
            {
                parent = this.RemoveLeaf(node);
            }
            else if (children == 1)
            {
                parent = this.RemoveWithOneChild(node);
            }
            else
            {
                parent = this.RemoveWithTwoChildren(node);
            }

            BinarySearchTree<T>.UpdateHeightBottomUp(parent);
        }

        /// <summary>Removes node iteratively.</summary>
        /// <returns>Removed node.</returns>
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

        /// <summary>Gets heaviness type of the subtree rooted with the specified node and balance factor.</summary>
        /// <remarks>Should be called for unbalanced subtrees so factor does not belong to [-1, 1].</remarks>
        private static HeavinessType GetHeaviness(Node node, int factor)
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

        /// <summary>Rotates subtree rooted at the specified node to the left.</summary>
        /// <returns>New subtree root.</returns>
        private Node RotateLeft(Node node)
        {
            var root = node.Right;

            var left = root.Left;
            root.Left = node;
            node.Right = left;

            BinarySearchTree<T>.UpdateHeight(root.Left);

            return root;
        }

        /// <summary>Rotates subtree rooted at the specified node to the right.</summary>
        /// <returns>New subtree root.</returns>
        private Node RotateRight(Node node)
        {
            var root = node.Left;

            var right = root.Right;
            root.Right = node;
            node.Left = right;

            BinarySearchTree<T>.UpdateHeight(root.Right);

            return root;
        }

        /// <summary>Calculates specified node's balance factor.</summary>
        private static int GetFactor(Node node)
        {
            int left = node.Left != null ? BinarySearchTree<T>.GetHeight(node.Left) + 1 : 0;
            int right = node.Right != null ? BinarySearchTree<T>.GetHeight(node.Right) + 1 : 0;

            return (left - right);
        }

        /// <summary>Balances subtree rooted at the specified node.</summary>
        /// <returns>New root.</returns>
        private Node BalanceSubtree(Node node)
        {
            int factor = BinarySearchTree<T>.GetFactor(node);

            if ((factor >= -1) && (factor <= 1))
            {
                return node;
            }

            var heavy = BinarySearchTree<T>.GetHeaviness(node, factor);
            if (heavy == HeavinessType.LeftLeft)
            {
                node = this.RotateRight(node);
            }
            else if (heavy == HeavinessType.LeftRight)
            {
                node.Left = this.RotateLeft(node.Left);
                node = this.RotateRight(node);
            }
            else if (heavy == HeavinessType.RightLeft)
            {
                node.Right = this.RotateRight(node.Right);
                node = this.RotateLeft(node);
            }
            else
            {
                node = this.RotateLeft(node);
            }

            // Updating new root's height
            BinarySearchTree<T>.UpdateHeight(node);

            return node;
        }

        /// <summary>Performs bottom up tree balancing starting with the specified node.</summary>
        private void Balance(Node node)
        {
            while (node != null)
            {
                var parent = node.Parent;
                var type = node.GetNodeType();

                node = this.BalanceSubtree(node);

                if (parent == null)
                {
                    this.root = node;
                }
                else
                {
                    parent.Set(node, type);
                    
                    // Parent height might change after balancing
                    BinarySearchTree<T>.UpdateHeight(parent);
                }

                node = parent;
            }
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

            BinarySearchTree<T>.UpdateHeightBottomUp(node);

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

            BinarySearchTree<T>.UpdateHeightBottomUp(node);

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
