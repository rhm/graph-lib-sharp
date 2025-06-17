using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class DenseDirectedGraphTests
{
    [Fact]
    public void Constructor_WithPositiveNodeCount_CreatesEmptyGraph()
    {
        // Act
        var graph = new DenseDirectedGraph(10);

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void Constructor_WithZeroNodeCount_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DenseDirectedGraph(0));
    }

    [Fact]
    public void Constructor_WithNegativeNodeCount_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DenseDirectedGraph(-1));
    }

    [Fact]
    public void AddNode_IncreasesNodeCount()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);

        // Act
        var node = graph.AddNode();

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Contains(node, graph.Nodes);
        Assert.True(graph.ContainsNode(node));
    }

    [Fact]
    public void AddNode_ExceedsCapacity_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new DenseDirectedGraph(2);
        graph.AddNode();
        graph.AddNode();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddNode());
    }

    [Fact]
    public void AddEdge_ValidNodes_AddsEdge()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act
        graph.AddEdge(source, target);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(source, target));
        Assert.False(graph.HasEdge(target, source)); // Directed graph
    }

    [Fact]
    public void AddEdge_NonExistentSource_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var target = graph.AddNode();
        var fakeSource = new NodeId(999);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(fakeSource, target));
    }

    [Fact]
    public void AddEdge_NonExistentTarget_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var source = graph.AddNode();
        var fakeTarget = new NodeId(999);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(source, fakeTarget));
    }

    [Fact]
    public void AddEdge_ExceedsMatrixCapacity_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new DenseDirectedGraph(2);
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        
        // Manually create nodes with IDs beyond capacity
        var invalidNode = new NodeId(10);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(node1, invalidNode));
    }

    [Fact]
    public void HasEdge_ConstantTimeOperation()
    {
        // Arrange
        var graph = new DenseDirectedGraph(100);
        var nodes = new List<NodeId>();
        for (int i = 0; i < 50; i++)
        {
            nodes.Add(graph.AddNode());
        }

        // Add some edges
        graph.AddEdge(nodes[0], nodes[10]);
        graph.AddEdge(nodes[5], nodes[25]);

        // Act & Assert
        Assert.True(graph.HasEdge(nodes[0], nodes[10]));
        Assert.True(graph.HasEdge(nodes[5], nodes[25]));
        Assert.False(graph.HasEdge(nodes[10], nodes[0])); // Reverse direction
        Assert.False(graph.HasEdge(nodes[0], nodes[5])); // Non-existent edge
    }

    [Fact]
    public void OutNeighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var source = graph.AddNode();
        var target1 = graph.AddNode();
        var target2 = graph.AddNode();
        var target3 = graph.AddNode();
        
        graph.AddEdge(source, target1);
        graph.AddEdge(source, target2);

        // Act
        var neighbors = graph.OutNeighbors(source).ToList();

        // Assert
        Assert.Equal(2, neighbors.Count);
        Assert.Contains(target1, neighbors);
        Assert.Contains(target2, neighbors);
        Assert.DoesNotContain(target3, neighbors);
    }

    [Fact]
    public void InNeighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var target = graph.AddNode();
        var source1 = graph.AddNode();
        var source2 = graph.AddNode();
        var source3 = graph.AddNode();
        
        graph.AddEdge(source1, target);
        graph.AddEdge(source2, target);

        // Act
        var neighbors = graph.InNeighbors(target).ToList();

        // Assert
        Assert.Equal(2, neighbors.Count);
        Assert.Contains(source1, neighbors);
        Assert.Contains(source2, neighbors);
        Assert.DoesNotContain(source3, neighbors);
    }

    [Fact]
    public void OutDegree_CountsOutgoingEdges()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var source = graph.AddNode();
        var target1 = graph.AddNode();
        var target2 = graph.AddNode();
        var target3 = graph.AddNode();
        
        graph.AddEdge(source, target1);
        graph.AddEdge(source, target2);
        graph.AddEdge(target3, source); // Incoming edge, shouldn't count

        // Act
        var outDegree = graph.OutDegree(source);

        // Assert
        Assert.Equal(2, outDegree);
    }

    [Fact]
    public void InDegree_CountsIncomingEdges()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var target = graph.AddNode();
        var source1 = graph.AddNode();
        var source2 = graph.AddNode();
        var source3 = graph.AddNode();
        
        graph.AddEdge(source1, target);
        graph.AddEdge(source2, target);
        graph.AddEdge(target, source3); // Outgoing edge, shouldn't count

        // Act
        var inDegree = graph.InDegree(target);

        // Assert
        Assert.Equal(2, inDegree);
    }

    [Fact]
    public void Degree_SumOfInAndOutDegree()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var node = graph.AddNode();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node, node1); // out edge
        graph.AddEdge(node, node2); // out edge
        graph.AddEdge(node3, node); // in edge

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
    public void RemoveNode_RemovesNodeAndIncidentEdges()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node3, node1);

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
        var graph = new DenseDirectedGraph(5);
        var source = graph.AddNode();
        var target = graph.AddNode();
        graph.AddEdge(source, target);

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
        var graph = new DenseDirectedGraph(5);
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
    public void SelfLoop_AllowedInDirectedGraph()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var node = graph.AddNode();

        // Act
        graph.AddEdge(node, node);

        // Assert
        Assert.True(graph.HasEdge(node, node));
        Assert.Equal(2, graph.Degree(node)); // Self-loop counts as both in and out
        Assert.Equal(1, graph.OutDegree(node));
        Assert.Equal(1, graph.InDegree(node));
    }

    [Fact]
    public void AddEdge_DuplicateEdge_DoesNotIncreaseEdgeCount()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var source = graph.AddNode();
        var target = graph.AddNode();
        graph.AddEdge(source, target);

        // Act
        graph.AddEdge(source, target); // Add same edge again

        // Assert
        Assert.Equal(1, graph.EdgeCount); // Should still be 1
        Assert.True(graph.HasEdge(source, target));
    }

    [Fact]
    public void WithEdge_FluentInterface_AddsEdgeAndReturnsGraph()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var source = graph.AddNode();
        var target = graph.AddNode();

        // Act
        var result = graph.WithEdge(source, target);

        // Assert
        Assert.Same(graph, result);
        Assert.True(graph.HasEdge(source, target));
    }

    [Fact]
    public void WithEdges_FluentInterface_AddsMultipleEdges()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();

        var edges = new[]
        {
            new Edge(node1, node2),
            new Edge(node2, node3),
            new Edge(node3, node1)
        };

        // Act
        var result = graph.WithEdges(edges);

        // Assert
        Assert.Same(graph, result);
        Assert.Equal(3, graph.EdgeCount);
        Assert.True(graph.HasEdge(node1, node2));
        Assert.True(graph.HasEdge(node2, node3));
        Assert.True(graph.HasEdge(node3, node1));
    }

    [Fact]
    public void CompleteGraph_AllNodesConnected()
    {
        // Arrange
        var graph = new DenseDirectedGraph(4);
        var nodes = new List<NodeId>();
        for (int i = 0; i < 4; i++)
        {
            nodes.Add(graph.AddNode());
        }

        // Create complete directed graph (all possible edges)
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (i != j) // No self-loops
                {
                    graph.AddEdge(nodes[i], nodes[j]);
                }
            }
        }

        // Act & Assert
        Assert.Equal(4, graph.NodeCount);
        Assert.Equal(12, graph.EdgeCount); // 4 * 3 = 12 (n * (n-1) for complete directed graph)
        
        // Each node should have out-degree and in-degree of 3
        foreach (var node in nodes)
        {
            Assert.Equal(3, graph.OutDegree(node));
            Assert.Equal(3, graph.InDegree(node));
            Assert.Equal(6, graph.Degree(node));
        }
    }

    [Fact]
    public void EmptyNeighbors_NonExistentNode_ReturnsEmpty()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var fakeNode = new NodeId(999);

        // Act
        var outNeighbors = graph.OutNeighbors(fakeNode);
        var inNeighbors = graph.InNeighbors(fakeNode);

        // Assert
        Assert.Empty(outNeighbors);
        Assert.Empty(inNeighbors);
    }

    [Fact]
    public void Degrees_NonExistentNode_ReturnsZero()
    {
        // Arrange
        var graph = new DenseDirectedGraph(5);
        var fakeNode = new NodeId(999);

        // Act & Assert
        Assert.Equal(0, graph.OutDegree(fakeNode));
        Assert.Equal(0, graph.InDegree(fakeNode));
        Assert.Equal(0, graph.Degree(fakeNode));
    }

    [Fact]
    public void LargeCapacity_HandlesEfficiently()
    {
        // Arrange & Act
        var graph = new DenseDirectedGraph(1000);
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(node1, node2));
    }
}