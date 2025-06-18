using GraphLib.Algorithms;
using GraphLib.Core;
using GraphLib.Implementations;
using GraphLib.Interfaces;
using Xunit;

namespace GraphLib.Tests.Algorithms;

public class DagProcessorTests
{
    [Fact]
    public void Constructor_WithValidDag_InitializesCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);

        // Act
        var processor = new DagProcessor(graph);

        // Assert
        Assert.NotNull(processor);
        Assert.Single(processor.LeadingNodes);
        Assert.Contains(node1, processor.LeadingNodes);
        Assert.Empty(processor.NewLeadingNodes);
    }

    [Fact]
    public void Constructor_WithCyclicGraph_ThrowsException()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node3, node1); // Creates a cycle

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new DagProcessor(graph));
        Assert.Contains("cycles", ex.Message);
    }

    [Fact]
    public void Constructor_WithNullGraph_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DagProcessor(null!));
    }

    [Fact]
    public void LeadingNodes_IdentifiesMultipleRoots()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node3, node4);

        // Act
        var processor = new DagProcessor(graph);

        // Assert
        Assert.Equal(2, processor.LeadingNodes.Count);
        Assert.Contains(node1, processor.LeadingNodes);
        Assert.Contains(node2, processor.LeadingNodes);
    }

    [Fact]
    public void RemoveNode_UpdatesLeadingSet()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        var processor = new DagProcessor(graph);

        // Act
        var result = processor.RemoveNode(node1);

        // Assert
        Assert.True(result);
        Assert.Single(processor.LeadingNodes);
        Assert.Contains(node2, processor.LeadingNodes);
        Assert.Single(processor.NewLeadingNodes);
        Assert.Contains(node2, processor.NewLeadingNodes);
    }

    [Fact]
    public void RemoveNode_MultipleSuccessors_UpdatesCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node3);
        graph.AddEdge(node1, node4);
        var processor = new DagProcessor(graph);

        // Act
        var result = processor.RemoveNode(node1);

        // Assert
        Assert.True(result);
        Assert.Equal(3, processor.LeadingNodes.Count);
        Assert.Contains(node2, processor.LeadingNodes);
        Assert.Contains(node3, processor.LeadingNodes);
        Assert.Contains(node4, processor.LeadingNodes);
        Assert.Equal(3, processor.NewLeadingNodes.Count);
    }

    [Fact]
    public void RemoveNode_ComplexDag_HandlesMultipleIncomingEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node3, node4);
        var processor = new DagProcessor(graph);

        // Act - Remove first root
        processor.RemoveNode(node1);

        // Assert - node3 should not be in leading set yet
        Assert.Single(processor.LeadingNodes);
        Assert.Contains(node2, processor.LeadingNodes);
        Assert.Empty(processor.NewLeadingNodes);

        // Act - Remove second root
        processor.RemoveNode(node2);

        // Assert - now node3 should be in leading set
        Assert.Single(processor.LeadingNodes);
        Assert.Contains(node3, processor.LeadingNodes);
        Assert.Single(processor.NewLeadingNodes);
        Assert.Contains(node3, processor.NewLeadingNodes);
    }

    [Fact]
    public void RemoveNode_NonLeadingNode_ReturnsFalse()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);
        var processor = new DagProcessor(graph);

        // Act
        var result = processor.RemoveNode(node2);

        // Assert
        Assert.False(result);
        Assert.Single(processor.LeadingNodes);
        Assert.Contains(node1, processor.LeadingNodes);
    }

    [Fact]
    public void RemoveNode_AlreadyProcessedNode_ReturnsFalse()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);
        var processor = new DagProcessor(graph);

        // Act
        processor.RemoveNode(node1);
        var result = processor.RemoveNode(node1); // Try to remove again

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsProcessed_TracksProcessedNodes()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);
        var processor = new DagProcessor(graph);

        // Act & Assert - Initially not processed
        Assert.False(processor.IsProcessed(node1));
        Assert.False(processor.IsProcessed(node2));

        // Act & Assert - After removal
        processor.RemoveNode(node1);
        Assert.True(processor.IsProcessed(node1));
        Assert.False(processor.IsProcessed(node2));
    }

    [Fact]
    public void Reset_RestoresInitialState()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        var processor = new DagProcessor(graph);

        // Act - Process some nodes
        processor.RemoveNode(node1);
        processor.RemoveNode(node2);
        
        // Assert - State has changed
        Assert.Single(processor.LeadingNodes);
        Assert.Contains(node3, processor.LeadingNodes);
        Assert.True(processor.IsProcessed(node1));
        Assert.True(processor.IsProcessed(node2));

        // Act - Reset
        processor.Reset();

        // Assert - Back to initial state
        Assert.Single(processor.LeadingNodes);
        Assert.Contains(node1, processor.LeadingNodes);
        Assert.False(processor.IsProcessed(node1));
        Assert.False(processor.IsProcessed(node2));
        Assert.Empty(processor.NewLeadingNodes);
    }

    [Fact]
    public void GetTopologicalOrder_ReturnsCorrectOrder()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node4);
        graph.AddEdge(node3, node4);
        var processor = new DagProcessor(graph);

        // Act
        var order = processor.GetTopologicalOrder().ToList();

        // Assert
        Assert.Equal(4, order.Count);
        Assert.Equal(node1, order[0]);
        Assert.True(order.IndexOf(node2) < order.IndexOf(node4));
        Assert.True(order.IndexOf(node3) < order.IndexOf(node4));
    }

    [Fact]
    public void GetTopologicalOrder_WithPartiallyProcessedGraph_ReturnsRemainingNodes()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        var processor = new DagProcessor(graph);

        // Act - Process first node
        processor.RemoveNode(node1);
        var order = processor.GetTopologicalOrder().ToList();

        // Assert - Should only return unprocessed nodes
        Assert.Equal(2, order.Count);
        Assert.Equal(node2, order[0]);
        Assert.Equal(node3, order[1]);
    }

    [Fact]
    public void EmptyGraph_HandledCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();

        // Act
        var processor = new DagProcessor(graph);

        // Assert
        Assert.Empty(processor.LeadingNodes);
        Assert.Empty(processor.NewLeadingNodes);
        Assert.Empty(processor.GetTopologicalOrder());
    }

    [Fact]
    public void SingleNode_HandledCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node = graph.AddNode();

        // Act
        var processor = new DagProcessor(graph);

        // Assert
        Assert.Single(processor.LeadingNodes);
        Assert.Contains(node, processor.LeadingNodes);

        // Act - Remove the node
        var result = processor.RemoveNode(node);

        // Assert
        Assert.True(result);
        Assert.Empty(processor.LeadingNodes);
        Assert.Empty(processor.NewLeadingNodes);
        Assert.True(processor.IsProcessed(node));
    }

    [Fact]
    public void DisconnectedComponents_AllRootsIdentified()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        graph.AddEdge(node1, node2); // Component 1
        graph.AddEdge(node3, node4); // Component 2

        // Act
        var processor = new DagProcessor(graph);

        // Assert
        Assert.Equal(2, processor.LeadingNodes.Count);
        Assert.Contains(node1, processor.LeadingNodes);
        Assert.Contains(node3, processor.LeadingNodes);
    }

    [Fact]
    public void NewLeadingNodes_ClearedOnEachRemoval()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        var node4 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node4);
        graph.AddEdge(node3, node4);
        var processor = new DagProcessor(graph);

        // Act - First removal
        processor.RemoveNode(node1);
        Assert.Equal(2, processor.NewLeadingNodes.Count);

        // Act - Second removal
        processor.RemoveNode(node2);
        
        // Assert - NewLeadingNodes should only contain nodes from last removal
        Assert.Empty(processor.NewLeadingNodes); // node4 still has incoming edge from node3
    }
}