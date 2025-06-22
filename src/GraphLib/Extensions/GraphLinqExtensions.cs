using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Extensions;

/// <summary>
/// LINQ extension methods for graphs.
/// </summary>
public static class GraphLinqExtensions
{
    /// <summary>
    /// Filters nodes based on a predicate.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="predicate">The predicate to filter nodes.</param>
    /// <returns>An enumerable of nodes that satisfy the predicate.</returns>
    public static IEnumerable<NodeId> Where<TGraph>(
        this TGraph graph,
        Func<NodeId, bool> predicate)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return graph.Nodes.Where(predicate);
    }

    /// <summary>
    /// Projects each node to a new form.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <typeparam name="TResult">The type of the projected result.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="selector">The selector function to apply to each node.</param>
    /// <returns>An enumerable of projected values.</returns>
    public static IEnumerable<TResult> Select<TGraph, TResult>(
        this TGraph graph,
        Func<NodeId, TResult> selector)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        return graph.Nodes.Select(selector);
    }

    /// <summary>
    /// Projects each node to a new form based on the node and its degree.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <typeparam name="TResult">The type of the projected result.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="selector">The selector function that takes a node and its degree.</param>
    /// <returns>An enumerable of projected values.</returns>
    public static IEnumerable<TResult> Select<TGraph, TResult>(
        this TGraph graph,
        Func<NodeId, int, TResult> selector)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (selector == null) throw new ArgumentNullException(nameof(selector));

        return graph.Nodes.Select(node => selector(node, graph.Degree(node)));
    }

    /// <summary>
    /// Filters nodes based on their degree.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="degreePredicate">The predicate to apply to node degrees.</param>
    /// <returns>An enumerable of nodes whose degrees satisfy the predicate.</returns>
    public static IEnumerable<NodeId> WhereDegree<TGraph>(
        this TGraph graph,
        Func<int, bool> degreePredicate)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (degreePredicate == null) throw new ArgumentNullException(nameof(degreePredicate));

        return graph.Nodes.Where(node => degreePredicate(graph.Degree(node)));
    }

    /// <summary>
    /// Filters directed graph nodes based on their out-degree.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The directed graph.</param>
    /// <param name="outDegreePredicate">The predicate to apply to node out-degrees.</param>
    /// <returns>An enumerable of nodes whose out-degrees satisfy the predicate.</returns>
    public static IEnumerable<NodeId> WhereOutDegree<TGraph>(
        this TGraph graph,
        Func<int, bool> outDegreePredicate)
        where TGraph : IDirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (outDegreePredicate == null) throw new ArgumentNullException(nameof(outDegreePredicate));

        return graph.Nodes.Where(node => outDegreePredicate(graph.OutDegree(node)));
    }

    /// <summary>
    /// Filters directed graph nodes based on their in-degree.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The directed graph.</param>
    /// <param name="inDegreePredicate">The predicate to apply to node in-degrees.</param>
    /// <returns>An enumerable of nodes whose in-degrees satisfy the predicate.</returns>
    public static IEnumerable<NodeId> WhereInDegree<TGraph>(
        this TGraph graph,
        Func<int, bool> inDegreePredicate)
        where TGraph : IDirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (inDegreePredicate == null) throw new ArgumentNullException(nameof(inDegreePredicate));

        return graph.Nodes.Where(node => inDegreePredicate(graph.InDegree(node)));
    }

    /// <summary>
    /// Orders nodes by their degree in ascending order.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <returns>An ordered enumerable of nodes sorted by degree.</returns>
    public static IOrderedEnumerable<NodeId> OrderByDegree<TGraph>(
        this TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        return graph.Nodes.OrderBy(node => graph.Degree(node));
    }

    /// <summary>
    /// Orders nodes by their degree in descending order.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <returns>An ordered enumerable of nodes sorted by degree in descending order.</returns>
    public static IOrderedEnumerable<NodeId> OrderByDegreeDescending<TGraph>(
        this TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        return graph.Nodes.OrderByDescending(node => graph.Degree(node));
    }

    /// <summary>
    /// Orders directed graph nodes by their out-degree in ascending order.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The directed graph.</param>
    /// <returns>An ordered enumerable of nodes sorted by out-degree.</returns>
    public static IOrderedEnumerable<NodeId> OrderByOutDegree<TGraph>(
        this TGraph graph)
        where TGraph : IDirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        return graph.Nodes.OrderBy(node => graph.OutDegree(node));
    }

    /// <summary>
    /// Orders directed graph nodes by their in-degree in ascending order.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The directed graph.</param>
    /// <returns>An ordered enumerable of nodes sorted by in-degree.</returns>
    public static IOrderedEnumerable<NodeId> OrderByInDegree<TGraph>(
        this TGraph graph)
        where TGraph : IDirectedGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        return graph.Nodes.OrderBy(node => graph.InDegree(node));
    }

    /// <summary>
    /// Groups nodes by their degree.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <returns>An enumerable of groups where each group contains nodes with the same degree.</returns>
    public static IEnumerable<IGrouping<int, NodeId>> GroupByDegree<TGraph>(
        this TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        return graph.Nodes.GroupBy(node => graph.Degree(node));
    }

    /// <summary>
    /// Groups nodes by a key derived from the node.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="keySelector">The function to extract the key from each node.</param>
    /// <returns>An enumerable of groups where each group contains nodes with the same key.</returns>
    public static IEnumerable<IGrouping<TKey, NodeId>> GroupBy<TGraph, TKey>(
        this TGraph graph,
        Func<NodeId, TKey> keySelector)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        return graph.Nodes.GroupBy(keySelector);
    }

    /// <summary>
    /// Gets nodes with the minimum degree in the graph.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <returns>An enumerable of nodes with the minimum degree.</returns>
    public static IEnumerable<NodeId> MinDegreeNodes<TGraph>(
        this TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        if (!graph.Nodes.Any())
            return Enumerable.Empty<NodeId>();

        var minDegree = graph.Nodes.Min(node => graph.Degree(node));
        return graph.Nodes.Where(node => graph.Degree(node) == minDegree);
    }

    /// <summary>
    /// Gets nodes with the maximum degree in the graph.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <returns>An enumerable of nodes with the maximum degree.</returns>
    public static IEnumerable<NodeId> MaxDegreeNodes<TGraph>(
        this TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        if (!graph.Nodes.Any())
            return Enumerable.Empty<NodeId>();

        var maxDegree = graph.Nodes.Max(node => graph.Degree(node));
        return graph.Nodes.Where(node => graph.Degree(node) == maxDegree);
    }

    /// <summary>
    /// Gets the degree sequence of the graph (sorted in descending order).
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <returns>An enumerable of degrees sorted in descending order.</returns>
    public static IEnumerable<int> DegreeSequence<TGraph>(
        this TGraph graph)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        return graph.Nodes.Select(node => graph.Degree(node)).OrderByDescending(d => d);
    }

    /// <summary>
    /// Checks if any node satisfies the specified condition.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>True if any node satisfies the condition; otherwise, false.</returns>
    public static bool AnyNode<TGraph>(
        this TGraph graph,
        Func<NodeId, bool> predicate)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return graph.Nodes.Any(predicate);
    }

    /// <summary>
    /// Checks if all nodes satisfy the specified condition.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>True if all nodes satisfy the condition; otherwise, false.</returns>
    public static bool AllNodes<TGraph>(
        this TGraph graph,
        Func<NodeId, bool> predicate)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return graph.Nodes.All(predicate);
    }

    /// <summary>
    /// Counts the nodes that satisfy the specified condition.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>The number of nodes that satisfy the condition.</returns>
    public static int CountNodes<TGraph>(
        this TGraph graph,
        Func<NodeId, bool> predicate)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return graph.Nodes.Count(predicate);
    }

    /// <summary>
    /// Gets the first node that satisfies the specified condition.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>The first node that satisfies the condition.</returns>
    /// <exception cref="InvalidOperationException">No node satisfies the condition.</exception>
    public static NodeId FirstNode<TGraph>(
        this TGraph graph,
        Func<NodeId, bool> predicate)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return graph.Nodes.First(predicate);
    }

    /// <summary>
    /// Gets the first node that satisfies the specified condition, or a default value if no such node exists.
    /// </summary>
    /// <typeparam name="TGraph">The type of the graph.</typeparam>
    /// <param name="graph">The graph.</param>
    /// <param name="predicate">The condition to check.</param>
    /// <returns>The first node that satisfies the condition, or default if no such node exists.</returns>
    public static NodeId FirstNodeOrDefault<TGraph>(
        this TGraph graph,
        Func<NodeId, bool> predicate)
        where TGraph : IGraph
    {
        if (graph == null) throw new ArgumentNullException(nameof(graph));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));

        return graph.Nodes.FirstOrDefault(predicate);
    }
}