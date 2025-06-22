using GraphLib.Algorithms;
using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Algorithms;

public class CliqueFindingTests
{
    [Fact]
    public void FindMaximumClique_SimpleClique_FindsCorrectClique()
    {
        // Arrange - Create a triangle (3-clique)
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);

        // Act
        var clique = CliqueFinding.FindMaximumClique(graph);

        // Assert
        Assert.Equal(3, clique.Count);
        Assert.Contains(nodeA, clique);
        Assert.Contains(nodeB, clique);
        Assert.Contains(nodeC, clique);
    }

    [Fact]
    public void FindMaximumClique_LargerGraphWithClique_FindsMaximumClique()
    {
        // Arrange - Create a graph with a 4-clique and additional nodes
        var graph = new UndirectedGraph();
        var nodes = new NodeId[6];
        for (int i = 0; i < 6; i++)
        {
            nodes[i] = graph.AddNode();
        }

        // Create 4-clique with nodes 0, 1, 2, 3
        for (int i = 0; i < 4; i++)
        {
            for (int j = i + 1; j < 4; j++)
            {
                graph.AddEdge(nodes[i], nodes[j]);
            }
        }

        // Add some additional edges that don't extend the clique
        graph.AddEdge(nodes[0], nodes[4]);
        graph.AddEdge(nodes[1], nodes[5]);

        // Act
        var clique = CliqueFinding.FindMaximumClique(graph);

        // Assert
        Assert.Equal(4, clique.Count);
        for (int i = 0; i < 4; i++)
        {
            Assert.Contains(nodes[i], clique);
        }
    }

    [Fact]
    public void FindMaximumClique_EmptyGraph_ReturnsEmpty()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act
        var clique = CliqueFinding.FindMaximumClique(graph);

        // Assert
        Assert.Empty(clique);
    }

    [Fact]
    public void FindMaximumClique_SingleNode_ReturnsSingleNode()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();

        // Act
        var clique = CliqueFinding.FindMaximumClique(graph);

        // Assert
        Assert.Single(clique);
        Assert.Equal(nodeA, clique[0]);
    }

    [Fact]
    public void FindMaximumClique_NoEdges_ReturnsAnyNode()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        // No edges - each node is a clique of size 1

        // Act
        var clique = CliqueFinding.FindMaximumClique(graph);

        // Assert
        Assert.Single(clique);
        Assert.True(clique[0].Equals(nodeA) || clique[0].Equals(nodeB) || clique[0].Equals(nodeC));
    }

    [Fact]
    public void FindAllMaximalCliques_SimpleGraph_FindsAllMaximalCliques()
    {
        // Arrange - Create a graph with two triangles sharing one edge
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        // First triangle: A-B-C
        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);

        // Second triangle: B-C-D
        graph.AddEdge(nodeB, nodeD);
        graph.AddEdge(nodeC, nodeD);

        // Act
        var cliques = CliqueFinding.FindAllMaximalCliques(graph).ToList();

        // Assert
        Assert.Equal(2, cliques.Count);
        
        // Each clique should be a triangle
        Assert.All(cliques, clique => Assert.Equal(3, clique.Count));
        
        // Check that we have the two expected triangles
        var hasABC = cliques.Any(c => c.Contains(nodeA) && c.Contains(nodeB) && c.Contains(nodeC));
        var hasBCD = cliques.Any(c => c.Contains(nodeB) && c.Contains(nodeC) && c.Contains(nodeD));
        
        Assert.True(hasABC);
        Assert.True(hasBCD);
    }

    [Fact]
    public void FindAllMaximalCliques_CompleteGraph_ReturnsSingleClique()
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
        var cliques = CliqueFinding.FindAllMaximalCliques(graph).ToList();

        // Assert
        Assert.Single(cliques);
        Assert.Equal(4, cliques[0].Count);
        
        // All nodes should be in the single maximal clique
        for (int i = 0; i < 4; i++)
        {
            Assert.Contains(nodes[i], cliques[0]);
        }
    }

    [Fact]
    public void FindCliquesOfSize_SpecificSize_FindsCorrectCliques()
    {
        // Arrange - Create a graph with multiple triangles
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();
        var nodeE = graph.AddNode();

        // First triangle: A-B-C
        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);

        // Second triangle: C-D-E
        graph.AddEdge(nodeC, nodeD);
        graph.AddEdge(nodeD, nodeE);
        graph.AddEdge(nodeE, nodeC);

        // Act
        var triangles = CliqueFinding.FindCliquesOfSize(graph, 3).ToList();

        // Assert
        Assert.Equal(2, triangles.Count);
        Assert.All(triangles, triangle => Assert.Equal(3, triangle.Count));
    }

    [Fact]
    public void FindCliquesOfSize_SizeOne_ReturnsAllNodes()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB); // Only one edge

        // Act
        var cliques = CliqueFinding.FindCliquesOfSize(graph, 1).ToList();

        // Assert
        Assert.Equal(3, cliques.Count);
        Assert.All(cliques, clique => Assert.Single(clique));
    }

    [Fact]
    public void FindCliquesOfSize_SizeLargerThanMaxClique_ReturnsEmpty()
    {
        // Arrange - Create a triangle (max clique size 3)
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);

        // Act
        var cliques = CliqueFinding.FindCliquesOfSize(graph, 4).ToList();

        // Assert
        Assert.Empty(cliques);
    }

    [Fact]
    public void HasCliqueOfSize_ExistingClique_ReturnsTrue()
    {
        // Arrange - Create a 4-clique
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

        // Act & Assert
        Assert.True(CliqueFinding.HasCliqueOfSize(graph, 4));
        Assert.True(CliqueFinding.HasCliqueOfSize(graph, 3));
        Assert.True(CliqueFinding.HasCliqueOfSize(graph, 2));
        Assert.True(CliqueFinding.HasCliqueOfSize(graph, 1));
    }

    [Fact]
    public void HasCliqueOfSize_NonExistingClique_ReturnsFalse()
    {
        // Arrange - Create a triangle (max clique size 3)
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);

        // Act & Assert
        Assert.False(CliqueFinding.HasCliqueOfSize(graph, 4));
        Assert.False(CliqueFinding.HasCliqueOfSize(graph, 5));
    }

    [Fact]
    public void HasCliqueOfSize_EmptyGraph_ReturnsFalseForPositiveSize()
    {
        // Arrange
        var graph = new UndirectedGraph();

        // Act & Assert
        Assert.False(CliqueFinding.HasCliqueOfSize(graph, 1));
        Assert.False(CliqueFinding.HasCliqueOfSize(graph, 2));
    }

    [Fact]
    public void HasCliqueOfSize_GraphWithNodes_ReturnsTrueForSizeOne()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();

        // Act & Assert
        Assert.True(CliqueFinding.HasCliqueOfSize(graph, 1));
    }

    [Fact]
    public void FindMaximumClique_NullGraph_ThrowsException()
    {
        // Arrange
        UndirectedGraph graph = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CliqueFinding.FindMaximumClique(graph));
    }

    [Fact]
    public void FindCliquesOfSize_InvalidSize_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => CliqueFinding.FindCliquesOfSize(graph, 0));
        Assert.Throws<ArgumentException>(() => CliqueFinding.FindCliquesOfSize(graph, -1));
    }

    [Fact]
    public void HasCliqueOfSize_InvalidSize_ThrowsException()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => CliqueFinding.HasCliqueOfSize(graph, 0));
        Assert.Throws<ArgumentException>(() => CliqueFinding.HasCliqueOfSize(graph, -1));
    }

    [Fact]
    public void FindAllMaximalCliques_PathGraph_FindsCorrectCliques()
    {
        // Arrange - Create a path graph: A-B-C-D
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();
        var nodeD = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeD);

        // Act
        var cliques = CliqueFinding.FindAllMaximalCliques(graph).ToList();

        // Assert - In a path graph, maximal cliques are the edges
        Assert.Equal(3, cliques.Count);
        Assert.All(cliques, clique => Assert.Equal(2, clique.Count));
    }

    [Fact]
    public void FindCliquesOfSize_DuplicateResults_ReturnsUniqueCliques()
    {
        // Arrange - Create a simple triangle
        var graph = new UndirectedGraph();
        var nodeA = graph.AddNode();
        var nodeB = graph.AddNode();
        var nodeC = graph.AddNode();

        graph.AddEdge(nodeA, nodeB);
        graph.AddEdge(nodeB, nodeC);
        graph.AddEdge(nodeC, nodeA);

        // Act
        var pairs = CliqueFinding.FindCliquesOfSize(graph, 2).ToList();

        // Assert - Should find exactly 3 pairs (edges) in the triangle
        Assert.Equal(3, pairs.Count);
        Assert.All(pairs, pair => Assert.Equal(2, pair.Count));
        
        // Verify all pairs are unique
        var pairSets = pairs.Select(p => p.ToHashSet()).ToList();
        for (int i = 0; i < pairSets.Count; i++)
        {
            for (int j = i + 1; j < pairSets.Count; j++)
            {
                Assert.False(pairSets[i].SetEquals(pairSets[j]));
            }
        }
    }
}