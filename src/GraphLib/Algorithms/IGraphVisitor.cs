using GraphLib.Core;

namespace GraphLib.Algorithms;

/// <summary>
/// Interface for implementing the visitor pattern in graph traversal algorithms.
/// </summary>
public interface IGraphVisitor
{
    /// <summary>
    /// Called when a node is first discovered during traversal.
    /// </summary>
    /// <param name="node">The discovered node.</param>
    void DiscoverNode(NodeId node);

    /// <summary>
    /// Called when all neighbors of a node have been explored.
    /// </summary>
    /// <param name="node">The finished node.</param>
    void FinishNode(NodeId node);

    /// <summary>
    /// Called when an edge is examined during traversal.
    /// </summary>
    /// <param name="source">The source node of the edge.</param>
    /// <param name="target">The target node of the edge.</param>
    void ExamineEdge(NodeId source, NodeId target);

    /// <summary>
    /// Called when an edge leads to an undiscovered node (tree edge).
    /// </summary>
    /// <param name="source">The source node of the edge.</param>
    /// <param name="target">The target node of the edge.</param>
    void TreeEdge(NodeId source, NodeId target);

    /// <summary>
    /// Called when an edge leads to an ancestor in the DFS tree (back edge).
    /// </summary>
    /// <param name="source">The source node of the edge.</param>
    /// <param name="target">The target node of the edge.</param>
    void BackEdge(NodeId source, NodeId target);

    /// <summary>
    /// Called when an edge leads to a descendant in the DFS tree (forward edge).
    /// </summary>
    /// <param name="source">The source node of the edge.</param>
    /// <param name="target">The target node of the edge.</param>
    void ForwardEdge(NodeId source, NodeId target);

    /// <summary>
    /// Called when an edge leads to a node in a different subtree (cross edge).
    /// </summary>
    /// <param name="source">The source node of the edge.</param>
    /// <param name="target">The target node of the edge.</param>
    void CrossEdge(NodeId source, NodeId target);
}