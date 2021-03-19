using System;

namespace NetCollections
{
    /// <summary>Binary tree node.</summary>
    public class Node<T> where T : IComparable<T>
    {
        public T Value { get; set; }

        public Node<T> Left { get; set; }

        public Node<T> Right { get; set; }

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
            this.Height = 0;
            this.Count = 1;
        }

        public override string ToString()
        {
            return string.Format("Val={0} Height={1} Duplicates={2} Children={3}", 
                                 this.Value, this.Height, this.Count, this.Children);
        }
    }

    /// <summary>Binary tree node with the link to its parent.</summary>
    public class NodeP<T> : Node<T> where T : IComparable<T>
    {
        /// <summary>Node type relative to its parent.</summary>
        public enum NodeType
        {
            Root,
            Left,
            Right
        }

        /// <summary>Parent is needed because most of the methods are iterative.</summary>
        public NodeP<T> Parent { get; set; }

        public NodeP(T value) : base(value)
        {
            this.Parent = null;
        }

        public NodeP(T value, NodeP<T> parent) : this(value)
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

        /// <summary>Sets child node of the specified type.</summary>
        public void Set(NodeP<T> node, NodeType type)
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

            return string.Format("{0} Parent={1}", base.ToString(), parent);
        }
    }
}