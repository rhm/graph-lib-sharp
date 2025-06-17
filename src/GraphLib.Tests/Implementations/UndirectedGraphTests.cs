using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class UndirectedGraphTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        // Act
        var graph = new UndirectedGraph();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void Constructor_WithCapacity_CreatesEmptyGraph()
    {
        // Act
        var graph = new UndirectedGraph(100);

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Constructor_WithNegativeCapacity_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new UndirectedGraph(-1));
    }

    [Fact]
    public void AddNode_AddsNewNode()
    {
        // Arrange
        var graph = new UndirectedGraph();

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
        var graph = new UndirectedGraph();

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
        var graph = new UndirectedGraph();
        graph.AddNode();

        // Act & Assert
        Assert.False(graph.ContainsNode(new NodeId(999)));
    }

    [Fact]
    public void AddEdge_AddsEdgeBetweenExistingNodes()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act
        graph.AddEdge(node1, node2);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(node1, node2));
        Assert.True(graph.HasEdge(node2, node1)); // Undirected graph - symmetric
    }

    [Fact]
    public void AddEdge_DoesNotDuplicateExistingEdge()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node2); // Add same edge again
        graph.AddEdge(node2, node1); // Add reverse edge (same for undirected)

        // Assert
        Assert.Equal(1, graph.EdgeCount);
    }

    [Fact]
    public void AddEdge_WithNonExistentFirstNode_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node = graph.AddNode();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.AddEdge(new NodeId(999), node));
    }

    [Fact]
    public void AddEdge_WithNonExistentSecondNode_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node = graph.AddNode();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.AddEdge(node, new NodeId(999)));
    }

    [Fact]
    public void RemoveEdge_RemovesExistingEdge()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Act
        graph.RemoveEdge(node1, node2);

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node1));
    }

    [Fact]
    public void RemoveEdge_WithReversedOrder_RemovesEdge()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Act
        graph.RemoveEdge(node2, node1); // Remove in reverse order

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node1));
    }

    [Fact]
    public void RemoveEdge_NonExistentEdge_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.RemoveEdge(node1, node2));
    }

    [Fact]
    public void Neighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node4);

        // Act
        var neighborsOfNode1 = graph.Neighbors(node1).OrderBy(n => n.Value).ToList();
        var neighborsOfNode2 = graph.Neighbors(node2).OrderBy(n => n.Value).ToList();

        // Assert
        Assert.Equal(2, neighborsOfNode1.Count);
        Assert.Contains(node2, neighborsOfNode1);
        Assert.Contains(node3, neighborsOfNode1);

        Assert.Equal(2, neighborsOfNode2.Count);
        Assert.Contains(node1, neighborsOfNode2);
        Assert.Contains(node4, neighborsOfNode2);
    }

    [Fact]
    public void Neighbors_ForNonExistentNode_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.Neighbors(new NodeId(999)));
    }

    [Fact]
    public void Degree_ReturnsCorrectValue()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node3);

        // Act & Assert
        Assert.Equal(2, graph.Degree(node1));
        Assert.Equal(1, graph.Degree(node2));
        Assert.Equal(1, graph.Degree(node3));
    }

    [Fact]
    public void Degree_ForNonExistentNode_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.Degree(new NodeId(999)));
    }

    [Fact]
    public void RemoveNode_RemovesNodeAndAllIncidentEdges()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node2, node4);

        // Act
        graph.RemoveNode(node2);

        // Assert
        Assert.Equal(3, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.ContainsNode(node2));
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node3));
        Assert.False(graph.HasEdge(node2, node4));
    }

    [Fact]
    public void RemoveNode_NonExistentNode_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => graph.RemoveNode(new NodeId(999)));
    }

    [Fact]
    public void Clear_RemovesAllNodesAndEdges()
    {
        // Arrange
        var graph = new UndirectedGraph();
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
        var graph = new UndirectedGraph();

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
        
        // Check symmetry
        Assert.True(graph.HasEdge(1, 0));
        Assert.True(graph.HasEdge(2, 1));
        Assert.True(graph.HasEdge(0, 2));
    }

    [Fact]
    public void WithEdges_AddsMultipleEdges()
    {
        // Arrange
        var graph = new UndirectedGraph();
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
            Assert.True(graph.HasEdge(edge.Target, edge.Source));
        }
    }

    [Fact]
    public void GetEdges_ReturnsAllEdgesOnce()
    {
        // Arrange
        var graph = new UndirectedGraph();
        graph.WithEdge(0, 1)
             .WithEdge(1, 2)
             .WithEdge(0, 2);

        // Act
        var edges = graph.GetEdges().ToList();

        // Assert
        Assert.Equal(3, edges.Count);
        
        // Check that each undirected edge appears only once
        var edgeSet = new HashSet<(int, int)>();
        foreach (var edge in edges)
        {
            var normalizedEdge = edge.Source.Value < edge.Target.Value 
                ? (edge.Source.Value, edge.Target.Value)
                : (edge.Target.Value, edge.Source.Value);
            Assert.True(edgeSet.Add(normalizedEdge), $"Duplicate edge found: {normalizedEdge}");
        }
    }

    [Fact]
    public void SelfLoop_IsAllowed()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node = graph.AddNode();

        // Act
        graph.AddEdge(node, node);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(node, node));
        Assert.Equal(1, graph.Degree(node)); // Self-loop counts as degree 1
        
        var neighbors = graph.Neighbors(node).ToList();
        Assert.Single(neighbors);
        Assert.Equal(node, neighbors[0]);
    }

    [Fact]
    public void Nodes_EnumeratesAllNodes()
    {
        // Arrange
        var graph = new UndirectedGraph();
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
    public void CompleteGraph_WorksCorrectly()
    {
        // Arrange - Create a complete graph with 4 nodes
        var graph = new UndirectedGraph();
        var nodes = new NodeId[4];
        for (int i = 0; i < 4; i++)
        {
            nodes[i] = graph.AddNode();
        }

        // Add all possible edges
        for (int i = 0; i < 4; i++)
        {
            for (int j = i + 1; j < 4; j++)
            {
                graph.AddEdge(nodes[i], nodes[j]);
            }
        }

        // Assert
        Assert.Equal(4, graph.NodeCount);
        Assert.Equal(6, graph.EdgeCount); // C(4,2) = 6 edges in complete graph
        
        // Check that every node has degree 3
        for (int i = 0; i < 4; i++)
        {
            Assert.Equal(3, graph.Degree(nodes[i]));
        }

        // Check that all pairs are connected
        for (int i = 0; i < 4; i++)
        {
            for (int j = i + 1; j < 4; j++)
            {
                Assert.True(graph.HasEdge(nodes[i], nodes[j]));
                Assert.True(graph.HasEdge(nodes[j], nodes[i]));
            }
        }
    }

    [Fact]
    public void PathGraph_WorksCorrectly()
    {
        // Arrange - Create a path graph: 0-1-2-3-4
        var graph = new UndirectedGraph();
        graph.WithEdge(0, 1)
             .WithEdge(1, 2)
             .WithEdge(2, 3)
             .WithEdge(3, 4);

        // Assert
        Assert.Equal(5, graph.NodeCount);
        Assert.Equal(4, graph.EdgeCount);
        
        // Check degrees: endpoints have degree 1, middle nodes have degree 2
        Assert.Equal(1, graph.Degree(0));
        Assert.Equal(2, graph.Degree(1));
        Assert.Equal(2, graph.Degree(2));
        Assert.Equal(2, graph.Degree(3));
        Assert.Equal(1, graph.Degree(4));

        // Check neighbors
        Assert.Single(graph.Neighbors(0));
        Assert.Contains(new NodeId(1), graph.Neighbors(0));
        
        Assert.Single(graph.Neighbors(4));
        Assert.Contains(new NodeId(3), graph.Neighbors(4));
        
        var middleNeighbors = graph.Neighbors(2).OrderBy(n => n.Value).ToList();
        Assert.Equal(2, middleNeighbors.Count);
        Assert.Equal(1, middleNeighbors[0].Value);
        Assert.Equal(3, middleNeighbors[1].Value);
    }
}