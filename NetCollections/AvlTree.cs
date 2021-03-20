using System;
using System.Collections.Generic;

namespace NetCollections
{
    /// <summary>AVL tree.</summary>
    public partial class AvlTree<T> : BinarySearchTree<T> where T : IComparable<T>
    {
        #region Details

        /// <summary>Sets node as root.</summary>
        private NodeP<T> SetRoot(NodeP<T> node)
        {
            this.root = node;

            if (node != null)
            {
                node.Parent = null;
            }

            return (NodeP<T>)this.root;
        }

        /// <summary>Updates heights from the specified node and up to the root.</summary>
        private static void UpdateHeightBottomUp(NodeP<T> node)
        {
            while (node != null)
            {
                BinarySearchTree<T>.UpdateHeight(node);
                node = node.Parent;
            }
        }

        /// <summary>Adds node iteratively.</summary>
        /// <returns>Newly added node.</returns>
        private NodeP<T> AddNode(T value)
        {
            if (this.root == null)
            {
                return this.SetRoot(new NodeP<T>(value));
            }

            int compare = 0;
            NodeP<T> parent = null;
            var current = (NodeP<T>)this.root;
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
                    current = (NodeP<T>)current.Right;
                }
                else
                {
                    current = (NodeP<T>)current.Left;
                }
            }

            var node = new NodeP<T>(value, parent);

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

        /// <summary>Removes leaf node.</summary>
        /// <returns>Node from which heights should be invalidated.</returns>
        private NodeP<T> RemoveLeaf(NodeP<T> node)
        {
            var type = node.GetNodeType();

            var parent = node.Parent;
            node.Parent = null;

            if (type == NodeP<T>.NodeType.Root)
            {
                return this.SetRoot(null);
            }

            parent.Set(null, type);
            
            return parent;
        }

        /// <summary>Removes node with the single child.</summary>
        /// <returns>Node from which heights should be invalidated.</returns>
        private NodeP<T> RemoveWithOneChild(NodeP<T> node)
        {
            var child = (NodeP<T>) (node.Left != null ? node.Left : node.Right);

            var type = node.GetNodeType();

            var parent = node.Parent;
            node.Parent = null;

            if (type == NodeP<T>.NodeType.Root)
            {
                return this.SetRoot(child);
            }

            parent.Set(child, type);

            return parent;
        }

        /// <summary>Removes node with both children.</summary>
        /// <returns>Node from which heights should be invalidated.</returns>
        private NodeP<T> RemoveWithTwoChildren(NodeP<T> node)
        {
            var successor = (NodeP<T>)BinarySearchTree<T>.GetInorderSuccessor(node);
            
            node.Value = successor.Value;
            node.Count = successor.Count;

            return this.RemoveWithOneChild(successor);
        }

        /// <summary>Removes node iteratively.</summary>
        /// <returns>Node from which heights should be invalidated.</returns>
        private NodeP<T> Remove(NodeP<T> node)
        {
            // Node with the height to be fixed
            NodeP<T> updateNode = null;

            int children = node.Children;
            if (children == 0)
            {
                updateNode = this.RemoveLeaf(node);
            }
            else if (children == 1)
            {
                updateNode = this.RemoveWithOneChild(node);
            }
            else
            {
                updateNode = this.RemoveWithTwoChildren(node);
            }

            return updateNode;
        }

        /// <summary>Rotates subtree rooted at the specified node to the left.</summary>
        /// <returns>New subtree root.</returns>
        private NodeP<T> RotateLeft(NodeP<T> node)
        {
            var root = (NodeP<T>)node.Right;
            var left = (NodeP<T>)root.Left;

            root.Set(node, NodeP<T>.NodeType.Left);
            node.Set(left, NodeP<T>.NodeType.Right);

            BinarySearchTree<T>.UpdateHeight(root.Left);

            return root;
        }

        /// <summary>Rotates subtree rooted at the specified node to the right.</summary>
        /// <returns>New subtree root.</returns>
        private NodeP<T> RotateRight(NodeP<T> node)
        {
            var root = (NodeP<T>)node.Left;
            var right = (NodeP<T>)root.Right;

            root.Set(node, NodeP<T>.NodeType.Right);
            node.Set(right, NodeP<T>.NodeType.Left);

            BinarySearchTree<T>.UpdateHeight(root.Right);

            return root;
        }

        /// <summary>Balances subtree rooted at the specified node.</summary>
        /// <returns>New root.</returns>
        private NodeP<T> BalanceSubtree(NodeP<T> node)
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
                var subroot = this.RotateLeft((NodeP<T>)node.Left);
                node.Set(subroot, NodeP<T>.NodeType.Left);

                node = this.RotateRight(node);
            }
            else if (heavy == HeavinessType.RightLeft)
            {
                var subroot = this.RotateRight((NodeP<T>)node.Right);
                node.Set(subroot, NodeP<T>.NodeType.Right);

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
        private void Balance(NodeP<T> node)
        {
            while (node != null)
            {
                var parent = node.Parent;
                var type = node.GetNodeType();

                node = this.BalanceSubtree(node);

                if (parent == null)
                {
                    this.SetRoot(node);
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

        /// <summary>Default constructor.</summary>
        public AvlTree() : base()
        {
        }

        /// <summary>Constructor with the specified input values.</summary>
        /// <timecomplexity>O(NlogN).</timecomplexity>
        public AvlTree(IEnumerable<T> values) : base(values)
        {
        }

        /// <summary>Adds value to the tree.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        public override void Add(T value)
        {
            var node = this.AddNode(value);

            AvlTree<T>.UpdateHeightBottomUp(node);

            this.Balance(node);

            ++this.Count;
        }

        /// <summary>Removes value from the tree.</summary>
        /// <timecomplexity>O(logN).</timecomplexity>
        public override bool Remove(T value)
        {
            var node = this.Find(value);
            if (node == null)
            {
                // Value does not exist
                return false;
            }

            --this.Count;

            --node.Count;
            if (node.Count > 0)            
            {
                // Node should not be deleted because it still contains duplicates
                return true;
            }
            
            var nodeToBalance = this.Remove((NodeP<T>)node);
            
            AvlTree<T>.UpdateHeightBottomUp(nodeToBalance);

            this.Balance(nodeToBalance);

            return true;
        }
    }
}
