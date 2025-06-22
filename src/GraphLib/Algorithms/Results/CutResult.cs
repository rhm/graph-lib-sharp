using GraphLib.Core;

namespace GraphLib.Algorithms.Results;

/// <summary>
/// Represents the result of a minimum cut algorithm.
/// </summary>
/// <typeparam name="TCapacity">The type of the cut capacity.</typeparam>
public class CutResult<TCapacity>
{
    /// <summary>
    /// Gets the total capacity of the minimum cut.
    /// </summary>
    public TCapacity CutCapacity { get; }

    /// <summary>
    /// Gets the edges that form the minimum cut.
    /// </summary>
    public IReadOnlyList<Edge> CutEdges { get; }

    /// <summary>
    /// Gets the nodes on the source side of the cut.
    /// </summary>
    public IReadOnlyList<NodeId> SourceSideNodes { get; }

    /// <summary>
    /// Gets the nodes on the sink side of the cut.
    /// </summary>
    public IReadOnlyList<NodeId> SinkSideNodes { get; }

    /// <summary>
    /// Initializes a new instance of the CutResult class.
    /// </summary>
    /// <param name="cutCapacity">The total capacity of the cut.</param>
    /// <param name="cutEdges">The edges forming the cut.</param>
    /// <param name="sourceSideNodes">The nodes on the source side.</param>
    /// <param name="sinkSideNodes">The nodes on the sink side.</param>
    public CutResult(TCapacity cutCapacity, IReadOnlyList<Edge> cutEdges, 
        IReadOnlyList<NodeId> sourceSideNodes, IReadOnlyList<NodeId> sinkSideNodes)
    {
        CutCapacity = cutCapacity;
        CutEdges = cutEdges ?? throw new ArgumentNullException(nameof(cutEdges));
        SourceSideNodes = sourceSideNodes ?? throw new ArgumentNullException(nameof(sourceSideNodes));
        SinkSideNodes = sinkSideNodes ?? throw new ArgumentNullException(nameof(sinkSideNodes));
    }
}