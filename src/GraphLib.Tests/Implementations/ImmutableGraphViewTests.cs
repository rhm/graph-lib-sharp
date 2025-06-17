using GraphLib.Core;
using GraphLib.Implementations;
using GraphLib.Interfaces;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class ImmutableGraphViewTests
{
    [Fact]
    public void Constructor_WithValidGraph_CreatesView()
    {
        // Arrange
        var graph = new DirectedGraph();
        graph.AddNode();
        graph.AddNode();

        // Act
        var view = new ImmutableGraphView<DirectedGraph>(graph);

        // Assert
        Assert.NotNull(view);
        Assert.Same(graph, view.UnderlyingGraph);
        Assert.Equal(graph.NodeCount, view.NodeCount);
        Assert.Equal(graph.EdgeCount, view.EdgeCount);
    }

    [Fact]
    public void Constructor_WithNullGraph_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ImmutableGraphView<DirectedGraph>(null!));
    }

    [Fact]
    public void Properties_ReflectUnderlyingGraph()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);

        // Act
        var view = new ImmutableGraphView<DirectedGraph>(graph);

        // Assert
        Assert.Equal(3, view.NodeCount);
        Assert.Equal(2, view.EdgeCount);
        Assert.Equal(graph.Nodes.Count(), view.Nodes.Count());
        
        foreach (var node in graph.Nodes)
        {
            Assert.Contains(node, view.Nodes);
            Assert.True(view.ContainsNode(node));
            Assert.Equal(graph.Degree(node), view.Degree(node));
        }
    }

    [Fact]
    public void View_ReflectsChangesToUnderlyingGraph()
    {
        // Arrange
        var graph = new DirectedGraph();
        var view = new ImmutableGraphView<DirectedGraph>(graph);
        
        Assert.Equal(0, view.NodeCount);
        Assert.Equal(0, view.EdgeCount);

        // Act - Modify underlying graph
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Assert - View reflects changes
        Assert.Equal(2, view.NodeCount);
        Assert.Equal(1, view.EdgeCount);
        Assert.True(view.ContainsNode(node1));
        Assert.True(view.ContainsNode(node2));
    }

    [Fact]
    public void ImmutableDirectedGraphView_PreservesDirectedBehavior()
    {
        // Arrange
        var graph = new DirectedGraph();
        var source = graph.AddNode();
        var target = graph.AddNode();
        graph.AddEdge(source, target);

        // Act
        var view = new ImmutableDirectedGraphView<DirectedGraph>(graph);

        // Assert
        Assert.True(view.HasEdge(source, target));
        Assert.False(view.HasEdge(target, source)); // Directed graph behavior
        
        Assert.Equal(1, view.OutDegree(source));
        Assert.Equal(0, view.InDegree(source));
        Assert.Equal(0, view.OutDegree(target));
        Assert.Equal(1, view.InDegree(target));
        
        Assert.Single(view.OutNeighbors(source));
        Assert.Contains(target, view.OutNeighbors(source));
        Assert.Single(view.InNeighbors(target));
        Assert.Contains(source, view.InNeighbors(target));
    }

    [Fact]
    public void ImmutableUndirectedGraphView_PreservesUndirectedBehavior()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Act
        var view = new ImmutableUndirectedGraphView<UndirectedGraph>(graph);

        // Assert
        Assert.True(view.HasEdge(node1, node2));
        Assert.True(view.HasEdge(node2, node1)); // Undirected graph behavior
        
        Assert.Single(view.Neighbors(node1));
        Assert.Contains(node2, view.Neighbors(node1));
        Assert.Single(view.Neighbors(node2));
        Assert.Contains(node1, view.Neighbors(node2));
    }

    [Fact]
    public void ImmutableWeightedGraphView_PreservesWeightedBehavior()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var source = graph.AddNode();
        var target = graph.AddNode();
        var weight = 3.14;
        graph.AddEdge(source, target, weight);

        // Act
        var view = new ImmutableWeightedGraphView<WeightedDirectedGraph<double>, double>(graph);

        // Assert
        Assert.Equal(weight, view.GetEdgeWeight(source, target));
        Assert.True(view.TryGetEdgeWeight(source, target, out var retrievedWeight));
        Assert.Equal(weight, retrievedWeight);
    }

    [Fact]
    public void ImmutableNodeDataGraphView_PreservesNodeData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var node1 = graph.AddNode("data1");
        var node2 = graph.AddNode("data2");

        // Act
        var view = new ImmutableNodeDataGraphView<DirectedGraphWithNodeData<string>, string>(graph);

        // Assert
        Assert.Equal("data1", view.GetNodeData(node1));
        Assert.Equal("data2", view.GetNodeData(node2));
        Assert.True(view.TryGetNodeData(node1, out var data1));
        Assert.Equal("data1", data1);
    }

    [Fact]
    public void ImmutableEdgeDataGraphView_PreservesEdgeData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");
        var edgeData = 42;
        graph.AddEdge(source, target, edgeData);

        // Act
        var view = new ImmutableEdgeDataGraphView<DirectedGraphWithData<string, int>, int>(graph);

        // Assert
        Assert.Equal(edgeData, view.GetEdgeData(source, target));
        Assert.True(view.TryGetEdgeData(source, target, out var retrievedData));
        Assert.Equal(edgeData, retrievedData);
    }

    [Fact]
    public void MultipleViews_ShareSameUnderlyingGraph()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node = graph.AddNode();

        // Act
        var view1 = new ImmutableGraphView<DirectedGraph>(graph);
        var view2 = new ImmutableGraphView<DirectedGraph>(graph);

        // Assert
        Assert.Same(graph, view1.UnderlyingGraph);
        Assert.Same(graph, view2.UnderlyingGraph);
        Assert.Equal(view1.NodeCount, view2.NodeCount);
        
        // Both views should reflect the same changes
        var newNode = graph.AddNode();
        Assert.Equal(2, view1.NodeCount);
        Assert.Equal(2, view2.NodeCount);
    }

    [Fact]
    public void ComplexGraph_ViewReflectsAllProperties()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodes = new List<NodeId>();
        for (int i = 0; i < 5; i++)
        {
            nodes.Add(graph.AddNode());
        }

        // Create a more complex structure
        graph.AddEdge(nodes[0], nodes[1]);
        graph.AddEdge(nodes[0], nodes[2]);
        graph.AddEdge(nodes[1], nodes[3]);
        graph.AddEdge(nodes[2], nodes[3]);
        graph.AddEdge(nodes[3], nodes[4]);

        // Act
        var view = new ImmutableDirectedGraphView<DirectedGraph>(graph);

        // Assert
        Assert.Equal(5, view.NodeCount);
        Assert.Equal(5, view.EdgeCount);
        
        // Verify specific relationships
        Assert.Equal(2, view.OutDegree(nodes[0])); // 0 -> 1, 2
        Assert.Equal(0, view.InDegree(nodes[0]));
        Assert.Equal(1, view.OutDegree(nodes[3])); // 3 -> 4
        Assert.Equal(2, view.InDegree(nodes[3])); // 1 -> 3, 2 -> 3
        Assert.Equal(0, view.OutDegree(nodes[4])); // Terminal node
        Assert.Equal(1, view.InDegree(nodes[4])); // 3 -> 4
    }

    [Fact]
    public void EmptyGraph_ViewHandlesCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();

        // Act
        var view = new ImmutableGraphView<DirectedGraph>(graph);

        // Assert
        Assert.Equal(0, view.NodeCount);
        Assert.Equal(0, view.EdgeCount);
        Assert.Empty(view.Nodes);
    }

    [Fact]
    public void ViewInterfaces_ImplementedCorrectly()
    {
        // Arrange
        var directedGraph = new DirectedGraph();
        var undirectedGraph = new UndirectedGraph();
        var weightedGraph = new WeightedDirectedGraph<double>();

        // Act
        var directedView = new ImmutableDirectedGraphView<DirectedGraph>(directedGraph);
        var undirectedView = new ImmutableUndirectedGraphView<UndirectedGraph>(undirectedGraph);
        var weightedView = new ImmutableWeightedGraphView<WeightedDirectedGraph<double>, double>(weightedGraph);

        // Assert
        Assert.IsAssignableFrom<IGraph>(directedView);
        Assert.IsAssignableFrom<IDirectedGraph>(directedView);
        Assert.IsAssignableFrom<IUndirectedGraph>(undirectedView);
        Assert.IsAssignableFrom<IWeightedGraph<double>>(weightedView);
    }

    [Fact]
    public void CombinedInterfaces_WorkTogether()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<string>();
        var source = graph.AddNode();
        var target = graph.AddNode();
        graph.AddEdge(source, target, "test-weight");

        // Act - Create view that implements both IGraph and IWeightedGraph
        var view = new ImmutableWeightedGraphView<WeightedDirectedGraph<string>, string>(graph);

        // Assert - Can use both interfaces
        Assert.Equal(2, view.NodeCount); // IGraph
        Assert.Equal("test-weight", view.GetEdgeWeight(source, target)); // IWeightedGraph
    }
}