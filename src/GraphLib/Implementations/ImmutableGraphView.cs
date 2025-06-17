using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class ImmutableGraphView<TGraph> : IGraph where TGraph : IGraph
{
    private readonly TGraph _graph;

    public ImmutableGraphView(TGraph graph)
    {
        _graph = graph ?? throw new ArgumentNullException(nameof(graph));
    }

    public int NodeCount => _graph.NodeCount;
    public int EdgeCount => _graph.EdgeCount;
    public IEnumerable<NodeId> Nodes => _graph.Nodes;

    public bool ContainsNode(NodeId node)
    {
        return _graph.ContainsNode(node);
    }

    public int Degree(NodeId node)
    {
        return _graph.Degree(node);
    }

    public TGraph UnderlyingGraph => _graph;
}

public class ImmutableDirectedGraphView<TGraph> : ImmutableGraphView<TGraph>, IDirectedGraph 
    where TGraph : IDirectedGraph
{
    public ImmutableDirectedGraphView(TGraph graph) : base(graph)
    {
    }

    public IEnumerable<NodeId> OutNeighbors(NodeId node)
    {
        return ((IDirectedGraph)UnderlyingGraph).OutNeighbors(node);
    }

    public IEnumerable<NodeId> InNeighbors(NodeId node)
    {
        return ((IDirectedGraph)UnderlyingGraph).InNeighbors(node);
    }

    public int OutDegree(NodeId node)
    {
        return ((IDirectedGraph)UnderlyingGraph).OutDegree(node);
    }

    public int InDegree(NodeId node)
    {
        return ((IDirectedGraph)UnderlyingGraph).InDegree(node);
    }

    public bool HasEdge(NodeId source, NodeId target)
    {
        return ((IDirectedGraph)UnderlyingGraph).HasEdge(source, target);
    }
}

public class ImmutableUndirectedGraphView<TGraph> : ImmutableGraphView<TGraph>, IUndirectedGraph 
    where TGraph : IUndirectedGraph
{
    public ImmutableUndirectedGraphView(TGraph graph) : base(graph)
    {
    }

    public IEnumerable<NodeId> Neighbors(NodeId node)
    {
        return ((IUndirectedGraph)UnderlyingGraph).Neighbors(node);
    }

    public bool HasEdge(NodeId node1, NodeId node2)
    {
        return ((IUndirectedGraph)UnderlyingGraph).HasEdge(node1, node2);
    }
}

public class ImmutableWeightedGraphView<TGraph, TWeight> : ImmutableGraphView<TGraph>, IWeightedGraph<TWeight>
    where TGraph : IGraph, IWeightedGraph<TWeight>
{
    public ImmutableWeightedGraphView(TGraph graph) : base(graph)
    {
    }

    public TWeight GetEdgeWeight(NodeId source, NodeId target)
    {
        return ((IWeightedGraph<TWeight>)UnderlyingGraph).GetEdgeWeight(source, target);
    }

    public bool TryGetEdgeWeight(NodeId source, NodeId target, out TWeight weight)
    {
        return ((IWeightedGraph<TWeight>)UnderlyingGraph).TryGetEdgeWeight(source, target, out weight);
    }
}

public class ImmutableNodeDataGraphView<TGraph, TNodeData> : ImmutableGraphView<TGraph>, INodeDataGraph<TNodeData>
    where TGraph : IGraph, INodeDataGraph<TNodeData>
{
    public ImmutableNodeDataGraphView(TGraph graph) : base(graph)
    {
    }

    public TNodeData GetNodeData(NodeId node)
    {
        return ((INodeDataGraph<TNodeData>)UnderlyingGraph).GetNodeData(node);
    }

    public bool TryGetNodeData(NodeId node, out TNodeData data)
    {
        return ((INodeDataGraph<TNodeData>)UnderlyingGraph).TryGetNodeData(node, out data);
    }
}

public class ImmutableEdgeDataGraphView<TGraph, TEdgeData> : ImmutableGraphView<TGraph>, IEdgeDataGraph<TEdgeData>
    where TGraph : IGraph, IEdgeDataGraph<TEdgeData>
{
    public ImmutableEdgeDataGraphView(TGraph graph) : base(graph)
    {
    }

    public TEdgeData GetEdgeData(NodeId source, NodeId target)
    {
        return ((IEdgeDataGraph<TEdgeData>)UnderlyingGraph).GetEdgeData(source, target);
    }

    public bool TryGetEdgeData(NodeId source, NodeId target, out TEdgeData data)
    {
        return ((IEdgeDataGraph<TEdgeData>)UnderlyingGraph).TryGetEdgeData(source, target, out data);
    }
}