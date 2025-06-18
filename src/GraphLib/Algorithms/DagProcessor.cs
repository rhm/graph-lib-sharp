using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Algorithms;

/// <summary>
/// Provides functionality for processing directed acyclic graphs (DAGs) by tracking nodes with zero incoming edges
/// and allowing logical removal of nodes while maintaining the leading set of nodes.
/// </summary>
public class DagProcessor
{
    private readonly IDirectedGraph _graph;
    private readonly Dictionary<NodeId, int> _incomingEdgeCounts;
    private readonly HashSet<NodeId> _leadingNodes;
    private readonly HashSet<NodeId> _newLeadingNodes;
    private readonly HashSet<NodeId> _processedNodes;

    /// <summary>
    /// Gets the current set of nodes with zero incoming edges (not yet processed).
    /// </summary>
    public IReadOnlySet<NodeId> LeadingNodes => _leadingNodes;

    /// <summary>
    /// Gets the set of nodes that were newly added to the leading set by the most recent RemoveNode operation.
    /// </summary>
    public IReadOnlySet<NodeId> NewLeadingNodes => _newLeadingNodes;

    /// <summary>
    /// Initializes a new instance of the DagProcessor class with the specified directed graph.
    /// </summary>
    /// <param name="graph">The directed acyclic graph to process.</param>
    /// <exception cref="ArgumentNullException">Thrown when graph is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the graph contains cycles (is not a DAG).</exception>
    public DagProcessor(IDirectedGraph graph)
    {
        _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        _incomingEdgeCounts = new Dictionary<NodeId, int>();
        _leadingNodes = new HashSet<NodeId>();
        _newLeadingNodes = new HashSet<NodeId>();
        _processedNodes = new HashSet<NodeId>();

        Initialize();
        ValidateIsAcyclic();
    }

    /// <summary>
    /// Logically removes a node from processing, updating the leading set based on edge dependencies.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    /// <returns>True if the node was successfully removed; false if it was already processed or not in the leading set.</returns>
    public bool RemoveNode(NodeId node)
    {
        // Clear new leading nodes from previous operation
        _newLeadingNodes.Clear();

        // Check if node can be removed
        if (!_leadingNodes.Contains(node))
        {
            return false;
        }

        // Remove from leading set and mark as processed
        _leadingNodes.Remove(node);
        _processedNodes.Add(node);

        // Update incoming edge counts for all out-neighbors
        foreach (var neighbor in _graph.OutNeighbors(node))
        {
            // Skip already processed nodes
            if (_processedNodes.Contains(neighbor))
            {
                continue;
            }

            // Decrement incoming edge count
            _incomingEdgeCounts[neighbor]--;

            // If this neighbor now has zero incoming edges, add to leading set
            if (_incomingEdgeCounts[neighbor] == 0)
            {
                _leadingNodes.Add(neighbor);
                _newLeadingNodes.Add(neighbor);
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether a node has been processed (logically removed).
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>True if the node has been processed; otherwise, false.</returns>
    public bool IsProcessed(NodeId node)
    {
        return _processedNodes.Contains(node);
    }

    /// <summary>
    /// Resets the processor to its initial state, clearing all processed nodes and recalculating the leading set.
    /// </summary>
    public void Reset()
    {
        _processedNodes.Clear();
        _newLeadingNodes.Clear();
        Initialize();
    }

    /// <summary>
    /// Gets a topological ordering of all nodes by processing them in dependency order.
    /// This method does not modify the current state of the processor.
    /// </summary>
    /// <returns>An enumerable of nodes in topological order.</returns>
    public IEnumerable<NodeId> GetTopologicalOrder()
    {
        // Create a copy of current state
        var tempIncomingCounts = new Dictionary<NodeId, int>(_incomingEdgeCounts);
        var tempLeadingNodes = new HashSet<NodeId>(_leadingNodes);
        var tempProcessedNodes = new HashSet<NodeId>(_processedNodes);
        var result = new List<NodeId>();

        // Process all nodes
        while (tempLeadingNodes.Count > 0)
        {
            var node = tempLeadingNodes.First();
            tempLeadingNodes.Remove(node);
            
            // Skip if already processed in the actual processor
            if (!tempProcessedNodes.Contains(node))
            {
                result.Add(node);
                tempProcessedNodes.Add(node);

                // Update counts for neighbors
                foreach (var neighbor in _graph.OutNeighbors(node))
                {
                    if (tempProcessedNodes.Contains(neighbor))
                    {
                        continue;
                    }

                    tempIncomingCounts[neighbor]--;
                    if (tempIncomingCounts[neighbor] == 0)
                    {
                        tempLeadingNodes.Add(neighbor);
                    }
                }
            }
        }

        return result;
    }

    private void Initialize()
    {
        _incomingEdgeCounts.Clear();
        _leadingNodes.Clear();

        // Initialize all nodes with zero incoming edges
        foreach (var node in _graph.Nodes)
        {
            _incomingEdgeCounts[node] = 0;
        }

        // Count incoming edges for each node
        foreach (var node in _graph.Nodes)
        {
            foreach (var neighbor in _graph.OutNeighbors(node))
            {
                _incomingEdgeCounts[neighbor]++;
            }
        }

        // Find initial leading nodes (zero incoming edges and not processed)
        foreach (var kvp in _incomingEdgeCounts)
        {
            if (kvp.Value == 0 && !_processedNodes.Contains(kvp.Key))
            {
                _leadingNodes.Add(kvp.Key);
            }
        }
    }

    private void ValidateIsAcyclic()
    {
        // Perform a simple cycle detection using the topological sort approach
        var tempIncomingCounts = new Dictionary<NodeId, int>(_incomingEdgeCounts);
        var tempLeadingNodes = new HashSet<NodeId>(_leadingNodes);
        var processedCount = 0;

        while (tempLeadingNodes.Count > 0)
        {
            var node = tempLeadingNodes.First();
            tempLeadingNodes.Remove(node);
            processedCount++;

            foreach (var neighbor in _graph.OutNeighbors(node))
            {
                tempIncomingCounts[neighbor]--;
                if (tempIncomingCounts[neighbor] == 0)
                {
                    tempLeadingNodes.Add(neighbor);
                }
            }
        }

        // If we couldn't process all nodes, there's a cycle
        if (processedCount < _graph.NodeCount)
        {
            throw new ArgumentException("The provided graph contains cycles and is not a valid DAG.");
        }
    }
}