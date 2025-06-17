using GraphLib.Builders;
using GraphLib.Core;
using GraphLib.Implementations;
using GraphLib.Interfaces;
using Xunit;

namespace GraphLib.Tests.Builders;

public class GraphBuilderTests
{
    [Fact]
    public void Create_DirectedGraph_CreatesCorrectBuilder()
    {
        // Act
        var builder = GraphBuilder.Create<DirectedGraph>();

        // Assert
        Assert.NotNull(builder);
        Assert.Empty(builder.NodeMap);
    }

    [Fact]
    public void AddNode_IncreasesNodeCount()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();

        // Act
        var result = builder.AddNode();

        // Assert
        Assert.Same(builder, result); // Fluent interface
        Assert.Single(builder.NodeMap);
        Assert.Contains(0, builder.NodeMap.Keys);
    }

    [Fact]
    public void AddNodes_AddsMultipleNodes()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();

        // Act
        var result = builder.AddNodes(3);

        // Assert
        Assert.Same(builder, result);
        Assert.Equal(3, builder.NodeMap.Count);
        Assert.Contains(0, builder.NodeMap.Keys);
        Assert.Contains(1, builder.NodeMap.Keys);
        Assert.Contains(2, builder.NodeMap.Keys);
    }

    [Fact]
    public void AddEdge_ByIndex_AddsEdgeCorrectly()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(3);

        // Act
        var result = builder.AddEdge(0, 1);

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        var node0 = builder.GetNode(0);
        var node1 = builder.GetNode(1);
        Assert.True(graph.HasEdge(node0, node1));
    }

    [Fact]
    public void AddEdge_ByNodeId_AddsEdgeCorrectly()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(2);
        var node0 = builder.GetNode(0);
        var node1 = builder.GetNode(1);

        // Act
        var result = builder.AddEdge(node0, node1);

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.True(graph.HasEdge(node0, node1));
    }

    [Fact]
    public void AddEdge_InvalidSourceIndex_ThrowsArgumentException()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.AddEdge(5, 1));
    }

    [Fact]
    public void AddEdge_InvalidTargetIndex_ThrowsArgumentException()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.AddEdge(0, 5));
    }

    [Fact]
    public void AddEdges_AddsMultipleEdges()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(3);
        var node0 = builder.GetNode(0);
        var node1 = builder.GetNode(1);
        var node2 = builder.GetNode(2);

        var edges = new[]
        {
            new Edge(node0, node1),
            new Edge(node1, node2),
            new Edge(node2, node0)
        };

        // Act
        var result = builder.AddEdges(edges);

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(3, graph.EdgeCount);
        Assert.True(graph.HasEdge(node0, node1));
        Assert.True(graph.HasEdge(node1, node2));
        Assert.True(graph.HasEdge(node2, node0));
    }

    [Fact]
    public void AddPath_ByIndices_CreatesContinuousPath()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(4);

        // Act
        var result = builder.AddPath(0, 1, 2, 3);

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(3, graph.EdgeCount); // n-1 edges for n nodes
        Assert.True(graph.HasEdge(builder.GetNode(0), builder.GetNode(1)));
        Assert.True(graph.HasEdge(builder.GetNode(1), builder.GetNode(2)));
        Assert.True(graph.HasEdge(builder.GetNode(2), builder.GetNode(3)));
    }

    [Fact]
    public void AddPath_ByNodeIds_CreatesContinuousPath()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(3);
        var nodes = new[] { builder.GetNode(0), builder.GetNode(1), builder.GetNode(2) };

        // Act
        var result = builder.AddPath(nodes);

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(2, graph.EdgeCount);
        Assert.True(graph.HasEdge(nodes[0], nodes[1]));
        Assert.True(graph.HasEdge(nodes[1], nodes[2]));
    }

    [Fact]
    public void AddPath_SingleNode_DoesNotAddEdges()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(1);

        // Act
        var result = builder.AddPath(0);

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void AddCycle_ByIndices_CreatesClosedLoop()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(4);

        // Act
        var result = builder.AddCycle(0, 1, 2, 3);

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(4, graph.EdgeCount); // n edges for n nodes in cycle
        Assert.True(graph.HasEdge(builder.GetNode(0), builder.GetNode(1)));
        Assert.True(graph.HasEdge(builder.GetNode(1), builder.GetNode(2)));
        Assert.True(graph.HasEdge(builder.GetNode(2), builder.GetNode(3)));
        Assert.True(graph.HasEdge(builder.GetNode(3), builder.GetNode(0))); // Closes the cycle
    }

    [Fact]
    public void AddCycle_TooFewNodes_ThrowsArgumentException()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.AddCycle(0, 1));
    }

    [Fact]
    public void AddClique_DirectedGraph_AddsAllPossibleEdges()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(3);

        // Act
        var result = builder.AddClique(new[] { 0, 1, 2 });

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(6, graph.EdgeCount); // n*(n-1) edges for directed clique
        
        // Check all edges exist in both directions
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != j)
                {
                    Assert.True(graph.HasEdge(builder.GetNode(i), builder.GetNode(j)));
                }
            }
        }
    }

    [Fact]
    public void AddClique_UndirectedGraph_AddsSymmetricEdges()
    {
        // Arrange
        var builder = GraphBuilder.Create<UndirectedGraph>();
        builder.AddNodes(3);

        // Act
        var result = builder.AddClique(new[] { 0, 1, 2 });

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(3, graph.EdgeCount); // n*(n-1)/2 edges for undirected clique
        
        // Check all unique pairs are connected
        Assert.True(graph.HasEdge(builder.GetNode(0), builder.GetNode(1)));
        Assert.True(graph.HasEdge(builder.GetNode(1), builder.GetNode(2)));
        Assert.True(graph.HasEdge(builder.GetNode(0), builder.GetNode(2)));
    }

    [Fact]
    public void AddStar_ByIndices_ConnectsCenterToAllLeaves()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(4);

        // Act
        var result = builder.AddStar(0, new[] { 1, 2, 3 });

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(3, graph.EdgeCount);
        Assert.True(graph.HasEdge(builder.GetNode(0), builder.GetNode(1)));
        Assert.True(graph.HasEdge(builder.GetNode(0), builder.GetNode(2)));
        Assert.True(graph.HasEdge(builder.GetNode(0), builder.GetNode(3)));
    }

    [Fact]
    public void AddStar_ByNodeIds_ConnectsCenterToAllLeaves()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(4);
        var center = builder.GetNode(0);
        var leaves = new[] { builder.GetNode(1), builder.GetNode(2), builder.GetNode(3) };

        // Act
        var result = builder.AddStar(center, leaves);

        // Assert
        Assert.Same(builder, result);
        
        var graph = builder.Build();
        Assert.Equal(3, graph.EdgeCount);
        foreach (var leaf in leaves)
        {
            Assert.True(graph.HasEdge(center, leaf));
        }
    }

    [Fact]
    public void GetNode_ValidIndex_ReturnsCorrectNode()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(3);

        // Act
        var node = builder.GetNode(1);

        // Assert
        Assert.Equal(builder.NodeMap[1], node);
    }

    [Fact]
    public void GetNode_InvalidIndex_ThrowsArgumentException()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(2);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => builder.GetNode(5));
    }

    [Fact]
    public void Build_ReturnsConfiguredGraph()
    {
        // Arrange
        var builder = GraphBuilder.Create<DirectedGraph>();
        builder.AddNodes(3);
        builder.AddEdge(0, 1);
        builder.AddEdge(1, 2);

        // Act
        var graph = builder.Build();

        // Assert
        Assert.IsType<DirectedGraph>(graph);
        Assert.Equal(3, graph.NodeCount);
        Assert.Equal(2, graph.EdgeCount);
    }

    [Fact]
    public void FluentInterface_ChainsCorrectly()
    {
        // Arrange & Act
        var graph = GraphBuilder.Create<DirectedGraph>()
            .AddNodes(4)
            .AddEdge(0, 1)
            .AddEdge(1, 2)
            .AddPath(2, 3)
            .Build();

        // Assert
        Assert.Equal(4, graph.NodeCount);
        Assert.Equal(3, graph.EdgeCount);
    }

    [Fact]
    public void WorksWithDifferentGraphTypes()
    {
        // Arrange & Act
        var directedGraph = GraphBuilder.Create<DirectedGraph>()
            .AddNodes(2)
            .AddEdge(0, 1)
            .Build();

        var undirectedGraph = GraphBuilder.Create<UndirectedGraph>()
            .AddNodes(2)
            .AddEdge(0, 1)
            .Build();

        // Assert
        Assert.IsType<DirectedGraph>(directedGraph);
        Assert.IsType<UndirectedGraph>(undirectedGraph);
        
        Assert.All(new IGraph[] { directedGraph, undirectedGraph }, 
                  g => Assert.Equal(2, g.NodeCount));
    }

    [Fact]
    public void ComplexGraph_BuildsCorrectly()
    {
        // Arrange & Act - Build a small network with mixed patterns
        var graph = GraphBuilder.Create<DirectedGraph>()
            .AddNodes(6)
            .AddPath(0, 1, 2) // Linear chain
            .AddCycle(3, 4, 5) // Triangle
            .AddEdge(2, 3) // Connect chain to triangle
            .AddStar(0, new[] { 4, 5 }) // Star from first node
            .Build();

        // Assert
        Assert.Equal(6, graph.NodeCount);
        Assert.Equal(8, graph.EdgeCount); // 2 (path) + 3 (cycle) + 1 (connector) + 2 (star) = 8
        
        // Verify specific connections
        var builder = GraphBuilder.Create<DirectedGraph>().AddNodes(6);
        Assert.True(graph.HasEdge(builder.GetNode(0), builder.GetNode(1))); // Path
        Assert.True(graph.HasEdge(builder.GetNode(1), builder.GetNode(2))); // Path
        Assert.True(graph.HasEdge(builder.GetNode(2), builder.GetNode(3))); // Connector
        Assert.True(graph.HasEdge(builder.GetNode(5), builder.GetNode(3))); // Cycle
    }
}