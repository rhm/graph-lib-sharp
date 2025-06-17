using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Implementations;

public class SubgraphView<TGraph> : IGraph where TGraph : IGraph
{
    private readonly TGraph _parentGraph;
    private readonly Predicate<NodeId>? _nodeFilter;
    protected readonly Predicate<Edge>? _edgeFilter;

    public SubgraphView(TGraph parentGraph, Predicate<NodeId>? nodeFilter = null, Predicate<Edge>? edgeFilter = null)
    {
        _parentGraph = parentGraph ?? throw new ArgumentNullException(nameof(parentGraph));
        _nodeFilter = nodeFilter;
        _edgeFilter = edgeFilter;
    }

    public TGraph ParentGraph => _parentGraph;

    public int NodeCount => FilteredNodes().Count();

    public int EdgeCount => FilteredEdges().Count();

    public IEnumerable<NodeId> Nodes => FilteredNodes();

    public bool ContainsNode(NodeId node)
    {
        return _parentGraph.ContainsNode(node) && (_nodeFilter?.Invoke(node) ?? true);
    }

    public int Degree(NodeId node)
    {
        if (!ContainsNode(node))
            return 0;

        return FilteredEdges()
            .Count(edge => edge.Source.Equals(node) || edge.Target.Equals(node));
    }

    private IEnumerable<NodeId> FilteredNodes()
    {
        var nodes = _parentGraph.Nodes;
        return _nodeFilter != null ? nodes.Where(n => _nodeFilter(n)) : nodes;
    }

    private IEnumerable<Edge> FilteredEdges()
    {
        var filteredNodes = FilteredNodes().ToHashSet();
        var edges = GetAllEdges();

        return edges.Where(edge => 
            filteredNodes.Contains(edge.Source) && 
            filteredNodes.Contains(edge.Target) &&
            (_edgeFilter?.Invoke(edge) ?? true));
    }

    private IEnumerable<Edge> GetAllEdges()
    {
        if (_parentGraph is IDirectedGraph directedGraph)
        {
            return _parentGraph.Nodes
                .SelectMany(node => directedGraph.OutNeighbors(node)
                    .Select(neighbor => new Edge(node, neighbor)));
        }
        else if (_parentGraph is IUndirectedGraph undirectedGraph)
        {
            var edges = new HashSet<Edge>();
            foreach (var node in _parentGraph.Nodes)
            {
                foreach (var neighbor in undirectedGraph.Neighbors(node))
                {
                    var edge1 = new Edge(node, neighbor);
                    var edge2 = new Edge(neighbor, node);
                    
                    if (!edges.Contains(edge1) && !edges.Contains(edge2))
                    {
                        edges.Add(edge1);
                    }
                }
            }
            return edges;
        }

        return Enumerable.Empty<Edge>();
    }
}

public class DirectedSubgraphView<TGraph> : SubgraphView<TGraph>, IDirectedGraph 
    where TGraph : IDirectedGraph
{
    public DirectedSubgraphView(TGraph parentGraph, Predicate<NodeId>? nodeFilter = null, Predicate<Edge>? edgeFilter = null)
        : base(parentGraph, nodeFilter, edgeFilter)
    {
    }

    public IEnumerable<NodeId> OutNeighbors(NodeId node)
    {
        if (!ContainsNode(node))
            return Enumerable.Empty<NodeId>();

        return ((IDirectedGraph)ParentGraph).OutNeighbors(node)
            .Where(neighbor => ContainsNode(neighbor) && 
                   (_edgeFilter?.Invoke(new Edge(node, neighbor)) ?? true));
    }

    public IEnumerable<NodeId> InNeighbors(NodeId node)
    {
        if (!ContainsNode(node))
            return Enumerable.Empty<NodeId>();

        return ((IDirectedGraph)ParentGraph).InNeighbors(node)
            .Where(neighbor => ContainsNode(neighbor) && 
                   (_edgeFilter?.Invoke(new Edge(neighbor, node)) ?? true));
    }

    public int OutDegree(NodeId node)
    {
        return OutNeighbors(node).Count();
    }

    public int InDegree(NodeId node)
    {
        return InNeighbors(node).Count();
    }

    public bool HasEdge(NodeId source, NodeId target)
    {
        if (!ContainsNode(source) || !ContainsNode(target))
            return false;

        var edge = new Edge(source, target);
        return ((IDirectedGraph)ParentGraph).HasEdge(source, target) &&
               (_edgeFilter?.Invoke(edge) ?? true);
    }
}

public class UndirectedSubgraphView<TGraph> : SubgraphView<TGraph>, IUndirectedGraph 
    where TGraph : IUndirectedGraph
{
    public UndirectedSubgraphView(TGraph parentGraph, Predicate<NodeId>? nodeFilter = null, Predicate<Edge>? edgeFilter = null)
        : base(parentGraph, nodeFilter, edgeFilter)
    {
    }

    public IEnumerable<NodeId> Neighbors(NodeId node)
    {
        if (!ContainsNode(node))
            return Enumerable.Empty<NodeId>();

        return ((IUndirectedGraph)ParentGraph).Neighbors(node)
            .Where(neighbor => ContainsNode(neighbor) && 
                   (_edgeFilter?.Invoke(new Edge(node, neighbor)) ?? true));
    }

    public bool HasEdge(NodeId node1, NodeId node2)
    {
        if (!ContainsNode(node1) || !ContainsNode(node2))
            return false;

        var edge = new Edge(node1, node2);
        return ((IUndirectedGraph)ParentGraph).HasEdge(node1, node2) &&
               (_edgeFilter?.Invoke(edge) ?? true);
    }
}

public static class SubgraphExtensions
{
    public static SubgraphView<TGraph> Subgraph<TGraph>(
        this TGraph graph, 
        Predicate<NodeId>? nodeFilter = null, 
        Predicate<Edge>? edgeFilter = null) 
        where TGraph : IGraph
    {
        return new SubgraphView<TGraph>(graph, nodeFilter, edgeFilter);
    }

    public static DirectedSubgraphView<TGraph> DirectedSubgraph<TGraph>(
        this TGraph graph, 
        Predicate<NodeId>? nodeFilter = null, 
        Predicate<Edge>? edgeFilter = null) 
        where TGraph : IDirectedGraph
    {
        return new DirectedSubgraphView<TGraph>(graph, nodeFilter, edgeFilter);
    }

    public static UndirectedSubgraphView<TGraph> UndirectedSubgraph<TGraph>(
        this TGraph graph, 
        Predicate<NodeId>? nodeFilter = null, 
        Predicate<Edge>? edgeFilter = null) 
        where TGraph : IUndirectedGraph
    {
        return new UndirectedSubgraphView<TGraph>(graph, nodeFilter, edgeFilter);
    }

    public static SubgraphView<TGraph> InducedSubgraph<TGraph>(
        this TGraph graph, 
        IEnumerable<NodeId> nodes) 
        where TGraph : IGraph
    {
        var nodeSet = nodes.ToHashSet();
        return new SubgraphView<TGraph>(graph, node => nodeSet.Contains(node));
    }

    public static DirectedSubgraphView<TGraph> DirectedInducedSubgraph<TGraph>(
        this TGraph graph, 
        IEnumerable<NodeId> nodes) 
        where TGraph : IDirectedGraph
    {
        var nodeSet = nodes.ToHashSet();
        return new DirectedSubgraphView<TGraph>(graph, node => nodeSet.Contains(node));
    }

    public static UndirectedSubgraphView<TGraph> UndirectedInducedSubgraph<TGraph>(
        this TGraph graph, 
        IEnumerable<NodeId> nodes) 
        where TGraph : IUndirectedGraph
    {
        var nodeSet = nodes.ToHashSet();
        return new UndirectedSubgraphView<TGraph>(graph, node => nodeSet.Contains(node));
    }
}