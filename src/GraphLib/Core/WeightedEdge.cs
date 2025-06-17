namespace GraphLib.Core;

/// <summary>
/// Represents a weighted edge between two nodes in a graph.
/// </summary>
/// <typeparam name="TWeight">The type of the edge weight.</typeparam>
public readonly struct WeightedEdge<TWeight> : IEquatable<WeightedEdge<TWeight>>
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
    /// Gets the weight of the edge.
    /// </summary>
    public TWeight Weight { get; }

    /// <summary>
    /// Initializes a new instance of the WeightedEdge struct.
    /// </summary>
    /// <param name="source">The source node identifier.</param>
    /// <param name="target">The target node identifier.</param>
    /// <param name="weight">The weight of the edge.</param>
    public WeightedEdge(NodeId source, NodeId target, TWeight weight)
    {
        Source = source;
        Target = target;
        Weight = weight;
    }

    /// <summary>
    /// Creates a reversed edge (swapping source and target) with the same weight.
    /// </summary>
    public WeightedEdge<TWeight> Reverse() => new(Target, Source, Weight);

    /// <summary>
    /// Creates a new weighted edge with a different weight.
    /// </summary>
    public WeightedEdge<TWeight> WithWeight(TWeight newWeight) => new(Source, Target, newWeight);

    /// <summary>
    /// Converts this weighted edge to an unweighted edge.
    /// </summary>
    public Edge ToEdge() => new(Source, Target);

    /// <summary>
    /// Determines whether this weighted edge is equal to another weighted edge.
    /// </summary>
    public bool Equals(WeightedEdge<TWeight> other)
    {
        return Source == other.Source && 
               Target == other.Target && 
               EqualityComparer<TWeight>.Default.Equals(Weight, other.Weight);
    }

    /// <summary>
    /// Determines whether this weighted edge is equal to the specified object.
    /// </summary>
    public override bool Equals(object? obj) => obj is WeightedEdge<TWeight> other && Equals(other);

    /// <summary>
    /// Returns the hash code for this weighted edge.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Source, Target, Weight);

    /// <summary>
    /// Returns a string representation of this weighted edge.
    /// </summary>
    public override string ToString() => $"{Source} -> {Target} (Weight: {Weight})";

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(WeightedEdge<TWeight> left, WeightedEdge<TWeight> right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(WeightedEdge<TWeight> left, WeightedEdge<TWeight> right) => !left.Equals(right);

    /// <summary>
    /// Deconstructs the weighted edge into its components.
    /// </summary>
    public void Deconstruct(out NodeId source, out NodeId target, out TWeight weight)
    {
        source = Source;
        target = Target;
        weight = Weight;
    }

    /// <summary>
    /// Implicit conversion from WeightedEdge to Edge (loses weight information).
    /// </summary>
    public static implicit operator Edge(WeightedEdge<TWeight> weightedEdge) => weightedEdge.ToEdge();
}