using GraphLib.Core;
using GraphLib.Implementations;
using Xunit;

namespace GraphLib.Tests.Implementations;

public class UndirectedGraphWithDataTests
{
    [Fact]
    public void Constructor_CreatesEmptyGraph()
    {
        // Act
        var graph = new UndirectedGraphWithData<string, int>();

        // Assert
        Assert.Equal(0, graph.NodeCount);
        Assert.Equal(0, graph.EdgeCount);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public void AddEdge_WithEdgeData_CreatesSymmetricEdgeWithSameData()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var edgeData = 42;

        // Act
        graph.AddEdge(node1, node2, edgeData);

        // Assert
        Assert.Equal(1, graph.EdgeCount);
        Assert.True(graph.HasEdge(node1, node2));
        Assert.True(graph.HasEdge(node2, node1)); // Undirected - symmetric
        Assert.Equal(edgeData, graph.GetEdgeData(node1, node2));
        Assert.Equal(edgeData, graph.GetEdgeData(node2, node1)); // Same data both ways
    }

    [Fact]
    public void GetEdgeData_BothDirections_ReturnsSameData()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, string>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var edgeData = "connection-data";
        graph.AddEdge(node1, node2, edgeData);

        // Act & Assert
        Assert.Equal(edgeData, graph.GetEdgeData(node1, node2));
        Assert.Equal(edgeData, graph.GetEdgeData(node2, node1));
    }

    [Fact]
    public void SetEdgeData_UpdatesBothDirections()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        graph.AddEdge(node1, node2, 100);

        // Act
        graph.SetEdgeData(node1, node2, 200);

        // Assert
        Assert.Equal(200, graph.GetEdgeData(node1, node2));
        Assert.Equal(200, graph.GetEdgeData(node2, node1));
    }

    [Fact]
    public void SetEdgeData_ReverseDirection_UpdatesEdge()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        graph.AddEdge(node1, node2, 100);

        // Act
        graph.SetEdgeData(node2, node1, 300); // Set in reverse direction

        // Assert
        Assert.Equal(300, graph.GetEdgeData(node1, node2));
        Assert.Equal(300, graph.GetEdgeData(node2, node1));
    }

    [Fact]
    public void TryGetEdgeData_BothDirections_Work()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, string>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var edgeData = "test-data";
        graph.AddEdge(node1, node2, edgeData);

        // Act & Assert
        Assert.True(graph.TryGetEdgeData(node1, node2, out var data1));
        Assert.True(graph.TryGetEdgeData(node2, node1, out var data2));
        Assert.Equal(edgeData, data1);
        Assert.Equal(edgeData, data2);
    }

    [Fact]
    public void RemoveEdge_RemovesSymmetricEdgeAndData()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        graph.AddEdge(node1, node2, 42);

        // Act
        graph.RemoveEdge(node1, node2);

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node1));
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeData(node1, node2));
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeData(node2, node1));
    }

    [Fact]
    public void RemoveEdge_ReverseDirection_RemovesEdge()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        graph.AddEdge(node1, node2, 42);

        // Act
        graph.RemoveEdge(node2, node1); // Remove in reverse direction

        // Assert
        Assert.Equal(0, graph.EdgeCount);
        Assert.False(graph.HasEdge(node1, node2));
        Assert.False(graph.HasEdge(node2, node1));
    }

    [Fact]
    public void RemoveNode_RemovesNodeDataEdgeDataAndIncidentEdges()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var node3 = graph.AddNode("node3");
        
        graph.AddEdge(node1, node2, 12);
        graph.AddEdge(node2, node3, 23);
        graph.AddEdge(node1, node3, 13);

        // Act
        graph.RemoveNode(node2);

        // Assert
        Assert.Equal(2, graph.NodeCount);
        Assert.Equal(1, graph.EdgeCount); // Only node1-node3 should remain
        Assert.False(graph.ContainsNode(node2));
        
        // Node data should be gone
        Assert.Throws<InvalidOperationException>(() => graph.GetNodeData(node2));
        
        // Edge data involving node2 should be gone
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeData(node1, node2));
        Assert.Throws<InvalidOperationException>(() => graph.GetEdgeData(node2, node3));
        
        // Remaining edge should still have its data
        Assert.Equal(13, graph.GetEdgeData(node1, node3));
        Assert.Equal(13, graph.GetEdgeData(node3, node1));
    }

    [Fact]
    public void AddEdge_ExistingEdge_UpdatesEdgeData()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        graph.AddEdge(node1, node2, 100);

        // Act
        graph.AddEdge(node1, node2, 200); // Update existing edge

        // Assert
        Assert.Equal(1, graph.EdgeCount); // Still only one edge
        Assert.Equal(200, graph.GetEdgeData(node1, node2));
        Assert.Equal(200, graph.GetEdgeData(node2, node1));
    }

    [Fact]
    public void AddEdge_ReverseDirection_UpdatesExistingEdge()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        graph.AddEdge(node1, node2, 100);

        // Act
        graph.AddEdge(node2, node1, 300); // Update in reverse direction

        // Assert
        Assert.Equal(1, graph.EdgeCount); // Still only one edge
        Assert.Equal(300, graph.GetEdgeData(node1, node2));
        Assert.Equal(300, graph.GetEdgeData(node2, node1));
    }

    [Fact]
    public void Neighbors_ReturnsCorrectNeighbors()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var center = graph.AddNode("center");
        var neighbor1 = graph.AddNode("neighbor1");
        var neighbor2 = graph.AddNode("neighbor2");
        var neighbor3 = graph.AddNode("neighbor3");
        
        graph.AddEdge(center, neighbor1, 1);
        graph.AddEdge(center, neighbor2, 2);
        graph.AddEdge(neighbor3, center, 3); // Order shouldn't matter

        // Act
        var neighbors = graph.Neighbors(center).ToList();

        // Assert
        Assert.Equal(3, neighbors.Count);
        Assert.Contains(neighbor1, neighbors);
        Assert.Contains(neighbor2, neighbors);
        Assert.Contains(neighbor3, neighbors);
    }

    [Fact]
    public void Degree_CountsIncidentEdges()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var center = graph.AddNode("center");
        var neighbor1 = graph.AddNode("neighbor1");
        var neighbor2 = graph.AddNode("neighbor2");
        var neighbor3 = graph.AddNode("neighbor3");
        
        graph.AddEdge(center, neighbor1, 1);
        graph.AddEdge(center, neighbor2, 2);
        graph.AddEdge(center, neighbor3, 3);

        // Act
        var degree = graph.Degree(center);

        // Assert
        Assert.Equal(3, degree);
    }

    [Fact]
    public void SelfLoop_AllowedWithData()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, string>();
        var node = graph.AddNode("self-referential");

        // Act
        graph.AddEdge(node, node, "self-loop-data");

        // Assert
        Assert.True(graph.HasEdge(node, node));
        Assert.Equal("self-loop-data", graph.GetEdgeData(node, node));
        Assert.Equal(1, graph.Degree(node)); // Self-loop counts as 1 in undirected graph
    }

    [Fact]
    public void WithEdge_FluentInterface_AddsEdgeWithDataAndReturnsGraph()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");

        // Act
        var result = graph.WithEdge(node1, node2, 999);

        // Assert
        Assert.Same(graph, result);
        Assert.True(graph.HasEdge(node1, node2));
        Assert.Equal(999, graph.GetEdgeData(node1, node2));
    }

    [Fact]
    public void WithNodeData_FluentInterface_SetsDataAndReturnsGraph()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
        var node = graph.AddNode("initial");

        // Act
        var result = graph.WithNodeData(node, "updated");

        // Assert
        Assert.Same(graph, result);
        Assert.Equal("updated", graph.GetNodeData(node));
    }

    [Fact]
    public void Triangle_CorrectDegreesAndEdgeData()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, string>();
        var node1 = graph.AddNode("node1");
        var node2 = graph.AddNode("node2");
        var node3 = graph.AddNode("node3");
        
        graph.AddEdge(node1, node2, "edge1-2");
        graph.AddEdge(node2, node3, "edge2-3");
        graph.AddEdge(node3, node1, "edge3-1");

        // Act & Assert
        Assert.Equal(3, graph.EdgeCount);
        Assert.Equal(2, graph.Degree(node1));
        Assert.Equal(2, graph.Degree(node2));
        Assert.Equal(2, graph.Degree(node3));

        // Check edge data from both directions
        Assert.Equal("edge1-2", graph.GetEdgeData(node1, node2));
        Assert.Equal("edge1-2", graph.GetEdgeData(node2, node1));
        Assert.Equal("edge2-3", graph.GetEdgeData(node2, node3));
        Assert.Equal("edge2-3", graph.GetEdgeData(node3, node2));
        Assert.Equal("edge3-1", graph.GetEdgeData(node3, node1));
        Assert.Equal("edge3-1", graph.GetEdgeData(node1, node3));
    }

    [Fact]
    public void CustomDataTypes_WorkCorrectly()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<Location, Road>();
        var cityA = new Location("City A", 0, 0);
        var cityB = new Location("City B", 10, 10);
        var highway = new Road("Highway 101", 150);

        // Act
        var nodeA = graph.AddNode(cityA);
        var nodeB = graph.AddNode(cityB);
        graph.AddEdge(nodeA, nodeB, highway);

        // Assert
        Assert.Equal(cityA, graph.GetNodeData(nodeA));
        Assert.Equal(cityB, graph.GetNodeData(nodeB));
        Assert.Equal(highway, graph.GetEdgeData(nodeA, nodeB));
        Assert.Equal(highway, graph.GetEdgeData(nodeB, nodeA));
    }

    [Fact]
    public void Clear_RemovesAllNodesEdgesAndData()
    {
        // Arrange
        var graph = new UndirectedGraphWithData<string, int>();
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

    private class Location : IEquatable<Location>
    {
        public string Name { get; }
        public double X { get; }
        public double Y { get; }

        public Location(string name, double x, double y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public bool Equals(Location? other)
        {
            return other != null && Name == other.Name && X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Location);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, X, Y);
        }
    }

    private class Road : IEquatable<Road>
    {
        public string Name { get; }
        public double DistanceKm { get; }

        public Road(string name, double distanceKm)
        {
            Name = name;
            DistanceKm = distanceKm;
        }

        public bool Equals(Road? other)
        {
            return other != null && Name == other.Name && DistanceKm == other.DistanceKm;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Road);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, DistanceKm);
        }
    }
}