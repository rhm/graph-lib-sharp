using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class DenseDirectedGraph : IMutableDirectedGraph
{
    private readonly bool[,] _adjacencyMatrix;
    private readonly HashSet<int> _nodes;
    private readonly int _maxNodeCount;
    private int _nextNodeId;
    private int _edgeCount;

    public DenseDirectedGraph(int nodeCount)
    {
        if (nodeCount <= 0)
            throw new ArgumentException("Node count must be positive", nameof(nodeCount));

        _maxNodeCount = nodeCount;
        _adjacencyMatrix = new bool[nodeCount, nodeCount];
        _nodes = new HashSet<int>();
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
        if (!_nodes.Contains(node.Value) || node.Value >= _maxNodeCount)
            return Enumerable.Empty<NodeId>();

        var neighbors = new List<NodeId>();
        for (int i = 0; i < _maxNodeCount; i++)
        {
            if (_adjacencyMatrix[node.Value, i] && _nodes.Contains(i))
            {
                neighbors.Add(new NodeId(i));
            }
        }
        return neighbors;
    }

    public IEnumerable<NodeId> InNeighbors(NodeId node)
    {
        if (!_nodes.Contains(node.Value) || node.Value >= _maxNodeCount)
            return Enumerable.Empty<NodeId>();

        var neighbors = new List<NodeId>();
        for (int i = 0; i < _maxNodeCount; i++)
        {
            if (_adjacencyMatrix[i, node.Value] && _nodes.Contains(i))
            {
                neighbors.Add(new NodeId(i));
            }
        }
        return neighbors;
    }

    public int OutDegree(NodeId node)
    {
        if (!_nodes.Contains(node.Value) || node.Value >= _maxNodeCount)
            return 0;

        int degree = 0;
        for (int i = 0; i < _maxNodeCount; i++)
        {
            if (_adjacencyMatrix[node.Value, i] && _nodes.Contains(i))
            {
                degree++;
            }
        }
        return degree;
    }

    public int InDegree(NodeId node)
    {
        if (!_nodes.Contains(node.Value) || node.Value >= _maxNodeCount)
            return 0;

        int degree = 0;
        for (int i = 0; i < _maxNodeCount; i++)
        {
            if (_adjacencyMatrix[i, node.Value] && _nodes.Contains(i))
            {
                degree++;
            }
        }
        return degree;
    }

    public bool HasEdge(NodeId source, NodeId target)
    {
        if (!_nodes.Contains(source.Value) || !_nodes.Contains(target.Value) ||
            source.Value >= _maxNodeCount || target.Value >= _maxNodeCount)
            return false;

        return _adjacencyMatrix[source.Value, target.Value];
    }

    public NodeId AddNode()
    {
        if (_nextNodeId >= _maxNodeCount)
            throw new InvalidOperationException($"Cannot add more than {_maxNodeCount} nodes to dense graph");

        var nodeId = new NodeId(_nextNodeId++);
        _nodes.Add(nodeId.Value);
        return nodeId;
    }

    public void RemoveNode(NodeId node)
    {
        if (!_nodes.Contains(node.Value) || node.Value >= _maxNodeCount)
            return;

        for (int i = 0; i < _maxNodeCount; i++)
        {
            if (_adjacencyMatrix[node.Value, i])
            {
                _adjacencyMatrix[node.Value, i] = false;
                _edgeCount--;
            }
            if (_adjacencyMatrix[i, node.Value])
            {
                _adjacencyMatrix[i, node.Value] = false;
                _edgeCount--;
            }
        }

        _nodes.Remove(node.Value);
    }

    public void Clear()
    {
        for (int i = 0; i < _maxNodeCount; i++)
        {
            for (int j = 0; j < _maxNodeCount; j++)
            {
                _adjacencyMatrix[i, j] = false;
            }
        }
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
        if (source.Value >= _maxNodeCount || target.Value >= _maxNodeCount)
            throw new InvalidOperationException("Node ID exceeds matrix capacity");

        if (!_adjacencyMatrix[source.Value, target.Value])
        {
            _adjacencyMatrix[source.Value, target.Value] = true;
            _edgeCount++;
        }
    }

    public void RemoveEdge(NodeId source, NodeId target)
    {
        if (source.Value >= _maxNodeCount || target.Value >= _maxNodeCount)
            return;

        if (_adjacencyMatrix[source.Value, target.Value])
        {
            _adjacencyMatrix[source.Value, target.Value] = false;
            _edgeCount--;
        }
    }

    public DenseDirectedGraph WithEdge(NodeId source, NodeId target)
    {
        if (!ContainsNode(source))
            AddNode();
        if (!ContainsNode(target))
            AddNode();
        
        AddEdge(source, target);
        return this;
    }

    public DenseDirectedGraph WithEdges(IEnumerable<Edge> edges)
    {
        foreach (var edge in edges)
        {
            WithEdge(edge.Source, edge.Target);
        }
        return this;
    }
}