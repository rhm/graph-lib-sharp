using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class DirectedGraphWithData<TNodeData, TEdgeData> : 
    IMutableDirectedGraph, 
    INodeDataGraph<TNodeData>, 
    IEdgeDataGraph<TEdgeData>
{
    private readonly Dictionary<int, List<int>> _adjacencyList;
    private readonly Dictionary<int, TNodeData> _nodeData;
    private readonly Dictionary<(int source, int target), TEdgeData> _edgeData;
    private readonly HashSet<int> _nodes;
    private int _nextNodeId;
    private int _edgeCount;

    public DirectedGraphWithData()
    {
        _adjacencyList = new Dictionary<int, List<int>>();
        _nodeData = new Dictionary<int, TNodeData>();
        _edgeData = new Dictionary<(int, int), TEdgeData>();
        _nodes = new HashSet<int>();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public DirectedGraphWithData(int initialCapacity)
    {
        _adjacencyList = new Dictionary<int, List<int>>(initialCapacity);
        _nodeData = new Dictionary<int, TNodeData>(initialCapacity);
        _edgeData = new Dictionary<(int, int), TEdgeData>();
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

    public TEdgeData GetEdgeData(NodeId source, NodeId target)
    {
        if (!_edgeData.TryGetValue((source.Value, target.Value), out var data))
            throw new InvalidOperationException($"Edge from {source.Value} to {target.Value} not found");
        
        return data;
    }

    public bool TryGetEdgeData(NodeId source, NodeId target, out TEdgeData data)
    {
        return _edgeData.TryGetValue((source.Value, target.Value), out data!);
    }

    public NodeId AddNode()
    {
        return AddNode(default(TNodeData)!);
    }

    public NodeId AddNode(TNodeData nodeData)
    {
        var nodeId = new NodeId(_nextNodeId++);
        _nodes.Add(nodeId.Value);
        _adjacencyList[nodeId.Value] = new List<int>();
        _nodeData[nodeId.Value] = nodeData;
        return nodeId;
    }

    public void SetNodeData(NodeId node, TNodeData data)
    {
        if (!_nodes.Contains(node.Value))
            throw new InvalidOperationException($"Node {node.Value} not found");
        
        _nodeData[node.Value] = data;
    }

    public void AddEdge(NodeId source, NodeId target)
    {
        AddEdge(source, target, default(TEdgeData)!);
    }

    public void AddEdge(NodeId source, NodeId target, TEdgeData edgeData)
    {
        if (!_nodes.Contains(source.Value))
            throw new InvalidOperationException($"Source node {source.Value} not found");
        if (!_nodes.Contains(target.Value))
            throw new InvalidOperationException($"Target node {target.Value} not found");

        var neighbors = _adjacencyList[source.Value];
        var edgeKey = (source.Value, target.Value);

        if (!neighbors.Contains(target.Value))
        {
            neighbors.Add(target.Value);
            _edgeCount++;
        }

        _edgeData[edgeKey] = edgeData;
    }

    public void SetEdgeData(NodeId source, NodeId target, TEdgeData data)
    {
        var edgeKey = (source.Value, target.Value);
        if (!_edgeData.ContainsKey(edgeKey))
            throw new InvalidOperationException($"Edge from {source.Value} to {target.Value} not found");
        
        _edgeData[edgeKey] = data;
    }

    public void RemoveNode(NodeId node)
    {
        if (!_nodes.Contains(node.Value))
            return;

        if (_adjacencyList.TryGetValue(node.Value, out var outEdges))
        {
            foreach (var target in outEdges)
            {
                _edgeData.Remove((node.Value, target));
            }
            _edgeCount -= outEdges.Count;
        }

        foreach (var kvp in _adjacencyList)
        {
            var removed = kvp.Value.RemoveAll(n => n == node.Value);
            for (int i = 0; i < removed; i++)
            {
                _edgeData.Remove((kvp.Key, node.Value));
            }
            _edgeCount -= removed;
        }

        _adjacencyList.Remove(node.Value);
        _nodeData.Remove(node.Value);
        _nodes.Remove(node.Value);
    }

    public void RemoveEdge(NodeId source, NodeId target)
    {
        if (!_adjacencyList.TryGetValue(source.Value, out var neighbors))
            return;

        var removed = neighbors.RemoveAll(n => n == target.Value);
        if (removed > 0)
        {
            _edgeData.Remove((source.Value, target.Value));
            _edgeCount -= removed;
        }
    }

    public void Clear()
    {
        _adjacencyList.Clear();
        _nodeData.Clear();
        _edgeData.Clear();
        _nodes.Clear();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public DirectedGraphWithData<TNodeData, TEdgeData> WithEdge(NodeId source, NodeId target, TEdgeData edgeData)
    {
        if (!ContainsNode(source))
            AddNode();
        if (!ContainsNode(target))
            AddNode();
        
        AddEdge(source, target, edgeData);
        return this;
    }

    public DirectedGraphWithData<TNodeData, TEdgeData> WithNodeData(NodeId node, TNodeData nodeData)
    {
        if (!ContainsNode(node))
            AddNode(nodeData);
        else
            SetNodeData(node, nodeData);
        
        return this;
    }
}