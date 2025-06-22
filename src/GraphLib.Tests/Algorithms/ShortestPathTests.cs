using GraphLib.Algorithms;
using GraphLib.Algorithms.Results;
using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Algorithms;

public class ShortestPathTests
{
    [Fact]
    public void Dijkstra_SimpleGraph_FindsShortestPath()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 2);
        graph.AddEdge(nodeA, nodeC, 4);
        graph.AddEdge(nodeC, nodeD, 1);

        // Act
        var result = ShortestPath.Dijkstra<WeightedDirectedGraph<int>, int>(graph, nodeA, nodeD);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(4, result.TotalWeight);
        Assert.Equal(new[] { nodeA, nodeB, nodeC, nodeD }, result.Path);
    }

    [Fact]
    public void Dijkstra_NoPath_ReturnsNoPath()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        // No path from A to C

        // Act
        var result = ShortestPath.Dijkstra<WeightedDirectedGraph<int>, int>(graph, nodeA, nodeC);

        // Assert
        Assert.False(result.PathExists);
    }

    [Fact]
    public void Dijkstra_SameSourceAndTarget_ReturnsZeroPath()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();

        // Act
        var result = ShortestPath.Dijkstra<WeightedDirectedGraph<int>, int>(graph, nodeA, nodeA);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(0, result.TotalWeight);
        Assert.Equal(new[] { nodeA }, result.Path);
    }

    [Fact]
    public void Dijkstra_DoubleWeights_WorksCorrectly()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1.5);
        graph.AddEdge(nodeB, nodeC, 2.3);
        graph.AddEdge(nodeA, nodeC, 4.0);

        // Act
        var result = ShortestPath.Dijkstra<WeightedDirectedGraph<double>, double>(graph, nodeA, nodeC);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(3.8, result.TotalWeight, 0.001);
        Assert.Equal(new[] { nodeA, nodeB, nodeC }, result.Path);
    }

    [Fact]
    public void AStar_SimpleGraph_FindsShortestPath()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 2);
        graph.AddEdge(nodeA, nodeC, 4);
        graph.AddEdge(nodeC, nodeD, 1);

        // Simple heuristic (always 0 for simplicity)
        int Heuristic(NodeId from, NodeId to) => 0;

        // Act
        var result = ShortestPath.AStar(graph, nodeA, nodeD, Heuristic);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(4, result.TotalWeight);
        Assert.Equal(new[] { nodeA, nodeB, nodeC, nodeD }, result.Path);
    }

    [Fact]
    public void AStar_WithHeuristic_UsesHeuristic()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 1);
        graph.AddEdge(nodeA, nodeC, 3);

        // Heuristic that prefers direct path
        int Heuristic(NodeId from, NodeId to)
        {
            if (from.Equals(nodeA) && to.Equals(nodeC)) return 1;
            if (from.Equals(nodeB) && to.Equals(nodeC)) return 1;
            return 0;
        }

        // Act
        var result = ShortestPath.AStar(graph, nodeA, nodeC, Heuristic);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(2, result.TotalWeight);
        Assert.Equal(new[] { nodeA, nodeB, nodeC }, result.Path);
    }

    [Fact]
    public void BellmanFord_SimpleGraph_FindsShortestPath()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 2);
        graph.AddEdge(nodeA, nodeC, 4);
        graph.AddEdge(nodeC, nodeD, 1);

        // Act
        var result = ShortestPath.BellmanFord(graph, nodeA, nodeD, 
            comparer: null, 
            add: (a, b) => a + b, 
            zero: 0, 
            infinity: int.MaxValue);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(4, result.TotalWeight);
        Assert.Equal(new[] { nodeA, nodeB, nodeC, nodeD }, result.Path);
    }

    [Fact]
    public void BellmanFord_NegativeWeights_FindsShortestPath()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 5);
        graph.AddEdge(nodeB, nodeC, -2);
        graph.AddEdge(nodeA, nodeC, 2);

        // Act
        var result = ShortestPath.BellmanFord(graph, nodeA, nodeC,
            comparer: null,
            add: (a, b) => a + b,
            zero: 0,
            infinity: int.MaxValue);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(2, result.TotalWeight);
        Assert.Equal(new[] { nodeA, nodeC }, result.Path);
    }

    [Fact]
    public void BellmanFord_NegativeCycle_ThrowsException()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, -2);
        graph.AddEdge(nodeC, nodeA, -1); // Creates negative cycle

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            ShortestPath.BellmanFord(graph, nodeA, nodeC,
                comparer: null,
                add: (a, b) => a + b,
                zero: 0,
                infinity: int.MaxValue));
    }

    [Fact]
    public void BreadthFirstSearch_UnweightedGraph_FindsShortestPath()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeC, nodeD);

        // Act
        var result = ShortestPath.BreadthFirstSearch(graph, nodeA, nodeD);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(2, result.TotalWeight);
        Assert.Equal(new[] { nodeA, nodeC, nodeD }, result.Path);
    }

    [Fact]
    public void BreadthFirstSearch_UndirectedGraph_FindsShortestPath()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeA, nodeD);
        graph.AddEdge(nodeD, nodeC);

        // Act
        var result = ShortestPath.BreadthFirstSearch(graph, nodeA, nodeC);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(2, result.TotalWeight);
        // Should find one of the two shortest paths
        Assert.True(
            result.Path.SequenceEqual(new[] { nodeA, nodeB, nodeC }) ||
            result.Path.SequenceEqual(new[] { nodeA, nodeD, nodeC }));
    }

    [Fact]
    public void FloydWarshall_AllPairsShortestPaths_ComputesCorrectly()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeC, 2);
        graph.AddEdge(nodeA, nodeC, 4);

        // Act
        var distances = ShortestPath.FloydWarshall<WeightedDirectedGraph<int>, int>(graph);

        // Assert
        Assert.Equal(0, distances[(nodeA, nodeA)]);
        Assert.Equal(1, distances[(nodeA, nodeB)]);
        Assert.Equal(3, distances[(nodeA, nodeC)]);
        Assert.Equal(2, distances[(nodeB, nodeC)]);
    }

    [Fact]
    public void FloydWarshall_NegativeCycle_ThrowsException()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1);
        graph.AddEdge(nodeB, nodeA, -2); // Creates negative cycle

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            ShortestPath.FloydWarshall<WeightedDirectedGraph<int>, int>(graph));
    }

    [Fact]
    public void Dijkstra_InvalidSource_ThrowsException()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var invalidNode = new NodeId(999);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            ShortestPath.Dijkstra<WeightedDirectedGraph<int>, int>(graph, invalidNode, nodeA));
    }

    [Fact]
    public void Dijkstra_InvalidTarget_ThrowsException()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<int>();
        var nodeA = graph.AddNode();
        var invalidNode = new NodeId(999);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            ShortestPath.Dijkstra<WeightedDirectedGraph<int>, int>(graph, nodeA, invalidNode));
    }

    [Fact]
    public void Dijkstra_NullGraph_ThrowsException()
    {
        // Arrange
        WeightedDirectedGraph<int> graph = null!;
        var nodeA = new NodeId(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            ShortestPath.Dijkstra<WeightedDirectedGraph<int>, int>(graph, nodeA, nodeA));
    }

    [Fact]
    public void Dijkstra_CustomWeightType_WorksCorrectly()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<decimal>();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB, 1.5m);
        graph.AddEdge(nodeB, nodeC, 2.3m);
        graph.AddEdge(nodeA, nodeC, 4.0m);

        // Act
        var result = ShortestPath.Dijkstra<WeightedDirectedGraph<decimal>, decimal>(graph, nodeA, nodeC);

        // Assert
        Assert.True(result.PathExists);
        Assert.Equal(3.8m, result.TotalWeight);
        Assert.Equal(new[] { nodeA, nodeB, nodeC }, result.Path);
    }
}