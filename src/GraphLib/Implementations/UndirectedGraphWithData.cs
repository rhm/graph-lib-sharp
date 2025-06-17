using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class UndirectedGraphWithData<TNodeData, TEdgeData> : 
    IMutableUndirectedGraph, 
    INodeDataGraph<TNodeData>, 
    IEdgeDataGraph<TEdgeData>
{
    private readonly Dictionary<int, List<int>> _adjacencyList;
    private readonly Dictionary<int, TNodeData> _nodeData;
    private readonly Dictionary<(int node1, int node2), TEdgeData> _edgeData;
    private readonly HashSet<int> _nodes;
    private int _nextNodeId;
    private int _edgeCount;

    public UndirectedGraphWithData()
    {
        _adjacencyList = new Dictionary<int, List<int>>();
        _nodeData = new Dictionary<int, TNodeData>();
        _edgeData = new Dictionary<(int, int), TEdgeData>();
        _nodes = new HashSet<int>();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public UndirectedGraphWithData(int initialCapacity)
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
        return _adjacencyList.TryGetValue(node.Value, out var neighbors) ? neighbors.Count : 0;
    }

    public IEnumerable<NodeId> Neighbors(NodeId node)
    {
        if (!_adjacencyList.TryGetValue(node.Value, out var neighbors))
            return Enumerable.Empty<NodeId>();
        
        return neighbors.Select(n => new NodeId(n));
    }

    public bool HasEdge(NodeId node1, NodeId node2)
    {
        return _adjacencyList.TryGetValue(node1.Value, out var neighbors) &&
               neighbors.Contains(node2.Value);
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
        var edgeKey = GetEdgeKey(source.Value, target.Value);
        if (!_edgeData.TryGetValue(edgeKey, out var data))
            throw new InvalidOperationException($"Edge between {source.Value} and {target.Value} not found");
        
        return data;
    }

    public bool TryGetEdgeData(NodeId source, NodeId target, out TEdgeData data)
    {
        var edgeKey = GetEdgeKey(source.Value, target.Value);
        return _edgeData.TryGetValue(edgeKey, out data!);
    }

    private (int, int) GetEdgeKey(int node1, int node2)
    {
        return node1 < node2 ? (node1, node2) : (node2, node1);
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

    public void AddEdge(NodeId node1, NodeId node2)
    {
        AddEdge(node1, node2, default(TEdgeData)!);
    }

    public void AddEdge(NodeId node1, NodeId node2, TEdgeData edgeData)
    {
        if (!_nodes.Contains(node1.Value))
            throw new InvalidOperationException($"Node {node1.Value} not found");
        if (!_nodes.Contains(node2.Value))
            throw new InvalidOperationException($"Node {node2.Value} not found");

        var edgeKey = GetEdgeKey(node1.Value, node2.Value);

        // Handle self-loop case
        if (node1.Value == node2.Value)
        {
            var neighbors = _adjacencyList[node1.Value];
            if (!neighbors.Contains(node1.Value))
            {
                neighbors.Add(node1.Value);
                _edgeCount++;
            }
            _edgeData[edgeKey] = edgeData;
            return;
        }

        var neighbors1 = _adjacencyList[node1.Value];
        var neighbors2 = _adjacencyList[node2.Value];

        if (!neighbors1.Contains(node2.Value))
        {
            neighbors1.Add(node2.Value);
            neighbors2.Add(node1.Value);
            _edgeCount++;
        }

        _edgeData[edgeKey] = edgeData;
    }

    public void SetEdgeData(NodeId node1, NodeId node2, TEdgeData data)
    {
        var edgeKey = GetEdgeKey(node1.Value, node2.Value);
        if (!_edgeData.ContainsKey(edgeKey))
            throw new InvalidOperationException($"Edge between {node1.Value} and {node2.Value} not found");
        
        _edgeData[edgeKey] = data;
    }

    public void RemoveNode(NodeId node)
    {
        if (!_nodes.Contains(node.Value))
            return;

        if (_adjacencyList.TryGetValue(node.Value, out var nodeNeighbors))
        {
            foreach (var neighbor in nodeNeighbors)
            {
                if (_adjacencyList.TryGetValue(neighbor, out var neighborList))
                {
                    neighborList.Remove(node.Value);
                }
                
                var edgeKey = GetEdgeKey(node.Value, neighbor);
                _edgeData.Remove(edgeKey);
            }
            _edgeCount -= nodeNeighbors.Count;
        }

        _adjacencyList.Remove(node.Value);
        _nodeData.Remove(node.Value);
        _nodes.Remove(node.Value);
    }

    public void RemoveEdge(NodeId node1, NodeId node2)
    {
        if (_adjacencyList.TryGetValue(node1.Value, out var neighbors1) &&
            neighbors1.Remove(node2.Value))
        {
            if (_adjacencyList.TryGetValue(node2.Value, out var neighbors2))
            {
                neighbors2.Remove(node1.Value);
            }
            
            var edgeKey = GetEdgeKey(node1.Value, node2.Value);
            _edgeData.Remove(edgeKey);
            _edgeCount--;
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

    public UndirectedGraphWithData<TNodeData, TEdgeData> WithEdge(NodeId node1, NodeId node2, TEdgeData edgeData)
    {
        if (!ContainsNode(node1))
            AddNode();
        if (!ContainsNode(node2))
            AddNode();
        
        AddEdge(node1, node2, edgeData);
        return this;
    }

    public UndirectedGraphWithData<TNodeData, TEdgeData> WithNodeData(NodeId node, TNodeData nodeData)
    {
        if (!ContainsNode(node))
            AddNode(nodeData);
        else
            SetNodeData(node, nodeData);
        
        return this;
    }
}