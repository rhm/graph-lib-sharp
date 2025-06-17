using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Interface for mutable graphs that support adding and removing nodes.
/// </summary>
public interface IMutableGraph
{
    /// <summary>
    /// Adds a new node to the graph and returns its identifier.
    /// </summary>
    /// <returns>The identifier of the newly added node.</returns>
    NodeId AddNode();

    /// <summary>
    /// Removes the specified node from the graph, along with all incident edges.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    /// <exception cref="ArgumentException">Thrown when the node does not exist in the graph.</exception>
    void RemoveNode(NodeId node);

    /// <summary>
    /// Removes all nodes and edges from the graph.
    /// </summary>
    void Clear();
}