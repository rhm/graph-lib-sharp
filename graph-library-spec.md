# Graph Library Software Specification

## Overview

This specification defines a comprehensive graph library for C# that follows the principle of "pay only for what you use". The library provides multiple graph implementations optimized for different use cases, along with a rich set of algorithms and utilities.

## Design Principles

1. **Type Safety**: Leverage C#'s strong typing to prevent runtime errors
2. **Performance**: Minimize memory overhead and computational complexity
3. **Composability**: Support method chaining and LINQ integration
4. **Extensibility**: Allow users to extend functionality through interfaces and extension methods
5. **Zero-Cost Abstractions**: Features that aren't used shouldn't impact performance or memory

## Core Types and Interfaces

### Node and Edge Representations

```csharp
// Basic node identifier
public struct NodeId : IEquatable<NodeId>, IComparable<NodeId>
{
    public int Value { get; }
}

// Edge representation for unweighted graphs
public struct Edge : IEquatable<Edge>
{
    public NodeId Source { get; }
    public NodeId Target { get; }
}

// Edge representation for weighted graphs
public struct WeightedEdge<TWeight> : IEquatable<WeightedEdge<TWeight>>
{
    public NodeId Source { get; }
    public NodeId Target { get; }
    public TWeight Weight { get; }
}
```

### Core Graph Interfaces

```csharp
// Base interface for all graphs
public interface IGraph
{
    int NodeCount { get; }
    int EdgeCount { get; }
    IEnumerable<NodeId> Nodes { get; }
    bool ContainsNode(NodeId node);
    int Degree(NodeId node);
}

// Interface for directed graphs
public interface IDirectedGraph : IGraph
{
    IEnumerable<NodeId> OutNeighbors(NodeId node);
    IEnumerable<NodeId> InNeighbors(NodeId node);
    int OutDegree(NodeId node);
    int InDegree(NodeId node);
    bool HasEdge(NodeId source, NodeId target);
}

// Interface for undirected graphs
public interface IUndirectedGraph : IGraph
{
    IEnumerable<NodeId> Neighbors(NodeId node);
    bool HasEdge(NodeId node1, NodeId node2);
}

// Interface for weighted graphs
public interface IWeightedGraph<TWeight>
{
    TWeight GetEdgeWeight(NodeId source, NodeId target);
    bool TryGetEdgeWeight(NodeId source, NodeId target, out TWeight weight);
}

// Interface for graphs with node data
public interface INodeDataGraph<TNodeData>
{
    TNodeData GetNodeData(NodeId node);
    bool TryGetNodeData(NodeId node, out TNodeData data);
}

// Interface for graphs with edge data
public interface IEdgeDataGraph<TEdgeData>
{
    TEdgeData GetEdgeData(NodeId source, NodeId target);
    bool TryGetEdgeData(NodeId source, NodeId target, out TEdgeData data);
}

// Interface for mutable graphs
public interface IMutableGraph
{
    NodeId AddNode();
    void RemoveNode(NodeId node);
    void Clear();
}

// Interface for mutable directed graphs
public interface IMutableDirectedGraph : IMutableGraph, IDirectedGraph
{
    void AddEdge(NodeId source, NodeId target);
    void RemoveEdge(NodeId source, NodeId target);
}

// Interface for mutable undirected graphs
public interface IMutableUndirectedGraph : IMutableGraph, IUndirectedGraph
{
    void AddEdge(NodeId node1, NodeId node2);
    void RemoveEdge(NodeId node1, NodeId node2);
}
```

## Concrete Graph Implementations

### Basic Graphs

```csharp
// Simple directed graph (adjacency list based)
public class DirectedGraph : IMutableDirectedGraph
{
    // Constructor
    public DirectedGraph();
    public DirectedGraph(int initialCapacity);
    
    // Builder pattern support
    public DirectedGraph WithEdge(NodeId source, NodeId target);
    public DirectedGraph WithEdges(IEnumerable<Edge> edges);
}

// Simple undirected graph
public class UndirectedGraph : IMutableUndirectedGraph
{
    // Similar constructors and builder methods
}

// Weighted directed graph
public class WeightedDirectedGraph<TWeight> : IMutableDirectedGraph, IWeightedGraph<TWeight>
{
    public void AddEdge(NodeId source, NodeId target, TWeight weight);
    public void UpdateEdgeWeight(NodeId source, NodeId target, TWeight weight);
    public WeightedDirectedGraph<TWeight> WithEdge(NodeId source, NodeId target, TWeight weight);
}

// Weighted undirected graph
public class WeightedUndirectedGraph<TWeight> : IMutableUndirectedGraph, IWeightedGraph<TWeight>
{
    // Similar methods for weighted edges
}
```

### Graphs with Associated Data

```csharp
// Directed graph with node data
public class DirectedGraphWithNodeData<TNodeData> : IMutableDirectedGraph, INodeDataGraph<TNodeData>
{
    public NodeId AddNode(TNodeData data);
    public void SetNodeData(NodeId node, TNodeData data);
}

// Directed graph with both node and edge data
public class DirectedGraphWithData<TNodeData, TEdgeData> : 
    IMutableDirectedGraph, 
    INodeDataGraph<TNodeData>, 
    IEdgeDataGraph<TEdgeData>
{
    public NodeId AddNode(TNodeData nodeData);
    public void AddEdge(NodeId source, NodeId target, TEdgeData edgeData);
    public void SetNodeData(NodeId node, TNodeData data);
    public void SetEdgeData(NodeId source, NodeId target, TEdgeData data);
}

// Undirected versions
public class UndirectedGraphWithNodeData<TNodeData> : IMutableUndirectedGraph, INodeDataGraph<TNodeData>
{
    // Similar to directed version
}

public class UndirectedGraphWithData<TNodeData, TEdgeData> : 
    IMutableUndirectedGraph, 
    INodeDataGraph<TNodeData>, 
    IEdgeDataGraph<TEdgeData>
{
    // Similar to directed version
}
```

### Specialized Graphs

```csharp
// Bipartite graph
public class BipartiteGraph : IUndirectedGraph
{
    public enum NodeSet { Left, Right }
    
    public BipartiteGraph();
    public NodeId AddNodeToSet(NodeSet set);
    public void AddEdge(NodeId leftNode, NodeId rightNode);
    public NodeSet GetNodeSet(NodeId node);
    public IEnumerable<NodeId> GetNodesInSet(NodeSet set);
    public bool IsBipartite { get; } // Always true for this type
}

// Dense graph implementation using adjacency matrix
public class DenseDirectedGraph : IMutableDirectedGraph
{
    public DenseDirectedGraph(int nodeCount);
    // Optimized for dense graphs with O(1) edge lookups
}

// Immutable graph wrapper
public class ImmutableGraphView<TGraph> : IGraph where TGraph : IGraph
{
    public ImmutableGraphView(TGraph graph);
    // Provides read-only view of underlying graph
}
```

## Subgraph Support

```csharp
// Subgraph that shares data with parent graph
public class SubgraphView<TGraph> : IGraph where TGraph : IGraph
{
    public SubgraphView(TGraph parentGraph, Predicate<NodeId> nodeFilter, Predicate<Edge> edgeFilter);
    public TGraph ParentGraph { get; }
    // All operations are filtered through predicates
}

// Extension methods for creating subgraphs
public static class SubgraphExtensions
{
    public static SubgraphView<TGraph> Subgraph<TGraph>(
        this TGraph graph, 
        Predicate<NodeId> nodeFilter, 
        Predicate<Edge> edgeFilter = null) 
        where TGraph : IGraph;
    
    public static SubgraphView<TGraph> InducedSubgraph<TGraph>(
        this TGraph graph, 
        IEnumerable<NodeId> nodes) 
        where TGraph : IGraph;
}
```

## Graph Algorithms

### Path Finding

```csharp
public static class ShortestPath
{
    // Dijkstra's algorithm for weighted graphs
    public static PathResult<TWeight> Dijkstra<TGraph, TWeight>(
        TGraph graph,
        NodeId source,
        NodeId target,
        IComparer<TWeight> comparer = null)
        where TGraph : IDirectedGraph, IWeightedGraph<TWeight>;
    
    // A* algorithm
    public static PathResult<TWeight> AStar<TGraph, TWeight>(
        TGraph graph,
        NodeId source,
        NodeId target,
        Func<NodeId, NodeId, TWeight> heuristic,
        IComparer<TWeight> comparer = null)
        where TGraph : IDirectedGraph, IWeightedGraph<TWeight>;
    
    // Bellman-Ford for graphs with negative weights
    public static PathResult<TWeight> BellmanFord<TGraph, TWeight>(
        TGraph graph,
        NodeId source,
        NodeId target)
        where TGraph : IDirectedGraph, IWeightedGraph<TWeight>;
    
    // BFS for unweighted graphs
    public static PathResult<int> BreadthFirstSearch<TGraph>(
        TGraph graph,
        NodeId source,
        NodeId target)
        where TGraph : IGraph;
    
    // All pairs shortest paths
    public static Dictionary<(NodeId, NodeId), TWeight> FloydWarshall<TGraph, TWeight>(
        TGraph graph)
        where TGraph : IDirectedGraph, IWeightedGraph<TWeight>;
}

// Result type for path queries
public class PathResult<TWeight>
{
    public bool PathExists { get; }
    public IReadOnlyList<NodeId> Path { get; }
    public TWeight TotalWeight { get; }
}
```

### Spanning Trees

```csharp
public static class SpanningTree
{
    // Minimum spanning tree using Kruskal's algorithm
    public static SpanningTreeResult<TWeight> Kruskal<TGraph, TWeight>(
        TGraph graph,
        IComparer<TWeight> comparer = null)
        where TGraph : IUndirectedGraph, IWeightedGraph<TWeight>;
    
    // Minimum spanning tree using Prim's algorithm
    public static SpanningTreeResult<TWeight> Prim<TGraph, TWeight>(
        TGraph graph,
        NodeId startNode,
        IComparer<TWeight> comparer = null)
        where TGraph : IUndirectedGraph, IWeightedGraph<TWeight>;
    
    // Maximum spanning tree
    public static SpanningTreeResult<TWeight> MaximumSpanningTree<TGraph, TWeight>(
        TGraph graph,
        IComparer<TWeight> comparer = null)
        where TGraph : IUndirectedGraph, IWeightedGraph<TWeight>;
}

public class SpanningTreeResult<TWeight>
{
    public IReadOnlyList<WeightedEdge<TWeight>> Edges { get; }
    public TWeight TotalWeight { get; }
    public SubgraphView<IUndirectedGraph> AsSubgraph(IUndirectedGraph originalGraph);
}
```

### Flow Algorithms

```csharp
public static class NetworkFlow
{
    // Ford-Fulkerson max flow
    public static FlowResult<TCapacity> MaxFlow<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Func<TCapacity, TCapacity, TCapacity> add,
        Func<TCapacity, TCapacity, TCapacity> subtract,
        IComparer<TCapacity> comparer = null)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>;
    
    // Edmonds-Karp implementation
    public static FlowResult<TCapacity> EdmondsKarp<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Func<TCapacity, TCapacity, TCapacity> add,
        Func<TCapacity, TCapacity, TCapacity> subtract,
        IComparer<TCapacity> comparer = null)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>;
    
    // Min-cut
    public static CutResult<TCapacity> MinCut<TGraph, TCapacity>(
        TGraph graph,
        NodeId source,
        NodeId sink,
        Func<TCapacity, TCapacity, TCapacity> add,
        IComparer<TCapacity> comparer = null)
        where TGraph : IDirectedGraph, IWeightedGraph<TCapacity>;
}

public class FlowResult<TCapacity>
{
    public TCapacity MaxFlow { get; }
    public Dictionary<Edge, TCapacity> EdgeFlows { get; }
}

public class CutResult<TCapacity>
{
    public TCapacity CutCapacity { get; }
    public IReadOnlyList<Edge> CutEdges { get; }
    public IReadOnlyList<NodeId> SourceSideNodes { get; }
    public IReadOnlyList<NodeId> SinkSideNodes { get; }
}
```

### Graph Isomorphism

```csharp
public static class Isomorphism
{
    // Check if two graphs are isomorphic
    public static bool AreIsomorphic<TGraph1, TGraph2>(
        TGraph1 graph1,
        TGraph2 graph2)
        where TGraph1 : IGraph
        where TGraph2 : IGraph;
    
    // Find isomorphism mapping if exists
    public static IsomorphismResult FindIsomorphism<TGraph1, TGraph2>(
        TGraph1 graph1,
        TGraph2 graph2)
        where TGraph1 : IGraph
        where TGraph2 : IGraph;
    
    // Subgraph isomorphism
    public static bool ContainsSubgraphIsomorphicTo<TGraph1, TGraph2>(
        TGraph1 graph,
        TGraph2 pattern)
        where TGraph1 : IGraph
        where TGraph2 : IGraph;
}

public class IsomorphismResult
{
    public bool IsIsomorphic { get; }
    public IReadOnlyDictionary<NodeId, NodeId> NodeMapping { get; }
}
```

### Clique Finding

```csharp
public static class CliqueFinding
{
    // Find maximum clique
    public static IReadOnlyList<NodeId> FindMaximumClique<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph;
    
    // Find all maximal cliques
    public static IEnumerable<IReadOnlyList<NodeId>> FindAllMaximalCliques<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph;
    
    // Find cliques of specific size
    public static IEnumerable<IReadOnlyList<NodeId>> FindCliquesOfSize<TGraph>(
        TGraph graph,
        int size)
        where TGraph : IUndirectedGraph;
    
    // Check if graph has clique of given size
    public static bool HasCliqueOfSize<TGraph>(TGraph graph, int size)
        where TGraph : IUndirectedGraph;
}
```

### Additional Algorithms

```csharp
public static class GraphAlgorithms
{
    // Topological sort
    public static TopologicalSortResult TopologicalSort<TGraph>(TGraph graph)
        where TGraph : IDirectedGraph;
    
    // Strongly connected components
    public static IReadOnlyList<IReadOnlyList<NodeId>> StronglyConnectedComponents<TGraph>(TGraph graph)
        where TGraph : IDirectedGraph;
    
    // Connected components for undirected graphs
    public static IReadOnlyList<IReadOnlyList<NodeId>> ConnectedComponents<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph;
    
    // Cycle detection
    public static bool HasCycle<TGraph>(TGraph graph)
        where TGraph : IGraph;
    
    public static IReadOnlyList<NodeId> FindCycle<TGraph>(TGraph graph)
        where TGraph : IGraph;
    
    // Graph coloring
    public static ColoringResult GreedyColoring<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph;
    
    // Bipartite checking
    public static BipartiteCheckResult IsBipartite<TGraph>(TGraph graph)
        where TGraph : IUndirectedGraph;
    
    // Euler path/circuit
    public static bool HasEulerianPath<TGraph>(TGraph graph)
        where TGraph : IGraph;
    
    public static IReadOnlyList<NodeId> FindEulerianPath<TGraph>(TGraph graph)
        where TGraph : IGraph;
    
    // Hamiltonian path/circuit
    public static bool HasHamiltonianPath<TGraph>(TGraph graph)
        where TGraph : IGraph;
    
    public static IReadOnlyList<NodeId> FindHamiltonianPath<TGraph>(TGraph graph)
        where TGraph : IGraph;
}

public class TopologicalSortResult
{
    public bool IsAcyclic { get; }
    public IReadOnlyList<NodeId> Order { get; }
}

public class ColoringResult
{
    public int ChromaticNumber { get; }
    public IReadOnlyDictionary<NodeId, int> NodeColors { get; }
}

public class BipartiteCheckResult
{
    public bool IsBipartite { get; }
    public IReadOnlyList<NodeId> LeftSet { get; }
    public IReadOnlyList<NodeId> RightSet { get; }
}
```

### Graph Traversal

```csharp
public static class GraphTraversal
{
    // Depth-first search with visitor pattern
    public static void DepthFirstSearch<TGraph>(
        TGraph graph,
        NodeId start,
        IGraphVisitor visitor)
        where TGraph : IGraph;
    
    // Breadth-first search with visitor pattern
    public static void BreadthFirstSearch<TGraph>(
        TGraph graph,
        NodeId start,
        IGraphVisitor visitor)
        where TGraph : IGraph;
    
    // Iterate nodes in DFS order
    public static IEnumerable<NodeId> DepthFirstNodes<TGraph>(
        TGraph graph,
        NodeId start)
        where TGraph : IGraph;
    
    // Iterate nodes in BFS order
    public static IEnumerable<NodeId> BreadthFirstNodes<TGraph>(
        TGraph graph,
        NodeId start)
        where TGraph : IGraph;
}

public interface IGraphVisitor
{
    void DiscoverNode(NodeId node);
    void FinishNode(NodeId node);
    void ExamineEdge(NodeId source, NodeId target);
    void TreeEdge(NodeId source, NodeId target);
    void BackEdge(NodeId source, NodeId target);
    void ForwardEdge(NodeId source, NodeId target);
    void CrossEdge(NodeId source, NodeId target);
}
```

## LINQ Integration

```csharp
public static class GraphLinqExtensions
{
    // Filter nodes
    public static IEnumerable<NodeId> Where<TGraph>(
        this TGraph graph,
        Func<NodeId, bool> predicate)
        where TGraph : IGraph;
    
    // Project nodes
    public static IEnumerable<TResult> Select<TGraph, TResult>(
        this TGraph graph,
        Func<NodeId, TResult> selector)
        where TGraph : IGraph;
    
    // Get nodes with specific degree
    public static IEnumerable<NodeId> WhereDegreee<TGraph>(
        this TGraph graph,
        Func<int, bool> degreePredicate)
        where TGraph : IGraph;
    
    // Order nodes by property
    public static IOrderedEnumerable<NodeId> OrderByDegree<TGraph>(
        this TGraph graph)
        where TGraph : IGraph;
    
    // Group nodes by property
    public static IEnumerable<IGrouping<TKey, NodeId>> GroupBy<TGraph, TKey>(
        this TGraph graph,
        Func<NodeId, TKey> keySelector)
        where TGraph : IGraph;
}
```

## Graph Builders

```csharp
// Fluent builder for complex graph construction
public class GraphBuilder<TGraph> where TGraph : IMutableGraph, new()
{
    public GraphBuilder<TGraph> AddNode();
    public GraphBuilder<TGraph> AddNodes(int count);
    public GraphBuilder<TGraph> AddEdge(NodeId source, NodeId target);
    public GraphBuilder<TGraph> AddEdges(IEnumerable<Edge> edges);
    public GraphBuilder<TGraph> AddPath(params NodeId[] nodes);
    public GraphBuilder<TGraph> AddCycle(params NodeId[] nodes);
    public GraphBuilder<TGraph> AddClique(IEnumerable<NodeId> nodes);
    public TGraph Build();
}

// Factory methods for common graph structures
public static class GraphFactory
{
    public static TGraph Complete<TGraph>(int nodeCount) where TGraph : IMutableGraph, new();
    public static TGraph Cycle<TGraph>(int nodeCount) where TGraph : IMutableGraph, new();
    public static TGraph Path<TGraph>(int nodeCount) where TGraph : IMutableGraph, new();
    public static TGraph Star<TGraph>(int nodeCount) where TGraph : IMutableGraph, new();
    public static TGraph Grid<TGraph>(int rows, int cols) where TGraph : IMutableGraph, new();
    public static TGraph Random<TGraph>(int nodeCount, double edgeProbability, int seed = 0) 
        where TGraph : IMutableGraph, new();
}
```

## Serialization Support

```csharp
public interface IGraphSerializer<TGraph> where TGraph : IGraph
{
    void Serialize(TGraph graph, Stream stream);
    TGraph Deserialize(Stream stream);
}

public class GraphSerializers
{
    // Built-in serializers
    public static IGraphSerializer<TGraph> Json<TGraph>() where TGraph : IGraph, new();
    public static IGraphSerializer<TGraph> Binary<TGraph>() where TGraph : IGraph, new();
    public static IGraphSerializer<TGraph> GraphML<TGraph>() where TGraph : IGraph, new();
    public static IGraphSerializer<TGraph> Dot<TGraph>() where TGraph : IGraph, new();
}
```

## Performance Considerations

### Memory Layout
- `DirectedGraph` and `UndirectedGraph` use adjacency lists (sparse graphs)
- `DenseDirectedGraph` uses adjacency matrix (dense graphs)
- Node IDs are sequential integers for cache efficiency
- Subgraphs share underlying data structures with parent graphs

### Algorithm Complexity
- Dijkstra: O((V + E) log V) with binary heap
- A*: O((V + E) log V) with admissible heuristic
- BFS/DFS: O(V + E)
- Kruskal: O(E log E)
- Prim: O((V + E) log V)
- Max Flow: O(V²E) for Edmonds-Karp
- Clique Finding: Exponential worst case

### Extension Points
- Custom node/edge data types
- Custom weight types (must support comparison and arithmetic operations)
- Custom visitors for traversal algorithms
- Custom serialization formats

## Usage Examples

```csharp
// Example 1: Simple directed graph
var graph = new DirectedGraph()
    .WithEdge(0, 1)
    .WithEdge(1, 2)
    .WithEdge(2, 3);

// Example 2: Weighted graph with Dijkstra
var weighted = new WeightedDirectedGraph<double>();
weighted.AddEdge(0, 1, 4.0);
weighted.AddEdge(0, 2, 2.0);
var path = ShortestPath.Dijkstra(weighted, 0, 2);

// Example 3: Subgraph filtering
var subgraph = graph.Subgraph(
    node => node.Value < 10,
    edge => edge.Source.Value != 5
);

// Example 4: LINQ integration
var highDegreeNodes = graph
    .Where(node => graph.Degree(node) > 3)
    .OrderByDegree()
    .Take(10);
```