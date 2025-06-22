using GraphLib.Core;
using GraphLib.Interfaces;
using GraphLib.Algorithms.Results;

namespace GraphLib.Algorithms;

/// <summary>
/// Provides algorithms for finding shortest paths in graphs.
/// </summary>
public static class ShortestPath
{
    /// <summary>
    /// Finds the shortest path from source to target using Dijkstra's algorithm.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <typeparam name="TWeight">The type of the edge weights.</typeparam>
    /// <param name="graph">The graph to search.</param>
    /// <param name="source">The source node.</param>
    /// <param name="target">The target node.</param>
    /// <param name="comparer">The comparer for weights. If null, default comparer is used.</param>
    /// <param name="add">Function to add two weights.</param>
    /// <param name="zero">The zero value for the weight type.</param>
    /// <returns>The shortest path result.</returns>
    public static PathResult<TWeight> Dijkstra<TGraph, TWeight>(
        TGraph graph,
        NodeId source,
        NodeId target,
        IComparer<TWeight>? comparer = null,
        Func<TWeight, TWeight, TWeight>? add = null,
        TWeight? zero = default)
        where TGraph : IDirectedGraph, IWeightedGraph<TWeight>
        where TWeight : IComparable<TWeight>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (!graph.ContainsNode(source)) throw new ArgumentException("Source node not in graph", nameof(source));
        if (!graph.ContainsNode(target)) throw new ArgumentException("Target node not in graph", nameof(target));

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

        var distances = new Dictionary<NodeId, TWeight>();
        var previous = new Dictionary<NodeId, NodeId>();
        var visited = new HashSet<NodeId>();
        var pq = new SortedSet<(TWeight Distance, NodeId Node)>(
            Comparer<(TWeight, NodeId)>.Create((a, b) =>
            {
                var cmp = comparer.Compare(a.Item1, b.Item1);
                return cmp != 0 ? cmp : a.Item2.CompareTo(b.Item2);
            }));

        // Initialize
        distances[source] = zero!;
        pq.Add((zero!, source));

        while (pq.Count > 0)
        {
            var (currentDist, current) = pq.Min;
            pq.Remove(pq.Min);

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            if (current.Equals(target))
            {
                // Reconstruct path
                var path = new List<NodeId>();
                var node = target;
                while (!node.Equals(source))
                {
                    path.Add(node);
                    node = previous[node];
                }
                path.Add(source);
                path.Reverse();

                return new PathResult<TWeight>(path, distances[target]);
            }

            foreach (var neighbor in graph.OutNeighbors(current))
            {
                if (visited.Contains(neighbor))
                    continue;

                var edgeWeight = graph.GetEdgeWeight(current, neighbor);
                var altDistance = add(currentDist, edgeWeight);

                if (!distances.ContainsKey(neighbor) || comparer.Compare(altDistance, distances[neighbor]) < 0)
                {
                    distances[neighbor] = altDistance;
                    previous[neighbor] = current;
                    pq.Add((altDistance, neighbor));
                }
            }
        }

        return PathResult<TWeight>.NoPath();
    }

    /// <summary>
    /// Finds the shortest path using A* algorithm with a heuristic function.
    /// </summary>
    public static PathResult<TWeight> AStar<TGraph, TWeight>(
        TGraph graph,
        NodeId source,
        NodeId target,
        Func<NodeId, NodeId, TWeight> heuristic,
        IComparer<TWeight>? comparer = null,
        Func<TWeight, TWeight, TWeight>? add = null,
        TWeight? zero = default)
        where TGraph : IDirectedGraph, IWeightedGraph<TWeight>
        where TWeight : IComparable<TWeight>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (heuristic == null) throw new ArgumentNullException(nameof(heuristic));
        if (!graph.ContainsNode(source)) throw new ArgumentException("Source node not in graph", nameof(source));
        if (!graph.ContainsNode(target)) throw new ArgumentException("Target node not in graph", nameof(target));

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

        var gScore = new Dictionary<NodeId, TWeight> { [source] = zero! };
        var fScore = new Dictionary<NodeId, TWeight> { [source] = heuristic(source, target) };
        var previous = new Dictionary<NodeId, NodeId>();
        var openSet = new SortedSet<(TWeight FScore, NodeId Node)>(
            Comparer<(TWeight, NodeId)>.Create((a, b) =>
            {
                var cmp = comparer.Compare(a.Item1, b.Item1);
                return cmp != 0 ? cmp : a.Item2.CompareTo(b.Item2);
            }));
        var inOpenSet = new HashSet<NodeId> { source };

        openSet.Add((fScore[source], source));

        while (openSet.Count > 0)
        {
            var current = openSet.Min.Node;
            
            if (current.Equals(target))
            {
                // Reconstruct path
                var path = new List<NodeId>();
                var node = target;
                while (!node.Equals(source))
                {
                    path.Add(node);
                    node = previous[node];
                }
                path.Add(source);
                path.Reverse();

                return new PathResult<TWeight>(path, gScore[target]);
            }

            openSet.Remove(openSet.Min);
            inOpenSet.Remove(current);

            foreach (var neighbor in graph.OutNeighbors(current))
            {
                var tentativeGScore = add(gScore[current], graph.GetEdgeWeight(current, neighbor));

                if (!gScore.ContainsKey(neighbor) || comparer.Compare(tentativeGScore, gScore[neighbor]) < 0)
                {
                    previous[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    var newFScore = add(tentativeGScore, heuristic(neighbor, target));

                    if (inOpenSet.Contains(neighbor))
                    {
                        // Remove old entry
                        openSet.RemoveWhere(x => x.Node.Equals(neighbor));
                    }

                    fScore[neighbor] = newFScore;
                    openSet.Add((newFScore, neighbor));
                    inOpenSet.Add(neighbor);
                }
            }
        }

        return PathResult<TWeight>.NoPath();
    }

    /// <summary>
    /// Finds the shortest path using Bellman-Ford algorithm (handles negative weights).
    /// </summary>
    public static PathResult<TWeight> BellmanFord<TGraph, TWeight>(
        TGraph graph,
        NodeId source,
        NodeId target,
        IComparer<TWeight>? comparer = null,
        Func<TWeight, TWeight, TWeight>? add = null,
        TWeight? zero = default,
        TWeight? infinity = default)
        where TGraph : IDirectedGraph, IWeightedGraph<TWeight>
        where TWeight : IComparable<TWeight>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (!graph.ContainsNode(source)) throw new ArgumentException("Source node not in graph", nameof(source));
        if (!graph.ContainsNode(target)) throw new ArgumentException("Target node not in graph", nameof(target));

        comparer ??= Comparer<TWeight>.Default;
        
        // Handle numeric types automatically
        if (add == null || infinity == null)
        {
            if (typeof(TWeight) == typeof(int))
            {
                add ??= (a, b) => (TWeight)(object)((int)(object)a! + (int)(object)b!);
                zero ??= (TWeight)(object)0;
                infinity ??= (TWeight)(object)int.MaxValue;
            }
            else if (typeof(TWeight) == typeof(double))
            {
                add ??= (a, b) => (TWeight)(object)((double)(object)a! + (double)(object)b!);
                zero ??= (TWeight)(object)0.0;
                infinity ??= (TWeight)(object)double.PositiveInfinity;
            }
            else if (typeof(TWeight) == typeof(float))
            {
                add ??= (a, b) => (TWeight)(object)((float)(object)a! + (float)(object)b!);
                zero ??= (TWeight)(object)0.0f;
                infinity ??= (TWeight)(object)float.PositiveInfinity;
            }
            else if (typeof(TWeight) == typeof(long))
            {
                add ??= (a, b) => (TWeight)(object)((long)(object)a! + (long)(object)b!);
                zero ??= (TWeight)(object)0L;
                infinity ??= (TWeight)(object)long.MaxValue;
            }
            else if (typeof(TWeight) == typeof(decimal))
            {
                add ??= (a, b) => (TWeight)(object)((decimal)(object)a! + (decimal)(object)b!);
                zero ??= (TWeight)(object)0m;
                infinity ??= (TWeight)(object)decimal.MaxValue;
            }
            else
            {
                throw new ArgumentException($"Add function and infinity value must be provided for type {typeof(TWeight).Name}");
            }
        }

        var distances = new Dictionary<NodeId, TWeight>();
        var previous = new Dictionary<NodeId, NodeId>();

        // Initialize distances - all nodes get infinity except source
        foreach (var node in graph.Nodes)
        {
            distances[node] = infinity!;
        }
        distances[source] = zero!;

        // Bellman-Ford relaxation: repeat V-1 times
        for (int i = 0; i < graph.NodeCount - 1; i++)
        {
            bool relaxed = false;
            
            // For each edge in the graph
            foreach (var u in graph.Nodes)
            {
                // Skip if u is unreachable
                if (comparer.Compare(distances[u], infinity!) == 0)
                    continue;

                foreach (var v in graph.OutNeighbors(u))
                {
                    var edgeWeight = graph.GetEdgeWeight(u, v);
                    var newDistance = add(distances[u], edgeWeight);

                    // Relax the edge if we found a shorter path
                    if (comparer.Compare(newDistance, distances[v]) < 0)
                    {
                        distances[v] = newDistance;
                        previous[v] = u;
                        relaxed = true;
                    }
                }
            }

            // Early termination if no edges were relaxed
            if (!relaxed)
                break;
        }

        // Check for negative weight cycles
        foreach (var u in graph.Nodes)
        {
            if (comparer.Compare(distances[u], infinity!) == 0)
                continue;

            foreach (var v in graph.OutNeighbors(u))
            {
                var edgeWeight = graph.GetEdgeWeight(u, v);
                var newDistance = add(distances[u], edgeWeight);

                if (comparer.Compare(newDistance, distances[v]) < 0)
                {
                    throw new InvalidOperationException("Graph contains a negative weight cycle");
                }
            }
        }

        // Check if target is reachable
        if (comparer.Compare(distances[target], infinity!) == 0)
        {
            return PathResult<TWeight>.NoPath();
        }

        // Handle source == target case
        if (source.Equals(target))
        {
            return new PathResult<TWeight>(new[] { source }, zero!);
        }

        // Reconstruct path from target back to source
        var path = new List<NodeId>();
        var current = target;
        
        while (!current.Equals(source))
        {
            path.Add(current);
            if (!previous.ContainsKey(current))
            {
                // This should not happen if distances[target] != infinity
                return PathResult<TWeight>.NoPath();
            }
            current = previous[current];
        }
        path.Add(source);
        path.Reverse();

        return new PathResult<TWeight>(path, distances[target]);
    }

    /// <summary>
    /// Finds the shortest path in an unweighted graph using breadth-first search.
    /// </summary>
    public static PathResult<int> BreadthFirstSearch<TGraph>(
        TGraph graph,
        NodeId source,
        NodeId target)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (!graph.ContainsNode(source)) throw new ArgumentException("Source node not in graph", nameof(source));
        if (!graph.ContainsNode(target)) throw new ArgumentException("Target node not in graph", nameof(target));

        if (source.Equals(target))
        {
            return new PathResult<int>(new[] { source }, 0);
        }

        var queue = new Queue<NodeId>();
        var visited = new HashSet<NodeId>();
        var previous = new Dictionary<NodeId, NodeId>();
        var distances = new Dictionary<NodeId, int>();

        queue.Enqueue(source);
        visited.Add(source);
        distances[source] = 0;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            IEnumerable<NodeId> neighbors;
            if (graph is IDirectedGraph directedGraph)
            {
                neighbors = directedGraph.OutNeighbors(current);
            }
            else if (graph is IUndirectedGraph undirectedGraph)
            {
                neighbors = undirectedGraph.Neighbors(current);
            }
            else
            {
                // Fallback for generic IGraph - this is less efficient
                neighbors = GetNeighbors(graph, current);
            }

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    previous[neighbor] = current;
                    distances[neighbor] = distances[current] + 1;
                    queue.Enqueue(neighbor);

                    if (neighbor.Equals(target))
                    {
                        // Reconstruct path
                        var path = new List<NodeId>();
                        var node = target;
                        while (!node.Equals(source))
                        {
                            path.Add(node);
                            node = previous[node];
                        }
                        path.Add(source);
                        path.Reverse();

                        return new PathResult<int>(path, distances[target]);
                    }
                }
            }
        }

        return PathResult<int>.NoPath();
    }

    /// <summary>
    /// Computes shortest paths between all pairs of nodes using Floyd-Warshall algorithm.
    /// </summary>
    public static Dictionary<(NodeId, NodeId), TWeight> FloydWarshall<TGraph, TWeight>(
        TGraph graph,
        IComparer<TWeight>? comparer = null,
        Func<TWeight, TWeight, TWeight>? add = null,
        TWeight? infinity = default)
        where TGraph : IDirectedGraph, IWeightedGraph<TWeight>
        where TWeight : IComparable<TWeight>
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        comparer ??= Comparer<TWeight>.Default;
        
        // Handle numeric types automatically
        if (add == null || infinity == null)
        {
            if (typeof(TWeight) == typeof(int))
            {
                add ??= (a, b) => (TWeight)(object)((int)(object)a! + (int)(object)b!);
                infinity ??= (TWeight)(object)(int.MaxValue / 2); // Avoid overflow
            }
            else if (typeof(TWeight) == typeof(double))
            {
                add ??= (a, b) => (TWeight)(object)((double)(object)a! + (double)(object)b!);
                infinity ??= (TWeight)(object)double.PositiveInfinity;
            }
            else if (typeof(TWeight) == typeof(float))
            {
                add ??= (a, b) => (TWeight)(object)((float)(object)a! + (float)(object)b!);
                infinity ??= (TWeight)(object)float.PositiveInfinity;
            }
            else if (typeof(TWeight) == typeof(long))
            {
                add ??= (a, b) => (TWeight)(object)((long)(object)a! + (long)(object)b!);
                infinity ??= (TWeight)(object)(long.MaxValue / 2); // Avoid overflow
            }
            else if (typeof(TWeight) == typeof(decimal))
            {
                add ??= (a, b) => (TWeight)(object)((decimal)(object)a! + (decimal)(object)b!);
                infinity ??= (TWeight)(object)(decimal.MaxValue / 2); // Avoid overflow
            }
            else
            {
                throw new ArgumentException($"Add function and infinity value must be provided for type {typeof(TWeight).Name}");
            }
        }

        var nodes = graph.Nodes.ToList();
        var dist = new Dictionary<(NodeId, NodeId), TWeight>();

        // Initialize distances
        foreach (var i in nodes)
        {
            foreach (var j in nodes)
            {
                if (i.Equals(j))
                {
                    dist[(i, j)] = default(TWeight)!;
                }
                else if (graph.HasEdge(i, j))
                {
                    dist[(i, j)] = graph.GetEdgeWeight(i, j);
                }
                else
                {
                    dist[(i, j)] = infinity!;
                }
            }
        }

        // Floyd-Warshall algorithm
        foreach (var k in nodes)
        {
            foreach (var i in nodes)
            {
                foreach (var j in nodes)
                {
                    var ikDist = dist[(i, k)];
                    var kjDist = dist[(k, j)];
                    
                    // Skip if either distance is infinity
                    if (comparer.Compare(ikDist, infinity!) == 0 || comparer.Compare(kjDist, infinity!) == 0)
                        continue;

                    var throughK = add(ikDist, kjDist);
                    if (comparer.Compare(throughK, dist[(i, j)]) < 0)
                    {
                        dist[(i, j)] = throughK;
                    }
                }
            }
        }

        // Check for negative cycles
        foreach (var i in nodes)
        {
            if (comparer.Compare(dist[(i, i)], default(TWeight)!) < 0)
            {
                throw new InvalidOperationException("Graph contains a negative weight cycle");
            }
        }

        return dist;
    }

    // Helper method to get neighbors for generic IGraph
    private static IEnumerable<NodeId> GetNeighbors<TGraph>(TGraph graph, NodeId node)
        where TGraph : IGraph
    {
        var neighbors = new List<NodeId>();
        foreach (var other in graph.Nodes)
        {
            if (!other.Equals(node))
            {
                // This is inefficient but works for any IGraph
                // In practice, graphs should implement IDirectedGraph or IUndirectedGraph
                if (graph is IDirectedGraph dg && dg.HasEdge(node, other))
                {
                    neighbors.Add(other);
                }
                else if (graph is IUndirectedGraph ug && ug.HasEdge(node, other))
                {
                    neighbors.Add(other);
                }
            }
        }
        return neighbors;
    }
}