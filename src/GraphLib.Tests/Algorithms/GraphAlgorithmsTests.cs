using GraphLib.Algorithms;
using GraphLib.Algorithms.Results;
using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Algorithms;

public class GraphAlgorithmsTests
{
    [Fact]
    public void TopologicalSort_DAG_ReturnsValidOrdering()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        // Create a DAG: A -> B -> D, A -> C -> D
        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeB, nodeD);
        graph.AddEdge(nodeC, nodeD);

        // Act
        var result = GraphAlgorithms.TopologicalSort(graph);

        // Assert
        Assert.True(result.IsAcyclic);
        Assert.Equal(4, result.Order.Count);
        
        // Verify topological property: for every edge (u,v), u comes before v
        var order = result.Order.ToList();
        Assert.True(order.IndexOf(nodeA) < order.IndexOf(nodeB));
        Assert.True(order.IndexOf(nodeA) < order.IndexOf(nodeC));
        Assert.True(order.IndexOf(nodeB) < order.IndexOf(nodeD));
        Assert.True(order.IndexOf(nodeC) < order.IndexOf(nodeD));
    }

    [Fact]
    public void TopologicalSort_CyclicGraph_ReturnsInvalid()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        // Create a cycle: A -> B -> C -> A
        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);

        // Act
        var result = GraphAlgorithms.TopologicalSort(graph);

        // Assert
        Assert.False(result.IsAcyclic);
        Assert.Empty(result.Order);
    }

    [Fact]
    public void StronglyConnectedComponents_SimpleGraph_FindsCorrectComponents()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        // Create two SCCs: {A, B} and {C}, and isolated node D
        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeA);
        graph.AddEdge(nodeA, nodeC);

        // Act
        var components = GraphAlgorithms.StronglyConnectedComponents(graph);

        // Assert
        Assert.Equal(3, components.Count);
        
        // Find the component containing A and B
        var abComponent = components.FirstOrDefault(c => c.Contains(nodeA));
        Assert.NotNull(abComponent);
        Assert.Contains(nodeB, abComponent);
        Assert.Equal(2, abComponent.Count);

        // C should be in its own component
        var cComponent = components.FirstOrDefault(c => c.Contains(nodeC));
        Assert.NotNull(cComponent);
        Assert.Single(cComponent);

        // D should be in its own component
        var dComponent = components.FirstOrDefault(c => c.Contains(nodeD));
        Assert.NotNull(dComponent);
        Assert.Single(dComponent);
    }

    [Fact]
    public void HasCycle_CyclicDirectedGraph_ReturnsTrue()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA); // Creates cycle

        // Act
        var hasCycle = GraphAlgorithms.HasCycle(graph);

        // Assert
        Assert.True(hasCycle);
    }

    [Fact]
    public void HasCycle_AcyclicDirectedGraph_ReturnsFalse()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var hasCycle = GraphAlgorithms.HasCycle(graph);

        // Assert
        Assert.False(hasCycle);
    }

    [Fact]
    public void HasCycle_UndirectedGraphWithCycle_ReturnsTrue()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA); // Creates cycle

        // Act
        var hasCycle = GraphAlgorithms.HasCycle(graph);

        // Assert
        Assert.True(hasCycle);
    }

    [Fact]
    public void HasCycle_UndirectedTree_ReturnsFalse()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeA, nodeD);

        // Act
        var hasCycle = GraphAlgorithms.HasCycle(graph);

        // Assert
        Assert.False(hasCycle);
    }

    [Fact]
    public void ConnectedComponents_UndirectedGraph_FindsCorrectComponents()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();
        var nodeE = graph.AddNode();

        // Create two components: {A, B, C} and {D, E}
        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeD, nodeE);

        // Act
        var components = GraphAlgorithms.ConnectedComponents(graph);

        // Assert
        Assert.Equal(2, components.Count);
        
        var component1 = components.First();
        var component2 = components.Last();
        
        // One component should have 3 nodes, the other 2
        Assert.True((component1.Count == 3 && component2.Count == 2) ||
                   (component1.Count == 2 && component2.Count == 3));
    }

    [Fact]
    public void IsConnected_ConnectedGraph_ReturnsTrue()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var isConnected = GraphAlgorithms.IsConnected(graph);

        // Assert
        Assert.True(isConnected);
    }

    [Fact]
    public void IsConnected_DisconnectedGraph_ReturnsFalse()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeC, nodeD); // Separate component

        // Act
        var isConnected = GraphAlgorithms.IsConnected(graph);

        // Assert
        Assert.False(isConnected);
    }

    [Fact]
    public void GreedyColoring_SimpleGraph_FindsValidColoring()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        // Create a triangle (A-B-C-A) with an additional node D connected to A
        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);
        graph.AddEdge(nodeA, nodeD);

        // Act
        var result = GraphAlgorithms.GreedyColoring(graph);

        // Assert
        Assert.True(result.ChromaticNumber > 0);
        Assert.True(result.NodeColors.Count > 0);
        
        // Verify that adjacent nodes have different colors
        Assert.NotEqual(result.NodeColors[nodeA], result.NodeColors[nodeB]);
        Assert.NotEqual(result.NodeColors[nodeB], result.NodeColors[nodeC]);
        Assert.NotEqual(result.NodeColors[nodeC], result.NodeColors[nodeA]);
        Assert.NotEqual(result.NodeColors[nodeA], result.NodeColors[nodeD]);
    }

    [Fact]
    public void GreedyColoring_CompleteGraph_UsesAllColors()
    {
        // Arrange - Create K4 (complete graph with 4 nodes)
        var graph = new UndirectedGraph();
        var nodes = new NodeId[4];
        for (int i = 0; i < 4; i++)
        {
            nodes[i] = graph.AddNode();
        }

        // Connect every pair of nodes
        for (int i = 0; i < 4; i++)
        {
            for (int j = i + 1; j < 4; j++)
            {
                graph.AddEdge(nodes[i], nodes[j]);
            }
        }

        // Act
        var result = GraphAlgorithms.GreedyColoring(graph);

        // Assert
        // Property doesn't exist, commenting out
        // Assert.True(result.IsValidColoring);
        Assert.Equal(4, result.ChromaticNumber); // K4 requires 4 colors
        
        // All nodes should have different colors
        var colors = result.NodeColors.Values.ToHashSet();
        Assert.Equal(4, colors.Count);
    }

    [Fact]
    public void IsBipartite_BipartiteGraph_ReturnsTrue()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        // Create bipartite graph: {A, C} and {B, D}
        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeA, nodeD);
        graph.AddEdge(nodeC, nodeB);
        graph.AddEdge(nodeC, nodeD);

        // Act
        var result = GraphAlgorithms.IsBipartite(graph);

        // Assert
        Assert.True(result.IsBipartite);
        Assert.NotNull(result.LeftSet);
        Assert.NotNull(result.RightSet);
        
        // Verify partitions
        var partition1 = result.LeftSet;
        var partition2 = result.RightSet;
        
        Assert.True((partition1.Contains(nodeA) && partition1.Contains(nodeC)) ||
                   (partition2.Contains(nodeA) && partition2.Contains(nodeC)));
    }

    [Fact]
    public void IsBipartite_NonBipartiteGraph_ReturnsFalse()
    {
        // Arrange - Create triangle (odd cycle)
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);

        // Act
        var result = GraphAlgorithms.IsBipartite(graph);

        // Assert
        Assert.False(result.IsBipartite);
        Assert.Empty(result.LeftSet);
        Assert.Empty(result.RightSet);
    }

    [Fact]
    public void TopologicalSort_EmptyGraph_ReturnsEmpty()
    {
        // Arrange
        var graph = new DirectedGraph();

        // Act
        var result = GraphAlgorithms.TopologicalSort(graph);

        // Assert
        Assert.True(result.IsAcyclic);
        Assert.Empty(result.Order);
    }

    [Fact]
    public void TopologicalSort_SingleNode_ReturnsSingleNode()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();

        // Act
        var result = GraphAlgorithms.TopologicalSort(graph);

        // Assert
        Assert.True(result.IsAcyclic);
        Assert.Single(result.Order);
        Assert.Equal(nodeA, result.Order.First());
    }

    [Fact]
    public void StronglyConnectedComponents_SingleNode_ReturnsSingleComponent()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();

        // Act
        var components = GraphAlgorithms.StronglyConnectedComponents(graph);

        // Assert
        Assert.Single(components);
        Assert.Single(components[0]);
        Assert.Equal(nodeA, components[0].First());
    }

    [Fact]
    public void ConnectedComponents_SingleNode_ReturnsSingleComponent()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();

        // Act
        var components = GraphAlgorithms.ConnectedComponents(graph);

        // Assert
        Assert.Single(components);
        Assert.Single(components[0]);
        Assert.Equal(nodeA, components[0].First());
    }

    [Fact]
    public void GreedyColoring_EmptyGraph_ReturnsEmptyColoring()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act
        var result = GraphAlgorithms.GreedyColoring(graph);

        // Assert
        // Property doesn't exist, commenting out
        // Assert.True(result.IsValidColoring);
        Assert.Empty(result.NodeColors);
        Assert.Equal(0, result.ChromaticNumber);
    }

    [Fact]
    public void IsBipartite_EmptyGraph_ReturnsTrue()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act
        var result = GraphAlgorithms.IsBipartite(graph);

        // Assert
        Assert.True(result.IsBipartite);
        Assert.Empty(result.LeftSet);
        Assert.Empty(result.RightSet);
    }

    [Fact]
    public void TopologicalSort_NullGraph_ThrowsException()
    {
        // Arrange
        DirectedGraph graph = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => GraphAlgorithms.TopologicalSort(graph));
    }

    [Fact]
    public void StronglyConnectedComponents_NullGraph_ThrowsException()
    {
        // Arrange
        DirectedGraph graph = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => GraphAlgorithms.StronglyConnectedComponents(graph));
    }
}