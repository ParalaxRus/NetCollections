using System;
using System.Collections.Generic;

namespace NetCollections
{
    /// <summary>AVL tree recursive implementation.</summary>
    public partial class AvlRecursiveTree<T> : BinarySearchTree<T> where T : IComparable<T>
    {
        #region Details

        /// <summary>Adds new node node.</summary>
        /// <returns>Root of the subtree.</returns>
        private Node<T> AddNode(Node<T> node, T value)
        {
            if (node == null)
            {
                return new Node<T>(value);
            }

            int compare = value.CompareTo(node.Value);
            if (compare == 0)
            {
                ++node.Count;

                return node;
            }

            if (compare > 0)
            {
                node.Right = this.AddNode(node.Right, value);
            }
            else
            {
                node.Left = this.AddNode(node.Left, value);
            }

            node = this.BalanceSubtree(node);

            BinarySearchTree<T>.UpdateHeight(node);

            return node;
        }

        /// <summary>Removes leaf node.</summary>
        private Node<T> RemoveLeaf(Node<T> node)
        {
            return null;
        }

        /// <summary>Removes node with one child.</summary>
        private Node<T> RemoveWithOneChild(Node<T> node)
        {
            return node.Left != null ? node.Left : node.Right;
        }

        /// <summary>Removes node with both children.</summary>
        private Node<T> RemoveWithTwoChildren(Node<T> node)
        {
            var successor = BinarySearchTree<T>.GetInorderSuccessor(node);

            var value = successor.Value;
            var count = successor.Count;

            // Recursively removing in-order successor starting from the current node
            // to make sure that node's subtree properly balanced before unwinding recursion 
            // recursion stack up from the call to RemoveWithTwoChildren()
            bool exists = false;
            successor.Count = 1; // Successor should be removed no matter how many duplicates it contains
            this.RemoveNode(node, successor.Value, ref exists);
            
            // Updating current node with the successor data which should be deleted by now
            node.Value = value;
            node.Count = count;

            return node;
        }

        /// <summary>Removes node iteratively.</summary>
        /// <returns>Root of the new subtree.</returns>
        private Node<T> RemoveNode(Node<T> node)
        {
            int children = node.Children;
            if (children == 0)
            {
                node = this.RemoveLeaf(node);
            }
            else if (children == 1)
            {
                node = this.RemoveWithOneChild(node);
            }
            else
            {
                node = this.RemoveWithTwoChildren(node);
            }
            
            return node;                            
        }

        /// <summary>Rotates subtree rooted at the specified node to the left.</summary>
        /// <returns>New subtree root.</returns>
        private Node<T> RotateLeft(Node<T> node)
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
        private Node<T> RotateRight(Node<T> node)
        {
            var root = node.Left;
            var right = root.Right;

            root.Right = node;
            node.Left = right;

            BinarySearchTree<T>.UpdateHeight(root.Right);

            return root;
        }

        /// <summary>Balances subtree starting at the specified node.</summary>
        /// <returns>Root of the balanced subtree.</returns>
        private Node<T> BalanceSubtree(Node<T> node)
        {
            int factor = BinarySearchTree<T>.GetFactor(node);

            if ((factor >= -1) && (factor <= 1))
            {
                return node;
            }

            var imbalance = BinarySearchTree<T>.GetImbalance(node, factor);
            if (imbalance == ImbalanceType.LeftLeft)
            {
                node = this.RotateRight(node);
            }
            else if (imbalance == ImbalanceType.LeftRight)
            {
                node.Left = this.RotateLeft(node.Left);
                node = this.RotateRight(node);
            }
            else if (imbalance == ImbalanceType.RightLeft)
            {
                node.Right = this.RotateRight(node.Right);
                node = this.RotateLeft(node);
            }
            else
            {
                node = this.RotateLeft(node);
            }

            return node;
        }

        /// <summary>Removes node and re-balances subtree recursively.</summary>
        /// <returns>New root of the subtree.</returns>
        private Node<T> RemoveNode(Node<T> node, T value, ref bool exists)
        {
            if (node == null)
            {
                return null;
            }

            int compare = value.CompareTo(node.Value);
            if (compare == 0)
            {
                --node.Count;

                if (node.Count == 0)
                {
                    node = RemoveNode(node);
                }
                
                exists = true;
            }
            else if (compare > 0)
            {
                node.Right = this.RemoveNode(node.Right, value, ref exists);
            }
            else
            {
                node.Left = this.RemoveNode(node.Left, value, ref exists);
            }

            node = this.BalanceSubtree(node);

            // Updating balanced subtree root's height
            BinarySearchTree<T>.UpdateHeight(node);

            return node;
        }

        #endregion

        /// <summary>Default constructor.</summary>
        public AvlRecursiveTree() : base()
        {
        }

        /// <summary>Constructor with the specified input values.</summary>
        /// <timecomplexity>O(NlogN).</timecomplexity>
        public AvlRecursiveTree(IEnumerable<T> values) : base(values)
        {
        }

        /// <summary>Adds value to the tree.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        public override void Add(T value)
        {
            this.root = this.AddNode(this.root, value);

            ++this.Count;
        }

        /// <summary>Removes value from the tree.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        public override bool Remove(T value)
        {
            bool exists = false;
            this.root = this.RemoveNode(this.root, value, ref exists);

            if (!exists)
            {
                return false;
            }

            --this.Count;
            return true;
        }
    }
}
