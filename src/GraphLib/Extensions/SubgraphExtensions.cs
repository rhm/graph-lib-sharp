using GraphLib.Core;
using GraphLib.Interfaces;
using GraphLib.Implementations;

namespace GraphLib.Extensions;

/// <summary>
/// Extension methods for creating subgraph views.
/// </summary>
public static class SubgraphExtensions
{
    /// <summary>
    /// Creates a subgraph view with the specified node and edge filters.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="nodeFilter">The predicate to filter nodes. If null, all nodes are included.</param>
    /// <param name="edgeFilter">The predicate to filter edges. If null, all edges are included.</param>
    /// <returns>A subgraph view of the parent graph.</returns>
    public static SubgraphView<TGraph> Subgraph<TGraph>(
        this TGraph graph,
        Predicate<NodeId>? nodeFilter = null,
        Predicate<Edge>? edgeFilter = null)
        where TGraph : IGraph
    {
        return new SubgraphView<TGraph>(graph, nodeFilter, edgeFilter);
    }

    /// <summary>
    /// Creates an induced subgraph containing only the specified nodes and edges between them.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="nodes">The nodes to include in the subgraph.</param>
    /// <returns>An induced subgraph view containing only the specified nodes.</returns>
    public static SubgraphView<TGraph> InducedSubgraph<TGraph>(
        this TGraph graph,
        IEnumerable<NodeId> nodes)
        where TGraph : IGraph
    {
        if (nodes == null) throw new ArgumentNullException(nameof(nodes));

        var nodeSet = nodes.ToHashSet();
        
        return new SubgraphView<TGraph>(
            graph,
            nodeFilter: node => nodeSet.Contains(node),
            edgeFilter: edge => nodeSet.Contains(edge.Source) && nodeSet.Contains(edge.Target));
    }

    /// <summary>
    /// Creates an induced subgraph containing only the specified nodes and edges between them.
    /// Preserves weights for weighted graphs.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <typeparam name="TWeight">The type of edge weights.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="nodes">The nodes to include in the subgraph.</param>
    /// <returns>An induced subgraph view containing only the specified nodes.</returns>
    public static WeightedSubgraphView<TGraph, TWeight> InducedSubgraph<TGraph, TWeight>(
        this TGraph graph,
        IEnumerable<NodeId> nodes)
        where TGraph : IGraph, IWeightedGraph<TWeight>
    {
        if (nodes == null) throw new ArgumentNullException(nameof(nodes));

        var nodeSet = nodes.ToHashSet();
        
        return new WeightedSubgraphView<TGraph, TWeight>(
            graph,
            nodeFilter: node => nodeSet.Contains(node),
            edgeFilter: edge => nodeSet.Contains(edge.Source) && nodeSet.Contains(edge.Target));
    }

    /// <summary>
    /// Creates a subgraph containing nodes with degrees within the specified range.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="minDegree">The minimum degree (inclusive).</param>
    /// <param name="maxDegree">The maximum degree (inclusive).</param>
    /// <returns>A subgraph view containing only nodes with degrees in the specified range.</returns>
    public static SubgraphView<TGraph> SubgraphByDegree<TGraph>(
        this TGraph graph,
        int minDegree,
        int maxDegree = int.MaxValue)
        where TGraph : IGraph
    {
        return new SubgraphView<TGraph>(
            graph,
            nodeFilter: node => 
            {
                var degree = graph.Degree(node);
                return degree >= minDegree && degree <= maxDegree;
            });
    }

    /// <summary>
    /// Creates a subgraph containing only nodes that are reachable from the specified start node.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="startNode">The node to start the reachability search from.</param>
    /// <returns>A subgraph view containing only reachable nodes.</returns>
    public static SubgraphView<TGraph> ReachableSubgraph<TGraph>(
        this TGraph graph,
        NodeId startNode)
        where TGraph : IGraph
    {
        if (!graph.ContainsNode(startNode))
            throw new ArgumentException("Start node not in graph", nameof(startNode));

        var reachableNodes = GetReachableNodes(graph, startNode);

        return new SubgraphView<TGraph>(
            graph,
            nodeFilter: node => reachableNodes.Contains(node));
    }

    /// <summary>
    /// Creates a subgraph containing nodes within a specified distance from a start node.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="startNode">The center node.</param>
    /// <param name="maxDistance">The maximum distance from the start node.</param>
    /// <returns>A subgraph view containing nodes within the specified distance.</returns>
    public static SubgraphView<TGraph> NeighborhoodSubgraph<TGraph>(
        this TGraph graph,
        NodeId startNode,
        int maxDistance)
        where TGraph : IGraph
    {
        if (!graph.ContainsNode(startNode))
            throw new ArgumentException("Start node not in graph", nameof(startNode));
        
        if (maxDistance < 0)
            throw new ArgumentException("Max distance must be non-negative", nameof(maxDistance));

        var nodesInRange = GetNodesWithinDistance(graph, startNode, maxDistance);

        return new SubgraphView<TGraph>(
            graph,
            nodeFilter: node => nodesInRange.Contains(node));
    }

    /// <summary>
    /// Creates a subgraph that excludes the specified nodes.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="nodesToExclude">The nodes to exclude from the subgraph.</param>
    /// <returns>A subgraph view that excludes the specified nodes.</returns>
    public static SubgraphView<TGraph> ExcludeNodes<TGraph>(
        this TGraph graph,
        IEnumerable<NodeId> nodesToExclude)
        where TGraph : IGraph
    {
        if (nodesToExclude == null) throw new ArgumentNullException(nameof(nodesToExclude));

        var excludeSet = nodesToExclude.ToHashSet();

        return new SubgraphView<TGraph>(
            graph,
            nodeFilter: node => !excludeSet.Contains(node));
    }

    /// <summary>
    /// Creates a subgraph that excludes edges with the specified endpoints.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="edgesToExclude">The edges to exclude from the subgraph.</param>
    /// <returns>A subgraph view that excludes the specified edges.</returns>
    public static SubgraphView<TGraph> ExcludeEdges<TGraph>(
        this TGraph graph,
        IEnumerable<Edge> edgesToExclude)
        where TGraph : IGraph
    {
        if (edgesToExclude == null) throw new ArgumentNullException(nameof(edgesToExclude));

        var excludeSet = edgesToExclude.ToHashSet();

        return new SubgraphView<TGraph>(
            graph,
            edgeFilter: edge => !excludeSet.Contains(edge) && 
                               !excludeSet.Contains(new Edge(edge.Target, edge.Source)));
    }

    // Helper method to find all reachable nodes from a start node
    private static HashSet<NodeId> GetReachableNodes<TGraph>(TGraph graph, NodeId startNode)
        where TGraph : IGraph
    {
        var reachable = new HashSet<NodeId>();
        var queue = new Queue<NodeId>();
        
        queue.Enqueue(startNode);
        reachable.Add(startNode);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var neighbors = GetNeighbors(graph, current);

            foreach (var neighbor in neighbors)
            {
                if (!reachable.Contains(neighbor))
                {
                    reachable.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return reachable;
    }

    // Helper method to find nodes within a specified distance
    private static HashSet<NodeId> GetNodesWithinDistance<TGraph>(TGraph graph, NodeId startNode, int maxDistance)
        where TGraph : IGraph
    {
        var nodesInRange = new HashSet<NodeId>();
        var queue = new Queue<(NodeId Node, int Distance)>();
        
        queue.Enqueue((startNode, 0));
        nodesInRange.Add(startNode);

        while (queue.Count > 0)
        {
            var (current, distance) = queue.Dequeue();
            
            if (distance < maxDistance)
            {
                var neighbors = GetNeighbors(graph, current);

                foreach (var neighbor in neighbors)
                {
                    if (!nodesInRange.Contains(neighbor))
                    {
                        nodesInRange.Add(neighbor);
                        queue.Enqueue((neighbor, distance + 1));
                    }
                }
            }
        }

        return nodesInRange;
    }

    // Helper method to get neighbors based on graph type
    private static IEnumerable<NodeId> GetNeighbors<TGraph>(TGraph graph, NodeId node)
        where TGraph : IGraph
    {
        if (graph is IDirectedGraph directedGraph)
        {
            return directedGraph.OutNeighbors(node);
        }
        else if (graph is IUndirectedGraph undirectedGraph)
        {
            return undirectedGraph.Neighbors(node);
        }
        else
        {
            // Fallback for generic IGraph - less efficient
            var neighbors = new List<NodeId>();
            foreach (var other in graph.Nodes)
            {
                if (!other.Equals(node))
                {
                    // This is inefficient but works for any IGraph
                    if (graph is IDirectedGraph dg && dg.HasEdge(node, other))
                    {
                        neighbors.Add(other);
                    }
                    else if (graph is IUndirectedGraph ug && ug.HasEdge(node, other))
                    {
                        neighbors.Add(other);
                    }
                }
            }
            return neighbors;
        }
    }

    /// <summary>
    /// Creates a subgraph containing only nodes matching the predicate.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="predicate">The predicate to filter nodes.</param>
    /// <returns>A subgraph view containing only matching nodes.</returns>
    public static SubgraphView<TGraph> FilterNodes<TGraph>(
        this TGraph graph,
        Predicate<NodeId> predicate)
        where TGraph : IGraph
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return new SubgraphView<TGraph>(graph, nodeFilter: predicate);
    }

    /// <summary>
    /// Creates a subgraph containing only edges matching the predicate.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="predicate">The predicate to filter edges.</param>
    /// <returns>A subgraph view containing only matching edges.</returns>
    public static SubgraphView<TGraph> FilterEdges<TGraph>(
        this TGraph graph,
        Predicate<Edge> predicate)
        where TGraph : IGraph
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return new SubgraphView<TGraph>(graph, edgeFilter: predicate);
    }

    /// <summary>
    /// Creates a subgraph containing the connected component that contains the specified node.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="node">The node whose component to extract.</param>
    /// <returns>A subgraph view containing the connected component.</returns>
    public static SubgraphView<TGraph> ComponentSubgraph<TGraph>(
        this TGraph graph,
        NodeId node)
        where TGraph : IGraph
    {
        if (!graph.ContainsNode(node))
            throw new ArgumentException("Node not in graph", nameof(node));

        var componentNodes = GetReachableNodes(graph, node);

        return new SubgraphView<TGraph>(
            graph,
            nodeFilter: n => componentNodes.Contains(n));
    }

    /// <summary>
    /// Creates a subgraph from the specified edges and their incident nodes.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="edges">The edges to include in the subgraph.</param>
    /// <returns>A subgraph view containing the specified edges and their incident nodes.</returns>
    public static SubgraphView<TGraph> EdgeSubgraph<TGraph>(
        this TGraph graph,
        IEnumerable<Edge> edges)
        where TGraph : IGraph
    {
        if (edges == null) throw new ArgumentNullException(nameof(edges));

        var edgeSet = edges.ToHashSet();
        var nodeSet = new HashSet<NodeId>();

        foreach (var edge in edgeSet)
        {
            nodeSet.Add(edge.Source);
            nodeSet.Add(edge.Target);
        }

        return new SubgraphView<TGraph>(
            graph,
            nodeFilter: node => nodeSet.Contains(node),
            edgeFilter: edge => edgeSet.Contains(edge) || edgeSet.Contains(new Edge(edge.Target, edge.Source)));
    }

    /// <summary>
    /// Creates a spanning subgraph with the specified edges.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The parent graph.</param>
    /// <param name="spanningEdges">The edges that form the spanning subgraph.</param>
    /// <returns>A subgraph view containing all nodes but only the specified edges.</returns>
    public static SubgraphView<TGraph> SpanningSubgraph<TGraph>(
        this TGraph graph,
        IEnumerable<Edge> spanningEdges)
        where TGraph : IGraph
    {
        if (spanningEdges == null) throw new ArgumentNullException(nameof(spanningEdges));

        var edgeSet = spanningEdges.ToHashSet();

        return new SubgraphView<TGraph>(
            graph,
            nodeFilter: null, // Include all nodes
            edgeFilter: edge => edgeSet.Contains(edge) || edgeSet.Contains(new Edge(edge.Target, edge.Source)));
    }
}