using GraphLib.Core;
using GraphLib.Extensions;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Extensions;

public class GraphLinqExtensionsTests
{
    [Fact]
    public void Where_FiltersByPredicate_ReturnsFilteredNodes()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var evenNodes = graph.Where(node => node.Value % 2 == 0).ToList();

        // Assert
        Assert.All(evenNodes, node => Assert.True(node.Value % 2 == 0));
    }

    [Fact]
    public void Select_ProjectsNodes_ReturnsProjectedValues()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        // Act
        var nodeValues = graph.Select(node => node.Value).ToList();

        // Assert
        Assert.Equal(3, nodeValues.Count);
        Assert.Contains(nodeA.Value, nodeValues);
        Assert.Contains(nodeB.Value, nodeValues);
        Assert.Contains(nodeC.Value, nodeValues);
    }

    [Fact]
    public void Select_WithDegree_ProjectsNodeAndDegree()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var nodeDegrees = graph.Select((node, degree) => new { Node = node, Degree = degree }).ToList();

        // Assert
        Assert.Equal(3, nodeDegrees.Count);
        
        var nodeAInfo = nodeDegrees.First(x => x.Node.Equals(nodeA));
        var nodeBInfo = nodeDegrees.First(x => x.Node.Equals(nodeB));
        var nodeCInfo = nodeDegrees.First(x => x.Node.Equals(nodeC));

        Assert.Equal(1, nodeAInfo.Degree);
        Assert.Equal(2, nodeBInfo.Degree);
        Assert.Equal(1, nodeCInfo.Degree);
    }

    [Fact]
    public void WhereDegree_FiltersByDegree_ReturnsNodesWithSpecificDegree()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeB, nodeD);

        // Act
        var highDegreeNodes = graph.WhereDegree(degree => degree > 1).ToList();

        // Assert
        Assert.Single(highDegreeNodes);
        Assert.Contains(nodeB, highDegreeNodes);
    }

    [Fact]
    public void WhereOutDegree_DirectedGraph_FiltersByOutDegree()
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
        var highOutDegreeNodes = graph.WhereOutDegree(outDegree => outDegree > 1).ToList();

        // Assert
        Assert.Single(highOutDegreeNodes);
        Assert.Contains(nodeA, highOutDegreeNodes);
    }

    [Fact]
    public void WhereInDegree_DirectedGraph_FiltersByInDegree()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var highInDegreeNodes = graph.WhereInDegree(inDegree => inDegree > 1).ToList();

        // Assert
        Assert.Single(highInDegreeNodes);
        Assert.Contains(nodeC, highInDegreeNodes);
    }

    [Fact]
    public void OrderByDegree_SortsNodesByDegree_ReturnsOrderedNodes()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var orderedNodes = graph.OrderByDegree().ToList();

        // Assert
        Assert.Equal(3, orderedNodes.Count);
        // NodeA and NodeC should come first (degree 1), then NodeB (degree 2)
        Assert.True(graph.Degree(orderedNodes[0]) <= graph.Degree(orderedNodes[1]));
        Assert.True(graph.Degree(orderedNodes[1]) <= graph.Degree(orderedNodes[2]));
    }

    [Fact]
    public void OrderByDegreeDescending_SortsNodesByDegreeDesc_ReturnsOrderedNodes()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var orderedNodes = graph.OrderByDegreeDescending().ToList();

        // Assert
        Assert.Equal(3, orderedNodes.Count);
        // NodeB should come first (degree 2), then NodeA and NodeC (degree 1)
        Assert.True(graph.Degree(orderedNodes[0]) >= graph.Degree(orderedNodes[1]));
        Assert.True(graph.Degree(orderedNodes[1]) >= graph.Degree(orderedNodes[2]));
    }

    [Fact]
    public void OrderByOutDegree_DirectedGraph_SortsByOutDegree()
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
        var orderedNodes = graph.OrderByOutDegree().ToList();

        // Assert
        Assert.Equal(3, orderedNodes.Count);
        Assert.True(graph.OutDegree(orderedNodes[0]) <= graph.OutDegree(orderedNodes[1]));
        Assert.True(graph.OutDegree(orderedNodes[1]) <= graph.OutDegree(orderedNodes[2]));
    }

    [Fact]
    public void OrderByInDegree_DirectedGraph_SortsByInDegree()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeC);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var orderedNodes = graph.OrderByInDegree().ToList();

        // Assert
        Assert.Equal(3, orderedNodes.Count);
        Assert.True(graph.InDegree(orderedNodes[0]) <= graph.InDegree(orderedNodes[1]));
        Assert.True(graph.InDegree(orderedNodes[1]) <= graph.InDegree(orderedNodes[2]));
    }

    [Fact]
    public void GroupByDegree_GroupsNodesByDegree_ReturnsGroupedNodes()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeB, nodeD);

        // Act
        var groups = graph.GroupByDegree().ToList();

        // Assert
        Assert.Equal(2, groups.Count); // Two different degrees: 1 and 3

        var degree1Group = groups.FirstOrDefault(g => g.Key == 1);
        var degree3Group = groups.FirstOrDefault(g => g.Key == 3);

        Assert.NotNull(degree1Group);
        Assert.NotNull(degree3Group);
        Assert.Equal(3, degree1Group.Count()); // NodeA, NodeC, NodeD
        Assert.Single(degree3Group); // NodeB
    }

    [Fact]
    public void GroupBy_CustomSelector_GroupsBySelector()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        // Act - Group by even/odd node values
        var groups = graph.GroupBy(node => node.Value % 2 == 0).ToList();

        // Assert
        Assert.Equal(2, groups.Count); // Even and odd groups
    }

    [Fact]
    public void MinDegreeNodes_ReturnsNodesWithMinimumDegree()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeB, nodeD);

        // Act
        var minNodes = graph.MinDegreeNodes().ToList();

        // Assert
        Assert.Equal(3, minNodes.Count); // NodeA, NodeC, NodeD all have degree 1
        Assert.Contains(nodeA, minNodes);
        Assert.Contains(nodeC, minNodes);
        Assert.Contains(nodeD, minNodes);
        Assert.DoesNotContain(nodeB, minNodes);
    }

    [Fact]
    public void MaxDegreeNodes_ReturnsNodesWithMaximumDegree()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeB, nodeD);

        // Act
        var maxNodes = graph.MaxDegreeNodes().ToList();

        // Assert
        Assert.Single(maxNodes); // Only NodeB has degree 3
        Assert.Contains(nodeB, maxNodes);
    }

    [Fact]
    public void DegreeSequence_ReturnsSortedDegrees()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeB, nodeD);

        // Act
        var sequence = graph.DegreeSequence().ToList();

        // Assert
        Assert.Equal(new[] { 3, 1, 1, 1 }, sequence); // Descending order
    }

    [Fact]
    public void AnyNode_WithPredicate_ReturnsTrueIfAnyMatches()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var hasHighDegreeNode = graph.AnyNode(node => graph.Degree(node) > 1);

        // Assert
        Assert.True(hasHighDegreeNode);
    }

    [Fact]
    public void AllNodes_WithPredicate_ReturnsTrueIfAllMatch()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);

        // Act
        var allConnected = graph.AllNodes(node => graph.Degree(node) > 0);

        // Assert
        Assert.True(allConnected);
    }

    [Fact]
    public void CountNodes_WithPredicate_ReturnsCorrectCount()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);

        // Act
        var isolatedCount = graph.CountNodes(node => graph.Degree(node) == 0);

        // Assert
        Assert.Equal(1, isolatedCount); // NodeC is isolated
    }

    [Fact]
    public void FirstNode_WithPredicate_ReturnsFirstMatch()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);

        // Act
        var highDegreeNode = graph.FirstNode(node => graph.Degree(node) > 1);

        // Assert
        Assert.Equal(nodeB, highDegreeNode);
    }

    [Fact]
    public void FirstNodeOrDefault_WithPredicate_ReturnsFirstMatchOrDefault()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();

        // Act
        var highDegreeNode = graph.FirstNodeOrDefault(node => graph.Degree(node) > 5);

        // Assert
        Assert.Equal(default(NodeId), highDegreeNode);
    }

    [Fact]
    public void Where_NullGraph_ThrowsException()
    {
        // Arrange
        UndirectedGraph graph = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => graph.Where(node => true));
    }

    [Fact]
    public void Where_NullPredicate_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => graph.Where(null!));
    }

    [Fact]
    public void EmptyGraph_ExtensionMethods_HandleCorrectly()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act & Assert
        Assert.Empty(graph.Where(node => true));
        Assert.Empty(graph.Select(node => node.Value));
        Assert.Empty(graph.MinDegreeNodes());
        Assert.Empty(graph.MaxDegreeNodes());
        Assert.Empty(graph.DegreeSequence());
        Assert.False(graph.AnyNode(node => true));
        Assert.True(graph.AllNodes(node => true)); // Vacuous truth
        Assert.Equal(0, graph.CountNodes(node => true));
    }

    [Fact]
    public void FirstNode_NoMatch_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => 
            graph.FirstNode(node => graph.Degree(node) > 5));
    }
}