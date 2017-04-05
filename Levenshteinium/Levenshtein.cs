// File: Levenshtein.cs
// Created: 04.04.2017
// 
// See <summary> tags for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Levenshteinium
{
    public static class Levenshtein
    {
        public const int MAX_RADIUS = 2;

        private static DistanceTree tree;

        private static readonly Dictionary<string, DistanceTree.Node> NodeDictionary =
            new Dictionary<string, DistanceTree.Node>();

        public static void LoadDictionary(Action<double> progress)
        {
            try
            {
                using (var stream = new FileStream("dictionary.txt", FileMode.Open))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var initLine = reader.ReadLine();
                        Levenshtein.tree =
                            new DistanceTree(new DistanceTree.Node(initLine, new List<DistanceTree.Edge>()));

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }

                            Levenshtein.AddToTree(line.ToUpper());

                            progress?.Invoke(stream.Position / (double) stream.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void AddToTree(string s)
        {
            var node = new DistanceTree.Node(s, null);

            List<DistanceTree.Node> neighbors;
            var radius = Levenshtein.MAX_RADIUS;
            var levenshtein = new Fastenshtein.Levenshtein(s);

            do
            {
                neighbors = Levenshtein.tree.Select(n =>
                {
                    n.Cache = levenshtein.Distance(n.Content);
                    return n.Cache <= radius;
                }).ToList();
                radius++;
            } while (neighbors.Count == 0);

            var edges = neighbors.Select(x => new DistanceTree.Edge(node, x, x.Cache));
            node.Edges = edges.ToList();

            Levenshtein.tree.Add(node);
            Levenshtein.NodeDictionary.Add(s, node);
        }

        public static IEnumerable<string> FindSimilar(string s)
        {
            var ss = s.ToUpper();
            var searchRadius = Math.Min(s.Length - 2, Levenshtein.MAX_RADIUS);

            if (!Levenshtein.NodeDictionary.ContainsKey(ss) || searchRadius == 0)
            {
                return new string[0];
            }

            var node = Levenshtein.NodeDictionary[ss];

            var retval = new List<DistanceTree.Node>();
            Levenshtein.FindSimilar(node, 0, retval, 0, searchRadius);

            return retval.Distinct().Where(x => x.Cache <= searchRadius && x.Content != ss).OrderBy(x => x.Cache).Select(x => x.Content.ToLower());
        }

        private static void FindSimilar(DistanceTree.Node node, int radius, List<DistanceTree.Node> nodes, int distance, int searchRadius)
        {
            if (radius > searchRadius)
            {
                return;
            }

            node.Cache = distance;
            nodes.Add(node);

            foreach (var edge in node.Edges)
            {
                Levenshtein.FindSimilar(edge.End, radius + 1, nodes, distance + edge.Distance, searchRadius);
            }
        }
    }
}