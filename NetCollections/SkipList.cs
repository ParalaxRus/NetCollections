using System;
using System.Collections.Generic;
using System.Text;

namespace NetCollections
{

/// <summary>Skip list class (randomized data structure).</summary>
public class SkipList<T> where T : struct, IComparable<T>
{
    private class Node
    {
        public T Value { get; private set; }
        public Node Next { get; set; }
        public Node Down { get; private set; }
        public Node() : this(default(T), null, null) { }
        public Node(T value, Node next, Node down)
        {
            this.Value = value;
            this.Next = next;
            this.Down = down;
        }
    }
   
    private Node head = null;
    private Random random = new Random();

    private static Node Add(Node insertAfter, Node bottomNode, T value) 
    {
        insertAfter.Next = new Node(value, insertAfter.Next, bottomNode);
        return insertAfter.Next;
    }
    
    private bool TossACoin() 
    {
        return (this.random.Next(2) == 0);
    }
    
    private bool Find(T value, Stack<Node> path)
    {
        Node prev = null;
        var cur = head;
        while (cur != null) 
        {
            prev = cur;
            
            if ((cur.Next != null) && (cur.Next.Value.CompareTo(value) < 0)) 
            {
                cur = cur.Next;
                continue;
            }
            
            if (path != null) 
            {
                path.Push(cur);
            }
                
            cur = cur.Down;
        }
        
        return (prev != null) && (prev.Next != null) && (prev.Next.Value.CompareTo(value) == 0);
    }
   
    private void Create() 
    {
        if (this.head == null) 
        {
            // It does not matter what value sentinel/head node has 
            // because find() never comares head value anyways
            this.head = new Node();
        }
    }
   
    private void Compact() 
    {
        while ((this.head != null) && (this.head.Next == null)) 
        {
            var s = head;
            head = head.Down;
        }
    }

    /// <summary>Gets number of elements in the skiplist.</summary>
    public int Count { get; private set; }

    /// <summary>Default ctor.</summary>
    public SkipList()
    {
        this.Count = 0;
    }
    
    /// <summary>Gets list as a string.</summary>
    public override string ToString()
    {
        var sb = new StringBuilder();

        var level = head;
        while (level != null) 
        {
            var cur = level;
            while (cur != null) 
            {
                sb.Append(string.Format("{0} ", cur.Value));
                cur = cur.Next;
            }
            --sb.Length;

            sb.Append("\n");
            level = level.Down;
        }        

        sb.Append("\n");

        return sb.ToString();
    }
   
    /// <summary>Checks whether list contains specified value or not</summary>
    /// <remarks>T=O(logN)</remarks>
    public bool Contains(T value) 
    {
        return this.Find(value, null);
    }
   
    /// <summary>Adds specified value to the list (supports duplicates)</summary>
    /// <remarks>T=O(logN)</remarks>
    public void Add(T value) 
    {
        this.Create();
        
        var path = new Stack<Node>();
        this.Find(value, path);
       
        Node prev = null;
        Node bottom = null;
        do 
        {
            if (path.Count == 0) 
            {
                head = new Node(default(T), null, head);
                path.Push(head);
            }
           
            prev = path.Pop();
           
            bottom = SkipList<T>.Add(prev, bottom, value);
        }
        while (this.TossACoin());

        ++this.Count;
    }
   
    /// <summary>Removes specified value from the list</summary>
    /// <remarks>T=O(logN)</remarks>
    public bool Remove(T value) 
    {    
        var path = new Stack<Node>();
        if (!this.Find(value, path)) 
        {
            return false;
        }
        
        while (path.Count != 0) 
        {
            var node = path.Pop();
            
            if ((node.Next == null) || (node.Next.Value.CompareTo(value) != 0)) 
            {
                break;
            }
            
            node.Next = node.Next.Next;
        }
        
        this.Compact();

        --this.Count;
       
        return true;
    }
}

}