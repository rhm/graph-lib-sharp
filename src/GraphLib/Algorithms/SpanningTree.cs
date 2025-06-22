using GraphLib.Core;
using GraphLib.Interfaces;
using GraphLib.Algorithms.Results;

namespace GraphLib.Algorithms;

/// <summary>
/// Provides algorithms for finding spanning trees in graphs.
/// </summary>
public static class SpanningTree
{
    /// <summary>
    /// Finds the minimum spanning tree using Kruskal's algorithm.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <typeparam name="TWeight">The type of the edge weights.</typeparam>
    /// <param name="graph">The undirected graph.</param>
    /// <param name="comparer">The comparer for weights. If null, default comparer is used.</param>
    /// <param name="add">Function to add two weights.</param>
    /// <param name="zero">The zero value for the weight type.</param>
    /// <returns>The minimum spanning tree result.</returns>
    public static SpanningTreeResult<TWeight> Kruskal<TGraph, TWeight>(
        TGraph graph,
        IComparer<TWeight>? comparer = null,
        Func<TWeight, TWeight, TWeight>? add = null,
        TWeight? zero = default)
        where TGraph : IUndirectedGraph, IWeightedGraph<TWeight>
        where TWeight : IComparable<TWeight>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        comparer ??= Comparer<TWeight>.Default;
        
        // Handle numeric types automatically
        if (add == null)
        {
            if (typeof(TWeight) == typeof(int))
            {
                add = (a, b) => (TWeight)(object)((int)(object)a! + (int)(object)b!);
                zero = (TWeight)(object)0;
            }
            else if (typeof(TWeight) == typeof(double))
            {
                add = (a, b) => (TWeight)(object)((double)(object)a! + (double)(object)b!);
                zero = (TWeight)(object)0.0;
            }
            else if (typeof(TWeight) == typeof(float))
            {
                add = (a, b) => (TWeight)(object)((float)(object)a! + (float)(object)b!);
                zero = (TWeight)(object)0.0f;
            }
            else if (typeof(TWeight) == typeof(long))
            {
                add = (a, b) => (TWeight)(object)((long)(object)a! + (long)(object)b!);
                zero = (TWeight)(object)0L;
            }
            else if (typeof(TWeight) == typeof(decimal))
            {
                add = (a, b) => (TWeight)(object)((decimal)(object)a! + (decimal)(object)b!);
                zero = (TWeight)(object)0m;
            }
            else
            {
                throw new ArgumentException($"Add function must be provided for type {typeof(TWeight).Name}");
            }
        }

        // Collect all edges
        var edges = new List<WeightedEdge<TWeight>>();
        var processedPairs = new HashSet<(NodeId, NodeId)>();

        foreach (var node in graph.Nodes)
        {
            foreach (var neighbor in graph.Neighbors(node))
            {
                var pair = node.CompareTo(neighbor) < 0 ? (node, neighbor) : (neighbor, node);
                if (!processedPairs.Contains(pair))
                {
                    processedPairs.Add(pair);
                    var weight = graph.GetEdgeWeight(node, neighbor);
                    edges.Add(new WeightedEdge<TWeight>(node, neighbor, weight));
                }
            }
        }

        // Sort edges by weight
        edges.Sort((a, b) => comparer.Compare(a.Weight, b.Weight));

        // Union-Find data structure
        var parent = new Dictionary<NodeId, NodeId>();
        var rank = new Dictionary<NodeId, int>();

        foreach (var node in graph.Nodes)
        {
            parent[node] = node;
            rank[node] = 0;
        }

        NodeId Find(NodeId x)
        {
            if (!parent[x].Equals(x))
            {
                parent[x] = Find(parent[x]); // Path compression
            }
            return parent[x];
        }

        void Union(NodeId x, NodeId y)
        {
            var rootX = Find(x);
            var rootY = Find(y);

            if (rootX.Equals(rootY))
                return;

            // Union by rank
            if (rank[rootX] < rank[rootY])
            {
                parent[rootX] = rootY;
            }
            else if (rank[rootX] > rank[rootY])
            {
                parent[rootY] = rootX;
            }
            else
            {
                parent[rootY] = rootX;
                rank[rootX]++;
            }
        }

        // Kruskal's algorithm
        var mstEdges = new List<WeightedEdge<TWeight>>();
        var totalWeight = zero!;

        foreach (var edge in edges)
        {
            if (!Find(edge.Source).Equals(Find(edge.Target)))
            {
                mstEdges.Add(edge);
                totalWeight = add(totalWeight, edge.Weight);
                Union(edge.Source, edge.Target);

                // Early termination when we have enough edges
                if (mstEdges.Count == graph.NodeCount - 1)
                    break;
            }
        }

        return new SpanningTreeResult<TWeight>(mstEdges, totalWeight);
    }

    /// <summary>
    /// Finds the minimum spanning tree using Prim's algorithm.
    /// </summary>
    public static SpanningTreeResult<TWeight> Prim<TGraph, TWeight>(
        TGraph graph,
        NodeId startNode,
        IComparer<TWeight>? comparer = null,
        Func<TWeight, TWeight, TWeight>? add = null,
        TWeight? zero = default)
        where TGraph : IUndirectedGraph, IWeightedGraph<TWeight>
        where TWeight : IComparable<TWeight>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (!graph.ContainsNode(startNode)) throw new ArgumentException("Start node not in graph", nameof(startNode));

        comparer ??= Comparer<TWeight>.Default;
        
        // Handle numeric types automatically
        if (add == null)
        {
            if (typeof(TWeight) == typeof(int))
            {
                add = (a, b) => (TWeight)(object)((int)(object)a! + (int)(object)b!);
                zero = (TWeight)(object)0;
            }
            else if (typeof(TWeight) == typeof(double))
            {
                add = (a, b) => (TWeight)(object)((double)(object)a! + (double)(object)b!);
                zero = (TWeight)(object)0.0;
            }
            else if (typeof(TWeight) == typeof(float))
            {
                add = (a, b) => (TWeight)(object)((float)(object)a! + (float)(object)b!);
                zero = (TWeight)(object)0.0f;
            }
            else if (typeof(TWeight) == typeof(long))
            {
                add = (a, b) => (TWeight)(object)((long)(object)a! + (long)(object)b!);
                zero = (TWeight)(object)0L;
            }
            else if (typeof(TWeight) == typeof(decimal))
            {
                add = (a, b) => (TWeight)(object)((decimal)(object)a! + (decimal)(object)b!);
                zero = (TWeight)(object)0m;
            }
            else
            {
                throw new ArgumentException($"Add function must be provided for type {typeof(TWeight).Name}");
            }
        }

        var inMst = new HashSet<NodeId>();
        var mstEdges = new List<WeightedEdge<TWeight>>();
        var totalWeight = zero!;

        // Priority queue of edges (weight, source, target)
        var pq = new SortedSet<(TWeight Weight, NodeId Source, NodeId Target)>(
            Comparer<(TWeight Weight, NodeId Source, NodeId Target)>.Create((a, b) =>
            {
                var cmp = comparer.Compare(a.Weight, b.Weight);
                if (cmp != 0) return cmp;
                cmp = a.Source.CompareTo(b.Source);
                if (cmp != 0) return cmp;
                return a.Target.CompareTo(b.Target);
            }));

        // Start with the given node
        inMst.Add(startNode);

        // Add all edges from start node
        foreach (var neighbor in graph.Neighbors(startNode))
        {
            var weight = graph.GetEdgeWeight(startNode, neighbor);
            pq.Add((weight, startNode, neighbor));
        }

        // Prim's algorithm
        while (pq.Count > 0 && inMst.Count < graph.NodeCount)
        {
            var (weight, source, target) = pq.Min;
            pq.Remove(pq.Min);

            if (inMst.Contains(target))
                continue;

            // Add edge to MST
            mstEdges.Add(new WeightedEdge<TWeight>(source, target, weight));
            totalWeight = add(totalWeight, weight);
            inMst.Add(target);

            // Add new edges from the newly included node
            foreach (var neighbor in graph.Neighbors(target))
            {
                if (!inMst.Contains(neighbor))
                {
                    var edgeWeight = graph.GetEdgeWeight(target, neighbor);
                    pq.Add((edgeWeight, target, neighbor));
                }
            }
        }

        return new SpanningTreeResult<TWeight>(mstEdges, totalWeight);
    }

    /// <summary>
    /// Finds the maximum spanning tree of the graph.
    /// </summary>
    public static SpanningTreeResult<TWeight> MaximumSpanningTree<TGraph, TWeight>(
        TGraph graph,
        IComparer<TWeight>? comparer = null,
        Func<TWeight, TWeight, TWeight>? add = null,
        TWeight? zero = default)
        where TGraph : IUndirectedGraph, IWeightedGraph<TWeight>
        where TWeight : IComparable<TWeight>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        // Use Kruskal's algorithm with inverted comparer
        var invertedComparer = comparer == null
            ? Comparer<TWeight>.Create((a, b) => Comparer<TWeight>.Default.Compare(b, a))
            : Comparer<TWeight>.Create((a, b) => comparer.Compare(b, a));

        return Kruskal(graph, invertedComparer, add, zero);
    }
}