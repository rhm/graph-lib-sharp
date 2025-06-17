using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class DirectedGraphWithNodeData<TNodeData> : IMutableDirectedGraph, INodeDataGraph<TNodeData>
{
    private readonly Dictionary<int, List<int>> _adjacencyList;
    private readonly Dictionary<int, TNodeData> _nodeData;
    private readonly HashSet<int> _nodes;
    private int _nextNodeId;
    private int _edgeCount;

    public DirectedGraphWithNodeData()
    {
        _adjacencyList = new Dictionary<int, List<int>>();
        _nodeData = new Dictionary<int, TNodeData>();
        _nodes = new HashSet<int>();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public DirectedGraphWithNodeData(int initialCapacity)
    {
        _adjacencyList = new Dictionary<int, List<int>>(initialCapacity);
        _nodeData = new Dictionary<int, TNodeData>(initialCapacity);
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
        
        return neighbors.Select(n => new NodeId(n));
    }

    public IEnumerable<NodeId> InNeighbors(NodeId node)
    {
        return _adjacencyList
            .Where(kvp => kvp.Value.Contains(node.Value))
            .Select(kvp => new NodeId(kvp.Key));
    }

    public int OutDegree(NodeId node)
    {
        return _adjacencyList.TryGetValue(node.Value, out var neighbors) ? neighbors.Count : 0;
    }

    public int InDegree(NodeId node)
    {
        return _adjacencyList.Values.Sum(neighbors => neighbors.Count(n => n == node.Value));
    }

    public bool HasEdge(NodeId source, NodeId target)
    {
        return _adjacencyList.TryGetValue(source.Value, out var neighbors) &&
               neighbors.Contains(target.Value);
    }

    public TNodeData GetNodeData(NodeId node)
    {
        if (!_nodeData.TryGetValue(node.Value, out var data))
            throw new InvalidOperationException($"Node {node.Value} not found");
        
        return data;
    }

    public bool TryGetNodeData(NodeId node, out TNodeData data)
    {
        return _nodeData.TryGetValue(node.Value, out data!);
    }

    public NodeId AddNode()
    {
        return AddNode(default(TNodeData)!);
    }

    public NodeId AddNode(TNodeData data)
    {
        var nodeId = new NodeId(_nextNodeId++);
        _nodes.Add(nodeId.Value);
        _adjacencyList[nodeId.Value] = new List<int>();
        _nodeData[nodeId.Value] = data;
        return nodeId;
    }

    public void SetNodeData(NodeId node, TNodeData data)
    {
        if (!_nodes.Contains(node.Value))
            throw new InvalidOperationException($"Node {node.Value} not found");
        
        _nodeData[node.Value] = data;
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
            var removed = kvp.Value.RemoveAll(n => n == node.Value);
            _edgeCount -= removed;
        }

        _adjacencyList.Remove(node.Value);
        _nodeData.Remove(node.Value);
        _nodes.Remove(node.Value);
    }

    public void Clear()
    {
        _adjacencyList.Clear();
        _nodeData.Clear();
        _nodes.Clear();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public void AddEdge(NodeId source, NodeId target)
    {
        if (!_nodes.Contains(source.Value))
            throw new InvalidOperationException($"Source node {source.Value} not found");
        if (!_nodes.Contains(target.Value))
            throw new InvalidOperationException($"Target node {target.Value} not found");

        var neighbors = _adjacencyList[source.Value];
        if (!neighbors.Contains(target.Value))
        {
            neighbors.Add(target.Value);
            _edgeCount++;
        }
    }

    public void RemoveEdge(NodeId source, NodeId target)
    {
        if (!_adjacencyList.TryGetValue(source.Value, out var neighbors))
            return;

        var removed = neighbors.RemoveAll(n => n == target.Value);
        _edgeCount -= removed;
    }

    public DirectedGraphWithNodeData<TNodeData> WithEdge(NodeId source, NodeId target)
    {
        if (!ContainsNode(source))
            AddNode();
        if (!ContainsNode(target))
            AddNode();
        
        AddEdge(source, target);
        return this;
    }

    public DirectedGraphWithNodeData<TNodeData> WithNodeData(NodeId node, TNodeData data)
    {
        if (!ContainsNode(node))
            AddNode(data);
        else
            SetNodeData(node, data);
        
        return this;
    }

    public DirectedGraphWithNodeData<TNodeData> WithEdges(IEnumerable<Edge> edges)
    {
        foreach (var edge in edges)
        {
            WithEdge(edge.Source, edge.Target);
        }
        return this;
    }
}