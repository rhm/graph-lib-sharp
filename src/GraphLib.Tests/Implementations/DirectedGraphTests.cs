using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class DirectedGraphTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        // Act
        var graph = new DirectedGraph();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void Constructor_WithCapacity_CreatesEmptyGraph()
    {
        // Act
        var graph = new DirectedGraph(100);

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Constructor_WithNegativeCapacity_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new DirectedGraph(-1));
    }

    [Fact]
    public void AddNode_AddsNewNode()
    {
        // Arrange
        var graph = new DirectedGraph();

        // Act
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.NotEqual(node1, node2);
        Assert.True(graph.ContainsNode(node1));
        Assert.True(graph.ContainsNode(node2));
    }

    [Fact]
    public void AddNode_ReturnsSequentialIds()
    {
        // Arrange
        var graph = new DirectedGraph();

        // Act
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();

        // Assert
        Assert.Equal(0, node1.Value);
        Assert.Equal(1, node2.Value);
        Assert.Equal(2, node3.Value);
    }

    [Fact]
    public void ContainsNode_ReturnsFalseForNonExistentNode()
    {
        // Arrange
        var graph = new DirectedGraph();
        graph.AddNode();

        // Act & Assert
        Assert.False(graph.ContainsNode(new NodeId(999)));
    }

    [Fact]
    public void AddEdge_AddsEdgeBetweenExistingNodes()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act
        graph.AddEdge(node1, node2);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node1)); // Directed graph
    }

    [Fact]
    public void AddEdge_DoesNotDuplicateExistingEdge()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node2); // Add same edge again

        // Assert
        Assert.Equal(1, graph.EdgeCount);
    }

    [Fact]
    public void AddEdge_WithNonExistentSource_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node = graph.AddNode();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.AddEdge(new NodeId(999), node));
    }

    [Fact]
    public void AddEdge_WithNonExistentTarget_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node = graph.AddNode();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.AddEdge(node, new NodeId(999)));
    }

    [Fact]
    public void RemoveEdge_RemovesExistingEdge()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Act
        graph.RemoveEdge(node1, node2);

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(node1, node2));
    }

    [Fact]
    public void RemoveEdge_NonExistentEdge_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.RemoveEdge(node1, node2));
    }

    [Fact]
    public void OutNeighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node3, node1);

        // Act
        var outNeighbors = graph.OutNeighbors(node1).ToList();

        // Assert
        Assert.Equal(2, outNeighbors.Count);
        Assert.Contains(node2, outNeighbors);
        Assert.Contains(node3, outNeighbors);
    }

    [Fact]
    public void OutNeighbors_ForNonExistentNode_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.OutNeighbors(new NodeId(999)));
    }

    [Fact]
    public void InNeighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node3);

        // Act
        var inNeighbors = graph.InNeighbors(node3).ToList();

        // Assert
        Assert.Equal(2, inNeighbors.Count);
        Assert.Contains(node1, inNeighbors);
        Assert.Contains(node2, inNeighbors);
    }

    [Fact]
    public void OutDegree_ReturnsCorrectValue()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node3);

        // Act & Assert
        Assert.Equal(2, graph.OutDegree(node1));
        Assert.Equal(0, graph.OutDegree(node2));
        Assert.Equal(0, graph.OutDegree(node3));
    }

    [Fact]
    public void InDegree_ReturnsCorrectValue()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node3);

        // Act & Assert
        Assert.Equal(0, graph.InDegree(node1));
        Assert.Equal(0, graph.InDegree(node2));
        Assert.Equal(2, graph.InDegree(node3));
    }

    [Fact]
    public void Degree_ReturnsSumOfInAndOutDegree()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node2); // node1 out-degree: 1
        graph.AddEdge(node3, node1); // node1 in-degree: 1

        // Act & Assert
        Assert.Equal(2, graph.Degree(node1)); // 1 + 1
        Assert.Equal(1, graph.Degree(node2)); // 0 + 1
        Assert.Equal(1, graph.Degree(node3)); // 1 + 0
    }

    [Fact]
    public void RemoveNode_RemovesNodeAndAllIncidentEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node3, node2);

        // Act
        graph.RemoveNode(node2);

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.ContainsNode(node2));
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node3));
        Assert.False(graph.HasEdge(node3, node2));
    }

    [Fact]
    public void RemoveNode_NonExistentNode_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.RemoveNode(new NodeId(999)));
    }

    [Fact]
    public void Clear_RemovesAllNodesAndEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Act
        graph.Clear();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void WithEdge_AddsNodesAndEdge()
    {
        // Arrange
        var graph = new DirectedGraph();

        // Act
        graph.WithEdge(0, 1)
             .WithEdge(1, 2)
             .WithEdge(2, 0);

        // Assert
        Assert.Equal(3, graph.NodeCount);
        Assert.Equal(3, graph.EdgeCount);
        Assert.True(graph.HasEdge(0, 1));
        Assert.True(graph.HasEdge(1, 2));
        Assert.True(graph.HasEdge(2, 0));
    }

    [Fact]
    public void WithEdges_AddsMultipleEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        var edges = new[]
        {
            new Edge(0, 1),
            new Edge(1, 2),
            new Edge(2, 3)
        };

        // Act
        graph.WithEdges(edges);

        // Assert
        Assert.Equal(4, graph.NodeCount);
        Assert.Equal(3, graph.EdgeCount);
        foreach (var edge in edges)
        {
            Assert.True(graph.HasEdge(edge.Source, edge.Target));
        }
    }

    [Fact]
    public void GetEdges_ReturnsAllEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        graph.WithEdge(0, 1)
             .WithEdge(1, 2)
             .WithEdge(0, 2);

        // Act
        var edges = graph.GetEdges().ToList();

        // Assert
        Assert.Equal(3, edges.Count);
        Assert.Contains(new Edge(0, 1), edges);
        Assert.Contains(new Edge(1, 2), edges);
        Assert.Contains(new Edge(0, 2), edges);
    }

    [Fact]
    public void SelfLoop_IsAllowed()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node = graph.AddNode();

        // Act
        graph.AddEdge(node, node);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(node, node));
        Assert.Equal(1, graph.OutDegree(node));
        Assert.Equal(1, graph.InDegree(node));
        Assert.Equal(2, graph.Degree(node)); // Counts both in and out
    }

    [Fact]
    public void Nodes_EnumeratesAllNodes()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();

        // Act
        var nodes = graph.Nodes.ToList();

        // Assert
        Assert.Equal(3, nodes.Count);
        Assert.Contains(node1, nodes);
        Assert.Contains(node2, nodes);
        Assert.Contains(node3, nodes);
    }

    [Fact]
    public void ComplexGraph_WorksCorrectly()
    {
        // Arrange - Create a more complex graph
        var graph = new DirectedGraph();
        
        // Create a graph with multiple components
        graph.WithEdge(0, 1)
             .WithEdge(1, 2)
             .WithEdge(2, 0)  // Cycle in first component
             .WithEdge(3, 4)
             .WithEdge(4, 5)  // Second component
             .WithEdge(6, 6); // Self-loop as third component

        // Assert
        Assert.Equal(7, graph.NodeCount);
        Assert.Equal(6, graph.EdgeCount);

        // Check first component
        Assert.Equal(2, graph.Degree(0));
        Assert.Equal(2, graph.Degree(1));
        Assert.Equal(2, graph.Degree(2));

        // Check second component
        Assert.Equal(1, graph.Degree(3));
        Assert.Equal(2, graph.Degree(4));
        Assert.Equal(1, graph.Degree(5));

        // Check self-loop
        Assert.Equal(2, graph.Degree(6));
    }
}