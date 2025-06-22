using GraphLib.Core;
using GraphLib.Interfaces;
using GraphLib.Implementations;

namespace GraphLib.Algorithms.Results;

/// <summary>
/// Represents the result of a spanning tree algorithm.
/// </summary>
/// <typeparam name="TWeight">The type of the edge weights.</typeparam>
public class SpanningTreeResult<TWeight>
{
    /// <summary>
    /// Gets the edges that form the spanning tree.
    /// </summary>
    public IReadOnlyList<WeightedEdge<TWeight>> Edges { get; }

    /// <summary>
    /// Gets the total weight of the spanning tree.
    /// </summary>
    public TWeight TotalWeight { get; }

    /// <summary>
    /// Initializes a new instance of the SpanningTreeResult class.
    /// </summary>
    /// <param name="edges">The edges forming the spanning tree.</param>
    /// <param name="totalWeight">The total weight of the spanning tree.</param>
    public SpanningTreeResult(IReadOnlyList<WeightedEdge<TWeight>> edges, TWeight totalWeight)
    {
        Edges = edges ?? throw new ArgumentNullException(nameof(edges));
        TotalWeight = totalWeight;
    }

    /// <summary>
    /// Creates a subgraph view of the original graph containing only the spanning tree edges.
    /// </summary>
    /// <param name="originalGraph">The original graph.</param>
    /// <returns>A subgraph view containing only the spanning tree edges.</returns>
    public SubgraphView<IUndirectedGraph> AsSubgraph(IUndirectedGraph originalGraph)
    {
        if (originalGraph == null)
            throw new ArgumentNullException(nameof(originalGraph));

        var spanningTreeEdges = new HashSet<Edge>(Edges.Select(e => new Edge(e.Source, e.Target)));

        return new SubgraphView<IUndirectedGraph>(
            originalGraph,
            nodeFilter: null, // Include all nodes
            edgeFilter: edge => spanningTreeEdges.Contains(edge) || spanningTreeEdges.Contains(new Edge(edge.Target, edge.Source))
        );
    }
}