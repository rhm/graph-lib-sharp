using GraphLib.Core;

namespace GraphLib.Algorithms.Results;

/// <summary>
/// Represents the result of a topological sort algorithm.
/// </summary>
public class TopologicalSortResult
{
    /// <summary>
    /// Gets a value indicating whether the graph is acyclic.
    /// </summary>
    public bool IsAcyclic { get; }

    /// <summary>
    /// Gets the topological ordering of nodes.
    /// Empty if the graph contains cycles.
    /// </summary>
    public IReadOnlyList<NodeId> Order { get; }

    /// <summary>
    /// Initializes a new instance of the TopologicalSortResult class for an acyclic graph.
    /// </summary>
    /// <param name="order">The topological ordering of nodes.</param>
    public TopologicalSortResult(IReadOnlyList<NodeId> order)
    {
        IsAcyclic = true;
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }

    /// <summary>
    /// Initializes a new instance of the TopologicalSortResult class for a cyclic graph.
    /// </summary>
    private TopologicalSortResult()
    {
        IsAcyclic = false;
        Order = Array.Empty<NodeId>();
    }

    /// <summary>
    /// Creates a result indicating the graph contains cycles.
    /// </summary>
    public static TopologicalSortResult Cyclic() => new TopologicalSortResult();
}