using GraphLib.Builders;
using GraphLib.Implementations;
using GraphLib.Interfaces;
using Xunit;

namespace GraphLib.Tests.Builders;

public class GraphFactoryTests
{
    [Fact]
    public void Complete_DirectedGraph_CreatesCompleteGraph()
    {
        // Act
        var graph = GraphFactory.Complete<DirectedGraph>(4);

        // Assert
        Assert.Equal(4, graph.NodeCount);
        Assert.Equal(12, graph.EdgeCount); // n*(n-1) for complete directed graph
        
        // Verify all nodes are connected to all other nodes
        var nodes = graph.Nodes.ToList();
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                if (i != j)
                {
                    Assert.True(graph.HasEdge(nodes[i], nodes[j]));
                }
            }
        }
    }

    [Fact]
    public void Complete_UndirectedGraph_CreatesCompleteGraph()
    {
        // Act
        var graph = GraphFactory.Complete<UndirectedGraph>(4);

        // Assert
        Assert.Equal(4, graph.NodeCount);
        Assert.Equal(6, graph.EdgeCount); // n*(n-1)/2 for complete undirected graph
        
        // Verify all unique pairs are connected
        var nodes = graph.Nodes.ToList();
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = i + 1; j < nodes.Count; j++)
            {
                Assert.True(graph.HasEdge(nodes[i], nodes[j]));
                Assert.True(graph.HasEdge(nodes[j], nodes[i])); // Undirected
            }
        }
    }

    [Fact]
    public void Complete_ZeroNodes_CreatesEmptyGraph()
    {
        // Act
        var graph = GraphFactory.Complete<DirectedGraph>(0);

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Complete_NegativeNodes_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GraphFactory.Complete<DirectedGraph>(-1));
    }

    [Fact]
    public void Cycle_CreatesCorrectCycle()
    {
        // Act
        var graph = GraphFactory.Cycle<DirectedGraph>(5);

        // Assert
        Assert.Equal(5, graph.NodeCount);
        Assert.Equal(5, graph.EdgeCount); // n edges for n-cycle
        
        // Verify each node has out-degree 1 and in-degree 1
        foreach (var node in graph.Nodes)
        {
            Assert.Equal(1, graph.OutDegree(node));
            Assert.Equal(1, graph.InDegree(node));
        }
    }

    [Fact]
    public void Cycle_TooFewNodes_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GraphFactory.Cycle<DirectedGraph>(2));
    }

    [Fact]
    public void Path_CreatesLinearPath()
    {
        // Act
        var graph = GraphFactory.Path<DirectedGraph>(5);

        // Assert
        Assert.Equal(5, graph.NodeCount);
        Assert.Equal(4, graph.EdgeCount); // n-1 edges for path
        
        // Count nodes by degree - should have 2 endpoints (degree 1) and 3 internal nodes (degree 2)
        var degrees = graph.Nodes.Select(n => graph.Degree(n)).OrderBy(d => d).ToList();
        Assert.Equal(new[] { 1, 1, 2, 2, 2 }, degrees);
    }

    [Fact]
    public void Path_SingleNode_CreatesIsolatedNode()
    {
        // Act
        var graph = GraphFactory.Path<DirectedGraph>(1);

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Path_ZeroNodes_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GraphFactory.Path<DirectedGraph>(0));
    }

    [Fact]
    public void Star_CreatesStarTopology()
    {
        // Act
        var graph = GraphFactory.Star<DirectedGraph>(5);

        // Assert
        Assert.Equal(5, graph.NodeCount);
        Assert.Equal(4, graph.EdgeCount); // n-1 edges for star
        
        // One node should have out-degree 4, others should have out-degree 0
        var outDegrees = graph.Nodes.Select(n => graph.OutDegree(n)).OrderByDescending(d => d).ToList();
        Assert.Equal(4, outDegrees[0]); // Center node
        Assert.All(outDegrees.Skip(1), d => Assert.Equal(0, d)); // Leaf nodes
    }

    [Fact]
    public void Star_SingleNode_CreatesIsolatedNode()
    {
        // Act
        var graph = GraphFactory.Star<DirectedGraph>(1);

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Grid_CreatesGridTopology()
    {
        // Act
        var graph = GraphFactory.Grid<UndirectedGraph>(3, 4);

        // Assert
        Assert.Equal(12, graph.NodeCount); // 3 * 4 = 12 nodes
        Assert.Equal(17, graph.EdgeCount); // (3-1)*4 + 3*(4-1) = 8 + 9 = 17 edges
        
        // Corner nodes should have degree 2, edge nodes degree 3, internal nodes degree 4
        var degrees = graph.Nodes.Select(n => graph.Degree(n)).OrderBy(d => d).ToList();
        Assert.Equal(4, degrees.Count(d => d == 2)); // 4 corners
        Assert.Equal(6, degrees.Count(d => d == 3)); // 6 edge nodes (2*(3-2) + 2*(4-2))
        Assert.Equal(2, degrees.Count(d => d == 4)); // 2 internal nodes ((3-2)*(4-2))
    }

    [Fact]
    public void Grid_1x1_CreatesSingleNode()
    {
        // Act
        var graph = GraphFactory.Grid<DirectedGraph>(1, 1);

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Grid_ZeroDimension_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GraphFactory.Grid<DirectedGraph>(0, 5));
        Assert.Throws<ArgumentException>(() => GraphFactory.Grid<DirectedGraph>(5, 0));
    }

    [Fact]
    public void Random_WithSeed_CreatesReproducibleGraph()
    {
        // Act
        var graph1 = GraphFactory.Random<DirectedGraph>(10, 0.3, seed: 42);
        var graph2 = GraphFactory.Random<DirectedGraph>(10, 0.3, seed: 42);

        // Assert
        Assert.Equal(graph1.NodeCount, graph2.NodeCount);
        Assert.Equal(graph1.EdgeCount, graph2.EdgeCount);
        
        // Verify same edges exist in both graphs
        var nodes1 = graph1.Nodes.ToList();
        var nodes2 = graph2.Nodes.ToList();
        
        for (int i = 0; i < nodes1.Count; i++)
        {
            for (int j = 0; j < nodes1.Count; j++)
            {
                Assert.Equal(graph1.HasEdge(nodes1[i], nodes1[j]), 
                           graph2.HasEdge(nodes2[i], nodes2[j]));
            }
        }
    }

    [Fact]
    public void Random_ZeroProbability_CreatesNoEdges()
    {
        // Act
        var graph = GraphFactory.Random<DirectedGraph>(5, 0.0);

        // Assert
        Assert.Equal(5, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Random_FullProbability_CreatesCompleteGraph()
    {
        // Act
        var graph = GraphFactory.Random<DirectedGraph>(4, 1.0);

        // Assert
        Assert.Equal(4, graph.NodeCount);
        Assert.Equal(12, graph.EdgeCount); // Complete directed graph
    }

    [Fact]
    public void Random_InvalidProbability_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GraphFactory.Random<DirectedGraph>(5, -0.1));
        Assert.Throws<ArgumentException>(() => GraphFactory.Random<DirectedGraph>(5, 1.1));
    }

    [Fact]
    public void Random_UndirectedGraph_CreatesSymmetricEdges()
    {
        // Act
        var graph = GraphFactory.Random<UndirectedGraph>(5, 1.0);

        // Assert
        Assert.Equal(5, graph.NodeCount);
        Assert.Equal(10, graph.EdgeCount); // Complete undirected graph: n*(n-1)/2
        
        // Verify symmetry
        var nodes = graph.Nodes.ToList();
        for (int i = 0; i < nodes.Count; i++)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                Assert.Equal(graph.HasEdge(nodes[i], nodes[j]), 
                           graph.HasEdge(nodes[j], nodes[i]));
            }
        }
    }

    [Fact]
    public void Tree_CreatesConnectedAcyclicGraph()
    {
        // Act
        var graph = GraphFactory.Tree<UndirectedGraph>(10);

        // Assert
        Assert.Equal(10, graph.NodeCount);
        Assert.Equal(9, graph.EdgeCount); // n-1 edges for tree
        
        // All nodes should be reachable from any starting node (connected)
        var startNode = graph.Nodes.First();
        var visited = new HashSet<GraphLib.Core.NodeId>();
        var queue = new Queue<GraphLib.Core.NodeId>();
        queue.Enqueue(startNode);
        visited.Add(startNode);
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var neighbor in graph.Neighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        Assert.Equal(graph.NodeCount, visited.Count); // All nodes reachable
    }

    [Fact]
    public void Tree_SingleNode_CreatesIsolatedNode()
    {
        // Act
        var graph = GraphFactory.Tree<DirectedGraph>(1);

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Tree_WithSeed_CreatesReproducibleTree()
    {
        // Act
        var tree1 = GraphFactory.Tree<UndirectedGraph>(8, seed: 123);
        var tree2 = GraphFactory.Tree<UndirectedGraph>(8, seed: 123);

        // Assert
        Assert.Equal(tree1.EdgeCount, tree2.EdgeCount);
        
        // Both should have same structure (same edges)
        var nodes1 = tree1.Nodes.ToList();
        var nodes2 = tree2.Nodes.ToList();
        
        for (int i = 0; i < nodes1.Count; i++)
        {
            for (int j = i + 1; j < nodes1.Count; j++)
            {
                Assert.Equal(tree1.HasEdge(nodes1[i], nodes1[j]), 
                           tree2.HasEdge(nodes2[i], nodes2[j]));
            }
        }
    }

    [Fact]
    public void Wheel_CreatesWheelTopology()
    {
        // Act
        var graph = GraphFactory.Wheel<UndirectedGraph>(6); // 1 center + 5 rim

        // Assert
        Assert.Equal(6, graph.NodeCount);
        Assert.Equal(10, graph.EdgeCount); // 5 rim edges + 5 spokes = 10
        
        // One node (center) should have degree 5, others (rim) should have degree 3
        var degrees = graph.Nodes.Select(n => graph.Degree(n)).OrderByDescending(d => d).ToList();
        Assert.Equal(5, degrees[0]); // Center
        Assert.All(degrees.Skip(1), d => Assert.Equal(3, d)); // Rim nodes
    }

    [Fact]
    public void Wheel_TooFewNodes_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => GraphFactory.Wheel<DirectedGraph>(3));
    }

    [Fact]
    public void Bipartite_FullyConnected_CreatesCompleteBipartiteGraph()
    {
        // Act
        var graph = GraphFactory.Bipartite<UndirectedGraph>(3, 4, edgeProbability: 1.0);

        // Assert
        Assert.Equal(7, graph.NodeCount); // 3 + 4 = 7
        Assert.Equal(12, graph.EdgeCount); // 3 * 4 = 12 (complete bipartite)
        
        // Each node in left partition should have degree 4, each in right should have degree 3
        var degrees = graph.Nodes.Select(n => graph.Degree(n)).OrderBy(d => d).ToList();
        Assert.Equal(3, degrees.Count(d => d == 4)); // Left partition
        Assert.Equal(4, degrees.Count(d => d == 3)); // Right partition
    }

    [Fact]
    public void Bipartite_NoEdges_CreatesDisconnectedSets()
    {
        // Act
        var graph = GraphFactory.Bipartite<DirectedGraph>(3, 4, edgeProbability: 0.0);

        // Assert
        Assert.Equal(7, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
    }

    [Fact]
    public void Bipartite_WithSeed_CreatesReproducibleGraph()
    {
        // Act
        var graph1 = GraphFactory.Bipartite<UndirectedGraph>(3, 3, 0.5, seed: 789);
        var graph2 = GraphFactory.Bipartite<UndirectedGraph>(3, 3, 0.5, seed: 789);

        // Assert
        Assert.Equal(graph1.EdgeCount, graph2.EdgeCount);
    }

    [Fact]
    public void AllMethods_WorkWithDifferentGraphTypes()
    {
        // Act & Assert - Test that all methods work with different graph implementations
        var directedComplete = GraphFactory.Complete<DirectedGraph>(3);
        var undirectedComplete = GraphFactory.Complete<UndirectedGraph>(3);

        Assert.IsType<DirectedGraph>(directedComplete);
        Assert.IsType<UndirectedGraph>(undirectedComplete);
        
        Assert.All(new IGraph[] { directedComplete, undirectedComplete }, 
                  g => Assert.Equal(3, g.NodeCount));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void ScalabilityTest_CreatesCorrectSizedGraphs(int size)
    {
        // Act
        var complete = GraphFactory.Complete<DirectedGraph>(size);
        var path = GraphFactory.Path<DirectedGraph>(size);
        var star = GraphFactory.Star<DirectedGraph>(size);

        // Assert
        Assert.Equal(size, complete.NodeCount);
        Assert.Equal(size, path.NodeCount);
        Assert.Equal(size, star.NodeCount);
        
        // Verify expected edge counts
        Assert.Equal(size * (size - 1), complete.EdgeCount);
        Assert.Equal(Math.Max(0, size - 1), path.EdgeCount);
        Assert.Equal(Math.Max(0, size - 1), star.EdgeCount);
    }
}