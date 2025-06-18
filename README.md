# GraphLib Sharp

A comprehensive and high-performance graph data structure library for .NET.

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/tests-364%20passing-brightgreen.svg)](src/GraphLib.Tests)

## Features

- **Multiple Graph Types**: Choose the right implementation for your use case
- **Type Safety**: Leverages C#'s strong typing to prevent runtime errors
- **High Performance**: Optimized data structures with minimal memory overhead
- **LINQ Integration**: Native support for LINQ queries and method chaining
- **Fluent API**: Intuitive builder patterns for graph construction
- **Comprehensive**: Support for weighted graphs, node/edge data, and specialized structures

## Quick Start

```csharp
using GraphLib;
using GraphLib.Implementations;
using GraphLib.Builders;

// Create a simple directed graph
var graph = new DirectedGraph();
var node1 = graph.AddNode();
var node2 = graph.AddNode();
graph.AddEdge(node1, node2);

// Use the fluent builder API
var triangle = GraphBuilder.Create<UndirectedGraph>()
    .AddNodes(3)
    .AddClique(new[] { 0, 1, 2 })
    .Build();

// Create graphs with the factory
var path = GraphFactory.Path<DirectedGraph>(5);
var complete = GraphFactory.Complete<UndirectedGraph>(4);
```

## Graph Types

### Core Implementations
- **`DirectedGraph`** - Basic directed graph with adjacency list
- **`UndirectedGraph`** - Basic undirected graph with symmetric edges
- **`DenseDirectedGraph`** - Matrix-based implementation for dense graphs
- **`BipartiteGraph`** - Specialized graph with two disjoint node sets

### Weighted Graphs
- **`WeightedDirectedGraph<TWeight>`** - Directed graph with edge weights
- **`WeightedUndirectedGraph<TWeight>`** - Undirected graph with edge weights

### Graphs with Data
- **`DirectedGraphWithNodeData<TNodeData>`** - Directed graph with node-associated data
- **`DirectedGraphWithData<TNodeData, TEdgeData>`** - Directed graph with both node and edge data
- **`UndirectedGraphWithData<TNodeData, TEdgeData>`** - Undirected graph with both node and edge data

### Advanced Features
- **`SubgraphView<TGraph>`** - Live filtered view of a parent graph
- **`ImmutableGraphView<TGraph>`** - Read-only wrapper for any graph

## Installation

```bash
# Clone the repository
git clone https://github.com/your-username/graph-lib-sharp.git
cd graph-lib-sharp

# Build the library
dotnet build src/GraphLib.sln

# Run tests
dotnet test src/GraphLib.Tests
```

## Examples

### Working with Weighted Graphs

```csharp
var graph = new WeightedDirectedGraph<double>();
var source = graph.AddNode();
var target = graph.AddNode();

graph.AddEdge(source, target, 3.14);
double weight = graph.GetEdgeWeight(source, target);

// Fluent interface
graph.WithEdge(source, target, 2.71)
     .WithEdge(target, source, 1.41);
```

### Using Node and Edge Data

```csharp
var graph = new DirectedGraphWithData<string, int>();
var alice = graph.AddNode("Alice");
var bob = graph.AddNode("Bob");

graph.AddEdge(alice, bob, 42); // Edge with data

string aliceData = graph.GetNodeData(alice);
int edgeData = graph.GetEdgeData(alice, bob);
```

### Creating Subgraphs

```csharp
var original = GraphFactory.Complete<DirectedGraph>(5);

// Create a subgraph with only even-numbered nodes
var evenNodes = original.DirectedSubgraph(
    nodeFilter: node => node.Value % 2 == 0,
    edgeFilter: edge => edge.Source.Value % 2 == 0 && edge.Target.Value % 2 == 0
);

Console.WriteLine($"Original: {original.NodeCount} nodes, {original.EdgeCount} edges");
Console.WriteLine($"Subgraph: {evenNodes.NodeCount} nodes, {evenNodes.EdgeCount} edges");
```

### Factory Methods for Common Structures

```csharp
// Create common graph topologies
var cycle = GraphFactory.Cycle<UndirectedGraph>(6);
var star = GraphFactory.Star<DirectedGraph>(5);
var grid = GraphFactory.Grid<UndirectedGraph>(3, 3);
var tree = GraphFactory.Tree<UndirectedGraph>(7, seed: 42);
var random = GraphFactory.Random<DirectedGraph>(10, edgeProbability: 0.3);

// Bipartite graphs
var bipartite = GraphFactory.Bipartite<BipartiteGraph>(3, 4, fullyConnected: true);
```

## Performance Characteristics

| Graph Type | Space Complexity | Edge Lookup | Add Edge | Use Case |
|------------|------------------|-------------|----------|----------|
| `DirectedGraph` | O(V + E) | O(degree) | O(1) | Sparse graphs |
| `DenseDirectedGraph` | O(V²) | O(1) | O(1) | Dense graphs |
| `WeightedDirectedGraph` | O(V + E) | O(degree) | O(1) | Weighted sparse graphs |
| `SubgraphView` | O(1) | O(degree) | N/A | Filtered views |

## Architecture

The library follows a clean architecture with separated concerns:

```
GraphLib/
├── Core/           # Basic types (NodeId, Edge, WeightedEdge)
├── Interfaces/     # Interface hierarchy (IGraph, IDirectedGraph, etc.)
├── Implementations/# Concrete graph implementations
├── Builders/       # Builder pattern and factory methods
├── Extensions/     # LINQ integration and extension methods
└── Algorithms/     # Graph algorithms (planned)
```

### Development Setup

1. Install [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
2. Clone the repository
3. Run tests: `dotnet test src/GraphLib.Tests`
4. Build: `dotnet build src/GraphLib.sln`

### Running Tests

```bash
# Run all tests
dotnet test src/GraphLib.Tests

# Run with coverage
dotnet test src/GraphLib.Tests --collect:"XPlat Code Coverage"

# Run specific test category
dotnet test src/GraphLib.Tests --filter "Category=Integration"
```


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

