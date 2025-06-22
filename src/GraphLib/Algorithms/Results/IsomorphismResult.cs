using GraphLib.Core;

namespace GraphLib.Algorithms.Results;

/// <summary>
/// Represents the result of a graph isomorphism check.
/// </summary>
public class IsomorphismResult
{
    /// <summary>
    /// Gets a value indicating whether the graphs are isomorphic.
    /// </summary>
    public bool IsIsomorphic { get; }

    /// <summary>
    /// Gets the mapping from nodes in the first graph to nodes in the second graph.
    /// Null if the graphs are not isomorphic.
    /// </summary>
    public IReadOnlyDictionary<NodeId, NodeId>? NodeMapping { get; }

    /// <summary>
    /// Initializes a new instance of the IsomorphismResult class for isomorphic graphs.
    /// </summary>
    /// <param name="nodeMapping">The node mapping between the graphs.</param>
    public IsomorphismResult(IReadOnlyDictionary<NodeId, NodeId> nodeMapping)
    {
        IsIsomorphic = true;
        NodeMapping = nodeMapping ?? throw new ArgumentNullException(nameof(nodeMapping));
    }

    /// <summary>
    /// Initializes a new instance of the IsomorphismResult class for non-isomorphic graphs.
    /// </summary>
    private IsomorphismResult()
    {
        IsIsomorphic = false;
        NodeMapping = null;
    }

    /// <summary>
    /// Creates a result indicating the graphs are not isomorphic.
    /// </summary>
    public static IsomorphismResult NotIsomorphic() => new IsomorphismResult();
}