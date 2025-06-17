namespace GraphLib.Core;

/// <summary>
/// Represents an unweighted edge between two nodes in a graph.
/// </summary>
public readonly struct Edge : IEquatable<Edge>
{
    /// <summary>
    /// Gets the source node of the edge.
    /// </summary>
    public NodeId Source { get; }

    /// <summary>
    /// Gets the target node of the edge.
    /// </summary>
    public NodeId Target { get; }

    /// <summary>
    /// Initializes a new instance of the Edge struct.
    /// </summary>
    /// <param name="source">The source node identifier.</param>
    /// <param name="target">The target node identifier.</param>
    public Edge(NodeId source, NodeId target)
    {
        Source = source;
        Target = target;
    }

    /// <summary>
    /// Creates a reversed edge (swapping source and target).
    /// </summary>
    public Edge Reverse() => new(Target, Source);

    /// <summary>
    /// Determines whether this edge is equal to another edge.
    /// </summary>
    public bool Equals(Edge other) => Source == other.Source && Target == other.Target;

    /// <summary>
    /// Determines whether this edge is equal to the specified object.
    /// </summary>
    public override bool Equals(object? obj) => obj is Edge other && Equals(other);

    /// <summary>
    /// Returns the hash code for this edge.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Source, Target);

    /// <summary>
    /// Returns a string representation of this edge.
    /// </summary>
    public override string ToString() => $"{Source} -> {Target}";

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(Edge left, Edge right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(Edge left, Edge right) => !left.Equals(right);

    /// <summary>
    /// Deconstructs the edge into its source and target components.
    /// </summary>
    public void Deconstruct(out NodeId source, out NodeId target)
    {
        source = Source;
        target = Target;
    }
}