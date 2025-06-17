using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Interface for directed graphs, extending IGraph with directed-specific operations.
/// </summary>
public interface IDirectedGraph : IGraph
{
    /// <summary>
    /// Gets the out-neighbors of the specified node (nodes that can be reached via outgoing edges).
    /// </summary>
    /// <param name="node">The node whose out-neighbors to get.</param>
    /// <returns>An enumerable collection of out-neighbor nodes.</returns>
    /// <exception cref="ArgumentException">Thrown when the node is not in the graph.</exception>
    IEnumerable<NodeId> OutNeighbors(NodeId node);

    /// <summary>
    /// Gets the in-neighbors of the specified node (nodes that have edges pointing to this node).
    /// </summary>
    /// <param name="node">The node whose in-neighbors to get.</param>
    /// <returns>An enumerable collection of in-neighbor nodes.</returns>
    /// <exception cref="ArgumentException">Thrown when the node is not in the graph.</exception>
    IEnumerable<NodeId> InNeighbors(NodeId node);

    /// <summary>
    /// Gets the out-degree of the specified node (number of outgoing edges).
    /// </summary>
    /// <param name="node">The node whose out-degree to get.</param>
    /// <returns>The out-degree of the node.</returns>
    /// <exception cref="ArgumentException">Thrown when the node is not in the graph.</exception>
    int OutDegree(NodeId node);

    /// <summary>
    /// Gets the in-degree of the specified node (number of incoming edges).
    /// </summary>
    /// <param name="node">The node whose in-degree to get.</param>
    /// <returns>The in-degree of the node.</returns>
    /// <exception cref="ArgumentException">Thrown when the node is not in the graph.</exception>
    int InDegree(NodeId node);

    /// <summary>
    /// Determines whether an edge exists from the source node to the target node.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <returns>true if the edge exists; otherwise, false.</returns>
    bool HasEdge(NodeId source, NodeId target);
}