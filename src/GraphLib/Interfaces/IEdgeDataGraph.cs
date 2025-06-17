using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Interface for graphs that store data associated with edges.
/// </summary>
/// <typeparam name="TEdgeData">The type of data associated with edges.</typeparam>
public interface IEdgeDataGraph<TEdgeData>
{
    /// <summary>
    /// Gets the data associated with the edge from source to target.
    /// </summary>
    /// <param name="source">The source node of the edge.</param>
    /// <param name="target">The target node of the edge.</param>
    /// <returns>The data associated with the edge.</returns>
    /// <exception cref="ArgumentException">Thrown when the edge does not exist.</exception>
    TEdgeData GetEdgeData(NodeId source, NodeId target);

    /// <summary>
    /// Tries to get the data associated with the edge from source to target.
    /// </summary>
    /// <param name="source">The source node of the edge.</param>
    /// <param name="target">The target node of the edge.</param>
    /// <param name="data">When this method returns, contains the data associated with the edge if it exists; otherwise, the default value for TEdgeData.</param>
    /// <returns>true if the edge exists and has data; otherwise, false.</returns>
    bool TryGetEdgeData(NodeId source, NodeId target, out TEdgeData data);
}