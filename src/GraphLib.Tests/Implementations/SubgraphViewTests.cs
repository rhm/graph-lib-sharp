using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class SubgraphViewTests
{
    [Fact]
    public void Constructor_WithValidGraph_CreatesSubgraph()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();

        // Act
        var subgraph = new SubgraphView<DirectedGraph>(graph);

        // Assert
        Assert.NotNull(subgraph);
        Assert.Same(graph, subgraph.ParentGraph);
        Assert.Equal(graph.NodeCount, subgraph.NodeCount);
        Assert.Equal(graph.EdgeCount, subgraph.EdgeCount);
    }

    [Fact]
    public void Constructor_WithNullGraph_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SubgraphView<DirectedGraph>(null!));
    }

    [Fact]
    public void NoFilters_ShowsAllNodesAndEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);

        // Act
        var subgraph = new SubgraphView<DirectedGraph>(graph);

        // Assert
        Assert.Equal(3, subgraph.NodeCount);
        Assert.Equal(2, subgraph.EdgeCount);
        Assert.Contains(node1, subgraph.Nodes);
        Assert.Contains(node2, subgraph.Nodes);
        Assert.Contains(node3, subgraph.Nodes);
    }

    [Fact]
    public void NodeFilter_FiltersNodesCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode(); // ID 0
        var node2 = graph.AddNode(); // ID 1
        var node3 = graph.AddNode(); // ID 2
        var node4 = graph.AddNode(); // ID 3
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node3, node4);

        // Act - Filter to only even-ID nodes
        var subgraph = new SubgraphView<DirectedGraph>(graph, 
            nodeFilter: node => node.Value % 2 == 0);

        // Assert
        Assert.Equal(2, subgraph.NodeCount); // nodes 0 and 2
        Assert.True(subgraph.ContainsNode(node1)); // ID 0
        Assert.False(subgraph.ContainsNode(node2)); // ID 1
        Assert.True(subgraph.ContainsNode(node3)); // ID 2
        Assert.False(subgraph.ContainsNode(node4)); // ID 3
    }

    [Fact]
    public void EdgeFilter_FiltersEdgesCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode(); // ID 0
        var node2 = graph.AddNode(); // ID 1
        var node3 = graph.AddNode(); // ID 2
        graph.AddEdge(node1, node2); // Edge 0->1
        graph.AddEdge(node2, node3); // Edge 1->2
        graph.AddEdge(node1, node3); // Edge 0->2

        // Act - Filter to only edges where source ID < target ID
        var subgraph = new SubgraphView<DirectedGraph>(graph,
            edgeFilter: edge => edge.Source.Value < edge.Target.Value);

        // Assert
        Assert.Equal(3, subgraph.NodeCount); // All nodes still present
        Assert.Equal(3, subgraph.EdgeCount); // All edges satisfy the condition
        
        // Try with stricter filter - only edges where source ID == 0
        var strictSubgraph = new SubgraphView<DirectedGraph>(graph,
            edgeFilter: edge => edge.Source.Value == 0);
        
        Assert.Equal(3, strictSubgraph.NodeCount); // All nodes
        Assert.Equal(2, strictSubgraph.EdgeCount); // Only 0->1 and 0->2
    }

    [Fact]
    public void CombinedFilters_ApplyBothNodeAndEdgeFilters()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodes = new List<NodeId>();
        for (int i = 0; i < 5; i++)
        {
            nodes.Add(graph.AddNode());
        }
        
        // Create edges: 0->1, 1->2, 2->3, 3->4, 0->3
        graph.AddEdge(nodes[0], nodes[1]);
        graph.AddEdge(nodes[1], nodes[2]);
        graph.AddEdge(nodes[2], nodes[3]);
        graph.AddEdge(nodes[3], nodes[4]);
        graph.AddEdge(nodes[0], nodes[3]);

        // Act - Keep only nodes 0,1,2,3 and edges where both endpoints are included
        var subgraph = new SubgraphView<DirectedGraph>(graph,
            nodeFilter: node => node.Value < 4,
            edgeFilter: edge => edge.Source.Value < 4 && edge.Target.Value < 4);

        // Assert
        Assert.Equal(4, subgraph.NodeCount); // Nodes 0,1,2,3
        Assert.Equal(4, subgraph.EdgeCount); // Edges 0->1, 1->2, 2->3, 0->3
        Assert.False(subgraph.ContainsNode(nodes[4]));
    }

    [Fact]
    public void Degree_ReflectsFilteredEdges()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode(); // ID 0
        var node2 = graph.AddNode(); // ID 1
        var node3 = graph.AddNode(); // ID 2
        graph.AddEdge(node1, node2);
        graph.AddEdge(node1, node3);
        graph.AddEdge(node2, node1);

        // Act - Filter out edges involving node2
        var subgraph = new SubgraphView<DirectedGraph>(graph,
            edgeFilter: edge => edge.Source != node2 && edge.Target != node2);

        // Assert
        Assert.Equal(3, subgraph.NodeCount); // All nodes present
        Assert.Equal(1, subgraph.EdgeCount); // Only node1->node3
        Assert.Equal(1, subgraph.Degree(node1)); // Only outgoing to node3
        Assert.Equal(0, subgraph.Degree(node2)); // No edges
        Assert.Equal(1, subgraph.Degree(node3)); // Only incoming from node1
    }

    [Fact]
    public void DirectedSubgraphView_PreservesDirectedBehavior()
    {
        // Arrange
        var graph = new DirectedGraph();
        var source = graph.AddNode();
        var target = graph.AddNode();
        var isolated = graph.AddNode();
        graph.AddEdge(source, target);

        // Act
        var subgraph = new DirectedSubgraphView<DirectedGraph>(graph,
            nodeFilter: node => node != isolated);

        // Assert
        Assert.True(subgraph.HasEdge(source, target));
        Assert.False(subgraph.HasEdge(target, source)); // Still directed
        Assert.Equal(1, subgraph.OutDegree(source));
        Assert.Equal(0, subgraph.InDegree(source));
        Assert.Single(subgraph.OutNeighbors(source));
        Assert.Contains(target, subgraph.OutNeighbors(source));
        Assert.False(subgraph.ContainsNode(isolated));
    }

    [Fact]
    public void UndirectedSubgraphView_PreservesUndirectedBehavior()
    {
        // Arrange
        var graph = new UndirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        var node3 = graph.AddNode();
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);

        // Act
        var subgraph = new UndirectedSubgraphView<UndirectedGraph>(graph,
            nodeFilter: node => node != node3);

        // Assert
        Assert.True(subgraph.HasEdge(node1, node2));
        Assert.True(subgraph.HasEdge(node2, node1)); // Still undirected
        Assert.False(subgraph.HasEdge(node2, node3)); // node3 filtered out
        Assert.Single(subgraph.Neighbors(node1));
        Assert.Contains(node2, subgraph.Neighbors(node1));
    }

    [Fact]
    public void InducedSubgraph_ExtensionMethod_WorksCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodes = new List<NodeId>();
        for (int i = 0; i < 4; i++)
        {
            nodes.Add(graph.AddNode());
        }
        
        graph.AddEdge(nodes[0], nodes[1]);
        graph.AddEdge(nodes[1], nodes[2]);
        graph.AddEdge(nodes[2], nodes[3]);
        graph.AddEdge(nodes[0], nodes[3]);

        // Act - Create induced subgraph with nodes 0, 1, 3
        var inducedSubgraph = graph.InducedSubgraph(new[] { nodes[0], nodes[1], nodes[3] });

        // Assert
        Assert.Equal(3, inducedSubgraph.NodeCount);
        Assert.Equal(2, inducedSubgraph.EdgeCount); // 0->1 and 0->3
        Assert.True(inducedSubgraph.ContainsNode(nodes[0]));
        Assert.True(inducedSubgraph.ContainsNode(nodes[1]));
        Assert.False(inducedSubgraph.ContainsNode(nodes[2])); // Not in induced set
        Assert.True(inducedSubgraph.ContainsNode(nodes[3]));
    }

    [Fact]
    public void Subgraph_ExtensionMethods_WorkCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodes = new List<NodeId>();
        for (int i = 0; i < 3; i++)
        {
            nodes.Add(graph.AddNode());
        }
        graph.AddEdge(nodes[0], nodes[1]);
        graph.AddEdge(nodes[1], nodes[2]);

        // Act
        var nodeFilteredSubgraph = graph.Subgraph(node => node.Value < 2);

        // Assert
        Assert.Equal(2, nodeFilteredSubgraph.NodeCount); // nodes 0, 1
        Assert.Equal(1, nodeFilteredSubgraph.EdgeCount); // edge 0->1
    }

    [Fact]
    public void EmptySubgraph_HandlesCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Act - Filter that excludes all nodes
        var emptySubgraph = new SubgraphView<DirectedGraph>(graph,
            nodeFilter: node => false);

        // Assert
        Assert.Equal(0, emptySubgraph.NodeCount);
        Assert.Equal(0, emptySubgraph.EdgeCount);
        Assert.Empty(emptySubgraph.Nodes);
    }

    [Fact]
    public void SubgraphView_ReflectsChangesToParentGraph()
    {
        // Arrange
        var graph = new DirectedGraph();
        var node1 = graph.AddNode();
        var subgraph = new SubgraphView<DirectedGraph>(graph);
        
        Assert.Equal(1, subgraph.NodeCount);

        // Act - Add nodes and edges to parent
        var node2 = graph.AddNode();
        graph.AddEdge(node1, node2);

        // Assert - Subgraph reflects changes
        Assert.Equal(2, subgraph.NodeCount);
        Assert.Equal(1, subgraph.EdgeCount);
        Assert.True(subgraph.ContainsNode(node2));
    }

    [Fact]
    public void ComplexFiltering_WorksCorrectly()
    {
        // Arrange - Create a more complex graph
        var graph = new DirectedGraph();
        var nodes = new List<NodeId>();
        for (int i = 0; i < 6; i++)
        {
            nodes.Add(graph.AddNode());
        }

        // Create edges forming a specific pattern
        graph.AddEdge(nodes[0], nodes[1]); // 0->1
        graph.AddEdge(nodes[1], nodes[2]); // 1->2  
        graph.AddEdge(nodes[2], nodes[3]); // 2->3
        graph.AddEdge(nodes[3], nodes[4]); // 3->4
        graph.AddEdge(nodes[4], nodes[5]); // 4->5
        graph.AddEdge(nodes[0], nodes[3]); // 0->3 (shortcut)
        graph.AddEdge(nodes[2], nodes[5]); // 2->5 (shortcut)

        // Act - Complex filter: keep nodes with even IDs and edges between them
        var subgraph = new SubgraphView<DirectedGraph>(graph,
            nodeFilter: node => node.Value % 2 == 0,
            edgeFilter: edge => edge.Source.Value % 2 == 0 && edge.Target.Value % 2 == 0);

        // Assert
        Assert.Equal(3, subgraph.NodeCount); // Nodes 0, 2, 4
        // Check which edges should exist: 0->2 (no), 2->4 (no), 0->4 (no)
        // Actually, none of our edges connect even-to-even directly
        // But let's check what we actually have
        var evenNodes = nodes.Where((n, i) => i % 2 == 0).ToList(); // nodes 0, 2, 4
        foreach (var node in evenNodes)
        {
            Assert.True(subgraph.ContainsNode(node));
        }
    }

    [Fact]
    public void NestedSubgraphs_WorkCorrectly()
    {
        // Arrange
        var graph = new DirectedGraph();
        var nodes = new List<NodeId>();
        for (int i = 0; i < 5; i++)
        {
            nodes.Add(graph.AddNode());
        }
        
        // Add edges
        for (int i = 0; i < 4; i++)
        {
            graph.AddEdge(nodes[i], nodes[i + 1]);
        }

        // Act - Create nested subgraphs
        var firstSubgraph = new SubgraphView<DirectedGraph>(graph,
            nodeFilter: node => node.Value < 4); // Exclude node 4
        
        var secondSubgraph = new SubgraphView<SubgraphView<DirectedGraph>>(firstSubgraph,
            nodeFilter: node => node.Value > 0); // Exclude node 0

        // Assert
        Assert.Equal(4, firstSubgraph.NodeCount); // nodes 0,1,2,3
        Assert.Equal(3, secondSubgraph.NodeCount); // nodes 1,2,3
        Assert.False(secondSubgraph.ContainsNode(nodes[0]));
        Assert.False(secondSubgraph.ContainsNode(nodes[4]));
        Assert.True(secondSubgraph.ContainsNode(nodes[1]));
        Assert.True(secondSubgraph.ContainsNode(nodes[2]));
        Assert.True(secondSubgraph.ContainsNode(nodes[3]));
    }
}