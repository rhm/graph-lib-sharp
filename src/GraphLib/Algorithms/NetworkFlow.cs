using GraphLib.Core;
using GraphLib.Interfaces;
using GraphLib.Algorithms.Results;

namespace GraphLib.Algorithms;

/// <summary>
/// Provides algorithms for network flow problems.
/// </summary>
public static class NetworkFlow
{
    /// <summary>
    /// Finds the maximum flow from source to sink using the Ford-Fulkerson method.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <typeparam name="TCapacity">The type of the edge capacities.</typeparam>
    /// <param name="graph">The directed graph with edge capacities.</param>
    /// <param name="source">The source node.</param>
    /// <param name="sink">The sink node.</param>
    /// <param name="add">Function to add capacities.</param>
    /// <param name="subtract">Function to subtract capacities.</param>
    /// <param name="comparer">The comparer for capacities.</param>
    /// <param name="zero">The zero value for the capacity type.</param>
    /// <returns>The maximum flow result.</returns>
    public static FlowResult<TCapacity> MaxFlow<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Func<TCapacity, TCapacity, TCapacity> add,
        Func<TCapacity, TCapacity, TCapacity> subtract,
        IComparer<TCapacity>? comparer = null,
        TCapacity? zero = default)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>
        where TCapacity : IComparable<TCapacity>
    {
        return FordFulkerson(graph, source, sink, add, subtract, comparer, zero, useBFS: false);
    }

    /// <summary>
    /// Finds the maximum flow using the Edmonds-Karp algorithm (BFS-based Ford-Fulkerson).
    /// </summary>
    public static FlowResult<TCapacity> EdmondsKarp<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Func<TCapacity, TCapacity, TCapacity> add,
        Func<TCapacity, TCapacity, TCapacity> subtract,
        IComparer<TCapacity>? comparer = null,
        TCapacity? zero = default)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>
        where TCapacity : IComparable<TCapacity>
    {
        return FordFulkerson(graph, source, sink, add, subtract, comparer, zero, useBFS: true);
    }

    /// <summary>
    /// Finds the minimum cut between source and sink.
    /// </summary>
    public static CutResult<TCapacity> MinCut<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Func<TCapacity, TCapacity, TCapacity> add,
        IComparer<TCapacity>? comparer = null,
        TCapacity? zero = default)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>
        where TCapacity : IComparable<TCapacity>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (!graph.ContainsNode(source)) throw new ArgumentException("Source node not in graph", nameof(source));
        if (!graph.ContainsNode(sink)) throw new ArgumentException("Sink node not in graph", nameof(sink));

        // Handle numeric types automatically
        Func<TCapacity, TCapacity, TCapacity>? subtract = null;
        if (typeof(TCapacity) == typeof(int))
        {
            add ??= (a, b) => (TCapacity)(object)((int)(object)a! + (int)(object)b!);
            subtract = (a, b) => (TCapacity)(object)((int)(object)a! - (int)(object)b!);
            zero ??= (TCapacity)(object)0;
        }
        else if (typeof(TCapacity) == typeof(double))
        {
            add ??= (a, b) => (TCapacity)(object)((double)(object)a! + (double)(object)b!);
            subtract = (a, b) => (TCapacity)(object)((double)(object)a! - (double)(object)b!);
            zero ??= (TCapacity)(object)0.0;
        }
        else if (typeof(TCapacity) == typeof(float))
        {
            add ??= (a, b) => (TCapacity)(object)((float)(object)a! + (float)(object)b!);
            subtract = (a, b) => (TCapacity)(object)((float)(object)a! - (float)(object)b!);
            zero ??= (TCapacity)(object)0.0f;
        }
        else if (typeof(TCapacity) == typeof(long))
        {
            add ??= (a, b) => (TCapacity)(object)((long)(object)a! + (long)(object)b!);
            subtract = (a, b) => (TCapacity)(object)((long)(object)a! - (long)(object)b!);
            zero ??= (TCapacity)(object)0L;
        }
        else if (typeof(TCapacity) == typeof(decimal))
        {
            add ??= (a, b) => (TCapacity)(object)((decimal)(object)a! + (decimal)(object)b!);
            subtract = (a, b) => (TCapacity)(object)((decimal)(object)a! - (decimal)(object)b!);
            zero ??= (TCapacity)(object)0m;
        }
        else
        {
            throw new ArgumentException($"Add and subtract functions must be provided for type {typeof(TCapacity).Name}");
        }

        comparer ??= Comparer<TCapacity>.Default;

        // First, compute max flow to get the residual graph
        var flowResult = EdmondsKarp(graph, source, sink, add, subtract, comparer, zero);

        // Build residual graph
        var residualCapacities = new Dictionary<(NodeId, NodeId), TCapacity>();
        
        // Initialize with original capacities
        foreach (var u in graph.Nodes)
        {
            foreach (var v in graph.OutNeighbors(u))
            {
                var capacity = graph.GetEdgeWeight(u, v);
                var flow = flowResult.EdgeFlows.TryGetValue(new Edge(u, v), out var f) ? f : zero!;
                var residual = subtract(capacity, flow);
                
                if (comparer.Compare(residual, zero!) > 0)
                {
                    residualCapacities[(u, v)] = residual;
                }
                
                // Add reverse edge with flow as capacity
                if (comparer.Compare(flow, zero!) > 0)
                {
                    residualCapacities[(v, u)] = flow;
                }
            }
        }

        // Find reachable nodes from source in residual graph
        var reachable = new HashSet<NodeId>();
        var queue = new Queue<NodeId>();
        queue.Enqueue(source);
        reachable.Add(source);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            foreach (var neighbor in graph.Nodes)
            {
                if (!reachable.Contains(neighbor) && 
                    residualCapacities.TryGetValue((current, neighbor), out var cap) &&
                    comparer.Compare(cap, zero!) > 0)
                {
                    reachable.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Find cut edges
        var cutEdges = new List<Edge>();
        var cutCapacity = zero!;

        foreach (var u in reachable)
        {
            foreach (var v in graph.OutNeighbors(u))
            {
                if (!reachable.Contains(v))
                {
                    cutEdges.Add(new Edge(u, v));
                    cutCapacity = add(cutCapacity, graph.GetEdgeWeight(u, v));
                }
            }
        }

        var sourceSideNodes = reachable.ToList();
        var sinkSideNodes = graph.Nodes.Where(n => !reachable.Contains(n)).ToList();

        return new CutResult<TCapacity>(cutCapacity, cutEdges, sourceSideNodes, sinkSideNodes);
    }

    private static FlowResult<TCapacity> FordFulkerson<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Func<TCapacity, TCapacity, TCapacity> add,
        Func<TCapacity, TCapacity, TCapacity> subtract,
        IComparer<TCapacity>? comparer,
        TCapacity? zero,
        bool useBFS)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>
        where TCapacity : IComparable<TCapacity>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (!graph.ContainsNode(source)) throw new ArgumentException("Source node not in graph", nameof(source));
        if (!graph.ContainsNode(sink)) throw new ArgumentException("Sink node not in graph", nameof(sink));
        if (add == null) throw new ArgumentNullException(nameof(add));
        if (subtract == null) throw new ArgumentNullException(nameof(subtract));

        comparer ??= Comparer<TCapacity>.Default;
        
        // Handle numeric types automatically for zero
        if (zero == null)
        {
            if (typeof(TCapacity) == typeof(int))
                zero = (TCapacity)(object)0;
            else if (typeof(TCapacity) == typeof(double))
                zero = (TCapacity)(object)0.0;
            else if (typeof(TCapacity) == typeof(float))
                zero = (TCapacity)(object)0.0f;
            else if (typeof(TCapacity) == typeof(long))
                zero = (TCapacity)(object)0L;
            else if (typeof(TCapacity) == typeof(decimal))
                zero = (TCapacity)(object)0m;
            else
                throw new ArgumentException($"Zero value must be provided for type {typeof(TCapacity).Name}");
        }

        // Initialize flow
        var flow = new Dictionary<Edge, TCapacity>();
        var totalFlow = zero;

        // Build adjacency structure for residual graph
        var residualGraph = new Dictionary<NodeId, List<NodeId>>();
        foreach (var node in graph.Nodes)
        {
            residualGraph[node] = new List<NodeId>();
        }

        // Build initial residual graph
        foreach (var u in graph.Nodes)
        {
            foreach (var v in graph.OutNeighbors(u))
            {
                residualGraph[u].Add(v);
                if (!residualGraph[v].Contains(u))
                {
                    residualGraph[v].Add(u);
                }
            }
        }

        // Find augmenting path function
        Func<Dictionary<NodeId, NodeId>?> findPath = useBFS
            ? () => FindPathBFS(graph, source, sink, flow, add, subtract, comparer, zero, residualGraph)
            : () => FindPathDFS(graph, source, sink, flow, add, subtract, comparer, zero, residualGraph);

        // Main Ford-Fulkerson loop
        Dictionary<NodeId, NodeId>? parent;
        while ((parent = findPath()) != null)
        {
            // Find bottleneck capacity
            var pathFlow = default(TCapacity);
            var current = sink;
            var first = true;

            while (!current.Equals(source))
            {
                var prev = parent[current];
                var edge = new Edge(prev, current);
                var reverseEdge = new Edge(current, prev);

                TCapacity residual;
                if (graph.HasEdge(prev, current))
                {
                    var capacity = graph.GetEdgeWeight(prev, current);
                    var currentFlow = flow.TryGetValue(edge, out var f) ? f : zero;
                    residual = subtract(capacity, currentFlow);
                }
                else
                {
                    // This is a reverse edge
                    residual = flow.TryGetValue(reverseEdge, out var f) ? f : zero;
                }

                if (first || comparer.Compare(residual, pathFlow!) < 0)
                {
                    pathFlow = residual;
                    first = false;
                }

                current = prev;
            }

            // Update flow along the path
            current = sink;
            while (!current.Equals(source))
            {
                var prev = parent[current];
                var edge = new Edge(prev, current);
                var reverseEdge = new Edge(current, prev);

                if (graph.HasEdge(prev, current))
                {
                    // Forward edge
                    flow[edge] = flow.TryGetValue(edge, out var f) 
                        ? add(f, pathFlow!) 
                        : pathFlow!;
                }
                else
                {
                    // Reverse edge - decrease flow
                    flow[reverseEdge] = subtract(flow[reverseEdge], pathFlow!);
                }

                current = prev;
            }

            totalFlow = add(totalFlow, pathFlow!);
        }

        // Clean up zero flows
        var nonZeroFlows = flow.Where(kvp => comparer.Compare(kvp.Value, zero) != 0)
                                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return new FlowResult<TCapacity>(totalFlow, nonZeroFlows);
    }

    private static Dictionary<NodeId, NodeId>? FindPathBFS<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Dictionary<Edge, TCapacity> flow,
        Func<TCapacity, TCapacity, TCapacity> add,
        Func<TCapacity, TCapacity, TCapacity> subtract,
        IComparer<TCapacity> comparer,
        TCapacity zero,
        Dictionary<NodeId, List<NodeId>> residualGraph)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>
        where TCapacity : IComparable<TCapacity>
    {
        var parent = new Dictionary<NodeId, NodeId>();
        var visited = new HashSet<NodeId> { source };
        var queue = new Queue<NodeId>();
        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var next in residualGraph[current])
            {
                if (visited.Contains(next))
                    continue;

                // Check if there's residual capacity
                var edge = new Edge(current, next);
                var reverseEdge = new Edge(next, current);
                TCapacity residual;

                if (graph.HasEdge(current, next))
                {
                    var capacity = graph.GetEdgeWeight(current, next);
                    var currentFlow = flow.TryGetValue(edge, out var f) ? f : zero;
                    residual = subtract(capacity, currentFlow);
                }
                else
                {
                    // This is a reverse edge
                    residual = flow.TryGetValue(reverseEdge, out var f) ? f : zero;
                }

                if (comparer.Compare(residual, zero) > 0)
                {
                    parent[next] = current;
                    visited.Add(next);
                    queue.Enqueue(next);

                    if (next.Equals(sink))
                    {
                        return parent;
                    }
                }
            }
        }

        return null;
    }

    private static Dictionary<NodeId, NodeId>? FindPathDFS<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Dictionary<Edge, TCapacity> flow,
        Func<TCapacity, TCapacity, TCapacity> add,
        Func<TCapacity, TCapacity, TCapacity> subtract,
        IComparer<TCapacity> comparer,
        TCapacity zero,
        Dictionary<NodeId, List<NodeId>> residualGraph)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>
        where TCapacity : IComparable<TCapacity>
    {
        var parent = new Dictionary<NodeId, NodeId>();
        var visited = new HashSet<NodeId>();

        bool DFS(NodeId current)
        {
            if (current.Equals(sink))
                return true;

            visited.Add(current);

            foreach (var next in residualGraph[current])
            {
                if (visited.Contains(next))
                    continue;

                // Check if there's residual capacity
                var edge = new Edge(current, next);
                var reverseEdge = new Edge(next, current);
                TCapacity residual;

                if (graph.HasEdge(current, next))
                {
                    var capacity = graph.GetEdgeWeight(current, next);
                    var currentFlow = flow.TryGetValue(edge, out var f) ? f : zero;
                    residual = subtract(capacity, currentFlow);
                }
                else
                {
                    // This is a reverse edge
                    residual = flow.TryGetValue(reverseEdge, out var f) ? f : zero;
                }

                if (comparer.Compare(residual, zero) > 0)
                {
                    parent[next] = current;
                    if (DFS(next))
                        return true;
                }
            }

            return false;
        }

        return DFS(source) ? parent : null;
    }
}