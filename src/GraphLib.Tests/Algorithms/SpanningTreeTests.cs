using GraphLib.Algorithms;
using GraphLib.Algorithms.Results;
using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Algorithms;

public class SpanningTreeTests
{
    [Fact]
    public void Kruskal_SimpleGraph_FindsMinimumSpanningTree()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 3);
        graph.AddEdge(nodeC, nodeD, 2);
        graph.AddEdge(nodeA, nodeD, 4);
        graph.AddEdge(nodeA, nodeC, 5);

        // Act
        var result = SpanningTree.Kruskal<WeightedUndirectedGraph<int>, int>(graph);

        // Assert
        Assert.Equal(6, result.TotalWeight); // 1 + 3 + 2
        Assert.Equal(3, result.Edges.Count); // n-1 edges for n nodes
    }

    [Fact]
    public void Kruskal_SingleNode_ReturnsEmptyTree()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodeA = graph.AddNode();

        // Act
        var result = SpanningTree.Kruskal<WeightedUndirectedGraph<int>, int>(graph);

        // Assert
        Assert.Equal(0, result.TotalWeight);
        Assert.Empty(result.Edges);
    }

    [Fact]
    public void Kruskal_DisconnectedGraph_ReturnsForest()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        // Two disconnected components
        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeC, nodeD, 2);

        // Act
        var result = SpanningTree.Kruskal<WeightedUndirectedGraph<int>, int>(graph);

        // Assert
        Assert.Equal(3, result.TotalWeight); // 1 + 2
        Assert.Equal(2, result.Edges.Count);
    }

    [Fact]
    public void Kruskal_DoubleWeights_WorksCorrectly()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<double>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1.5);
        graph.AddEdge(nodeB, nodeC, 2.3);
        graph.AddEdge(nodeA, nodeC, 4.0);

        // Act
        var result = SpanningTree.Kruskal<WeightedUndirectedGraph<double>, double>(graph);

        // Assert
        Assert.Equal(3.8, result.TotalWeight, 0.001); // 1.5 + 2.3
        Assert.Equal(2, result.Edges.Count);
    }

    [Fact]
    public void Prim_SimpleGraph_FindsMinimumSpanningTree()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 3);
        graph.AddEdge(nodeC, nodeD, 2);
        graph.AddEdge(nodeA, nodeD, 4);
        graph.AddEdge(nodeA, nodeC, 5);

        // Act
        var result = SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, nodeA);

        // Assert
        Assert.Equal(6, result.TotalWeight); // 1 + 3 + 2
        Assert.Equal(3, result.Edges.Count); // n-1 edges for n nodes
    }

    [Fact]
    public void Prim_SingleNode_ReturnsEmptyTree()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodeA = graph.AddNode();

        // Act
        var result = SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, nodeA);

        // Assert
        Assert.Equal(0, result.TotalWeight);
        Assert.Empty(result.Edges);
    }

    [Fact]
    public void Prim_DifferentStartNodes_ProduceSameWeight()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 3);
        graph.AddEdge(nodeC, nodeD, 2);
        graph.AddEdge(nodeA, nodeD, 4);

        // Act
        var resultA = SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, nodeA);
        var resultB = SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, nodeB);
        var resultC = SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, nodeC);

        // Assert
        Assert.Equal(resultA.TotalWeight, resultB.TotalWeight);
        Assert.Equal(resultB.TotalWeight, resultC.TotalWeight);
        Assert.Equal(6, resultA.TotalWeight);
    }

    [Fact]
    public void Prim_ComplexGraph_FindsOptimalMST()
    {
        // Arrange - Create a more complex graph
        var graph = new WeightedUndirectedGraph<int>();
        var nodes = new NodeId[6];
        for (int i = 0; i < 6; i++)
        {
            nodes[i] = graph.AddNode();
        }

        // Add edges with various weights
        graph.AddEdge(nodes[0], nodes[1], 4);
        graph.AddEdge(nodes[0], nodes[2], 2);
        graph.AddEdge(nodes[1], nodes[2], 1);
        graph.AddEdge(nodes[1], nodes[3], 5);
        graph.AddEdge(nodes[2], nodes[3], 8);
        graph.AddEdge(nodes[2], nodes[4], 10);
        graph.AddEdge(nodes[3], nodes[4], 2);
        graph.AddEdge(nodes[3], nodes[5], 6);
        graph.AddEdge(nodes[4], nodes[5], 3);

        // Act
        var result = SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, nodes[0]);

        // Assert
        Assert.Equal(5, result.Edges.Count); // n-1 edges for n nodes
        Assert.Equal(13, result.TotalWeight); // Optimal MST weight: 2+1+5+2+3=13
    }

    [Fact]
    public void MaximumSpanningTree_SimpleGraph_FindsMaximumSpanningTree()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 3);
        graph.AddEdge(nodeA, nodeC, 2);

        // Act
        var result = SpanningTree.MaximumSpanningTree<WeightedUndirectedGraph<int>, int>(graph);

        // Assert
        Assert.Equal(5, result.TotalWeight); // 3 + 2 (maximum weights)
        Assert.Equal(2, result.Edges.Count);
        
        // Check that edges have maximum weights
        // Total weight should be 5 (3 + 2)
    }

    [Fact]
    public void Kruskal_InvalidGraph_ThrowsException()
    {
        // Arrange
        WeightedUndirectedGraph<int> graph = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SpanningTree.Kruskal<WeightedUndirectedGraph<int>, int>(graph));
    }

    [Fact]
    public void Prim_InvalidStartNode_ThrowsException()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodeA = graph.AddNode();
        var invalidNode = new NodeId(999);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, invalidNode));
    }

    [Fact]
    public void Prim_NullGraph_ThrowsException()
    {
        // Arrange
        WeightedUndirectedGraph<int> graph = null!;
        var nodeA = new NodeId(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, nodeA));
    }

    [Fact]
    public void Kruskal_Prim_ProduceSameResult()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<int>();
        var nodes = new NodeId[4];
        for (int i = 0; i < 4; i++)
        {
            nodes[i] = graph.AddNode();
        }

        graph.AddEdge(nodes[0], nodes[1], 1);
        graph.AddEdge(nodes[1], nodes[2], 3);
        graph.AddEdge(nodes[2], nodes[3], 2);
        graph.AddEdge(nodes[0], nodes[3], 4);
        graph.AddEdge(nodes[0], nodes[2], 5);

        // Act
        var kruskalResult = SpanningTree.Kruskal<WeightedUndirectedGraph<int>, int>(graph);
        var primResult = SpanningTree.Prim<WeightedUndirectedGraph<int>, int>(graph, nodes[0]);

        // Assert
        Assert.Equal(kruskalResult.TotalWeight, primResult.TotalWeight);
        Assert.Equal(kruskalResult.Edges.Count, primResult.Edges.Count);
    }

    [Fact]
    public void Kruskal_CustomWeightType_WorksCorrectly()
    {
        // Arrange
        var graph = new WeightedUndirectedGraph<decimal>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1.1m);
        graph.AddEdge(nodeB, nodeC, 2.2m);
        graph.AddEdge(nodeA, nodeC, 3.3m);

        // Act
        var result = SpanningTree.Kruskal<WeightedUndirectedGraph<decimal>, decimal>(graph);

        // Assert
        Assert.Equal(3.3m, result.TotalWeight); // 1.1 + 2.2
        Assert.Equal(2, result.Edges.Count);
    }
}