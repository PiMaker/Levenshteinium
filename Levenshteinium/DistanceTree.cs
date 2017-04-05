// File: DistanceTree.cs
// Created: 04.04.2017
// 
// See <summary> tags for more information.

using System;
using System.Collections.Generic;

namespace Levenshteinium
{
    public class DistanceTree
    {
        private readonly Node root;

        public DistanceTree(Node root)
        {
            this.root = root;
        }

        public void Add(Node n)
        {
            foreach (var edge in n.Edges)
            {
                edge?.End?.Edges?.Add(new Edge(edge.End, edge.Start, edge.Distance));
            }
        }

        public IEnumerable<Node> Select(Func<Node, bool> selector)
        {
            var set = new HashSet<Node>();
            var allSet = new HashSet<Node>();
            this.Select(this.root, set, allSet, selector);
            return set;
        }

        private void Select(Node node, HashSet<Node> set, HashSet<Node> allSet, Func<Node, bool> selector)
        {
            if (allSet.Contains(node))
            {
                return;
            }

            allSet.Add(node);

            if (selector(node))
            {
                set.Add(node);
            }

            foreach (var neighbor in node.Edges)
            {
                this.Select(neighbor.End, set, allSet, selector);
            }
        }

        public class Node
        {
            public Node(string content, List<Edge> edges)
            {
                this.Content = content;
                this.Edges = edges;
            }

            public string Content { get; set; }
            public List<Edge> Edges { get; set; }

            public int Cache { get; set; }
        }

        public class Edge
        {
            public Edge(Node start, Node end, int distance)
            {
                this.Start = start;
                this.End = end;
                this.Distance = distance;
            }

            public Node Start { get; set; }
            public Node End { get; set; }

            public int Distance { get; set; }
        }
    }
}