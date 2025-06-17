using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class WeightedDirectedGraphTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        // Act
        var graph = new WeightedDirectedGraph<double>();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void Constructor_WithCapacity_CreatesEmptyGraph()
    {
        // Act
        var graph = new WeightedDirectedGraph<int>(100);

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void AddNode_IncreasesNodeCount()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();

        // Act
        var node = graph.AddNode();

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Contains(node, graph.Nodes);
        Assert.True(graph.ContainsNode(node));
    }

    [Fact]
    public void AddMultipleNodes_GeneratesUniqueIds()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();

        // Act
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();

        // Assert
        Assert.Equal(3, graph.NodeCount);
        Assert.NotEqual(node1, node2);
        Assert.NotEqual(node2, node3);
        Assert.NotEqual(node1, node3);
    }

    [Fact]
    public void AddEdge_WithWeight_IncreasesEdgeCount()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act
        graph.AddEdge(source, target, 5.5);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(source, target));
        Assert.False(graph.HasEdge(target, source)); // Directed graph
    }

    [Fact]
    public void AddEdge_WithoutWeight_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(source, target));
    }

    [Fact]
    public void AddEdge_NonExistentNodes_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var node1 = new NodeId(999);
        var node2 = new NodeId(1000);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(node1, node2, 1.0));
    }

    [Fact]
    public void GetEdgeWeight_ExistingEdge_ReturnsCorrectWeight()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();
        var weight = 3.14;
        graph.AddEdge(source, target, weight);

        // Act
        var retrievedWeight = graph.GetEdgeWeight(source, target);

        // Assert
        Assert.Equal(weight, retrievedWeight);
    }

    [Fact]
    public void GetEdgeWeight_NonExistentEdge_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeWeight(source, target));
    }

    [Fact]
    public void TryGetEdgeWeight_ExistingEdge_ReturnsTrueAndWeight()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();
        var weight = 2.5;
        graph.AddEdge(source, target, weight);

        // Act
        var result = graph.TryGetEdgeWeight(source, target, out var retrievedWeight);

        // Assert
        Assert.True(result);
        Assert.Equal(weight, retrievedWeight);
    }

    [Fact]
    public void TryGetEdgeWeight_NonExistentEdge_ReturnsFalse()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act
        var result = graph.TryGetEdgeWeight(source, target, out var weight);

        // Assert
        Assert.False(result);
        Assert.Equal(default(double), weight);
    }

    [Fact]
    public void UpdateEdgeWeight_ExistingEdge_UpdatesWeight()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();
        graph.AddEdge(source, target, 1.0);

        // Act
        graph.UpdateEdgeWeight(source, target, 2.0);

        // Assert
        Assert.Equal(2.0, graph.GetEdgeWeight(source, target));
    }

    [Fact]
    public void UpdateEdgeWeight_NonExistentEdge_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.UpdateEdgeWeight(source, target, 1.0));
    }

    [Fact]
    public void OutNeighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target1 = graph.AddNode();
        var target2 = graph.AddNode();
        graph.AddEdge(source, target1, 1.0);
        graph.AddEdge(source, target2, 2.0);

        // Act
        var neighbors = graph.OutNeighbors(source).ToList();

        // Assert
        Assert.Equal(2, neighbors.Count);
        Assert.Contains(target1, neighbors);
        Assert.Contains(target2, neighbors);
    }

    [Fact]
    public void InNeighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var target = graph.AddNode();
        var source1 = graph.AddNode();
        var source2 = graph.AddNode();
        graph.AddEdge(source1, target, 1.0);
        graph.AddEdge(source2, target, 2.0);

        // Act
        var neighbors = graph.InNeighbors(target).ToList();

        // Assert
        Assert.Equal(2, neighbors.Count);
        Assert.Contains(source1, neighbors);
        Assert.Contains(source2, neighbors);
    }

    [Fact]
    public void Degree_SumOfInAndOutDegree()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var node = graph.AddNode();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node, node1, 1.0); // out edge
        graph.AddEdge(node, node2, 2.0); // out edge
        graph.AddEdge(node3, node, 3.0); // in edge

        // Act
        var degree = graph.Degree(node);
        var outDegree = graph.OutDegree(node);
        var inDegree = graph.InDegree(node);

        // Assert
        Assert.Equal(3, degree);
        Assert.Equal(2, outDegree);
        Assert.Equal(1, inDegree);
        Assert.Equal(outDegree + inDegree, degree);
    }

    [Fact]
    public void RemoveNode_RemovesNodeAndAllIncidentEdges()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node2, 1.0);
        graph.AddEdge(node2, node3, 2.0);
        graph.AddEdge(node3, node1, 3.0);

        // Act
        graph.RemoveNode(node2);

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.Equal(1, graph.EdgeCount); // Only node3->node1 should remain
        Assert.False(graph.ContainsNode(node2));
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node3));
        Assert.True(graph.HasEdge(node3, node1));
    }

    [Fact]
    public void RemoveEdge_RemovesSpecificEdge()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();
        graph.AddEdge(source, target, 1.0);

        // Act
        graph.RemoveEdge(source, target);

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(source, target));
    }

    [Fact]
    public void Clear_RemovesAllNodesAndEdges()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2, 1.0);

        // Act
        graph.Clear();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void WithEdge_FluentInterface_AddsEdgeAndReturnsGraph()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act
        var result = graph.WithEdge(source, target, 1.5);

        // Assert
        Assert.Same(graph, result);
        Assert.True(graph.HasEdge(source, target));
        Assert.Equal(1.5, graph.GetEdgeWeight(source, target));
    }

    [Fact]
    public void WithEdges_FluentInterface_AddsMultipleEdges()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();

        var edges = new[]
        {
            new WeightedEdge<double>(node1, node2, 1.0),
            new WeightedEdge<double>(node2, node3, 2.0),
            new WeightedEdge<double>(node3, node1, 3.0)
        };

        // Act
        var result = graph.WithEdges(edges);

        // Assert
        Assert.Same(graph, result);
        Assert.Equal(3, graph.EdgeCount);
        Assert.Equal(1.0, graph.GetEdgeWeight(node1, node2));
        Assert.Equal(2.0, graph.GetEdgeWeight(node2, node3));
        Assert.Equal(3.0, graph.GetEdgeWeight(node3, node1));
    }

    [Theory]
    [InlineData(1.5)]
    [InlineData(-2.5)]
    [InlineData(0.0)]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    public void EdgeWeights_SupportsVariousDoubleValues(double weight)
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act
        graph.AddEdge(source, target, weight);

        // Assert
        Assert.Equal(weight, graph.GetEdgeWeight(source, target));
    }

    [Fact]
    public void GenericWeightType_SupportsStringWeights()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<string>();
        var source = graph.AddNode();
        var target = graph.AddNode();
        var weight = "test-weight";

        // Act
        graph.AddEdge(source, target, weight);

        // Assert
        Assert.Equal(weight, graph.GetEdgeWeight(source, target));
    }

    [Fact]
    public void SelfLoop_AllowedInDirectedGraph()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var node = graph.AddNode();

        // Act
        graph.AddEdge(node, node, 1.0);

        // Assert
        Assert.True(graph.HasEdge(node, node));
        Assert.Equal(1.0, graph.GetEdgeWeight(node, node));
        Assert.Equal(2, graph.Degree(node)); // Self-loop counts as both in and out
    }

    [Fact]
    public void AddEdge_ExistingEdge_UpdatesWeight()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();
        graph.AddEdge(source, target, 1.0);

        // Act
        graph.AddEdge(source, target, 2.0);

        // Assert
        Assert.Equal(1, graph.EdgeCount); // Still only one edge
        Assert.Equal(2.0, graph.GetEdgeWeight(source, target));
    }
}