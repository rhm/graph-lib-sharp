using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Interface for graphs that store data associated with nodes.
/// </summary>
/// <typeparam name="TNodeData">The type of data associated with nodes.</typeparam>
public interface INodeDataGraph<TNodeData>
{
    /// <summary>
    /// Gets the data associated with the specified node.
    /// </summary>
    /// <param name="node">The node whose data to get.</param>
    /// <returns>The data associated with the node.</returns>
    /// <exception cref="ArgumentException">Thrown when the node does not exist in the graph.</exception>
    TNodeData GetNodeData(NodeId node);

    /// <summary>
    /// Tries to get the data associated with the specified node.
    /// </summary>
    /// <param name="node">The node whose data to get.</param>
    /// <param name="data">When this method returns, contains the data associated with the node if it exists; otherwise, the default value for TNodeData.</param>
    /// <returns>true if the node exists and has data; otherwise, false.</returns>
    bool TryGetNodeData(NodeId node, out TNodeData data);
}