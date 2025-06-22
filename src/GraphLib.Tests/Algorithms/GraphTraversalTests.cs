using GraphLib.Algorithms;
using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Algorithms;

public class GraphTraversalTests
{
    private class TestVisitor : IGraphVisitor
    {
        public List<NodeId> DiscoveredNodes { get; } = new();
        public List<NodeId> FinishedNodes { get; } = new();
        public List<(NodeId, NodeId)> TreeEdges { get; } = new();
        public List<(NodeId, NodeId)> BackEdges { get; } = new();
        public List<(NodeId, NodeId)> ForwardEdges { get; } = new();
        public List<(NodeId, NodeId)> CrossEdges { get; } = new();
        public List<(NodeId, NodeId)> ExaminedEdges { get; } = new();

        public void DiscoverNode(NodeId node) => DiscoveredNodes.Add(node);
        public void FinishNode(NodeId node) => FinishedNodes.Add(node);
        public void TreeEdge(NodeId source, NodeId target) => TreeEdges.Add((source, target));
        public void BackEdge(NodeId source, NodeId target) => BackEdges.Add((source, target));
        public void ForwardEdge(NodeId source, NodeId target) => ForwardEdges.Add((source, target));
        public void CrossEdge(NodeId source, NodeId target) => CrossEdges.Add((source, target));
        public void ExamineEdge(NodeId source, NodeId target) => ExaminedEdges.Add((source, target));
    }

    [Fact]
    public void DepthFirstSearch_SimpleDirectedGraph_VisitsAllReachableNodes()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        var visitor = new TestVisitor();

        // Act
        GraphTraversal.DepthFirstSearch(graph, nodeA, visitor);

        // Assert
        Assert.Equal(3, visitor.DiscoveredNodes.Count);
        Assert.Equal(3, visitor.FinishedNodes.Count);
        Assert.Contains(nodeA, visitor.DiscoveredNodes);
        Assert.Contains(nodeB, visitor.DiscoveredNodes);
        Assert.Contains(nodeC, visitor.DiscoveredNodes);
    }

    [Fact]
    public void DepthFirstSearch_DetectsTreeEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        var visitor = new TestVisitor();

        // Act
        GraphTraversal.DepthFirstSearch(graph, nodeA, visitor);

        // Assert
        Assert.Equal(2, visitor.TreeEdges.Count);
        Assert.Contains((nodeA, nodeB), visitor.TreeEdges);
        Assert.Contains((nodeB, nodeC), visitor.TreeEdges);
    }

    [Fact]
    public void DepthFirstSearch_DetectsBackEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA); // Back edge creating a cycle

        var visitor = new TestVisitor();

        // Act
        GraphTraversal.DepthFirstSearch(graph, nodeA, visitor);

        // Assert
        Assert.Contains((nodeC, nodeA), visitor.BackEdges);
    }

    [Fact]
    public void BreadthFirstSearch_SimpleDirectedGraph_VisitsAllReachableNodes()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeB, nodeD);

        var visitor = new TestVisitor();

        // Act
        GraphTraversal.BreadthFirstSearch(graph, nodeA, visitor);

        // Assert
        Assert.Equal(4, visitor.DiscoveredNodes.Count);
        Assert.Equal(4, visitor.FinishedNodes.Count);
        Assert.Contains(nodeA, visitor.DiscoveredNodes);
        Assert.Contains(nodeB, visitor.DiscoveredNodes);
        Assert.Contains(nodeC, visitor.DiscoveredNodes);
        Assert.Contains(nodeD, visitor.DiscoveredNodes);
    }

    [Fact]
    public void BreadthFirstSearch_VisitsInBreadthFirstOrder()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeB, nodeD);

        var visitor = new TestVisitor();

        // Act
        GraphTraversal.BreadthFirstSearch(graph, nodeA, visitor);

        // Assert
        // Should discover A first, then B and C (level 1), then D (level 2)
        Assert.Equal(nodeA, visitor.DiscoveredNodes[0]);
        Assert.True(visitor.DiscoveredNodes.IndexOf(nodeB) < visitor.DiscoveredNodes.IndexOf(nodeD));
        Assert.True(visitor.DiscoveredNodes.IndexOf(nodeC) < visitor.DiscoveredNodes.IndexOf(nodeD));
    }

    [Fact]
    public void DepthFirstNodes_ReturnsNodesInDFSOrder()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeB, nodeD);

        // Act
        var nodes = GraphTraversal.DepthFirstNodes(graph, nodeA).ToList();

        // Assert
        Assert.Equal(4, nodes.Count);
        Assert.Equal(nodeA, nodes[0]); // Should start with the root
        Assert.Contains(nodeB, nodes);
        Assert.Contains(nodeC, nodes);
        Assert.Contains(nodeD, nodes);
    }

    [Fact]
    public void BreadthFirstNodes_ReturnsNodesInBFSOrder()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeB, nodeD);

        // Act
        var nodes = GraphTraversal.BreadthFirstNodes(graph, nodeA).ToList();

        // Assert
        Assert.Equal(4, nodes.Count);
        Assert.Equal(nodeA, nodes[0]); // Should start with the root
        
        // B and C should come before D (BFS property)
        var indexB = nodes.IndexOf(nodeB);
        var indexC = nodes.IndexOf(nodeC);
        var indexD = nodes.IndexOf(nodeD);
        
        Assert.True(indexB < indexD);
        Assert.True(indexC < indexD);
    }

    [Fact]
    public void DepthFirstSearch_UndirectedGraph_WorksCorrectly()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        var visitor = new TestVisitor();

        // Act
        GraphTraversal.DepthFirstSearch(graph, nodeA, visitor);

        // Assert
        Assert.Equal(3, visitor.DiscoveredNodes.Count);
        Assert.Contains(nodeA, visitor.DiscoveredNodes);
        Assert.Contains(nodeB, visitor.DiscoveredNodes);
        Assert.Contains(nodeC, visitor.DiscoveredNodes);
    }

    [Fact]
    public void BreadthFirstSearch_UndirectedGraph_WorksCorrectly()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        var visitor = new TestVisitor();

        // Act
        GraphTraversal.BreadthFirstSearch(graph, nodeA, visitor);

        // Assert
        Assert.Equal(3, visitor.DiscoveredNodes.Count);
        Assert.Contains(nodeA, visitor.DiscoveredNodes);
        Assert.Contains(nodeB, visitor.DiscoveredNodes);
        Assert.Contains(nodeC, visitor.DiscoveredNodes);
    }

    [Fact]
    public void DepthFirstSearch_DisconnectedGraph_VisitsOnlyReachableNodes()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        // nodeC and nodeD are disconnected

        var visitor = new TestVisitor();

        // Act
        GraphTraversal.DepthFirstSearch(graph, nodeA, visitor);

        // Assert
        Assert.Equal(2, visitor.DiscoveredNodes.Count);
        Assert.Contains(nodeA, visitor.DiscoveredNodes);
        Assert.Contains(nodeB, visitor.DiscoveredNodes);
        Assert.DoesNotContain(nodeC, visitor.DiscoveredNodes);
        Assert.DoesNotContain(nodeD, visitor.DiscoveredNodes);
    }

    [Fact]
    public void DepthFirstSearch_InvalidStartNode_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var invalidNode = new NodeId(999);
        var visitor = new TestVisitor();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            GraphTraversal.DepthFirstSearch(graph, invalidNode, visitor));
    }

    [Fact]
    public void BreadthFirstSearch_InvalidStartNode_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var invalidNode = new NodeId(999);
        var visitor = new TestVisitor();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            GraphTraversal.BreadthFirstSearch(graph, invalidNode, visitor));
    }

    [Fact]
    public void DepthFirstSearch_NullGraph_ThrowsException()
    {
        // Arrange
        DirectedGraph graph = null!;
        var nodeA = new NodeId(1);
        var visitor = new TestVisitor();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            GraphTraversal.DepthFirstSearch(graph, nodeA, visitor));
    }

    [Fact]
    public void DepthFirstSearch_NullVisitor_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        IGraphVisitor visitor = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            GraphTraversal.DepthFirstSearch(graph, nodeA, visitor));
    }

    [Fact]
    public void DepthFirstNodes_SingleNode_ReturnsNode()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();

        // Act
        var nodes = GraphTraversal.DepthFirstNodes(graph, nodeA).ToList();

        // Assert
        Assert.Single(nodes);
        Assert.Equal(nodeA, nodes[0]);
    }

    [Fact]
    public void BreadthFirstNodes_SingleNode_ReturnsNode()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();

        // Act
        var nodes = GraphTraversal.BreadthFirstNodes(graph, nodeA).ToList();

        // Assert
        Assert.Single(nodes);
        Assert.Equal(nodeA, nodes[0]);
    }

    [Fact]
    public void DepthFirstNodes_CyclicGraph_HandlesCycles()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA); // Cycle

        // Act
        var nodes = GraphTraversal.DepthFirstNodes(graph, nodeA).ToList();

        // Assert
        Assert.Equal(3, nodes.Count);
        Assert.Contains(nodeA, nodes);
        Assert.Contains(nodeB, nodes);
        Assert.Contains(nodeC, nodes);
        // Each node should appear exactly once despite the cycle
        Assert.Equal(nodes.Distinct().Count(), nodes.Count);
    }

    [Fact]
    public void BreadthFirstNodes_CyclicGraph_HandlesCycles()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA); // Cycle

        // Act
        var nodes = GraphTraversal.BreadthFirstNodes(graph, nodeA).ToList();

        // Assert
        Assert.Equal(3, nodes.Count);
        Assert.Contains(nodeA, nodes);
        Assert.Contains(nodeB, nodes);
        Assert.Contains(nodeC, nodes);
        // Each node should appear exactly once despite the cycle
        Assert.Equal(nodes.Distinct().Count(), nodes.Count);
    }
}