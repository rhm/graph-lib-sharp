using GraphLib.Core;
using Xunit;

namespace GraphLib.Tests.Core;

public class NodeIdTests
{
    [Fact]
    public void Constructor_SetsValueCorrectly()
    {
        // Arrange & Act
        var nodeId = new NodeId(42);

        // Assert
        Assert.Equal(42, nodeId.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Constructor_HandlesVariousValues(int value)
    {
        // Arrange & Act
        var nodeId = new NodeId(value);

        // Assert
        Assert.Equal(value, nodeId.Value);
    }

    [Fact]
    public void Equals_ReturnsTrueForSameValue()
    {
        // Arrange
        var nodeId1 = new NodeId(5);
        var nodeId2 = new NodeId(5);

        // Act & Assert
        Assert.True(nodeId1.Equals(nodeId2));
        Assert.True(nodeId1 == nodeId2);
        Assert.False(nodeId1 != nodeId2);
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentValue()
    {
        // Arrange
        var nodeId1 = new NodeId(5);
        var nodeId2 = new NodeId(10);

        // Act & Assert
        Assert.False(nodeId1.Equals(nodeId2));
        Assert.False(nodeId1 == nodeId2);
        Assert.True(nodeId1 != nodeId2);
    }

    [Fact]
    public void Equals_WithObject_WorksCorrectly()
    {
        // Arrange
        var nodeId = new NodeId(5);
        object sameValue = new NodeId(5);
        object differentValue = new NodeId(10);
        object notNodeId = "not a node id";

        // Act & Assert
        Assert.True(nodeId.Equals(sameValue));
        Assert.False(nodeId.Equals(differentValue));
        Assert.False(nodeId.Equals(notNodeId));
        Assert.False(nodeId.Equals(null));
    }

    [Fact]
    public void GetHashCode_SameForEqualValues()
    {
        // Arrange
        var nodeId1 = new NodeId(42);
        var nodeId2 = new NodeId(42);

        // Act & Assert
        Assert.Equal(nodeId1.GetHashCode(), nodeId2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentForDifferentValues()
    {
        // Arrange
        var nodeId1 = new NodeId(1);
        var nodeId2 = new NodeId(2);

        // Act & Assert
        Assert.NotEqual(nodeId1.GetHashCode(), nodeId2.GetHashCode());
    }

    [Theory]
    [InlineData(1, 2, -1)]
    [InlineData(2, 1, 1)]
    [InlineData(1, 1, 0)]
    [InlineData(-1, 1, -1)]
    [InlineData(0, 0, 0)]
    public void CompareTo_ReturnsCorrectResult(int value1, int value2, int expectedSign)
    {
        // Arrange
        var nodeId1 = new NodeId(value1);
        var nodeId2 = new NodeId(value2);

        // Act
        var result = nodeId1.CompareTo(nodeId2);

        // Assert
        Assert.Equal(expectedSign, Math.Sign(result));
    }

    [Fact]
    public void ComparisonOperators_WorkCorrectly()
    {
        // Arrange
        var smaller = new NodeId(1);
        var larger = new NodeId(5);
        var equal1 = new NodeId(3);
        var equal2 = new NodeId(3);

        // Act & Assert
        Assert.True(smaller < larger);
        Assert.False(larger < smaller);
        Assert.False(equal1 < equal2);

        Assert.True(smaller <= larger);
        Assert.True(equal1 <= equal2);
        Assert.False(larger <= smaller);

        Assert.True(larger > smaller);
        Assert.False(smaller > larger);
        Assert.False(equal1 > equal2);

        Assert.True(larger >= smaller);
        Assert.True(equal1 >= equal2);
        Assert.False(smaller >= larger);
    }

    [Fact]
    public void ToString_ReturnsValueAsString()
    {
        // Arrange
        var nodeId = new NodeId(123);

        // Act
        var result = nodeId.ToString();

        // Assert
        Assert.Equal("123", result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-10)]
    public void ImplicitConversion_FromInt_WorksCorrectly(int value)
    {
        // Act
        NodeId nodeId = value;

        // Assert
        Assert.Equal(value, nodeId.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-10)]
    public void ImplicitConversion_ToInt_WorksCorrectly(int value)
    {
        // Arrange
        var nodeId = new NodeId(value);

        // Act
        int result = nodeId;

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void NodeId_CanBeUsedInHashSet()
    {
        // Arrange
        var set = new HashSet<NodeId>();
        var nodeId1 = new NodeId(1);
        var nodeId2 = new NodeId(1); // Same value
        var nodeId3 = new NodeId(2);

        // Act
        set.Add(nodeId1);
        set.Add(nodeId2); // Should not be added (duplicate)
        set.Add(nodeId3);

        // Assert
        Assert.Equal(2, set.Count);
        Assert.Contains(nodeId1, set);
        Assert.Contains(nodeId3, set);
    }

    [Fact]
    public void NodeId_CanBeUsedAsDictionaryKey()
    {
        // Arrange
        var dict = new Dictionary<NodeId, string>();
        var nodeId1 = new NodeId(1);
        var nodeId2 = new NodeId(1); // Same value
        var nodeId3 = new NodeId(2);

        // Act
        dict[nodeId1] = "First";
        dict[nodeId2] = "Second"; // Should overwrite "First"
        dict[nodeId3] = "Third";

        // Assert
        Assert.Equal(2, dict.Count);
        Assert.Equal("Second", dict[nodeId1]);
        Assert.Equal("Third", dict[nodeId3]);
    }
}