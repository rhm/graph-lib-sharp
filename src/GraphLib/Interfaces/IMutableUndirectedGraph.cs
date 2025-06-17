using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Interface for mutable undirected graphs that support adding and removing edges.
/// </summary>
public interface IMutableUndirectedGraph : IMutableGraph, IUndirectedGraph
{
    /// <summary>
    /// Adds an undirected edge between two nodes.
    /// </summary>
    /// <param name="node1">The first node.</param>
    /// <param name="node2">The second node.</param>
    /// <exception cref="ArgumentException">Thrown when either node does not exist in the graph.</exception>
    void AddEdge(NodeId node1, NodeId node2);

    /// <summary>
    /// Removes the undirected edge between two nodes.
    /// </summary>
    /// <param name="node1">The first node.</param>
    /// <param name="node2">The second node.</param>
    /// <exception cref="ArgumentException">Thrown when the edge does not exist.</exception>
    void RemoveEdge(NodeId node1, NodeId node2);
}