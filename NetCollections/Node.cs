using System;

namespace NetCollections
{
    public partial class BinarySearchTree<T>
    {
        /// <summary>Binary tree node class.</summary>
        private class Node
        {
            /// <summary>Node type relative to its parent.</summary>
            public enum NodeType
            {
                Root,
                Left,
                Right
            }

            public T Value { get; set; }

            public Node Left { get; set; }

            public Node Right { get; set; }

            /// <summary>Parent is needed because most of the methods are iterative.</summary>
            public Node Parent { get; set; }

            public int Height { get; set; }

            /// <summary>Gets or sets duplicates count.</summary>
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

            /// <summary>Gets node type.</summary>
            public NodeType GetNodeType()
            {
                if (this.Parent == null)
                {
                    return NodeType.Root;
                }
                else if ((this.Parent.Left != null) && (this.Parent.Left == this))
                {
                    return NodeType.Left;
                }
                else
                {
                    return NodeType.Right;
                }
            }

            public void Set(Node node, NodeType type)
            {
                if (type == NodeType.Left)
                {
                    this.Left = node;
                }
                else if (type == NodeType.Right)
                {
                    this.Right = node;
                }
                else
                {
                    throw new ArgumentException("Can't set root node as a child");
                }

                if (node != null)
                {
                    node.Parent = this;
                }
            }

            public override string ToString()
            {
                string parent = this.Parent == null ? "null" : this.Parent.Value.ToString();
                return string.Format("Val={0} Height={1} Duplicates={2} Children={3} Parent={4}", 
                                     this.Value, this.Height, this.Count, this.Children, parent);
            }
        }
    }
}