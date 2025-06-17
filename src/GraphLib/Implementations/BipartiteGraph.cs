using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class BipartiteGraph : IUndirectedGraph
{
    public enum NodeSet { Left, Right }

    private readonly Dictionary<int, List<int>> _adjacencyList;
    private readonly HashSet<int> _leftNodes;
    private readonly HashSet<int> _rightNodes;
    private int _nextNodeId;
    private int _edgeCount;

    public BipartiteGraph()
    {
        _adjacencyList = new Dictionary<int, List<int>>();
        _leftNodes = new HashSet<int>();
        _rightNodes = new HashSet<int>();
        _nextNodeId = 0;
        _edgeCount = 0;
    }

    public int NodeCount => _leftNodes.Count + _rightNodes.Count;
    public int EdgeCount => _edgeCount;
    public IEnumerable<NodeId> Nodes => _leftNodes.Concat(_rightNodes).Select(id => new NodeId(id));

    public bool IsBipartite => true;

    public bool ContainsNode(NodeId node)
    {
        return _leftNodes.Contains(node.Value) || _rightNodes.Contains(node.Value);
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

    public NodeId AddNodeToSet(NodeSet set)
    {
        var nodeId = new NodeId(_nextNodeId++);
        
        switch (set)
        {
            case NodeSet.Left:
                _leftNodes.Add(nodeId.Value);
                break;
            case NodeSet.Right:
                _rightNodes.Add(nodeId.Value);
                break;
            default:
                throw new ArgumentException($"Invalid node set: {set}");
        }

        _adjacencyList[nodeId.Value] = new List<int>();
        return nodeId;
    }

    public void AddEdge(NodeId leftNode, NodeId rightNode)
    {
        if (!_leftNodes.Contains(leftNode.Value))
            throw new InvalidOperationException($"Left node {leftNode.Value} not found in left set");
        if (!_rightNodes.Contains(rightNode.Value))
            throw new InvalidOperationException($"Right node {rightNode.Value} not found in right set");

        var leftNeighbors = _adjacencyList[leftNode.Value];
        var rightNeighbors = _adjacencyList[rightNode.Value];

        if (!leftNeighbors.Contains(rightNode.Value))
        {
            leftNeighbors.Add(rightNode.Value);
            rightNeighbors.Add(leftNode.Value);
            _edgeCount++;
        }
    }

    public void RemoveEdge(NodeId leftNode, NodeId rightNode)
    {
        if (_adjacencyList.TryGetValue(leftNode.Value, out var leftNeighbors) &&
            leftNeighbors.Remove(rightNode.Value))
        {
            if (_adjacencyList.TryGetValue(rightNode.Value, out var rightNeighbors))
            {
                rightNeighbors.Remove(leftNode.Value);
            }
            _edgeCount--;
        }
    }

    public NodeSet GetNodeSet(NodeId node)
    {
        if (_leftNodes.Contains(node.Value))
            return NodeSet.Left;
        if (_rightNodes.Contains(node.Value))
            return NodeSet.Right;
        
        throw new InvalidOperationException($"Node {node.Value} not found in either set");
    }

    public IEnumerable<NodeId> GetNodesInSet(NodeSet set)
    {
        return set switch
        {
            NodeSet.Left => _leftNodes.Select(id => new NodeId(id)),
            NodeSet.Right => _rightNodes.Select(id => new NodeId(id)),
            _ => throw new ArgumentException($"Invalid node set: {set}")
        };
    }

    public void RemoveNode(NodeId node)
    {
        if (!ContainsNode(node))
            return;

        if (_adjacencyList.TryGetValue(node.Value, out var nodeNeighbors))
        {
            foreach (var neighbor in nodeNeighbors)
            {
                if (_adjacencyList.TryGetValue(neighbor, out var neighborList))
                {
                    neighborList.Remove(node.Value);
                }
            }
            _edgeCount -= nodeNeighbors.Count;
        }

        _adjacencyList.Remove(node.Value);
        _leftNodes.Remove(node.Value);
        _rightNodes.Remove(node.Value);
    }

    public void Clear()
    {
        _adjacencyList.Clear();
        _leftNodes.Clear();
        _rightNodes.Clear();
        _nextNodeId = 0;
        _edgeCount = 0;
    }
}