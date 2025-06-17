using GraphLib.Core;
using Xunit;

namespace GraphLib.Tests.Core;

public class EdgeTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var source = new NodeId(1);
        var target = new NodeId(2);

        // Act
        var edge = new Edge(source, target);

        // Assert
        Assert.Equal(source, edge.Source);
        Assert.Equal(target, edge.Target);
    }

    [Fact]
    public void Reverse_ReturnsEdgeWithSwappedNodes()
    {
        // Arrange
        var source = new NodeId(1);
        var target = new NodeId(2);
        var edge = new Edge(source, target);

        // Act
        var reversed = edge.Reverse();

        // Assert
        Assert.Equal(target, reversed.Source);
        Assert.Equal(source, reversed.Target);
    }

    [Fact]
    public void Equals_ReturnsTrueForSameSourceAndTarget()
    {
        // Arrange
        var edge1 = new Edge(new NodeId(1), new NodeId(2));
        var edge2 = new Edge(new NodeId(1), new NodeId(2));

        // Act & Assert
        Assert.True(edge1.Equals(edge2));
        Assert.True(edge1 == edge2);
        Assert.False(edge1 != edge2);
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentSource()
    {
        // Arrange
        var edge1 = new Edge(new NodeId(1), new NodeId(2));
        var edge2 = new Edge(new NodeId(3), new NodeId(2));

        // Act & Assert
        Assert.False(edge1.Equals(edge2));
        Assert.False(edge1 == edge2);
        Assert.True(edge1 != edge2);
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentTarget()
    {
        // Arrange
        var edge1 = new Edge(new NodeId(1), new NodeId(2));
        var edge2 = new Edge(new NodeId(1), new NodeId(3));

        // Act & Assert
        Assert.False(edge1.Equals(edge2));
        Assert.False(edge1 == edge2);
        Assert.True(edge1 != edge2);
    }

    [Fact]
    public void Equals_ReturnsFalseForReversedEdge()
    {
        // Arrange
        var edge1 = new Edge(new NodeId(1), new NodeId(2));
        var edge2 = new Edge(new NodeId(2), new NodeId(1));

        // Act & Assert
        Assert.False(edge1.Equals(edge2));
        Assert.NotEqual(edge1, edge2);
    }

    [Fact]
    public void Equals_WithObject_WorksCorrectly()
    {
        // Arrange
        var edge = new Edge(new NodeId(1), new NodeId(2));
        object sameEdge = new Edge(new NodeId(1), new NodeId(2));
        object differentEdge = new Edge(new NodeId(3), new NodeId(4));
        object notEdge = "not an edge";

        // Act & Assert
        Assert.True(edge.Equals(sameEdge));
        Assert.False(edge.Equals(differentEdge));
        Assert.False(edge.Equals(notEdge));
        Assert.False(edge.Equals(null));
    }

    [Fact]
    public void GetHashCode_SameForEqualEdges()
    {
        // Arrange
        var edge1 = new Edge(new NodeId(1), new NodeId(2));
        var edge2 = new Edge(new NodeId(1), new NodeId(2));

        // Act & Assert
        Assert.Equal(edge1.GetHashCode(), edge2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentForDifferentEdges()
    {
        // Arrange
        var edge1 = new Edge(new NodeId(1), new NodeId(2));
        var edge2 = new Edge(new NodeId(3), new NodeId(4));
        var edge3 = new Edge(new NodeId(2), new NodeId(1)); // Reversed

        // Act & Assert
        Assert.NotEqual(edge1.GetHashCode(), edge2.GetHashCode());
        Assert.NotEqual(edge1.GetHashCode(), edge3.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var edge = new Edge(new NodeId(1), new NodeId(2));

        // Act
        var result = edge.ToString();

        // Assert
        Assert.Equal("1 -> 2", result);
    }

    [Fact]
    public void Deconstruct_WorksCorrectly()
    {
        // Arrange
        var source = new NodeId(5);
        var target = new NodeId(10);
        var edge = new Edge(source, target);

        // Act
        var (deconstructedSource, deconstructedTarget) = edge;

        // Assert
        Assert.Equal(source, deconstructedSource);
        Assert.Equal(target, deconstructedTarget);
    }

    [Fact]
    public void Edge_CanBeUsedInHashSet()
    {
        // Arrange
        var set = new HashSet<Edge>();
        var edge1 = new Edge(new NodeId(1), new NodeId(2));
        var edge2 = new Edge(new NodeId(1), new NodeId(2)); // Same edge
        var edge3 = new Edge(new NodeId(2), new NodeId(1)); // Different (reversed)

        // Act
        set.Add(edge1);
        set.Add(edge2); // Should not be added (duplicate)
        set.Add(edge3);

        // Assert
        Assert.Equal(2, set.Count);
        Assert.Contains(edge1, set);
        Assert.Contains(edge3, set);
    }

    [Fact]
    public void Edge_CanBeUsedAsDictionaryKey()
    {
        // Arrange
        var dict = new Dictionary<Edge, string>();
        var edge1 = new Edge(new NodeId(1), new NodeId(2));
        var edge2 = new Edge(new NodeId(1), new NodeId(2)); // Same edge
        var edge3 = new Edge(new NodeId(3), new NodeId(4));

        // Act
        dict[edge1] = "First";
        dict[edge2] = "Second"; // Should overwrite "First"
        dict[edge3] = "Third";

        // Assert
        Assert.Equal(2, dict.Count);
        Assert.Equal("Second", dict[edge1]);
        Assert.Equal("Third", dict[edge3]);
    }

    [Fact]
    public void Edge_WithImplicitNodeIdConversion_WorksCorrectly()
    {
        // Act
        var edge = new Edge(1, 2); // Using implicit conversion from int to NodeId

        // Assert
        Assert.Equal(1, edge.Source.Value);
        Assert.Equal(2, edge.Target.Value);
    }
}