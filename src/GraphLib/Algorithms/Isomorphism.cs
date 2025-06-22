using GraphLib.Core;
using GraphLib.Interfaces;
using GraphLib.Algorithms.Results;

namespace GraphLib.Algorithms;

/// <summary>
/// Provides algorithms for graph isomorphism checking.
/// Note: Graph isomorphism is a complex problem and these algorithms may be slow for large graphs.
/// </summary>
public static class Isomorphism
{
    /// <summary>
    /// Checks if two graphs are isomorphic.
    /// </summary>
    /// <typeparam name="TGraph1">The type of the first graph.</typeparam>
    /// <typeparam name="TGraph2">The type of the second graph.</typeparam>
    /// <param name="graph1">The first graph.</param>
    /// <param name="graph2">The second graph.</param>
    /// <returns>True if the graphs are isomorphic; otherwise, false.</returns>
    public static bool AreIsomorphic<TGraph1, TGraph2>(
        TGraph1 graph1,
        TGraph2 graph2)
        where TGraph1 : IGraph
        where TGraph2 : IGraph
    {
        return FindIsomorphism(graph1, graph2).IsIsomorphic;
    }

    /// <summary>
    /// Finds an isomorphism mapping between two graphs if one exists.
    /// </summary>
    /// <typeparam name="TGraph1">The type of the first graph.</typeparam>
    /// <typeparam name="TGraph2">The type of the second graph.</typeparam>
    /// <param name="graph1">The first graph.</param>
    /// <param name="graph2">The second graph.</param>
    /// <returns>The isomorphism result containing the mapping if found.</returns>
    public static IsomorphismResult FindIsomorphism<TGraph1, TGraph2>(
        TGraph1 graph1,
        TGraph2 graph2)
        where TGraph1 : IGraph
        where TGraph2 : IGraph
    {
        if (graph1 == null) throw new ArgumentNullException(nameof(graph1));
        if (graph2 == null) throw new ArgumentNullException(nameof(graph2));

        // Quick checks for obvious non-isomorphism
        if (graph1.NodeCount != graph2.NodeCount || graph1.EdgeCount != graph2.EdgeCount)
        {
            return IsomorphismResult.NotIsomorphic();
        }

        if (graph1.NodeCount == 0)
        {
            return new IsomorphismResult(new Dictionary<NodeId, NodeId>());
        }

        // Check degree sequence
        if (!HaveSameDegreeSequence(graph1, graph2))
        {
            return IsomorphismResult.NotIsomorphic();
        }

        // Use backtracking to find isomorphism
        var nodes1 = graph1.Nodes.ToList();
        var nodes2 = graph2.Nodes.ToList();

        // Sort nodes by degree to optimize search
        nodes1.Sort((a, b) => graph1.Degree(b).CompareTo(graph1.Degree(a)));
        nodes2.Sort((a, b) => graph2.Degree(b).CompareTo(graph2.Degree(a)));

        var mapping = new Dictionary<NodeId, NodeId>();
        var used2 = new HashSet<NodeId>();

        if (FindMappingBacktrack(graph1, graph2, nodes1, nodes2, 0, mapping, used2))
        {
            return new IsomorphismResult(mapping);
        }

        return IsomorphismResult.NotIsomorphic();
    }

    /// <summary>
    /// Checks if the first graph contains a subgraph isomorphic to the second graph.
    /// </summary>
    /// <typeparam name="TGraph1">The type of the first graph.</typeparam>
    /// <typeparam name="TGraph2">The type of the second graph (pattern).</typeparam>
    /// <param name="graph">The larger graph to search in.</param>
    /// <param name="pattern">The pattern graph to search for.</param>
    /// <returns>True if a subgraph isomorphic to the pattern exists; otherwise, false.</returns>
    public static bool ContainsSubgraphIsomorphicTo<TGraph1, TGraph2>(
        TGraph1 graph,
        TGraph2 pattern)
        where TGraph1 : IGraph
        where TGraph2 : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (pattern == null) throw new ArgumentNullException(nameof(pattern));

        // Quick checks
        if (pattern.NodeCount > graph.NodeCount || pattern.EdgeCount > graph.EdgeCount)
        {
            return false;
        }

        if (pattern.NodeCount == 0)
        {
            return true;
        }

        var patternNodes = pattern.Nodes.ToList();
        var graphNodes = graph.Nodes.ToList();

        // Sort pattern nodes by degree (descending) to start with most constrained
        patternNodes.Sort((a, b) => pattern.Degree(b).CompareTo(pattern.Degree(a)));

        var mapping = new Dictionary<NodeId, NodeId>();
        var usedGraphNodes = new HashSet<NodeId>();

        return FindSubgraphMappingBacktrack(graph, pattern, graphNodes, patternNodes, 0, mapping, usedGraphNodes);
    }

    /// <summary>
    /// Finds a subgraph isomorphism mapping between the pattern and target graph.
    /// </summary>
    /// <typeparam name="TGraph">The type of the target graph.</typeparam>
    /// <typeparam name="TPattern">The type of the pattern graph.</typeparam>
    /// <param name="graph">The target graph.</param>
    /// <param name="pattern">The pattern graph to find.</param>
    /// <returns>The isomorphism result containing the mapping if found.</returns>
    public static IsomorphismResult IsSubgraphIsomorphic<TGraph, TPattern>(
        TGraph graph,
        TPattern pattern)
        where TGraph : IGraph
        where TPattern : IGraph
    {
        return FindSubgraphIsomorphism(graph, pattern);
    }

    /// <summary>
    /// Finds a subgraph isomorphism mapping between the pattern and target graph.
    /// </summary>
    /// <typeparam name="TGraph">The type of the target graph.</typeparam>
    /// <typeparam name="TPattern">The type of the pattern graph.</typeparam>
    /// <param name="graph">The target graph.</param>
    /// <param name="pattern">The pattern graph to find.</param>
    /// <returns>The isomorphism result containing the mapping if found.</returns>
    public static IsomorphismResult FindSubgraphIsomorphism<TGraph, TPattern>(
        TGraph graph,
        TPattern pattern)
        where TGraph : IGraph
        where TPattern : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (pattern == null) throw new ArgumentNullException(nameof(pattern));

        // Quick checks
        if (pattern.NodeCount > graph.NodeCount || pattern.EdgeCount > graph.EdgeCount)
        {
            return IsomorphismResult.NotIsomorphic();
        }

        if (pattern.NodeCount == 0)
        {
            return new IsomorphismResult(new Dictionary<NodeId, NodeId>());
        }

        var patternNodes = pattern.Nodes.ToList();
        var graphNodes = graph.Nodes.ToList();

        // Sort pattern nodes by degree (descending) to start with most constrained
        patternNodes.Sort((a, b) => pattern.Degree(b).CompareTo(pattern.Degree(a)));

        var mapping = new Dictionary<NodeId, NodeId>();
        var usedGraphNodes = new HashSet<NodeId>();

        if (FindSubgraphMappingBacktrack(graph, pattern, graphNodes, patternNodes, 0, mapping, usedGraphNodes))
        {
            return new IsomorphismResult(mapping);
        }

        return IsomorphismResult.NotIsomorphic();
    }

    // Helper method to check if two graphs have the same degree sequence
    private static bool HaveSameDegreeSequence<TGraph1, TGraph2>(TGraph1 graph1, TGraph2 graph2)
        where TGraph1 : IGraph
        where TGraph2 : IGraph
    {
        var degrees1 = graph1.Nodes.Select(n => graph1.Degree(n)).OrderBy(d => d).ToList();
        var degrees2 = graph2.Nodes.Select(n => graph2.Degree(n)).OrderBy(d => d).ToList();

        if (degrees1.Count != degrees2.Count)
            return false;

        for (int i = 0; i < degrees1.Count; i++)
        {
            if (degrees1[i] != degrees2[i])
                return false;
        }

        return true;
    }

    // Backtracking algorithm to find isomorphism mapping
    private static bool FindMappingBacktrack<TGraph1, TGraph2>(
        TGraph1 graph1,
        TGraph2 graph2,
        List<NodeId> nodes1,
        List<NodeId> nodes2,
        int index,
        Dictionary<NodeId, NodeId> mapping,
        HashSet<NodeId> used2)
        where TGraph1 : IGraph
        where TGraph2 : IGraph
    {
        if (index == nodes1.Count)
        {
            return true; // All nodes mapped successfully
        }

        var node1 = nodes1[index];

        foreach (var node2 in nodes2)
        {
            if (used2.Contains(node2))
                continue;

            // Check if this mapping is feasible
            if (graph1.Degree(node1) != graph2.Degree(node2))
                continue;

            // Check if edges are preserved
            bool isValidMapping = true;
            foreach (var mappedNode1 in mapping.Keys)
            {
                var mappedNode2 = mapping[mappedNode1];
                bool hasEdge1 = HasEdge(graph1, node1, mappedNode1);
                bool hasEdge2 = HasEdge(graph2, node2, mappedNode2);

                if (hasEdge1 != hasEdge2)
                {
                    isValidMapping = false;
                    break;
                }
            }

            if (isValidMapping)
            {
                mapping[node1] = node2;
                used2.Add(node2);

                if (FindMappingBacktrack(graph1, graph2, nodes1, nodes2, index + 1, mapping, used2))
                {
                    return true;
                }

                // Backtrack
                mapping.Remove(node1);
                used2.Remove(node2);
            }
        }

        return false;
    }

    // Backtracking algorithm for subgraph isomorphism
    private static bool FindSubgraphMappingBacktrack<TGraph1, TGraph2>(
        TGraph1 graph,
        TGraph2 pattern,
        List<NodeId> graphNodes,
        List<NodeId> patternNodes,
        int patternIndex,
        Dictionary<NodeId, NodeId> mapping,
        HashSet<NodeId> usedGraphNodes)
        where TGraph1 : IGraph
        where TGraph2 : IGraph
    {
        if (patternIndex == patternNodes.Count)
        {
            return true; // All pattern nodes mapped successfully
        }

        var patternNode = patternNodes[patternIndex];

        foreach (var graphNode in graphNodes)
        {
            if (usedGraphNodes.Contains(graphNode))
                continue;

            // Check degree constraint (graph node must have at least the degree of pattern node)
            if (graph.Degree(graphNode) < pattern.Degree(patternNode))
                continue;

            // Check if edges are preserved
            bool isValidMapping = true;
            foreach (var mappedPatternNode in mapping.Keys)
            {
                var mappedGraphNode = mapping[mappedPatternNode];
                bool hasEdgeInPattern = HasEdge(pattern, patternNode, mappedPatternNode);
                bool hasEdgeInGraph = HasEdge(graph, graphNode, mappedGraphNode);

                // For subgraph isomorphism, if pattern has edge, graph must have it too
                // But graph can have additional edges
                if (hasEdgeInPattern && !hasEdgeInGraph)
                {
                    isValidMapping = false;
                    break;
                }
            }

            if (isValidMapping)
            {
                mapping[patternNode] = graphNode;
                usedGraphNodes.Add(graphNode);

                if (FindSubgraphMappingBacktrack(graph, pattern, graphNodes, patternNodes, 
                    patternIndex + 1, mapping, usedGraphNodes))
                {
                    return true;
                }

                // Backtrack
                mapping.Remove(patternNode);
                usedGraphNodes.Remove(graphNode);
            }
        }

        return false;
    }

    // Helper method to check if an edge exists between two nodes
    private static bool HasEdge<TGraph>(TGraph graph, NodeId node1, NodeId node2)
        where TGraph : IGraph
    {
        if (graph is IDirectedGraph directedGraph)
        {
            return directedGraph.HasEdge(node1, node2);
        }
        else if (graph is IUndirectedGraph undirectedGraph)
        {
            return undirectedGraph.HasEdge(node1, node2);
        }
        else
        {
            // Fallback for generic IGraph - check if nodes are neighbors
            if (graph is IDirectedGraph dg)
            {
                return dg.OutNeighbors(node1).Contains(node2);
            }
            else if (graph is IUndirectedGraph ug)
            {
                return ug.Neighbors(node1).Contains(node2);
            }
            
            return false;
        }
    }
}