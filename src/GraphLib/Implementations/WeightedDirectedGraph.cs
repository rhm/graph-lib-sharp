using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class WeightedDirectedGraph<TWeight> : IMutableDirectedGraph, IWeightedGraph<TWeight>
{
    private readonly Dictionary<int, List<(int target, TWeight weight)>> _adjacencyList;
    private readonly HashSet<int> _nodes;
    private int _nextNodeId;
    private int _edgeCount;

    public WeightedDirectedGraph()
    {
        _adjacencyList = new Dictionary<int, List<(int, TWeight)>>();
        _nodes = new HashSet<int>();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public WeightedDirectedGraph(int initialCapacity)
    {
        _adjacencyList = new Dictionary<int, List<(int, TWeight)>>(initialCapacity);
        _nodes = new HashSet<int>(initialCapacity);
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public int NodeCount => _nodes.Count;
    public int EdgeCount => _edgeCount;
    public IEnumerable<NodeId> Nodes => _nodes.Select(id => new NodeId(id));

    public bool ContainsNode(NodeId node)
    {
        return _nodes.Contains(node.Value);
    }

    public int Degree(NodeId node)
    {
        return OutDegree(node) + InDegree(node);
    }

    public IEnumerable<NodeId> OutNeighbors(NodeId node)
    {
        if (!_adjacencyList.TryGetValue(node.Value, out var neighbors))
            return Enumerable.Empty<NodeId>();
        
        return neighbors.Select(n => new NodeId(n.target));
    }

    public IEnumerable<NodeId> InNeighbors(NodeId node)
    {
        return _adjacencyList
            .Where(kvp => kvp.Value.Any(neighbor => neighbor.target == node.Value))
            .Select(kvp => new NodeId(kvp.Key));
    }

    public int OutDegree(NodeId node)
    {
        return _adjacencyList.TryGetValue(node.Value, out var neighbors) ? neighbors.Count : 0;
    }

    public int InDegree(NodeId node)
    {
        return _adjacencyList.Values.Sum(neighbors => neighbors.Count(n => n.target == node.Value));
    }

    public bool HasEdge(NodeId source, NodeId target)
    {
        return _adjacencyList.TryGetValue(source.Value, out var neighbors) &&
               neighbors.Any(n => n.target == target.Value);
    }

    public TWeight GetEdgeWeight(NodeId source, NodeId target)
    {
        if (!_adjacencyList.TryGetValue(source.Value, out var neighbors))
            throw new InvalidOperationException($"Node {source.Value} not found");

        var neighbor = neighbors.FirstOrDefault(n => n.target == target.Value);
        if (neighbor.Equals(default))
            throw new InvalidOperationException($"Edge from {source.Value} to {target.Value} not found");

        return neighbor.weight;
    }

    public bool TryGetEdgeWeight(NodeId source, NodeId target, out TWeight weight)
    {
        weight = default!;
        
        if (!_adjacencyList.TryGetValue(source.Value, out var neighbors))
            return false;

        var neighbor = neighbors.FirstOrDefault(n => n.target == target.Value);
        if (neighbor.Equals(default))
            return false;

        weight = neighbor.weight;
        return true;
    }

    public NodeId AddNode()
    {
        var nodeId = new NodeId(_nextNodeId++);
        _nodes.Add(nodeId.Value);
        _adjacencyList[nodeId.Value] = new List<(int, TWeight)>();
        return nodeId;
    }

    public void RemoveNode(NodeId node)
    {
        if (!_nodes.Contains(node.Value))
            return;

        if (_adjacencyList.TryGetValue(node.Value, out var outEdges))
        {
            _edgeCount -= outEdges.Count;
        }

        foreach (var kvp in _adjacencyList)
        {
            var edgesToRemove = kvp.Value.Where(n => n.target == node.Value).ToList();
            foreach (var edge in edgesToRemove)
            {
                kvp.Value.Remove(edge);
                _edgeCount--;
            }
        }

        _adjacencyList.Remove(node.Value);
        _nodes.Remove(node.Value);
    }

    public void Clear()
    {
        _adjacencyList.Clear();
        _nodes.Clear();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public void AddEdge(NodeId source, NodeId target)
    {
        throw new InvalidOperationException("Use AddEdge(source, target, weight) for weighted graphs");
    }

    public void AddEdge(NodeId source, NodeId target, TWeight weight)
    {
        if (!_nodes.Contains(source.Value))
            throw new InvalidOperationException($"Source node {source.Value} not found");
        if (!_nodes.Contains(target.Value))
            throw new InvalidOperationException($"Target node {target.Value} not found");

        var neighbors = _adjacencyList[source.Value];
        var existingEdgeIndex = neighbors.FindIndex(n => n.target == target.Value);
        
        if (existingEdgeIndex >= 0)
        {
            neighbors[existingEdgeIndex] = (target.Value, weight);
        }
        else
        {
            neighbors.Add((target.Value, weight));
            _edgeCount++;
        }
    }

    public void RemoveEdge(NodeId source, NodeId target)
    {
        if (!_adjacencyList.TryGetValue(source.Value, out var neighbors))
            return;

        var removed = neighbors.RemoveAll(n => n.target == target.Value);
        _edgeCount -= removed;
    }

    public void UpdateEdgeWeight(NodeId source, NodeId target, TWeight weight)
    {
        if (!_adjacencyList.TryGetValue(source.Value, out var neighbors))
            throw new InvalidOperationException($"Source node {source.Value} not found");

        var existingEdgeIndex = neighbors.FindIndex(n => n.target == target.Value);
        if (existingEdgeIndex < 0)
            throw new InvalidOperationException($"Edge from {source.Value} to {target.Value} not found");

        neighbors[existingEdgeIndex] = (target.Value, weight);
    }

    public WeightedDirectedGraph<TWeight> WithEdge(NodeId source, NodeId target, TWeight weight)
    {
        if (!ContainsNode(source))
            AddNode();
        if (!ContainsNode(target))
            AddNode();
        
        AddEdge(source, target, weight);
        return this;
    }

    public WeightedDirectedGraph<TWeight> WithEdges(IEnumerable<WeightedEdge<TWeight>> edges)
    {
        foreach (var edge in edges)
        {
            WithEdge(edge.Source, edge.Target, edge.Weight);
        }
        return this;
    }
}