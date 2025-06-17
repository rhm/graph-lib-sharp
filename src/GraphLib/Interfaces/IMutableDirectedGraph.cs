using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Interface for mutable directed graphs that support adding and removing edges.
/// </summary>
public interface IMutableDirectedGraph : IMutableGraph, IDirectedGraph
{
    /// <summary>
    /// Adds a directed edge from the source node to the target node.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <exception cref="ArgumentException">Thrown when either node does not exist in the graph.</exception>
    void AddEdge(NodeId source, NodeId target);

    /// <summary>
    /// Removes the directed edge from the source node to the target node.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <exception cref="ArgumentException">Thrown when the edge does not exist.</exception>
    void RemoveEdge(NodeId source, NodeId target);
}