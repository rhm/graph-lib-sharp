using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class WeightedUndirectedGraph<TWeight> : IMutableUndirectedGraph, IWeightedGraph<TWeight>
{
    private readonly Dictionary<int, List<(int neighbor, TWeight weight)>> _adjacencyList;
    private readonly HashSet<int> _nodes;
    private int _nextNodeId;
    private int _edgeCount;

    public WeightedUndirectedGraph()
    {
        _adjacencyList = new Dictionary<int, List<(int, TWeight)>>();
        _nodes = new HashSet<int>();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public WeightedUndirectedGraph(int initialCapacity)
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
        return _adjacencyList.TryGetValue(node.Value, out var neighbors) ? neighbors.Count : 0;
    }

    public IEnumerable<NodeId> Neighbors(NodeId node)
    {
        if (!_adjacencyList.TryGetValue(node.Value, out var neighbors))
            return Enumerable.Empty<NodeId>();
        
        return neighbors.Select(n => new NodeId(n.neighbor));
    }

    public bool HasEdge(NodeId node1, NodeId node2)
    {
        return _adjacencyList.TryGetValue(node1.Value, out var neighbors) &&
               neighbors.Any(n => n.neighbor == node2.Value);
    }

    public TWeight GetEdgeWeight(NodeId source, NodeId target)
    {
        if (!_adjacencyList.TryGetValue(source.Value, out var neighbors))
            throw new InvalidOperationException($"Node {source.Value} not found");

        var neighbor = neighbors.FirstOrDefault(n => n.neighbor == target.Value);
        if (neighbor.Equals(default))
            throw new InvalidOperationException($"Edge between {source.Value} and {target.Value} not found");

        return neighbor.weight;
    }

    public bool TryGetEdgeWeight(NodeId source, NodeId target, out TWeight weight)
    {
        weight = default!;
        
        if (!_adjacencyList.TryGetValue(source.Value, out var neighbors))
            return false;

        var neighbor = neighbors.FirstOrDefault(n => n.neighbor == target.Value);
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

        if (_adjacencyList.TryGetValue(node.Value, out var nodeNeighbors))
        {
            foreach (var (neighbor, _) in nodeNeighbors)
            {
                if (_adjacencyList.TryGetValue(neighbor, out var neighborList))
                {
                    neighborList.RemoveAll(n => n.neighbor == node.Value);
                }
            }
            _edgeCount -= nodeNeighbors.Count;
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

    public void AddEdge(NodeId node1, NodeId node2)
    {
        throw new InvalidOperationException("Use AddEdge(node1, node2, weight) for weighted graphs");
    }

    public void AddEdge(NodeId node1, NodeId node2, TWeight weight)
    {
        if (!_nodes.Contains(node1.Value))
            throw new InvalidOperationException($"Node {node1.Value} not found");
        if (!_nodes.Contains(node2.Value))
            throw new InvalidOperationException($"Node {node2.Value} not found");

        // Handle self-loop case
        if (node1.Value == node2.Value)
        {
            var neighbors = _adjacencyList[node1.Value];
            var existingEdgeIndex = neighbors.FindIndex(n => n.neighbor == node1.Value);
            
            if (existingEdgeIndex >= 0)
            {
                neighbors[existingEdgeIndex] = (node1.Value, weight);
            }
            else
            {
                neighbors.Add((node1.Value, weight));
                _edgeCount++;
            }
            return;
        }

        var neighbors1 = _adjacencyList[node1.Value];
        var neighbors2 = _adjacencyList[node2.Value];

        var existingEdgeIndex1 = neighbors1.FindIndex(n => n.neighbor == node2.Value);
        var existingEdgeIndex2 = neighbors2.FindIndex(n => n.neighbor == node1.Value);

        if (existingEdgeIndex1 >= 0)
        {
            neighbors1[existingEdgeIndex1] = (node2.Value, weight);
            neighbors2[existingEdgeIndex2] = (node1.Value, weight);
        }
        else
        {
            neighbors1.Add((node2.Value, weight));
            neighbors2.Add((node1.Value, weight));
            _edgeCount++;
        }
    }

    public void RemoveEdge(NodeId node1, NodeId node2)
    {
        // Handle self-loop case
        if (node1.Value == node2.Value)
        {
            if (_adjacencyList.TryGetValue(node1.Value, out var neighbors))
            {
                var removed = neighbors.RemoveAll(n => n.neighbor == node1.Value);
                if (removed > 0)
                {
                    _edgeCount--;
                }
            }
            return;
        }

        if (_adjacencyList.TryGetValue(node1.Value, out var neighbors1))
        {
            var removed1 = neighbors1.RemoveAll(n => n.neighbor == node2.Value);
            if (removed1 > 0 && _adjacencyList.TryGetValue(node2.Value, out var neighbors2))
            {
                neighbors2.RemoveAll(n => n.neighbor == node1.Value);
                _edgeCount--;
            }
        }
    }

    public void UpdateEdgeWeight(NodeId node1, NodeId node2, TWeight weight)
    {
        if (!_adjacencyList.TryGetValue(node1.Value, out var neighbors1))
            throw new InvalidOperationException($"Node {node1.Value} not found");
        if (!_adjacencyList.TryGetValue(node2.Value, out var neighbors2))
            throw new InvalidOperationException($"Node {node2.Value} not found");

        var existingEdgeIndex1 = neighbors1.FindIndex(n => n.neighbor == node2.Value);
        var existingEdgeIndex2 = neighbors2.FindIndex(n => n.neighbor == node1.Value);

        if (existingEdgeIndex1 < 0)
            throw new InvalidOperationException($"Edge between {node1.Value} and {node2.Value} not found");

        neighbors1[existingEdgeIndex1] = (node2.Value, weight);
        neighbors2[existingEdgeIndex2] = (node1.Value, weight);
    }

    public WeightedUndirectedGraph<TWeight> WithEdge(NodeId node1, NodeId node2, TWeight weight)
    {
        if (!ContainsNode(node1))
            AddNode();
        if (!ContainsNode(node2))
            AddNode();
        
        AddEdge(node1, node2, weight);
        return this;
    }

    public WeightedUndirectedGraph<TWeight> WithEdges(IEnumerable<WeightedEdge<TWeight>> edges)
    {
        foreach (var edge in edges)
        {
            WithEdge(edge.Source, edge.Target, edge.Weight);
        }
        return this;
    }
}