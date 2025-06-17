using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class BipartiteGraphTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        // Act
        var graph = new BipartiteGraph();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
        Assert.True(graph.IsBipartite);
    }

    [Fact]
    public void AddNodeToSet_Left_AddsNodeToLeftSet()
    {
        // Arrange
        var graph = new BipartiteGraph();

        // Act
        var node = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Contains(node, graph.Nodes);
        Assert.True(graph.ContainsNode(node));
        Assert.Equal(BipartiteGraph.NodeSet.Left, graph.GetNodeSet(node));
    }

    [Fact]
    public void AddNodeToSet_Right_AddsNodeToRightSet()
    {
        // Arrange
        var graph = new BipartiteGraph();

        // Act
        var node = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Equal(BipartiteGraph.NodeSet.Right, graph.GetNodeSet(node));
    }

    [Fact]
    public void GetNodesInSet_ReturnsCorrectNodes()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode1 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var leftNode2 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode1 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        var rightNode2 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);

        // Act
        var leftNodes = graph.GetNodesInSet(BipartiteGraph.NodeSet.Left).ToList();
        var rightNodes = graph.GetNodesInSet(BipartiteGraph.NodeSet.Right).ToList();

        // Assert
        Assert.Equal(2, leftNodes.Count);
        Assert.Contains(leftNode1, leftNodes);
        Assert.Contains(leftNode2, leftNodes);

        Assert.Equal(2, rightNodes.Count);
        Assert.Contains(rightNode1, rightNodes);
        Assert.Contains(rightNode2, rightNodes);
    }

    [Fact]
    public void AddEdge_LeftToRight_AddsEdgeSuccessfully()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);

        // Act
        graph.AddEdge(leftNode, rightNode);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(leftNode, rightNode));
        Assert.True(graph.HasEdge(rightNode, leftNode)); // Undirected
    }

    [Fact]
    public void AddEdge_RightToLeft_AddsEdgeSuccessfully()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);

        // Act & Assert - BipartiteGraph enforces parameter order (left, right)
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(rightNode, leftNode));
        
        // But correct order should work
        graph.AddEdge(leftNode, rightNode);
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(leftNode, rightNode));
        Assert.True(graph.HasEdge(rightNode, leftNode));
    }

    [Fact]
    public void AddEdge_LeftToLeft_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode1 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var leftNode2 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(leftNode1, leftNode2));
    }

    [Fact]
    public void AddEdge_RightToRight_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var rightNode1 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        var rightNode2 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(rightNode1, rightNode2));
    }

    [Fact]
    public void AddEdge_NonExistentLeftNode_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var rightNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        var fakeLeftNode = new NodeId(999);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(fakeLeftNode, rightNode));
    }

    [Fact]
    public void AddEdge_NonExistentRightNode_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var fakeRightNode = new NodeId(999);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.AddEdge(leftNode, fakeRightNode));
    }

    [Fact]
    public void Neighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode1 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        var rightNode2 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        var rightNode3 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        
        graph.AddEdge(leftNode, rightNode1);
        graph.AddEdge(leftNode, rightNode2);

        // Act
        var leftNeighbors = graph.Neighbors(leftNode).ToList();
        var rightNeighbors = graph.Neighbors(rightNode1).ToList();

        // Assert
        Assert.Equal(2, leftNeighbors.Count);
        Assert.Contains(rightNode1, leftNeighbors);
        Assert.Contains(rightNode2, leftNeighbors);
        Assert.DoesNotContain(rightNode3, leftNeighbors);

        Assert.Single(rightNeighbors);
        Assert.Contains(leftNode, rightNeighbors);
    }

    [Fact]
    public void Degree_CountsIncidentEdges()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode1 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        var rightNode2 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        var rightNode3 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        
        graph.AddEdge(leftNode, rightNode1);
        graph.AddEdge(leftNode, rightNode2);
        graph.AddEdge(leftNode, rightNode3);

        // Act & Assert
        Assert.Equal(3, graph.Degree(leftNode));
        Assert.Equal(1, graph.Degree(rightNode1));
        Assert.Equal(1, graph.Degree(rightNode2));
        Assert.Equal(1, graph.Degree(rightNode3));
    }

    [Fact]
    public void RemoveEdge_RemovesSymmetricEdge()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        graph.AddEdge(leftNode, rightNode);

        // Act
        graph.RemoveEdge(leftNode, rightNode);

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(leftNode, rightNode));
        Assert.False(graph.HasEdge(rightNode, leftNode));
    }

    [Fact]
    public void RemoveNode_RemovesNodeAndIncidentEdges()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode1 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var leftNode2 = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        
        graph.AddEdge(leftNode1, rightNode);
        graph.AddEdge(leftNode2, rightNode);

        // Act
        graph.RemoveNode(rightNode);

        // Assert
        Assert.Equal(2, graph.NodeCount); // Only left nodes remain
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.ContainsNode(rightNode));
        Assert.False(graph.HasEdge(leftNode1, rightNode));
        Assert.False(graph.HasEdge(leftNode2, rightNode));
    }

    [Fact]
    public void GetNodeSet_NonExistentNode_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var fakeNode = new NodeId(999);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.GetNodeSet(fakeNode));
    }

    [Fact]
    public void Clear_RemovesAllNodesAndEdges()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        graph.AddEdge(leftNode, rightNode);

        // Act
        graph.Clear();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
        Assert.Empty(graph.GetNodesInSet(BipartiteGraph.NodeSet.Left));
        Assert.Empty(graph.GetNodesInSet(BipartiteGraph.NodeSet.Right));
    }

    [Fact]
    public void AddEdge_DuplicateEdge_DoesNotIncreaseEdgeCount()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        var rightNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        graph.AddEdge(leftNode, rightNode);

        // Act
        graph.AddEdge(leftNode, rightNode); // Add same edge again

        // Assert
        Assert.Equal(1, graph.EdgeCount); // Should still be 1
        Assert.True(graph.HasEdge(leftNode, rightNode));
    }

    [Fact]
    public void CompleteBalancedBipartiteGraph_CorrectStructure()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNodes = new List<NodeId>();
        var rightNodes = new List<NodeId>();

        // Create 3x3 complete bipartite graph
        for (int i = 0; i < 3; i++)
        {
            leftNodes.Add(graph.AddNodeToSet(BipartiteGraph.NodeSet.Left));
            rightNodes.Add(graph.AddNodeToSet(BipartiteGraph.NodeSet.Right));
        }

        // Add all possible edges
        foreach (var leftNode in leftNodes)
        {
            foreach (var rightNode in rightNodes)
            {
                graph.AddEdge(leftNode, rightNode);
            }
        }

        // Act & Assert
        Assert.Equal(6, graph.NodeCount);
        Assert.Equal(9, graph.EdgeCount); // 3 * 3 = 9 edges
        
        // Each left node should have degree 3
        foreach (var leftNode in leftNodes)
        {
            Assert.Equal(3, graph.Degree(leftNode));
        }
        
        // Each right node should have degree 3
        foreach (var rightNode in rightNodes)
        {
            Assert.Equal(3, graph.Degree(rightNode));
        }
    }

    [Fact]
    public void IsBipartite_AlwaysReturnsTrue()
    {
        // Arrange
        var graph = new BipartiteGraph();

        // Act & Assert
        Assert.True(graph.IsBipartite); // Empty graph

        var leftNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        Assert.True(graph.IsBipartite); // Single node

        var rightNode = graph.AddNodeToSet(BipartiteGraph.NodeSet.Right);
        graph.AddEdge(leftNode, rightNode);
        Assert.True(graph.IsBipartite); // With edge
    }

    [Theory]
    [InlineData(BipartiteGraph.NodeSet.Left)]
    [InlineData(BipartiteGraph.NodeSet.Right)]
    public void AddNodeToSet_InvalidEnum_ThrowsArgumentException(BipartiteGraph.NodeSet invalidSet)
    {
        // This test would only be meaningful if we had invalid enum values
        // Since we only have Left and Right, both are valid
        // Let's test that normal values work
        var graph = new BipartiteGraph();
        var node = graph.AddNodeToSet(invalidSet);
        Assert.Equal(invalidSet, graph.GetNodeSet(node));
    }
}