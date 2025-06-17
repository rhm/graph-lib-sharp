using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Builders;

public class GraphBuilder<TGraph> where TGraph : IMutableGraph, new()
{
    private readonly TGraph _graph;
    private readonly Dictionary<int, NodeId> _nodeMap;
    private int _nextSequentialId;

    public GraphBuilder()
    {
        _graph = new TGraph();
        _nodeMap = new Dictionary<int, NodeId>();
        _nextSequentialId = 0;
    }

    public GraphBuilder<TGraph> AddNode()
    {
        var nodeId = _graph.AddNode();
        _nodeMap[_nextSequentialId++] = nodeId;
        return this;
    }

    public GraphBuilder<TGraph> AddNodes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddNode();
        }
        return this;
    }

    public GraphBuilder<TGraph> AddEdge(int sourceIndex, int targetIndex)
    {
        if (!_nodeMap.TryGetValue(sourceIndex, out var source))
            throw new ArgumentException($"Node at index {sourceIndex} not found", nameof(sourceIndex));
        if (!_nodeMap.TryGetValue(targetIndex, out var target))
            throw new ArgumentException($"Node at index {targetIndex} not found", nameof(targetIndex));

        return AddEdge(source, target);
    }

    public GraphBuilder<TGraph> AddEdge(NodeId source, NodeId target)
    {
        switch (_graph)
        {
            case IMutableDirectedGraph directedGraph:
                directedGraph.AddEdge(source, target);
                break;
            case IMutableUndirectedGraph undirectedGraph:
                undirectedGraph.AddEdge(source, target);
                break;
            default:
                throw new InvalidOperationException("Graph type does not support edge addition");
        }
        return this;
    }

    public GraphBuilder<TGraph> AddEdges(IEnumerable<Edge> edges)
    {
        foreach (var edge in edges)
        {
            AddEdge(edge.Source, edge.Target);
        }
        return this;
    }

    public GraphBuilder<TGraph> AddPath(params int[] nodeIndices)
    {
        if (nodeIndices.Length < 2)
            return this;

        for (int i = 0; i < nodeIndices.Length - 1; i++)
        {
            AddEdge(nodeIndices[i], nodeIndices[i + 1]);
        }
        return this;
    }

    public GraphBuilder<TGraph> AddPath(params NodeId[] nodes)
    {
        if (nodes.Length < 2)
            return this;

        for (int i = 0; i < nodes.Length - 1; i++)
        {
            AddEdge(nodes[i], nodes[i + 1]);
        }
        return this;
    }

    public GraphBuilder<TGraph> AddCycle(params int[] nodeIndices)
    {
        if (nodeIndices.Length < 3)
            throw new ArgumentException("A cycle requires at least 3 nodes", nameof(nodeIndices));

        AddPath(nodeIndices);
        AddEdge(nodeIndices[^1], nodeIndices[0]);
        return this;
    }

    public GraphBuilder<TGraph> AddCycle(params NodeId[] nodes)
    {
        if (nodes.Length < 3)
            throw new ArgumentException("A cycle requires at least 3 nodes", nameof(nodes));

        AddPath(nodes);
        AddEdge(nodes[^1], nodes[0]);
        return this;
    }

    public GraphBuilder<TGraph> AddClique(IEnumerable<int> nodeIndices)
    {
        var indices = nodeIndices.ToList();
        for (int i = 0; i < indices.Count; i++)
        {
            for (int j = i + 1; j < indices.Count; j++)
            {
                AddEdge(indices[i], indices[j]);
                
                if (_graph is IMutableUndirectedGraph)
                {
                    continue;
                }
                
                AddEdge(indices[j], indices[i]);
            }
        }
        return this;
    }

    public GraphBuilder<TGraph> AddClique(IEnumerable<NodeId> nodes)
    {
        var nodeList = nodes.ToList();
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = i + 1; j < nodeList.Count; j++)
            {
                AddEdge(nodeList[i], nodeList[j]);
                
                if (_graph is IMutableUndirectedGraph)
                {
                    continue;
                }
                
                AddEdge(nodeList[j], nodeList[i]);
            }
        }
        return this;
    }

    public GraphBuilder<TGraph> AddStar(int centerIndex, IEnumerable<int> leafIndices)
    {
        foreach (var leafIndex in leafIndices)
        {
            AddEdge(centerIndex, leafIndex);
        }
        return this;
    }

    public GraphBuilder<TGraph> AddStar(NodeId center, IEnumerable<NodeId> leaves)
    {
        foreach (var leaf in leaves)
        {
            AddEdge(center, leaf);
        }
        return this;
    }

    public NodeId GetNode(int index)
    {
        if (!_nodeMap.TryGetValue(index, out var nodeId))
            throw new ArgumentException($"Node at index {index} not found", nameof(index));
        return nodeId;
    }

    public IReadOnlyDictionary<int, NodeId> NodeMap => _nodeMap;

    public TGraph Build()
    {
        return _graph;
    }
}

public static class GraphBuilder
{
    public static GraphBuilder<TGraph> Create<TGraph>() where TGraph : IMutableGraph, new()
    {
        return new GraphBuilder<TGraph>();
    }
}