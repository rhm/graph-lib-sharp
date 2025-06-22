using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Algorithms;

/// <summary>
/// Provides graph traversal algorithms.
/// </summary>
public static class GraphTraversal
{
    /// <summary>
    /// Performs a depth-first search traversal starting from the specified node.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to traverse.</param>
    /// <param name="start">The starting node.</param>
    /// <param name="visitor">The visitor to notify of traversal events.</param>
    public static void DepthFirstSearch<TGraph>(
        TGraph graph,
        NodeId start,
        IGraphVisitor visitor)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (visitor == null) throw new ArgumentNullException(nameof(visitor));
        if (!graph.ContainsNode(start)) throw new ArgumentException("Start node not in graph", nameof(start));

        var visited = new HashSet<NodeId>();
        var finished = new HashSet<NodeId>();
        var discoveryTime = new Dictionary<NodeId, int>();
        var finishTime = new Dictionary<NodeId, int>();
        var time = 0;

        void DfsVisit(NodeId u)
        {
            visited.Add(u);
            discoveryTime[u] = time++;
            visitor.DiscoverNode(u);

            var neighbors = GetNeighbors(graph, u);
            foreach (var v in neighbors)
            {
                visitor.ExamineEdge(u, v);

                if (!visited.Contains(v))
                {
                    visitor.TreeEdge(u, v);
                    DfsVisit(v);
                }
                else if (!finished.Contains(v))
                {
                    visitor.BackEdge(u, v);
                }
                else if (discoveryTime[u] < discoveryTime[v])
                {
                    visitor.ForwardEdge(u, v);
                }
                else
                {
                    visitor.CrossEdge(u, v);
                }
            }

            finished.Add(u);
            finishTime[u] = time++;
            visitor.FinishNode(u);
        }

        DfsVisit(start);
    }

    /// <summary>
    /// Performs a breadth-first search traversal starting from the specified node.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to traverse.</param>
    /// <param name="start">The starting node.</param>
    /// <param name="visitor">The visitor to notify of traversal events.</param>
    public static void BreadthFirstSearch<TGraph>(
        TGraph graph,
        NodeId start,
        IGraphVisitor visitor)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (visitor == null) throw new ArgumentNullException(nameof(visitor));
        if (!graph.ContainsNode(start)) throw new ArgumentException("Start node not in graph", nameof(start));

        var visited = new HashSet<NodeId>();
        var parent = new Dictionary<NodeId, NodeId?>();
        var queue = new Queue<NodeId>();

        visited.Add(start);
        parent[start] = null;
        queue.Enqueue(start);
        visitor.DiscoverNode(start);

        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            var neighbors = GetNeighbors(graph, u);

            foreach (var v in neighbors)
            {
                visitor.ExamineEdge(u, v);

                if (!visited.Contains(v))
                {
                    visited.Add(v);
                    parent[v] = u;
                    visitor.TreeEdge(u, v);
                    visitor.DiscoverNode(v);
                    queue.Enqueue(v);
                }
                else if (parent[u] != null && parent[u]!.Value.Equals(v))
                {
                    // Skip parent edge in undirected graphs
                    if (graph is IDirectedGraph)
                    {
                        visitor.BackEdge(u, v);
                    }
                }
                else
                {
                    // In BFS, non-tree edges are cross edges
                    visitor.CrossEdge(u, v);
                }
            }

            visitor.FinishNode(u);
        }
    }

    /// <summary>
    /// Returns an enumerable of nodes in depth-first order starting from the specified node.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to traverse.</param>
    /// <param name="start">The starting node.</param>
    /// <returns>An enumerable of nodes in DFS order.</returns>
    public static IEnumerable<NodeId> DepthFirstNodes<TGraph>(
        TGraph graph,
        NodeId start)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (!graph.ContainsNode(start)) throw new ArgumentException("Start node not in graph", nameof(start));

        var visited = new HashSet<NodeId>();
        var stack = new Stack<NodeId>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            
            if (visited.Contains(current))
                continue;

            visited.Add(current);
            yield return current;

            var neighbors = GetNeighbors(graph, current).Reverse(); // Reverse to maintain order
            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    stack.Push(neighbor);
                }
            }
        }
    }

    /// <summary>
    /// Returns an enumerable of nodes in breadth-first order starting from the specified node.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to traverse.</param>
    /// <param name="start">The starting node.</param>
    /// <returns>An enumerable of nodes in BFS order.</returns>
    public static IEnumerable<NodeId> BreadthFirstNodes<TGraph>(
        TGraph graph,
        NodeId start)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (!graph.ContainsNode(start)) throw new ArgumentException("Start node not in graph", nameof(start));

        var visited = new HashSet<NodeId>();
        var queue = new Queue<NodeId>();
        
        visited.Add(start);
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            yield return current;

            var neighbors = GetNeighbors(graph, current);
            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    // Helper method to get neighbors based on graph type
    private static IEnumerable<NodeId> GetNeighbors<TGraph>(TGraph graph, NodeId node)
        where TGraph : IGraph
    {
        if (graph is IDirectedGraph directedGraph)
        {
            return directedGraph.OutNeighbors(node);
        }
        else if (graph is IUndirectedGraph undirectedGraph)
        {
            return undirectedGraph.Neighbors(node);
        }
        else
        {
            // Fallback - less efficient but works for any graph
            var neighbors = new List<NodeId>();
            foreach (var other in graph.Nodes)
            {
                if (!other.Equals(node))
                {
                    if (graph is IDirectedGraph dg && dg.HasEdge(node, other))
                    {
                        neighbors.Add(other);
                    }
                }
            }
            return neighbors;
        }
    }
}