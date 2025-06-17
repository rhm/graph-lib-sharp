using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class DirectedGraphWithDataTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        // Act
        var graph = new DirectedGraphWithData<string, int>();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void AddNode_WithData_AddsNodeWithSpecifiedData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var nodeData = "test-node";

        // Act
        var node = graph.AddNode(nodeData);

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.True(graph.ContainsNode(node));
        Assert.Equal(nodeData, graph.GetNodeData(node));
    }

    [Fact]
    public void AddNode_WithoutData_AddsNodeWithDefaultData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();

        // Act
        var node = graph.AddNode();

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.Null(graph.GetNodeData(node)); // Default string is null
    }

    [Fact]
    public void AddEdge_WithEdgeData_AddsEdgeWithData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");
        var edgeData = 42;

        // Act
        graph.AddEdge(source, target, edgeData);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(source, target));
        Assert.Equal(edgeData, graph.GetEdgeData(source, target));
    }

    [Fact]
    public void AddEdge_WithoutEdgeData_AddsEdgeWithDefaultData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");

        // Act
        graph.AddEdge(source, target);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(source, target));
        Assert.Equal(0, graph.GetEdgeData(source, target)); // Default int is 0
    }

    [Fact]
    public void GetEdgeData_ExistingEdge_ReturnsCorrectData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, double>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");
        var edgeData = 3.14;
        graph.AddEdge(source, target, edgeData);

        // Act
        var retrievedData = graph.GetEdgeData(source, target);

        // Assert
        Assert.Equal(edgeData, retrievedData);
    }

    [Fact]
    public void GetEdgeData_NonExistentEdge_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeData(source, target));
    }

    [Fact]
    public void TryGetEdgeData_ExistingEdge_ReturnsTrueAndData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");
        var edgeData = 123;
        graph.AddEdge(source, target, edgeData);

        // Act
        var result = graph.TryGetEdgeData(source, target, out var retrievedData);

        // Assert
        Assert.True(result);
        Assert.Equal(edgeData, retrievedData);
    }

    [Fact]
    public void TryGetEdgeData_NonExistentEdge_ReturnsFalse()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");

        // Act
        var result = graph.TryGetEdgeData(source, target, out var data);

        // Assert
        Assert.False(result);
        Assert.Equal(0, data); // Default int
    }

    [Fact]
    public void SetNodeData_ExistingNode_UpdatesData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var node = graph.AddNode("initial");

        // Act
        graph.SetNodeData(node, "updated");

        // Assert
        Assert.Equal("updated", graph.GetNodeData(node));
    }

    [Fact]
    public void SetEdgeData_ExistingEdge_UpdatesData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");
        graph.AddEdge(source, target, 100);

        // Act
        graph.SetEdgeData(source, target, 200);

        // Assert
        Assert.Equal(200, graph.GetEdgeData(source, target));
    }

    [Fact]
    public void SetEdgeData_NonExistentEdge_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.SetEdgeData(source, target, 123));
    }

    [Fact]
    public void RemoveNode_RemovesNodeDataEdgeDataAndIncidentEdges()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var node3 = graph.AddNode("node3");
        
        graph.AddEdge(node1, node2, 12);
        graph.AddEdge(node2, node3, 23);
        graph.AddEdge(node3, node1, 31);

        // Act
        graph.RemoveNode(node2);

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.Equal(1, graph.EdgeCount);
        Assert.False(graph.ContainsNode(node2));
        
        // Node data should be gone
        Assert.Throws<InvalidOperationException>(() => graph.GetNodeData(node2));
        
        // Edge data should be gone
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeData(node1, node2));
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeData(node2, node3));
        
        // Remaining edge should still have its data
        Assert.Equal(31, graph.GetEdgeData(node3, node1));
    }

    [Fact]
    public void RemoveEdge_RemovesEdgeAndItsData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");
        graph.AddEdge(source, target, 42);

        // Act
        graph.RemoveEdge(source, target);

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(source, target));
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeData(source, target));
    }

    [Fact]
    public void Clear_RemovesAllNodesEdgesAndData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        graph.AddEdge(node1, node2, 123);

        // Act
        graph.Clear();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void WithEdge_FluentInterface_AddsEdgeWithDataAndReturnsGraph()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");

        // Act
        var result = graph.WithEdge(source, target, 999);

        // Assert
        Assert.Same(graph, result);
        Assert.True(graph.HasEdge(source, target));
        Assert.Equal(999, graph.GetEdgeData(source, target));
    }

    [Fact]
    public void WithNodeData_FluentInterface_SetsDataAndReturnsGraph()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var node = graph.AddNode("initial");

        // Act
        var result = graph.WithNodeData(node, "updated");

        // Assert
        Assert.Same(graph, result);
        Assert.Equal("updated", graph.GetNodeData(node));
    }

    [Fact]
    public void DirectedGraphBehavior_PreservedWithData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var node3 = graph.AddNode("node3");

        graph.AddEdge(node1, node2, 12);
        graph.AddEdge(node2, node3, 23);

        // Act & Assert - Directed graph properties
        Assert.False(graph.HasEdge(node2, node1)); // No reverse edge
        Assert.False(graph.HasEdge(node3, node2)); // No reverse edge

        Assert.Single(graph.OutNeighbors(node1));
        Assert.Contains(node2, graph.OutNeighbors(node1));

        Assert.Single(graph.InNeighbors(node3));
        Assert.Contains(node2, graph.InNeighbors(node3));
    }

    [Fact]
    public void CustomDataTypes_WorkCorrectly()
    {
        // Arrange
        var graph = new DirectedGraphWithData<Person, Relationship>();
        var alice = new Person("Alice", 30);
        var bob = new Person("Bob", 25);
        var friendship = new Relationship("friend", DateTime.Now);

        // Act
        var aliceNode = graph.AddNode(alice);
        var bobNode = graph.AddNode(bob);
        graph.AddEdge(aliceNode, bobNode, friendship);

        // Assert
        Assert.Equal(alice, graph.GetNodeData(aliceNode));
        Assert.Equal(bob, graph.GetNodeData(bobNode));
        Assert.Equal(friendship, graph.GetEdgeData(aliceNode, bobNode));
    }

    [Fact]
    public void AddEdge_ExistingEdge_UpdatesEdgeData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, int>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");
        graph.AddEdge(source, target, 100);

        // Act
        graph.AddEdge(source, target, 200); // Update existing edge

        // Assert
        Assert.Equal(1, graph.EdgeCount); // Still only one edge
        Assert.Equal(200, graph.GetEdgeData(source, target));
    }

    [Fact]
    public void MultipleEdges_WithDifferentData()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string, string>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var node3 = graph.AddNode("node3");

        // Act
        graph.AddEdge(node1, node2, "edge1-2");
        graph.AddEdge(node1, node3, "edge1-3");
        graph.AddEdge(node2, node3, "edge2-3");

        // Assert
        Assert.Equal(3, graph.EdgeCount);
        Assert.Equal("edge1-2", graph.GetEdgeData(node1, node2));
        Assert.Equal("edge1-3", graph.GetEdgeData(node1, node3));
        Assert.Equal("edge2-3", graph.GetEdgeData(node2, node3));
    }

    [Fact]
    public void NullableDataTypes_HandledCorrectly()
    {
        // Arrange
        var graph = new DirectedGraphWithData<string?, int?>();

        // Act
        var node1 = graph.AddNode(null);
        var node2 = graph.AddNode("not-null");
        graph.AddEdge(node1, node2, null);

        // Assert
        Assert.Null(graph.GetNodeData(node1));
        Assert.Equal("not-null", graph.GetNodeData(node2));
        Assert.Null(graph.GetEdgeData(node1, node2));
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

        public override bool Equals(object? obj)
        {
            return Equals(obj as Person);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age);
        }
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

        public override bool Equals(object? obj)
        {
            return Equals(obj as Relationship);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Since);
        }
    }
}