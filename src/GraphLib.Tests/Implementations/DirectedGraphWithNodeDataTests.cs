using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class DirectedGraphWithNodeDataTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        // Act
        var graph = new DirectedGraphWithNodeData<string>();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void AddNode_WithoutData_AddsNodeWithDefaultData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();

        // Act
        var node = graph.AddNode();

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.True(graph.ContainsNode(node));
        Assert.Null(graph.GetNodeData(node)); // Default string is null
    }

    [Fact]
    public void AddNode_WithData_AddsNodeWithSpecifiedData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var data = "test-data";

        // Act
        var node = graph.AddNode(data);

        // Assert
        Assert.Equal(1, graph.NodeCount);
        Assert.True(graph.ContainsNode(node));
        Assert.Equal(data, graph.GetNodeData(node));
    }

    [Fact]
    public void GetNodeData_ExistingNode_ReturnsCorrectData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<int>();
        var data = 42;
        var node = graph.AddNode(data);

        // Act
        var retrievedData = graph.GetNodeData(node);

        // Assert
        Assert.Equal(data, retrievedData);
    }

    [Fact]
    public void GetNodeData_NonExistentNode_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var fakeNode = new NodeId(999);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.GetNodeData(fakeNode));
    }

    [Fact]
    public void TryGetNodeData_ExistingNode_ReturnsTrueAndData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var data = "test-data";
        var node = graph.AddNode(data);

        // Act
        var result = graph.TryGetNodeData(node, out var retrievedData);

        // Assert
        Assert.True(result);
        Assert.Equal(data, retrievedData);
    }

    [Fact]
    public void TryGetNodeData_NonExistentNode_ReturnsFalse()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var fakeNode = new NodeId(999);

        // Act
        var result = graph.TryGetNodeData(fakeNode, out var data);

        // Assert
        Assert.False(result);
        Assert.Null(data);
    }

    [Fact]
    public void SetNodeData_ExistingNode_UpdatesData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var node = graph.AddNode("initial-data");

        // Act
        graph.SetNodeData(node, "updated-data");

        // Assert
        Assert.Equal("updated-data", graph.GetNodeData(node));
    }

    [Fact]
    public void SetNodeData_NonExistentNode_ThrowsInvalidOperationException()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var fakeNode = new NodeId(999);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => graph.SetNodeData(fakeNode, "data"));
    }

    [Fact]
    public void AddEdge_ValidNodes_AddsEdge()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var source = graph.AddNode("source-data");
        var target = graph.AddNode("target-data");

        // Act
        graph.AddEdge(source, target);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(source, target));
        Assert.False(graph.HasEdge(target, source)); // Directed graph
    }

    [Fact]
    public void RemoveNode_RemovesNodeDataAndIncidentEdges()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var node1 = graph.AddNode("data1");
        var node2 = graph.AddNode("data2");
        var node3 = graph.AddNode("data3");
        
        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);
        graph.AddEdge(node3, node1);

        // Act
        graph.RemoveNode(node2);

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.Equal(1, graph.EdgeCount);
        Assert.False(graph.ContainsNode(node2));
        Assert.Throws<InvalidOperationException>(() => graph.GetNodeData(node2));
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node3));
        Assert.True(graph.HasEdge(node3, node1));
    }

    [Fact]
    public void Clear_RemovesAllNodesEdgesAndData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var node1 = graph.AddNode("data1");
        var node2 = graph.AddNode("data2");
        graph.AddEdge(node1, node2);

        // Act
        graph.Clear();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void WithEdge_FluentInterface_AddsEdgeAndReturnsGraph()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var source = graph.AddNode("source");
        var target = graph.AddNode("target");

        // Act
        var result = graph.WithEdge(source, target);

        // Assert
        Assert.Same(graph, result);
        Assert.True(graph.HasEdge(source, target));
    }

    [Fact]
    public void WithNodeData_FluentInterface_SetsDataAndReturnsGraph()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var node = graph.AddNode("initial");

        // Act
        var result = graph.WithNodeData(node, "updated");

        // Assert
        Assert.Same(graph, result);
        Assert.Equal("updated", graph.GetNodeData(node));
    }

    [Fact]
    public void WithNodeData_NonExistentNode_AddsNodeWithData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var node = new NodeId(0); // Assuming this will be the first ID

        // Act
        var result = graph.WithNodeData(node, "new-data");

        // Assert
        Assert.Same(graph, result);
        Assert.Equal(1, graph.NodeCount);
        Assert.Equal("new-data", graph.GetNodeData(node));
    }

    [Fact]
    public void WithEdges_FluentInterface_AddsMultipleEdges()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var node1 = graph.AddNode("data1");
        var node2 = graph.AddNode("data2");
        var node3 = graph.AddNode("data3");

        var edges = new[]
        {
            new Edge(node1, node2),
            new Edge(node2, node3),
            new Edge(node3, node1)
        };

        // Act
        var result = graph.WithEdges(edges);

        // Assert
        Assert.Same(graph, result);
        Assert.Equal(3, graph.EdgeCount);
        Assert.True(graph.HasEdge(node1, node2));
        Assert.True(graph.HasEdge(node2, node3));
        Assert.True(graph.HasEdge(node3, node1));
    }

    [Fact]
    public void CustomDataType_WorksCorrectly()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<Person>();
        var person1 = new Person("Alice", 30);
        var person2 = new Person("Bob", 25);

        // Act
        var node1 = graph.AddNode(person1);
        var node2 = graph.AddNode(person2);
        graph.AddEdge(node1, node2);

        // Assert
        Assert.Equal(person1, graph.GetNodeData(node1));
        Assert.Equal(person2, graph.GetNodeData(node2));
        Assert.True(graph.HasEdge(node1, node2));
    }

    [Fact]
    public void NullableNodeData_HandledCorrectly()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string?>();

        // Act
        var node1 = graph.AddNode(null);
        var node2 = graph.AddNode("not-null");

        // Assert
        Assert.Null(graph.GetNodeData(node1));
        Assert.Equal("not-null", graph.GetNodeData(node2));
    }

    [Fact]
    public void DirectedGraphBehavior_PreservedWithNodeData()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var node3 = graph.AddNode("node3");

        graph.AddEdge(node1, node2);
        graph.AddEdge(node2, node3);

        // Act & Assert
        Assert.Single(graph.OutNeighbors(node1));
        Assert.Contains(node2, graph.OutNeighbors(node1));

        Assert.Single(graph.InNeighbors(node3));
        Assert.Contains(node2, graph.InNeighbors(node3));

        Assert.Equal(1, graph.OutDegree(node1));
        Assert.Equal(0, graph.InDegree(node1));
        Assert.Equal(1, graph.OutDegree(node2));
        Assert.Equal(1, graph.InDegree(node2));
        Assert.Equal(0, graph.OutDegree(node3));
        Assert.Equal(1, graph.InDegree(node3));
    }

    [Fact]
    public void MultipleNodesWithSameData_AllowedAndDistinct()
    {
        // Arrange
        var graph = new DirectedGraphWithNodeData<string>();
        var sharedData = "shared";

        // Act
        var node1 = graph.AddNode(sharedData);
        var node2 = graph.AddNode(sharedData);

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.NotEqual(node1, node2);
        Assert.Equal(sharedData, graph.GetNodeData(node1));
        Assert.Equal(sharedData, graph.GetNodeData(node2));
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
}