using GraphLib.Core;

namespace GraphLib.Algorithms.Results;

/// <summary>
/// Represents the result of a graph coloring algorithm.
/// </summary>
public class ColoringResult
{
    /// <summary>
    /// Gets the chromatic number (number of colors used).
    /// </summary>
    public int ChromaticNumber { get; }

    /// <summary>
    /// Gets the color assignment for each node.
    /// </summary>
    public IReadOnlyDictionary<NodeId, int> NodeColors { get; }

    /// <summary>
    /// Initializes a new instance of the ColoringResult class.
    /// </summary>
    /// <param name="nodeColors">The color assignment for each node.</param>
    public ColoringResult(IReadOnlyDictionary<NodeId, int> nodeColors)
    {
        NodeColors = nodeColors ?? throw new ArgumentNullException(nameof(nodeColors));
        ChromaticNumber = nodeColors.Count > 0 ? nodeColors.Values.Max() + 1 : 0;
    }
}