using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class WeightedUndirectedGraphTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        // Act
        var graph = new WeightedUndirectedGraph<double>();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void Constructor_WithCapacity_CreatesEmptyGraph()
    {
        // Act
        var graph = new WeightedUndirectedGraph<int>(100);

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void AddNode_IncreasesNodeCount()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();

        // Act
        var node = graph.AddNode();

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Contains(node, graph.Nodes);
        Assert.True(graph.ContainsNode(node));
    }

    [Fact]
    public void AddEdge_WithWeight_CreatesSymmetricEdge()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act
        graph.AddEdge(node1, node2, 5.5);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(node1, node2));
        Assert.True(graph.HasEdge(node2, node1)); // Undirected graph - symmetric
    }

    [Fact]
    public void AddEdge_WithoutWeight_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(node1, node2));
    }

    [Fact]
    public void GetEdgeWeight_BothDirections_ReturnsSameWeight()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var weight = 3.14;
        graph.AddEdge(node1, node2, weight);

        // Act & Assert
        Assert.Equal(weight, graph.GetEdgeWeight(node1, node2));
        Assert.Equal(weight, graph.GetEdgeWeight(node2, node1));
    }

    [Fact]
    public void UpdateEdgeWeight_UpdatesBothDirections()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2, 1.0);

        // Act
        graph.UpdateEdgeWeight(node1, node2, 2.0);

        // Assert
        Assert.Equal(2.0, graph.GetEdgeWeight(node1, node2));
        Assert.Equal(2.0, graph.GetEdgeWeight(node2, node1));
    }

    [Fact]
    public void Neighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var center = graph.AddNode();
        var neighbor1 = graph.AddNode();
        var neighbor2 = graph.AddNode();
        var neighbor3 = graph.AddNode();
        
        graph.AddEdge(center, neighbor1, 1.0);
        graph.AddEdge(center, neighbor2, 2.0);
        graph.AddEdge(neighbor3, center, 3.0); // Order shouldn't matter

        // Act
        var neighbors = graph.Neighbors(center).ToList();

        // Assert
        Assert.Equal(3, neighbors.Count);
        Assert.Contains(neighbor1, neighbors);
        Assert.Contains(neighbor2, neighbors);
        Assert.Contains(neighbor3, neighbors);
    }

    [Fact]
    public void Degree_CountsIncidentEdges()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var center = graph.AddNode();
        var neighbor1 = graph.AddNode();
        var neighbor2 = graph.AddNode();
        var neighbor3 = graph.AddNode();
        
        graph.AddEdge(center, neighbor1, 1.0);
        graph.AddEdge(center, neighbor2, 2.0);
        graph.AddEdge(center, neighbor3, 3.0);

        // Act
        var degree = graph.Degree(center);

        // Assert
        Assert.Equal(3, degree);
    }

    [Fact]
    public void RemoveNode_RemovesNodeAndAllIncidentEdges()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node2, 1.0);
        graph.AddEdge(node2, node3, 2.0);
        graph.AddEdge(node1, node3, 3.0);

        // Act
        graph.RemoveNode(node2);

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.Equal(1, graph.EdgeCount); // Only node1-node3 should remain
        Assert.False(graph.ContainsNode(node2));
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node3));
        Assert.True(graph.HasEdge(node1, node3));
    }

    [Fact]
    public void RemoveEdge_RemovesSymmetricEdge()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2, 1.0);

        // Act
        graph.RemoveEdge(node1, node2);

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node1));
    }

    [Fact]
    public void RemoveEdge_ReverseDirection_RemovesEdge()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2, 1.0);

        // Act
        graph.RemoveEdge(node2, node1); // Remove in reverse direction

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node1));
    }

    [Fact]
    public void SelfLoop_AllowedInUndirectedGraph()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node = graph.AddNode();

        // Act
        graph.AddEdge(node, node, 1.0);

        // Assert
        Assert.True(graph.HasEdge(node, node));
        Assert.Equal(1.0, graph.GetEdgeWeight(node, node));
        Assert.Equal(1, graph.Degree(node)); // Self-loop counts as 1 in undirected graph
    }

    [Fact]
    public void AddEdge_ExistingEdge_UpdatesWeight()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2, 1.0);

        // Act
        graph.AddEdge(node1, node2, 2.0);

        // Assert
        Assert.Equal(1, graph.EdgeCount); // Still only one edge
        Assert.Equal(2.0, graph.GetEdgeWeight(node1, node2));
        Assert.Equal(2.0, graph.GetEdgeWeight(node2, node1));
    }

    [Fact]
    public void AddEdge_ReverseDirection_UpdatesExistingEdge()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2, 1.0);

        // Act
        graph.AddEdge(node2, node1, 3.0); // Add in reverse direction

        // Assert
        Assert.Equal(1, graph.EdgeCount); // Still only one edge
        Assert.Equal(3.0, graph.GetEdgeWeight(node1, node2));
        Assert.Equal(3.0, graph.GetEdgeWeight(node2, node1));
    }

    [Fact]
    public void WithEdge_FluentInterface_AddsEdgeAndReturnsGraph()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act
        var result = graph.WithEdge(node1, node2, 1.5);

        // Assert
        Assert.Same(graph, result);
        Assert.True(graph.HasEdge(node1, node2));
        Assert.Equal(1.5, graph.GetEdgeWeight(node1, node2));
    }

    [Fact]
    public void WithEdges_FluentInterface_AddsMultipleEdges()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
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

    [Fact]
    public void Clear_RemovesAllNodesAndEdges()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
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
    public void GenericWeightType_SupportsCustomTypes()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<string>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var weight = "custom-weight";

        // Act
        graph.AddEdge(node1, node2, weight);

        // Assert
        Assert.Equal(weight, graph.GetEdgeWeight(node1, node2));
        Assert.Equal(weight, graph.GetEdgeWeight(node2, node1));
    }

    [Fact]
    public void TryGetEdgeWeight_BothDirections_Work()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var weight = 2.5;
        graph.AddEdge(node1, node2, weight);

        // Act & Assert
        Assert.True(graph.TryGetEdgeWeight(node1, node2, out var weight1));
        Assert.True(graph.TryGetEdgeWeight(node2, node1, out var weight2));
        Assert.Equal(weight, weight1);
        Assert.Equal(weight, weight2);
    }

    [Fact]
    public void Triangle_CorrectDegrees()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node2, 1.0);
        graph.AddEdge(node2, node3, 2.0);
        graph.AddEdge(node3, node1, 3.0);

        // Act & Assert
        Assert.Equal(3, graph.EdgeCount);
        Assert.Equal(2, graph.Degree(node1));
        Assert.Equal(2, graph.Degree(node2));
        Assert.Equal(2, graph.Degree(node3));
    }
}