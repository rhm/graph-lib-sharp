using System.Collections;
using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

/// <summary>
/// A simple directed graph implementation using adjacency lists.
/// </summary>
public class DirectedGraph : IMutableDirectedGraph
{
    private readonly Dictionary<NodeId, HashSet<NodeId>> _outEdges;
    private readonly Dictionary<NodeId, HashSet<NodeId>> _inEdges;
    private int _nextNodeId;
    private int _edgeCount;

    /// <summary>
    /// Initializes a new instance of the DirectedGraph class.
    /// </summary>
    public DirectedGraph()
    {
        _outEdges = new Dictionary<NodeId, HashSet<NodeId>>();
        _inEdges = new Dictionary<NodeId, HashSet<NodeId>>();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    /// <summary>
    /// Initializes a new instance of the DirectedGraph class with specified initial capacity.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity for node storage.</param>
    public DirectedGraph(int initialCapacity) : this()
    {
        if (initialCapacity < 0)
            throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity cannot be negative.");
            
        _outEdges = new Dictionary<NodeId, HashSet<NodeId>>(initialCapacity);
        _inEdges = new Dictionary<NodeId, HashSet<NodeId>>(initialCapacity);
    }

    /// <inheritdoc />
    public int NodeCount => _outEdges.Count;

    /// <inheritdoc />
    public int EdgeCount => _edgeCount;

    /// <inheritdoc />
    public IEnumerable<NodeId> Nodes => _outEdges.Keys;

    /// <inheritdoc />
    public bool ContainsNode(NodeId node) => _outEdges.ContainsKey(node);

    /// <inheritdoc />
    public int Degree(NodeId node)
    {
        if (!ContainsNode(node))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));
            
        return OutDegree(node) + InDegree(node);
    }

    /// <inheritdoc />
    public IEnumerable<NodeId> OutNeighbors(NodeId node)
    {
        if (!_outEdges.TryGetValue(node, out var neighbors))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));
            
        return neighbors;
    }

    /// <inheritdoc />
    public IEnumerable<NodeId> InNeighbors(NodeId node)
    {
        if (!_inEdges.TryGetValue(node, out var neighbors))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));
            
        return neighbors;
    }

    /// <inheritdoc />
    public int OutDegree(NodeId node)
    {
        if (!_outEdges.TryGetValue(node, out var neighbors))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));
            
        return neighbors.Count;
    }

    /// <inheritdoc />
    public int InDegree(NodeId node)
    {
        if (!_inEdges.TryGetValue(node, out var neighbors))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));
            
        return neighbors.Count;
    }

    /// <inheritdoc />
    public bool HasEdge(NodeId source, NodeId target)
    {
        return _outEdges.TryGetValue(source, out var neighbors) && neighbors.Contains(target);
    }

    /// <inheritdoc />
    public NodeId AddNode()
    {
        var node = new NodeId(_nextNodeId++);
        _outEdges[node] = new HashSet<NodeId>();
        _inEdges[node] = new HashSet<NodeId>();
        return node;
    }

    /// <inheritdoc />
    public void RemoveNode(NodeId node)
    {
        if (!ContainsNode(node))
            throw new ArgumentException($"Node {node} does not exist in the graph.", nameof(node));

        // Remove all outgoing edges
        if (_outEdges.TryGetValue(node, out var outNeighbors))
        {
            foreach (var neighbor in outNeighbors)
            {
                _inEdges[neighbor].Remove(node);
                _edgeCount--;
            }
        }

        // Remove all incoming edges
        if (_inEdges.TryGetValue(node, out var inNeighbors))
        {
            foreach (var neighbor in inNeighbors)
            {
                _outEdges[neighbor].Remove(node);
                _edgeCount--;
            }
        }

        // Remove the node
        _outEdges.Remove(node);
        _inEdges.Remove(node);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _outEdges.Clear();
        _inEdges.Clear();
        _edgeCount = 0;
        _nextNodeId = 0;
    }

    /// <inheritdoc />
    public void AddEdge(NodeId source, NodeId target)
    {
        if (!ContainsNode(source))
            throw new ArgumentException($"Source node {source} does not exist in the graph.", nameof(source));
        if (!ContainsNode(target))
            throw new ArgumentException($"Target node {target} does not exist in the graph.", nameof(target));

        if (_outEdges[source].Add(target))
        {
            _inEdges[target].Add(source);
            _edgeCount++;
        }
    }

    /// <inheritdoc />
    public void RemoveEdge(NodeId source, NodeId target)
    {
        if (!ContainsNode(source))
            throw new ArgumentException($"Source node {source} does not exist in the graph.", nameof(source));
        if (!ContainsNode(target))
            throw new ArgumentException($"Target node {target} does not exist in the graph.", nameof(target));

        if (_outEdges[source].Remove(target))
        {
            _inEdges[target].Remove(source);
            _edgeCount--;
        }
        else
        {
            throw new ArgumentException($"Edge from {source} to {target} does not exist.");
        }
    }

    /// <summary>
    /// Adds an edge to the graph and returns the graph for method chaining.
    /// </summary>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <returns>This graph instance.</returns>
    public DirectedGraph WithEdge(NodeId source, NodeId target)
    {
        // Ensure nodes exist
        while (_nextNodeId <= Math.Max(source.Value, target.Value))
        {
            AddNode();
        }
        
        AddEdge(source, target);
        return this;
    }

    /// <summary>
    /// Adds multiple edges to the graph and returns the graph for method chaining.
    /// </summary>
    /// <param name="edges">The edges to add.</param>
    /// <returns>This graph instance.</returns>
    public DirectedGraph WithEdges(IEnumerable<Edge> edges)
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
    /// <returns>An enumerable collection of all edges.</returns>
    public IEnumerable<Edge> GetEdges()
    {
        foreach (var (source, targets) in _outEdges)
        {
            foreach (var target in targets)
            {
                yield return new Edge(source, target);
            }
        }
    }
}