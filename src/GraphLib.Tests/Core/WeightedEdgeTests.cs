using GraphLib.Core;
using Xunit;

namespace GraphLib.Tests.Core;

public class WeightedEdgeTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var source = new NodeId(1);
        var target = new NodeId(2);
        var weight = 42.5;

        // Act
        var edge = new WeightedEdge<double>(source, target, weight);

        // Assert
        Assert.Equal(source, edge.Source);
        Assert.Equal(target, edge.Target);
        Assert.Equal(weight, edge.Weight);
    }

    [Fact]
    public void Constructor_WorksWithDifferentWeightTypes()
    {
        // Int weight
        var intEdge = new WeightedEdge<int>(1, 2, 10);
        Assert.Equal(10, intEdge.Weight);

        // String weight
        var stringEdge = new WeightedEdge<string>(1, 2, "high");
        Assert.Equal("high", stringEdge.Weight);

        // Custom type weight
        var customEdge = new WeightedEdge<DateTime>(new NodeId(1), new NodeId(2), DateTime.Today);
        Assert.Equal(DateTime.Today, customEdge.Weight);
    }

    [Fact]
    public void Reverse_ReturnsEdgeWithSwappedNodesAndSameWeight()
    {
        // Arrange
        var edge = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 100);

        // Act
        var reversed = edge.Reverse();

        // Assert
        Assert.Equal(new NodeId(2), reversed.Source);
        Assert.Equal(new NodeId(1), reversed.Target);
        Assert.Equal(100, reversed.Weight);
    }

    [Fact]
    public void WithWeight_CreatesNewEdgeWithDifferentWeight()
    {
        // Arrange
        var edge = new WeightedEdge<double>(new NodeId(1), new NodeId(2), 10.0);

        // Act
        var newEdge = edge.WithWeight(20.0);

        // Assert
        Assert.Equal(new NodeId(1), newEdge.Source);
        Assert.Equal(new NodeId(2), newEdge.Target);
        Assert.Equal(20.0, newEdge.Weight);
        Assert.Equal(10.0, edge.Weight); // Original unchanged
    }

    [Fact]
    public void ToEdge_ReturnsUnweightedEdge()
    {
        // Arrange
        var weightedEdge = new WeightedEdge<double>(new NodeId(3), new NodeId(4), 15.5);

        // Act
        var edge = weightedEdge.ToEdge();

        // Assert
        Assert.Equal(new NodeId(3), edge.Source);
        Assert.Equal(new NodeId(4), edge.Target);
    }

    [Fact]
    public void Equals_ReturnsTrueForSameValues()
    {
        // Arrange
        var edge1 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);
        var edge2 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);

        // Act & Assert
        Assert.True(edge1.Equals(edge2));
        Assert.True(edge1 == edge2);
        Assert.False(edge1 != edge2);
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentSource()
    {
        // Arrange
        var edge1 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);
        var edge2 = new WeightedEdge<int>(new NodeId(3), new NodeId(2), 10);

        // Act & Assert
        Assert.False(edge1.Equals(edge2));
        Assert.False(edge1 == edge2);
        Assert.True(edge1 != edge2);
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentTarget()
    {
        // Arrange
        var edge1 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);
        var edge2 = new WeightedEdge<int>(new NodeId(1), new NodeId(3), 10);

        // Act & Assert
        Assert.False(edge1.Equals(edge2));
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentWeight()
    {
        // Arrange
        var edge1 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);
        var edge2 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 20);

        // Act & Assert
        Assert.False(edge1.Equals(edge2));
    }

    [Fact]
    public void Equals_WithNullableWeight_WorksCorrectly()
    {
        // Arrange
        var edge1 = new WeightedEdge<string?>(new NodeId(1), new NodeId(2), null);
        var edge2 = new WeightedEdge<string?>(new NodeId(1), new NodeId(2), null);
        var edge3 = new WeightedEdge<string?>(new NodeId(1), new NodeId(2), "weight");

        // Act & Assert
        Assert.True(edge1.Equals(edge2));
        Assert.False(edge1.Equals(edge3));
    }

    [Fact]
    public void Equals_WithObject_WorksCorrectly()
    {
        // Arrange
        var edge = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);
        object sameEdge = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);
        object differentEdge = new WeightedEdge<int>(new NodeId(3), new NodeId(4), 20);
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
        var edge1 = new WeightedEdge<double>(new NodeId(1), new NodeId(2), 3.14);
        var edge2 = new WeightedEdge<double>(new NodeId(1), new NodeId(2), 3.14);

        // Act & Assert
        Assert.Equal(edge1.GetHashCode(), edge2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentForDifferentEdges()
    {
        // Arrange
        var edge1 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);
        var edge2 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 20);
        var edge3 = new WeightedEdge<int>(new NodeId(2), new NodeId(1), 10);

        // Act & Assert
        Assert.NotEqual(edge1.GetHashCode(), edge2.GetHashCode());
        Assert.NotEqual(edge1.GetHashCode(), edge3.GetHashCode());
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var edge = new WeightedEdge<double>(new NodeId(1), new NodeId(2), 3.14);

        // Act
        var result = edge.ToString();

        // Assert
        Assert.Equal("1 -> 2 (Weight: 3.14)", result);
    }

    [Fact]
    public void Deconstruct_WorksCorrectly()
    {
        // Arrange
        var edge = new WeightedEdge<string>(new NodeId(5), new NodeId(10), "high");

        // Act
        var (source, target, weight) = edge;

        // Assert
        Assert.Equal(new NodeId(5), source);
        Assert.Equal(new NodeId(10), target);
        Assert.Equal("high", weight);
    }

    [Fact]
    public void ImplicitConversion_ToEdge_WorksCorrectly()
    {
        // Arrange
        var weightedEdge = new WeightedEdge<double>(new NodeId(7), new NodeId(8), 99.9);

        // Act
        Edge edge = weightedEdge; // Implicit conversion

        // Assert
        Assert.Equal(new NodeId(7), edge.Source);
        Assert.Equal(new NodeId(8), edge.Target);
    }

    [Fact]
    public void WeightedEdge_CanBeUsedInHashSet()
    {
        // Arrange
        var set = new HashSet<WeightedEdge<int>>();
        var edge1 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10);
        var edge2 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 10); // Same edge
        var edge3 = new WeightedEdge<int>(new NodeId(1), new NodeId(2), 20); // Different weight

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
    public void WeightedEdge_CanBeUsedAsDictionaryKey()
    {
        // Arrange
        var dict = new Dictionary<WeightedEdge<double>, string>();
        var edge1 = new WeightedEdge<double>(new NodeId(1), new NodeId(2), 1.5);
        var edge2 = new WeightedEdge<double>(new NodeId(1), new NodeId(2), 1.5); // Same edge
        var edge3 = new WeightedEdge<double>(new NodeId(3), new NodeId(4), 2.5);

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
    public void WeightedEdge_WithCustomWeightType_WorksCorrectly()
    {
        // Arrange
        var weight1 = new CustomWeight { Value = 10 };
        var weight2 = new CustomWeight { Value = 10 };
        var weight3 = new CustomWeight { Value = 20 };

        var edge1 = new WeightedEdge<CustomWeight>(new NodeId(1), new NodeId(2), weight1);
        var edge2 = new WeightedEdge<CustomWeight>(new NodeId(1), new NodeId(2), weight2);
        var edge3 = new WeightedEdge<CustomWeight>(new NodeId(1), new NodeId(2), weight3);

        // Act & Assert
        Assert.True(edge1.Equals(edge2));
        Assert.False(edge1.Equals(edge3));
    }

    private class CustomWeight : IEquatable<CustomWeight>
    {
        public int Value { get; set; }

        public bool Equals(CustomWeight? other)
        {
            return other != null && Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CustomWeight);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}