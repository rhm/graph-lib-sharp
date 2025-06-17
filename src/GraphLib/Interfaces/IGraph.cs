using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Base interface for all graph types, providing fundamental graph operations.
/// </summary>
public interface IGraph
{
    /// <summary>
    /// Gets the number of nodes in the graph.
    /// </summary>
    int NodeCount { get; }

    /// <summary>
    /// Gets the number of edges in the graph.
    /// </summary>
    int EdgeCount { get; }

    /// <summary>
    /// Gets an enumerable collection of all nodes in the graph.
    /// </summary>
    IEnumerable<NodeId> Nodes { get; }

    /// <summary>
    /// Determines whether the graph contains the specified node.
    /// </summary>
    /// <param name="node">The node to check for.</param>
    /// <returns>true if the graph contains the node; otherwise, false.</returns>
    bool ContainsNode(NodeId node);

    /// <summary>
    /// Gets the degree of the specified node.
    /// For directed graphs, this is the sum of in-degree and out-degree.
    /// For undirected graphs, this is the number of incident edges.
    /// </summary>
    /// <param name="node">The node whose degree to get.</param>
    /// <returns>The degree of the node.</returns>
    /// <exception cref="ArgumentException">Thrown when the node is not in the graph.</exception>
    int Degree(NodeId node);
}