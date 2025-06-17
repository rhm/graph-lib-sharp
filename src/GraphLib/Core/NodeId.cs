namespace GraphLib.Core;

/// <summary>
/// Represents a unique identifier for a node in a graph.
/// </summary>
public readonly struct NodeId : IEquatable<NodeId>, IComparable<NodeId>
{
    /// <summary>
    /// Gets the integer value of the node identifier.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Initializes a new instance of the NodeId struct.
    /// </summary>
    /// <param name="value">The integer value for the node identifier.</param>
    public NodeId(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Determines whether this NodeId is equal to another NodeId.
    /// </summary>
    public bool Equals(NodeId other) => Value == other.Value;

    /// <summary>
    /// Determines whether this NodeId is equal to the specified object.
    /// </summary>
    public override bool Equals(object? obj) => obj is NodeId other && Equals(other);

    /// <summary>
    /// Returns the hash code for this NodeId.
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Compares this NodeId to another NodeId.
    /// </summary>
    public int CompareTo(NodeId other) => Value.CompareTo(other.Value);

    /// <summary>
    /// Returns a string representation of this NodeId.
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Implicit conversion from int to NodeId.
    /// </summary>
    public static implicit operator NodeId(int value) => new(value);

    /// <summary>
    /// Implicit conversion from NodeId to int.
    /// </summary>
    public static implicit operator int(NodeId nodeId) => nodeId.Value;

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(NodeId left, NodeId right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(NodeId left, NodeId right) => !left.Equals(right);

    /// <summary>
    /// Less than operator.
    /// </summary>
    public static bool operator <(NodeId left, NodeId right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Less than or equal operator.
    /// </summary>
    public static bool operator <=(NodeId left, NodeId right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Greater than operator.
    /// </summary>
    public static bool operator >(NodeId left, NodeId right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Greater than or equal operator.
    /// </summary>
    public static bool operator >=(NodeId left, NodeId right) => left.CompareTo(right) >= 0;
}