using System.Collections;
using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

/// <summary>
/// A simple undirected graph implementation using adjacency lists.
/// </summary>
public class UndirectedGraph : IMutableUndirectedGraph
{
    private readonly Dictionary<NodeId, HashSet<NodeId>> _adjacencyList;
    private int _nextNodeId;
    private int _edgeCount;

    /// <summary>
    /// Initializes a new instance of the UndirectedGraph class.
    /// </summary>
    public UndirectedGraph()
    {
        _adjacencyList = new Dictionary<NodeId, HashSet<NodeId>>();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    /// <summary>
    /// Initializes a new instance of the UndirectedGraph class with specified initial capacity.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity for node storage.</param>
    public UndirectedGraph(int initialCapacity) : this()
    {
        if (initialCapacity < 0)
            throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity cannot be negative.");
            
        _adjacencyList = new Dictionary<NodeId, HashSet<NodeId>>(initialCapacity);
    }

    /// <inheritdoc />
    public int NodeCount => _adjacencyList.Count;

    /// <inheritdoc />
    public int EdgeCount => _edgeCount;

    /// <inheritdoc />
    public IEnumerable<NodeId> Nodes => _adjacencyList.Keys;

    /// <inheritdoc />
    public bool ContainsNode(NodeId node) => _adjacencyList.ContainsKey(node);

    /// <inheritdoc />
    public int Degree(NodeId node)
    {
        if (!_adjacencyList.TryGetValue(node, out var neighbors))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));
            
        return neighbors.Count;
    }

    /// <inheritdoc />
    public IEnumerable<NodeId> Neighbors(NodeId node)
    {
        if (!_adjacencyList.TryGetValue(node, out var neighbors))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));
            
        return neighbors;
    }

    /// <inheritdoc />
    public bool HasEdge(NodeId node1, NodeId node2)
    {
        return _adjacencyList.TryGetValue(node1, out var neighbors) && neighbors.Contains(node2);
    }

    /// <inheritdoc />
    public NodeId AddNode()
    {
        var node = new NodeId(_nextNodeId++);
        _adjacencyList[node] = new HashSet<NodeId>();
        return node;
    }

    /// <inheritdoc />
    public void RemoveNode(NodeId node)
    {
        if (!ContainsNode(node))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));

        // Remove all edges incident to this node
        if (_adjacencyList.TryGetValue(node, out var neighbors))
        {
            foreach (var neighbor in neighbors)
            {
                _adjacencyList[neighbor].Remove(node);
                _edgeCount--;
            }
        }

        // Remove the node
        _adjacencyList.Remove(node);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _adjacencyList.Clear();
        _edgeCount = 0;
        _nextNodeId = 0;
    }

    /// <inheritdoc />
    public void AddEdge(NodeId node1, NodeId node2)
    {
        if (!ContainsNode(node1))
            throw new ArgumentException($"Node {node1} does not exist in the graph.", nameof(node1));
        if (!ContainsNode(node2))
            throw new ArgumentException($"Node {node2} does not exist in the graph.", nameof(node2));

        // For undirected graphs, add edge in both directions
        if (_adjacencyList[node1].Add(node2))
        {
            _adjacencyList[node2].Add(node1);
            _edgeCount++;
        }
    }

    /// <inheritdoc />
    public void RemoveEdge(NodeId node1, NodeId node2)
    {
        if (!ContainsNode(node1))
            throw new ArgumentException($"Node {node1} does not exist in the graph.", nameof(node1));
        if (!ContainsNode(node2))
            throw new ArgumentException($"Node {node2} does not exist in the graph.", nameof(node2));

        if (_adjacencyList[node1].Remove(node2))
        {
            _adjacencyList[node2].Remove(node1);
            _edgeCount--;
        }
        else
        {
            throw new ArgumentException($"Edge between {node1} and {node2} does not exist.");
        }
    }

    /// <summary>
    /// Adds an edge to the graph and returns the graph for method chaining.
    /// </summary>
    /// <param name="node1">The first node.</param>
    /// <param name="node2">The second node.</param>
    /// <returns>This graph instance.</returns>
    public UndirectedGraph WithEdge(NodeId node1, NodeId node2)
    {
        // Ensure nodes exist
        while (_nextNodeId <= Math.Max(node1.Value, node2.Value))
        {
            AddNode();
        }
        
        AddEdge(node1, node2);
        return this;
    }

    /// <summary>
    /// Adds multiple edges to the graph and returns the graph for method chaining.
    /// </summary>
    /// <param name="edges">The edges to add.</param>
    /// <returns>This graph instance.</returns>
    public UndirectedGraph WithEdges(IEnumerable<Edge> edges)
    {
        foreach (var edge in edges)
        {
            WithEdge(edge.Source, edge.Target);
        }
        return this;
    }

    /// <summary>
    /// Gets all edges in the graph.
    /// </summary>
    /// <returns>An enumerable collection of all edges (each edge appears once).</returns>
    public IEnumerable<Edge> GetEdges()
    {
        var visitedPairs = new HashSet<(NodeId, NodeId)>();
        
        foreach (var (node, neighbors) in _adjacencyList)
        {
            foreach (var neighbor in neighbors)
            {
                var edge = node.Value < neighbor.Value 
                    ? (node, neighbor) 
                    : (neighbor, node);
                    
                if (visitedPairs.Add(edge))
                {
                    yield return new Edge(edge.Item1, edge.Item2);
                }
            }
        }
    }
}