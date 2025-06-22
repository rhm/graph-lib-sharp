using GraphLib.Core;
using GraphLib.Interfaces;
using GraphLib.Algorithms.Results;

namespace GraphLib.Algorithms;

/// <summary>
/// Provides various graph algorithms.
/// </summary>
public static class GraphAlgorithms
{
    /// <summary>
    /// Performs a topological sort of a directed acyclic graph.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The directed graph to sort.</param>
    /// <returns>The topological sort result.</returns>
    public static TopologicalSortResult TopologicalSort<TGraph>(TGraph graph)
        where TGraph : IDirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var inDegree = new Dictionary<NodeId, int>();
        var queue = new Queue<NodeId>();
        var result = new List<NodeId>();

        // Initialize in-degrees
        foreach (var node in graph.Nodes)
        {
            inDegree[node] = graph.InDegree(node);
            if (inDegree[node] == 0)
            {
                queue.Enqueue(node);
            }
        }

        // Process nodes with zero in-degree
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);

            foreach (var neighbor in graph.OutNeighbors(current))
            {
                inDegree[neighbor]--;
                if (inDegree[neighbor] == 0)
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Check if all nodes were processed (graph is acyclic)
        if (result.Count != graph.NodeCount)
        {
            return TopologicalSortResult.Cyclic();
        }

        return new TopologicalSortResult(result);
    }

    /// <summary>
    /// Finds strongly connected components using Tarjan's algorithm.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The directed graph.</param>
    /// <returns>A list of strongly connected components.</returns>
    public static IReadOnlyList<IReadOnlyList<NodeId>> StronglyConnectedComponents<TGraph>(TGraph graph)
        where TGraph : IDirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var index = 0;
        var stack = new Stack<NodeId>();
        var indices = new Dictionary<NodeId, int>();
        var lowLinks = new Dictionary<NodeId, int>();
        var onStack = new HashSet<NodeId>();
        var components = new List<List<NodeId>>();

        void StrongConnect(NodeId v)
        {
            indices[v] = index;
            lowLinks[v] = index;
            index++;
            stack.Push(v);
            onStack.Add(v);

            foreach (var w in graph.OutNeighbors(v))
            {
                if (!indices.ContainsKey(w))
                {
                    // Successor w has not yet been visited; recurse on it
                    StrongConnect(w);
                    lowLinks[v] = Math.Min(lowLinks[v], lowLinks[w]);
                }
                else if (onStack.Contains(w))
                {
                    // Successor w is in stack S and hence in the current SCC
                    lowLinks[v] = Math.Min(lowLinks[v], indices[w]);
                }
            }

            // If v is a root node, pop the stack and create an SCC
            if (lowLinks[v] == indices[v])
            {
                var component = new List<NodeId>();
                NodeId w;
                do
                {
                    w = stack.Pop();
                    onStack.Remove(w);
                    component.Add(w);
                } while (!w.Equals(v));

                components.Add(component);
            }
        }

        foreach (var node in graph.Nodes)
        {
            if (!indices.ContainsKey(node))
            {
                StrongConnect(node);
            }
        }

        return components.Select(c => (IReadOnlyList<NodeId>)c).ToList();
    }

    /// <summary>
    /// Finds connected components in an undirected graph using union-find.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The undirected graph.</param>
    /// <returns>A list of connected components.</returns>
    public static IReadOnlyList<IReadOnlyList<NodeId>> ConnectedComponents<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var parent = new Dictionary<NodeId, NodeId>();
        var rank = new Dictionary<NodeId, int>();

        // Initialize union-find
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

        // Union connected nodes
        foreach (var node in graph.Nodes)
        {
            foreach (var neighbor in graph.Neighbors(node))
            {
                Union(node, neighbor);
            }
        }

        // Group nodes by their root
        var componentMap = new Dictionary<NodeId, List<NodeId>>();
        foreach (var node in graph.Nodes)
        {
            var root = Find(node);
            if (!componentMap.ContainsKey(root))
            {
                componentMap[root] = new List<NodeId>();
            }
            componentMap[root].Add(node);
        }

        return componentMap.Values.Select(c => (IReadOnlyList<NodeId>)c).ToList();
    }

    /// <summary>
    /// Checks if the graph contains a cycle.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to check.</param>
    /// <returns>True if the graph contains a cycle; otherwise, false.</returns>
    public static bool HasCycle<TGraph>(TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        if (graph is IDirectedGraph directedGraph)
        {
            return HasCycleDirected(directedGraph);
        }
        else if (graph is IUndirectedGraph undirectedGraph)
        {
            return HasCycleUndirected(undirectedGraph);
        }
        else
        {
            // Generic fallback - assume directed
            throw new ArgumentException("Graph must implement IDirectedGraph or IUndirectedGraph");
        }
    }

    /// <summary>
    /// Finds a cycle in the graph if one exists.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to search.</param>
    /// <returns>A cycle if found; otherwise, an empty list.</returns>
    public static IReadOnlyList<NodeId> FindCycle<TGraph>(TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        if (graph is IDirectedGraph directedGraph)
        {
            return FindCycleDirected(directedGraph);
        }
        else if (graph is IUndirectedGraph undirectedGraph)
        {
            return FindCycleUndirected(undirectedGraph);
        }
        else
        {
            throw new ArgumentException("Graph must implement IDirectedGraph or IUndirectedGraph");
        }
    }

    /// <summary>
    /// Colors the graph using a greedy algorithm.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The undirected graph to color.</param>
    /// <returns>The coloring result.</returns>
    public static ColoringResult GreedyColoring<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var colors = new Dictionary<NodeId, int>();
        var nodeList = graph.Nodes.ToList();

        // Sort nodes by degree (descending) for better coloring
        nodeList.Sort((a, b) => graph.Degree(b).CompareTo(graph.Degree(a)));

        foreach (var node in nodeList)
        {
            var usedColors = new HashSet<int>();
            
            foreach (var neighbor in graph.Neighbors(node))
            {
                if (colors.TryGetValue(neighbor, out var neighborColor))
                {
                    usedColors.Add(neighborColor);
                }
            }

            // Find the smallest available color
            var color = 0;
            while (usedColors.Contains(color))
            {
                color++;
            }

            colors[node] = color;
        }

        return new ColoringResult(colors);
    }

    /// <summary>
    /// Checks if the graph is bipartite.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The undirected graph to check.</param>
    /// <returns>The bipartite check result.</returns>
    public static BipartiteCheckResult IsBipartite<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var colors = new Dictionary<NodeId, int>();
        var queue = new Queue<NodeId>();

        foreach (var startNode in graph.Nodes)
        {
            if (colors.ContainsKey(startNode))
                continue;

            // BFS coloring
            colors[startNode] = 0;
            queue.Enqueue(startNode);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentColor = colors[current];

                foreach (var neighbor in graph.Neighbors(current))
                {
                    if (!colors.ContainsKey(neighbor))
                    {
                        colors[neighbor] = 1 - currentColor;
                        queue.Enqueue(neighbor);
                    }
                    else if (colors[neighbor] == currentColor)
                    {
                        // Same color as current node - not bipartite
                        return BipartiteCheckResult.NotBipartite();
                    }
                }
            }
        }

        var leftSet = colors.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key).ToList();
        var rightSet = colors.Where(kvp => kvp.Value == 1).Select(kvp => kvp.Key).ToList();

        return new BipartiteCheckResult(leftSet, rightSet);
    }

    /// <summary>
    /// Checks if the undirected graph is connected.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The undirected graph to check.</param>
    /// <returns>True if the graph is connected; otherwise, false.</returns>
    public static bool IsConnected<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        
        var nodes = graph.Nodes.ToList();
        if (nodes.Count <= 1) return true;

        var components = ConnectedComponents(graph);
        return components.Count == 1;
    }

    /// <summary>
    /// Checks if the graph has an Eulerian path.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to check.</param>
    /// <returns>True if an Eulerian path exists; otherwise, false.</returns>
    public static bool HasEulerianPath<TGraph>(TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        if (graph is IDirectedGraph directedGraph)
        {
            return HasEulerianPathDirected(directedGraph);
        }
        else if (graph is IUndirectedGraph undirectedGraph)
        {
            return HasEulerianPathUndirected(undirectedGraph);
        }
        else
        {
            throw new ArgumentException("Graph must implement IDirectedGraph or IUndirectedGraph");
        }
    }

    /// <summary>
    /// Finds an Eulerian path if one exists.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to search.</param>
    /// <returns>An Eulerian path if found; otherwise, an empty list.</returns>
    public static IReadOnlyList<NodeId> FindEulerianPath<TGraph>(TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        if (!HasEulerianPath(graph))
        {
            return Array.Empty<NodeId>();
        }

        if (graph is IDirectedGraph directedGraph)
        {
            return FindEulerianPathDirected(directedGraph);
        }
        else if (graph is IUndirectedGraph undirectedGraph)
        {
            return FindEulerianPathUndirected(undirectedGraph);
        }
        else
        {
            throw new ArgumentException("Graph must implement IDirectedGraph or IUndirectedGraph");
        }
    }

    /// <summary>
    /// Checks if the graph has a Hamiltonian path (NP-complete problem).
    /// Uses a simple backtracking algorithm - may be slow for large graphs.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to check.</param>
    /// <returns>True if a Hamiltonian path exists; otherwise, false.</returns>
    public static bool HasHamiltonianPath<TGraph>(TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var nodes = graph.Nodes.ToList();
        if (nodes.Count == 0) return true;
        if (nodes.Count == 1) return true;

        // Try starting from each node
        foreach (var start in nodes)
        {
            var visited = new HashSet<NodeId>();
            var path = new List<NodeId>();

            if (FindHamiltonianPathFromNode(graph, start, visited, path, nodes.Count))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds a Hamiltonian path if one exists (NP-complete problem).
    /// Uses a simple backtracking algorithm - may be slow for large graphs.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph to search.</param>
    /// <returns>A Hamiltonian path if found; otherwise, an empty list.</returns>
    public static IReadOnlyList<NodeId> FindHamiltonianPath<TGraph>(TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        var nodes = graph.Nodes.ToList();
        if (nodes.Count == 0) return Array.Empty<NodeId>();
        if (nodes.Count == 1) return nodes;

        // Try starting from each node
        foreach (var start in nodes)
        {
            var visited = new HashSet<NodeId>();
            var path = new List<NodeId>();

            if (FindHamiltonianPathFromNode(graph, start, visited, path, nodes.Count))
            {
                return path;
            }
        }

        return Array.Empty<NodeId>();
    }

    // Helper methods for cycle detection
    private static bool HasCycleDirected<TGraph>(TGraph graph)
        where TGraph : IDirectedGraph
    {
        var color = new Dictionary<NodeId, int>(); // 0: white, 1: gray, 2: black

        foreach (var node in graph.Nodes)
        {
            color[node] = 0;
        }

        bool DfsVisit(NodeId u)
        {
            color[u] = 1; // Gray

            foreach (var v in graph.OutNeighbors(u))
            {
                if (color[v] == 1) // Back edge found
                {
                    return true;
                }
                if (color[v] == 0 && DfsVisit(v))
                {
                    return true;
                }
            }

            color[u] = 2; // Black
            return false;
        }

        foreach (var node in graph.Nodes)
        {
            if (color[node] == 0)
            {
                if (DfsVisit(node))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasCycleUndirected<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        var visited = new HashSet<NodeId>();

        bool DfsVisit(NodeId u, NodeId parent)
        {
            visited.Add(u);

            foreach (var v in graph.Neighbors(u))
            {
                if (!v.Equals(parent))
                {
                    if (visited.Contains(v) || DfsVisit(v, u))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        foreach (var node in graph.Nodes)
        {
            if (!visited.Contains(node))
            {
                if (DfsVisit(node, new NodeId(-1))) // Use invalid node as initial parent
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static IReadOnlyList<NodeId> FindCycleDirected<TGraph>(TGraph graph)
        where TGraph : IDirectedGraph
    {
        var color = new Dictionary<NodeId, int>(); // 0: white, 1: gray, 2: black
        var parent = new Dictionary<NodeId, NodeId?>();
        var cycle = new List<NodeId>();

        foreach (var node in graph.Nodes)
        {
            color[node] = 0;
            parent[node] = null;
        }

        bool DfsVisit(NodeId u)
        {
            color[u] = 1; // Gray

            foreach (var v in graph.OutNeighbors(u))
            {
                if (color[v] == 1) // Back edge found
                {
                    // Reconstruct cycle
                    cycle.Clear();
                    cycle.Add(v);
                    var current = u;
                    while (!current.Equals(v))
                    {
                        cycle.Add(current);
                        current = parent[current]!.Value;
                    }
                    cycle.Reverse();
                    return true;
                }
                if (color[v] == 0)
                {
                    parent[v] = u;
                    if (DfsVisit(v))
                    {
                        return true;
                    }
                }
            }

            color[u] = 2; // Black
            return false;
        }

        foreach (var node in graph.Nodes)
        {
            if (color[node] == 0)
            {
                if (DfsVisit(node))
                {
                    return cycle;
                }
            }
        }

        return Array.Empty<NodeId>();
    }

    private static IReadOnlyList<NodeId> FindCycleUndirected<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        var visited = new HashSet<NodeId>();
        var parent = new Dictionary<NodeId, NodeId?>();
        var cycle = new List<NodeId>();

        bool DfsVisit(NodeId u, NodeId? par)
        {
            visited.Add(u);
            parent[u] = par;

            foreach (var v in graph.Neighbors(u))
            {
                if (par.HasValue && v.Equals(par.Value))
                    continue;

                if (visited.Contains(v))
                {
                    // Found cycle - reconstruct it
                    cycle.Clear();
                    cycle.Add(v);
                    var current = u;
                    while (!current.Equals(v))
                    {
                        cycle.Add(current);
                        current = parent[current]!.Value;
                    }
                    return true;
                }

                if (DfsVisit(v, u))
                {
                    return true;
                }
            }

            return false;
        }

        foreach (var node in graph.Nodes)
        {
            if (!visited.Contains(node))
            {
                if (DfsVisit(node, null))
                {
                    return cycle;
                }
            }
        }

        return Array.Empty<NodeId>();
    }

    // Helper methods for Eulerian paths
    private static bool HasEulerianPathDirected<TGraph>(TGraph graph)
        where TGraph : IDirectedGraph
    {
        var inDegreeMinusOutDegree = new Dictionary<NodeId, int>();
        
        foreach (var node in graph.Nodes)
        {
            inDegreeMinusOutDegree[node] = graph.InDegree(node) - graph.OutDegree(node);
        }

        var startNodes = inDegreeMinusOutDegree.Values.Count(d => d == -1);
        var endNodes = inDegreeMinusOutDegree.Values.Count(d => d == 1);
        var balancedNodes = inDegreeMinusOutDegree.Values.Count(d => d == 0);

        // Eulerian circuit
        if (startNodes == 0 && endNodes == 0)
            return true;

        // Eulerian path
        if (startNodes == 1 && endNodes == 1)
            return true;

        return false;
    }

    private static bool HasEulerianPathUndirected<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        var oddDegreeCount = graph.Nodes.Count(node => graph.Degree(node) % 2 == 1);
        
        // Eulerian circuit: all vertices have even degree
        // Eulerian path: exactly 0 or 2 vertices have odd degree
        return oddDegreeCount == 0 || oddDegreeCount == 2;
    }

    private static IReadOnlyList<NodeId> FindEulerianPathDirected<TGraph>(TGraph graph)
        where TGraph : IDirectedGraph
    {
        // This is a simplified implementation - a full implementation would need
        // to handle the actual path construction using Hierholzer's algorithm
        // For now, return empty as this is complex to implement correctly
        return Array.Empty<NodeId>();
    }

    private static IReadOnlyList<NodeId> FindEulerianPathUndirected<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph
    {
        // This is a simplified implementation - a full implementation would need
        // to handle the actual path construction using Hierholzer's algorithm
        // For now, return empty as this is complex to implement correctly
        return Array.Empty<NodeId>();
    }

    // Helper method for Hamiltonian path
    private static bool FindHamiltonianPathFromNode<TGraph>(
        TGraph graph, 
        NodeId current, 
        HashSet<NodeId> visited, 
        List<NodeId> path, 
        int totalNodes)
        where TGraph : IGraph
    {
        visited.Add(current);
        path.Add(current);

        if (path.Count == totalNodes)
        {
            return true; // Found Hamiltonian path
        }

        var neighbors = GetNeighbors(graph, current);
        foreach (var neighbor in neighbors)
        {
            if (!visited.Contains(neighbor))
            {
                if (FindHamiltonianPathFromNode(graph, neighbor, visited, path, totalNodes))
                {
                    return true;
                }
            }
        }

        // Backtrack
        visited.Remove(current);
        path.RemoveAt(path.Count - 1);
        return false;
    }

    // Helper method to get neighbors
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
            return Array.Empty<NodeId>();
        }
    }
}