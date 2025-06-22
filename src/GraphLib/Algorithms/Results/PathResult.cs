using GraphLib.Core;

namespace GraphLib.Algorithms.Results;

/// <summary>
/// Represents the result of a path-finding algorithm.
/// </summary>
/// <typeparam name="TWeight">The type of the path weight.</typeparam>
public class PathResult<TWeight>
{
    /// <summary>
    /// Gets a value indicating whether a path exists from source to target.
    /// </summary>
    public bool PathExists { get; }

    /// <summary>
    /// Gets the sequence of nodes in the path from source to target.
    /// Empty if no path exists.
    /// </summary>
    public IReadOnlyList<NodeId> Path { get; }

    /// <summary>
    /// Gets the total weight of the path.
    /// Default value if no path exists.
    /// </summary>
    public TWeight TotalWeight { get; }

    /// <summary>
    /// Initializes a new instance of the PathResult class for a successful path.
    /// </summary>
    /// <param name="path">The sequence of nodes in the path.</param>
    /// <param name="totalWeight">The total weight of the path.</param>
    public PathResult(IReadOnlyList<NodeId> path, TWeight totalWeight)
    {
        PathExists = true;
        Path = path ?? throw new ArgumentNullException(nameof(path));
        TotalWeight = totalWeight;
    }

    /// <summary>
    /// Initializes a new instance of the PathResult class for when no path exists.
    /// </summary>
    private PathResult()
    {
        PathExists = false;
        Path = Array.Empty<NodeId>();
        TotalWeight = default!;
    }

    /// <summary>
    /// Creates a PathResult indicating no path exists.
    /// </summary>
    public static PathResult<TWeight> NoPath() => new PathResult<TWeight>();
}