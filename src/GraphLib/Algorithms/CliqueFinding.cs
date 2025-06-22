using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Algorithms;

/// <summary>
/// Provides algorithms for finding cliques in graphs.
/// </summary>
public static class CliqueFinding
{
    /// <summary>
    /// Finds the maximum clique in the graph using a simple branch-and-bound algorithm.
    /// Note: This is an NP-hard problem and may be slow for large graphs.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The undirected graph.</param>
    /// <returns>The nodes forming the maximum clique.</returns>
    public static IReadOnlyList<NodeId> FindMaximumClique<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var nodes = graph.Nodes.ToList();
        if (nodes.Count == 0) return Array.Empty<NodeId>();

        var maxClique = new List<NodeId>();
        var currentClique = new List<NodeId>();

        void BranchAndBound(int index, List<NodeId> candidates)
        {
            if (index >= nodes.Count)
            {
                if (currentClique.Count > maxClique.Count)
                {
                    maxClique.Clear();
                    maxClique.AddRange(currentClique);
                }
                return;
            }

            // Pruning: if current clique + remaining candidates can't beat max, skip
            if (currentClique.Count + candidates.Count <= maxClique.Count)
                return;

            var node = nodes[index];

            // Try including current node if it's connected to all nodes in current clique
            if (IsConnectedToAll(graph, node, currentClique))
            {
                currentClique.Add(node);
                
                // Update candidates to only include nodes connected to the new node
                var newCandidates = candidates.Where(c => graph.HasEdge(node, c)).ToList();
                BranchAndBound(index + 1, newCandidates);
                
                currentClique.RemoveAt(currentClique.Count - 1);
            }

            // Try not including current node
            var remainingCandidates = candidates.Where(c => !c.Equals(node)).ToList();
            BranchAndBound(index + 1, remainingCandidates);
        }

        BranchAndBound(0, nodes);
        return maxClique;
    }

    /// <summary>
    /// Finds all maximal cliques using the Bron-Kerbosch algorithm.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The undirected graph.</param>
    /// <returns>An enumerable of maximal cliques.</returns>
    public static IEnumerable<IReadOnlyList<NodeId>> FindAllMaximalCliques<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var allNodes = graph.Nodes.ToHashSet();
        var cliques = new List<List<NodeId>>();

        // Bron-Kerbosch algorithm
        void BronKerbosch(HashSet<NodeId> r, HashSet<NodeId> p, HashSet<NodeId> x)
        {
            if (p.Count == 0 && x.Count == 0)
            {
                // R is a maximal clique
                cliques.Add(new List<NodeId>(r));
                return;
            }

            // Choose pivot to minimize branching
            var unionSet = p.Union(x);
            var candidates = unionSet.Any() 
                ? p.Except(GetNeighbors(graph, unionSet.First())).ToList()
                : p.ToList();

            foreach (var v in candidates)
            {
                var neighbors = GetNeighbors(graph, v);
                
                var newR = new HashSet<NodeId>(r) { v };
                var newP = new HashSet<NodeId>(p.Intersect(neighbors));
                var newX = new HashSet<NodeId>(x.Intersect(neighbors));

                BronKerbosch(newR, newP, newX);

                p.Remove(v);
                x.Add(v);
            }
        }

        BronKerbosch(new HashSet<NodeId>(), allNodes, new HashSet<NodeId>());
        
        return cliques.Select(c => (IReadOnlyList<NodeId>)c);
    }

    /// <summary>
    /// Finds all cliques of a specific size.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The undirected graph.</param>
    /// <param name="size">The size of cliques to find.</param>
    /// <returns>An enumerable of cliques of the specified size.</returns>
    public static IEnumerable<IReadOnlyList<NodeId>> FindCliquesOfSize<TGraph>(
        TGraph graph,
        int size)
        where TGraph : IUndirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (size <= 0) throw new ArgumentException("Size must be positive", nameof(size));

        var nodes = graph.Nodes.ToList();
        var cliques = new List<List<NodeId>>();

        void FindCliquesRecursive(List<NodeId> currentClique, int startIndex)
        {
            if (currentClique.Count == size)
            {
                cliques.Add(new List<NodeId>(currentClique));
                return;
            }

            for (int i = startIndex; i < nodes.Count; i++)
            {
                var candidate = nodes[i];

                // Check if candidate is connected to all nodes in current clique
                if (IsConnectedToAll(graph, candidate, currentClique))
                {
                    currentClique.Add(candidate);
                    FindCliquesRecursive(currentClique, i + 1);
                    currentClique.RemoveAt(currentClique.Count - 1);
                }
            }
        }

        FindCliquesRecursive(new List<NodeId>(), 0);
        return cliques.Select(c => (IReadOnlyList<NodeId>)c);
    }

    /// <summary>
    /// Checks if the graph contains a clique of the specified size.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The undirected graph.</param>
    /// <param name="size">The size of the clique to check for.</param>
    /// <returns>True if a clique of the specified size exists; otherwise, false.</returns>
    public static bool HasCliqueOfSize<TGraph>(TGraph graph, int size)
        where TGraph : IUndirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (size <= 0) throw new ArgumentException("Size must be positive", nameof(size));
        if (size == 1) return graph.NodeCount > 0;

        var nodes = graph.Nodes.ToList();
        
        bool FindCliqueRecursive(List<NodeId> currentClique, int startIndex)
        {
            if (currentClique.Count == size)
            {
                return true; // Found a clique of the desired size
            }

            // Pruning: if we can't possibly reach the target size, return false
            if (currentClique.Count + (nodes.Count - startIndex) < size)
                return false;

            for (int i = startIndex; i < nodes.Count; i++)
            {
                var candidate = nodes[i];

                // Check if candidate is connected to all nodes in current clique
                if (IsConnectedToAll(graph, candidate, currentClique))
                {
                    currentClique.Add(candidate);
                    
                    if (FindCliqueRecursive(currentClique, i + 1))
                    {
                        return true;
                    }
                    
                    currentClique.RemoveAt(currentClique.Count - 1);
                }
            }

            return false;
        }

        return FindCliqueRecursive(new List<NodeId>(), 0);
    }

    // Helper method to check if a node is connected to all nodes in a collection
    private static bool IsConnectedToAll<TGraph>(TGraph graph, NodeId node, IEnumerable<NodeId> nodes)
        where TGraph : IUndirectedGraph
    {
        return nodes.All(other => graph.HasEdge(node, other));
    }

    // Helper method to get neighbors of a node
    private static HashSet<NodeId> GetNeighbors<TGraph>(TGraph graph, NodeId node)
        where TGraph : IUndirectedGraph
    {
        return graph.Neighbors(node).ToHashSet();
    }
}