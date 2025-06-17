using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Interface for undirected graphs, extending IGraph with undirected-specific operations.
/// </summary>
public interface IUndirectedGraph : IGraph
{
    /// <summary>
    /// Gets the neighbors of the specified node (nodes connected by an edge).
    /// </summary>
    /// <param name="node">The node whose neighbors to get.</param>
    /// <returns>An enumerable collection of neighbor nodes.</returns>
    /// <exception cref="ArgumentException">Thrown when the node is not in the graph.</exception>
    IEnumerable<NodeId> Neighbors(NodeId node);

    /// <summary>
    /// Determines whether an edge exists between two nodes.
    /// Since the graph is undirected, HasEdge(a, b) == HasEdge(b, a).
    /// </summary>
    /// <param name="node1">The first node.</param>
    /// <param name="node2">The second node.</param>
    /// <returns>true if an edge exists between the nodes; otherwise, false.</returns>
    bool HasEdge(NodeId node1, NodeId node2);
}