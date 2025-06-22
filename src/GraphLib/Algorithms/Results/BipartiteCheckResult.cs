using GraphLib.Core;

namespace GraphLib.Algorithms.Results;

/// <summary>
/// Represents the result of a bipartite check algorithm.
/// </summary>
public class BipartiteCheckResult
{
    /// <summary>
    /// Gets a value indicating whether the graph is bipartite.
    /// </summary>
    public bool IsBipartite { get; }

    /// <summary>
    /// Gets the nodes in the left set of the bipartition.
    /// Empty if the graph is not bipartite.
    /// </summary>
    public IReadOnlyList<NodeId> LeftSet { get; }

    /// <summary>
    /// Gets the nodes in the right set of the bipartition.
    /// Empty if the graph is not bipartite.
    /// </summary>
    public IReadOnlyList<NodeId> RightSet { get; }

    /// <summary>
    /// Initializes a new instance of the BipartiteCheckResult class for a bipartite graph.
    /// </summary>
    /// <param name="leftSet">The nodes in the left set.</param>
    /// <param name="rightSet">The nodes in the right set.</param>
    public BipartiteCheckResult(IReadOnlyList<NodeId> leftSet, IReadOnlyList<NodeId> rightSet)
    {
        IsBipartite = true;
        LeftSet = leftSet ?? throw new ArgumentNullException(nameof(leftSet));
        RightSet = rightSet ?? throw new ArgumentNullException(nameof(rightSet));
    }

    /// <summary>
    /// Initializes a new instance of the BipartiteCheckResult class for a non-bipartite graph.
    /// </summary>
    private BipartiteCheckResult()
    {
        IsBipartite = false;
        LeftSet = Array.Empty<NodeId>();
        RightSet = Array.Empty<NodeId>();
    }

    /// <summary>
    /// Creates a result indicating the graph is not bipartite.
    /// </summary>
    public static BipartiteCheckResult NotBipartite() => new BipartiteCheckResult();
}