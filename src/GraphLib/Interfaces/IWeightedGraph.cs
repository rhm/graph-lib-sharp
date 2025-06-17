using GraphLib.Core;

namespace GraphLib.Interfaces;

/// <summary>
/// Interface for graphs with weighted edges.
/// </summary>
/// <typeparam name="TWeight">The type of the edge weights.</typeparam>
public interface IWeightedGraph<TWeight>
{
    /// <summary>
    /// Gets the weight of the edge from source to target.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <returns>The weight of the edge.</returns>
    /// <exception cref="ArgumentException">Thrown when the edge does not exist.</exception>
    TWeight GetEdgeWeight(NodeId source, NodeId target);

    /// <summary>
    /// Tries to get the weight of the edge from source to target.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <param name="weight">When this method returns, contains the weight of the edge if it exists; otherwise, the default value for TWeight.</param>
    /// <returns>true if the edge exists; otherwise, false.</returns>
    bool TryGetEdgeWeight(NodeId source, NodeId target, out TWeight weight);
}