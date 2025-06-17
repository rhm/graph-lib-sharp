using GraphLib.Builders;
using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Integration;

/// <summary>
/// Integration tests that verify different graph components work together correctly.
/// </summary>
public class GraphIntegrationTests
{
    [Fact]
    public void BuilderAndFactory_CreateEquivalentGraphs()
    {
        // Arrange & Act
        var builtGraph = GraphBuilder.Create<DirectedGraph>()
            .AddNodes(4)
            .AddCycle(0, 1, 2, 3)
            .Build();

        var factoryGraph = GraphFactory.Cycle<DirectedGraph>(4);

        // Assert
        Assert.Equal(builtGraph.NodeCount, factoryGraph.NodeCount);
        Assert.Equal(builtGraph.EdgeCount, factoryGraph.EdgeCount);
        
        // Both should have same structure (each node has in-degree 1 and out-degree 1)
        foreach (var node in builtGraph.Nodes)
        {
            Assert.Equal(1, builtGraph.OutDegree(node));
            Assert.Equal(1, builtGraph.InDegree(node));
        }
        
        foreach (var node in factoryGraph.Nodes)
        {
            Assert.Equal(1, factoryGraph.OutDegree(node));
            Assert.Equal(1, factoryGraph.InDegree(node));
        }
    }

    [Fact]
    public void WeightedGraph_WithSubgraphView_PreservesWeights()
    {
        // Arrange
        var graph = new WeightedDirectedGraph<double>();
        var nodes = new List<NodeId>();
        for (int i = 0; i < 4; i++)
        {
            nodes.Add(graph.AddNode());
        }

        graph.AddEdge(nodes[0], nodes[1], 1.5);
        graph.AddEdge(nodes[1], nodes[2], 2.5);
        graph.AddEdge(nodes[2], nodes[3], 3.5);
        graph.AddEdge(nodes[0], nodes[3], 4.5);

        // Act - Create subgraph excluding node 2
        var subgraph = graph.Subgraph(node => node != nodes[2]);

        // Assert
        Assert.Equal(3, subgraph.NodeCount);
        Assert.Equal(2, subgraph.EdgeCount); // Only edges 0->1 and 0->3 remain
        
        // Verify original graph still has all weights
        Assert.Equal(1.5, graph.GetEdgeWeight(nodes[0], nodes[1]));
        Assert.Equal(4.5, graph.GetEdgeWeight(nodes[0], nodes[3]));
    }

    [Fact]
    public void BipartiteGraph_WithImmutableView_MaintainsProperties()
    {
        // Arrange
        var graph = new BipartiteGraph();
        var leftNodes = new List<NodeId>();
        var rightNodes = new List<NodeId>();

        for (int i = 0; i < 3; i++)
        {
            leftNodes.Add(graph.AddNodeToSet(BipartiteGraph.NodeSet.Left));
            rightNodes.Add(graph.AddNodeToSet(BipartiteGraph.NodeSet.Right));
        }

        // Create complete bipartite graph
        foreach (var left in leftNodes)
        {
            foreach (var right in rightNodes)
            {
                graph.AddEdge(left, right);
            }
        }

        // Act
        var immutableView = new ImmutableGraphView<BipartiteGraph>(graph);

        // Assert
        Assert.Equal(6, immutableView.NodeCount);
        Assert.Equal(9, immutableView.EdgeCount);
        Assert.True(graph.IsBipartite); // Original property preserved
        
        // Verify view reflects changes
        var newLeft = graph.AddNodeToSet(BipartiteGraph.NodeSet.Left);
        Assert.Equal(7, immutableView.NodeCount);
    }

    [Fact]
    public void GraphWithData_UsingBuilder_WorksCorrectly()
    {
        // Arrange
        var graph = GraphBuilder.Create<DirectedGraphWithNodeData<string>>()
            .AddNodes(3)
            .AddPath(0, 1, 2)
            .Build();

        var builder = GraphBuilder.Create<DirectedGraphWithNodeData<string>>();
        builder.AddNodes(3);
        var node0 = builder.GetNode(0);
        var node1 = builder.GetNode(1);
        var node2 = builder.GetNode(2);

        // Act
        graph.SetNodeData(node0, "start");
        graph.SetNodeData(node1, "middle");
        graph.SetNodeData(node2, "end");

        // Assert
        Assert.Equal("start", graph.GetNodeData(node0));
        Assert.Equal("middle", graph.GetNodeData(node1));
        Assert.Equal("end", graph.GetNodeData(node2));
        Assert.True(graph.HasEdge(node0, node1));
        Assert.True(graph.HasEdge(node1, node2));
    }

    [Fact]
    public void ComplexWorkflow_BuildFilterTransform()
    {
        // Arrange - Build a complex social network graph
        var socialNetwork = new DirectedGraphWithData<Person, Relationship>();

        var alice = socialNetwork.AddNode(new Person("Alice", 25));
        var bob = socialNetwork.AddNode(new Person("Bob", 30));
        var charlie = socialNetwork.AddNode(new Person("Charlie", 35));
        var diana = socialNetwork.AddNode(new Person("Diana", 28));

        socialNetwork.AddEdge(alice, bob, new Relationship("friend", DateTime.Now.AddMonths(-6)));
        socialNetwork.AddEdge(bob, charlie, new Relationship("colleague", DateTime.Now.AddMonths(-3)));
        socialNetwork.AddEdge(charlie, diana, new Relationship("friend", DateTime.Now.AddMonths(-1)));
        socialNetwork.AddEdge(alice, diana, new Relationship("friend", DateTime.Now.AddMonths(-2)));

        // Act - Create subgraph of people under 30 and their friendships
        var youngFriendsNetwork = socialNetwork.Subgraph(
            node => socialNetwork.GetNodeData(node).Age < 30,
            edge => socialNetwork.GetEdgeData(edge.Source, edge.Target).Type == "friend"
        );

        // Assert
        Assert.Equal(2, youngFriendsNetwork.NodeCount); // Alice and Diana
        Assert.Equal(1, youngFriendsNetwork.EdgeCount); // Alice -> Diana friendship
        Assert.True(youngFriendsNetwork.ContainsNode(alice));
        Assert.True(youngFriendsNetwork.ContainsNode(diana));
        Assert.False(youngFriendsNetwork.ContainsNode(bob)); // Too old
        Assert.False(youngFriendsNetwork.ContainsNode(charlie)); // Too old
    }

    [Fact]
    public void DifferentGraphTypes_SameAlgorithm()
    {
        // This test demonstrates how the same algorithmic pattern 
        // (calculating total degree) works across different graph types

        // Arrange - Create different graph types with same structure
        var directedGraph = GraphFactory.Path<DirectedGraph>(5);
        var undirectedGraph = GraphFactory.Path<UndirectedGraph>(5);

        // Act - Calculate total degree for each graph type
        var directedTotalDegree = directedGraph.Nodes.Sum(n => directedGraph.Degree(n));
        var undirectedTotalDegree = undirectedGraph.Nodes.Sum(n => undirectedGraph.Degree(n));

        // Assert - Different expected values due to graph type differences
        Assert.Equal(8, directedTotalDegree); // 4 edges * 2 (each edge counted twice in directed)
        Assert.Equal(8, undirectedTotalDegree); // 4 edges * 2 (each edge counted twice in undirected)
    }

    [Fact]
    public void FluentInterface_ChainMultipleOperations()
    {
        // Arrange & Act - Chain multiple operations using fluent interface
        var graph = new WeightedDirectedGraph<string>()
            .WithEdge(new NodeId(0), new NodeId(1), "first")
            .WithEdge(new NodeId(1), new NodeId(2), "second")
            .WithEdge(new NodeId(2), new NodeId(0), "third");

        // Note: WithEdge adds nodes if they don't exist

        // Assert
        Assert.Equal(3, graph.NodeCount);
        Assert.Equal(3, graph.EdgeCount);
        Assert.Equal("first", graph.GetEdgeWeight(new NodeId(0), new NodeId(1)));
        Assert.Equal("second", graph.GetEdgeWeight(new NodeId(1), new NodeId(2)));
        Assert.Equal("third", graph.GetEdgeWeight(new NodeId(2), new NodeId(0)));
    }

    [Fact]
    public void LargeGraph_PerformanceTest()
    {
        // Arrange
        const int nodeCount = 1000;
        
        // Act - Create large graphs using different methods
        var startTime = DateTime.Now;
        var largeRandom = GraphFactory.Random<DirectedGraph>(nodeCount, 0.01);
        var randomTime = DateTime.Now - startTime;

        startTime = DateTime.Now;
        var largePath = GraphFactory.Path<DirectedGraph>(nodeCount);
        var pathTime = DateTime.Now - startTime;

        startTime = DateTime.Now;
        var denseLarge = new DenseDirectedGraph(nodeCount);
        for (int i = 0; i < nodeCount; i++)
        {
            denseLarge.AddNode();
        }
        var denseTime = DateTime.Now - startTime;

        // Assert - Verify correctness and reasonable performance
        Assert.Equal(nodeCount, largeRandom.NodeCount);
        Assert.Equal(nodeCount, largePath.NodeCount);
        Assert.Equal(nodeCount, denseLarge.NodeCount);

        Assert.Equal(nodeCount - 1, largePath.EdgeCount); // Path has n-1 edges
        
        // Performance should be reasonable (< 1 second for these operations)
        Assert.True(randomTime.TotalSeconds < 1.0);
        Assert.True(pathTime.TotalSeconds < 1.0);
        Assert.True(denseTime.TotalSeconds < 1.0);
    }

    [Fact]
    public void GraphSerialization_Roundtrip()
    {
        // This test demonstrates that graph structure is preserved
        // through various transformations (simulating serialization)

        // Arrange
        var originalGraph = GraphFactory.Complete<DirectedGraph>(4);
        
        // Act - Simulate serialization by extracting edges and rebuilding
        var edges = new List<Edge>();
        foreach (var source in originalGraph.Nodes)
        {
            foreach (var target in originalGraph.OutNeighbors(source))
            {
                edges.Add(new Edge(source, target));
            }
        }

        var rebuiltGraph = GraphBuilder.Create<DirectedGraph>()
            .AddNodes(originalGraph.NodeCount)
            .AddEdges(edges)
            .Build();

        // Assert
        Assert.Equal(originalGraph.NodeCount, rebuiltGraph.NodeCount);
        Assert.Equal(originalGraph.EdgeCount, rebuiltGraph.EdgeCount);
        
        // Verify same structure (all nodes have same degrees)
        var originalDegrees = originalGraph.Nodes.Select(n => originalGraph.Degree(n)).OrderBy(d => d).ToList();
        var rebuiltDegrees = rebuiltGraph.Nodes.Select(n => rebuiltGraph.Degree(n)).OrderBy(d => d).ToList();
        Assert.Equal(originalDegrees, rebuiltDegrees);
    }

    [Fact]
    public void ErrorHandling_AcrossComponents()
    {
        // Test that error handling is consistent across different components

        // Arrange
        var graph = new DirectedGraph();
        var node = graph.AddNode();
        var fakeNode = new NodeId(999);

        // Act & Assert - Test consistent error handling
        Assert.Throws<ArgumentException>(() => graph.AddEdge(fakeNode, node));
        Assert.Throws<ArgumentException>(() => graph.AddEdge(node, fakeNode));
        
        var subgraph = graph.Subgraph();
        // Subgraph should handle non-existent nodes gracefully
        Assert.False(subgraph.ContainsNode(fakeNode));
        Assert.Equal(0, subgraph.Degree(fakeNode));
        
        var immutableView = new ImmutableGraphView<DirectedGraph>(graph);
        Assert.False(immutableView.ContainsNode(fakeNode));
    }

    private class Person : IEquatable<Person>
    {
        public string Name { get; }
        public int Age { get; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public bool Equals(Person? other)
        {
            return other != null && Name == other.Name && Age == other.Age;
        }

        public override bool Equals(object? obj) => Equals(obj as Person);
        public override int GetHashCode() => HashCode.Combine(Name, Age);
    }

    private class Relationship : IEquatable<Relationship>
    {
        public string Type { get; }
        public DateTime Since { get; }

        public Relationship(string type, DateTime since)
        {
            Type = type;
            Since = since;
        }

        public bool Equals(Relationship? other)
        {
            return other != null && Type == other.Type && Since == other.Since;
        }

        public override bool Equals(object? obj) => Equals(obj as Relationship);
        public override int GetHashCode() => HashCode.Combine(Type, Since);
    }
}