# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# Graph Library for C# (graph-lib-sharp)

## Project Overview

This is a C# class library project for implementing a comprehensive graph data structure library. The project follows the principle of "pay only for what you use" and provides multiple graph implementations optimized for different use cases.

### Project Type
- **Type**: C# Class Library
- **Framework**: .NET 9.0
- **Language**: C# with nullable reference types enabled
- **Structure**: Single library project (no test project yet)

## Project Structure

```
graph-lib-sharp/
├── graph-library-spec.md        # Detailed specification document
├── src/
│   ├── GraphLib.sln            # Visual Studio solution file
│   └── GraphLib/               # Main library project
│       ├── GraphLib.csproj     # Project file targeting .NET 9.0
│       └── Class1.cs           # Placeholder file (to be replaced)
```

## Key Features from Specification

The library should implement:

1. **Core Types**:
   - `NodeId` - Basic node identifier
   - `Edge` - Unweighted edge representation
   - `WeightedEdge<TWeight>` - Weighted edge representation

2. **Graph Interfaces**:
   - `IGraph` - Base interface for all graphs
   - `IDirectedGraph` - Interface for directed graphs
   - `IUndirectedGraph` - Interface for undirected graphs
   - `IWeightedGraph<TWeight>` - Interface for weighted graphs
   - `INodeDataGraph<TNodeData>` - Interface for graphs with node data
   - `IEdgeDataGraph<TEdgeData>` - Interface for graphs with edge data
   - `IMutableGraph` - Interface for mutable graphs

3. **Concrete Implementations**:
   - `DirectedGraph` - Simple directed graph (adjacency list)
   - `UndirectedGraph` - Simple undirected graph
   - `WeightedDirectedGraph<TWeight>` - Weighted directed graph
   - `WeightedUndirectedGraph<TWeight>` - Weighted undirected graph
   - `BipartiteGraph` - Specialized bipartite graph
   - `DenseDirectedGraph` - Dense graph using adjacency matrix

4. **Algorithms**:
   - Path finding (Dijkstra, A*, Bellman-Ford, BFS)
   - Spanning trees (Kruskal, Prim)
   - Network flow (Ford-Fulkerson, Edmonds-Karp)
   - Graph isomorphism
   - Clique finding
   - Topological sort
   - Connected components
   - Graph coloring
   - Cycle detection

5. **Features**:
   - LINQ integration
   - Fluent builder pattern
   - Subgraph support
   - Serialization support
   - Visitor pattern for traversal

## Development Setup

### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code with C# Dev Kit

### Build Commands

```bash
# Build the solution
dotnet build src/GraphLib.sln

# Build in Release mode
dotnet build src/GraphLib.sln -c Release

# Clean build artifacts
dotnet clean src/GraphLib.sln
```

### Test Commands

Currently no test project exists. When tests are added:

```bash
# Run all tests
dotnet test src/GraphLib.sln

# Run tests with coverage
dotnet test src/GraphLib.sln --collect:"XPlat Code Coverage"

# Run specific test
dotnet test src/GraphLib.sln --filter "FullyQualifiedName~GraphLib.Tests.SpecificTest"
```

### Linting and Code Quality

```bash
# Format code (when .editorconfig is added)
dotnet format src/GraphLib.sln

# Analyze code
dotnet build src/GraphLib.sln /p:RunAnalyzers=true
```

## Implementation Guidelines

### Design Principles
1. **Type Safety**: Leverage C#'s strong typing to prevent runtime errors
2. **Performance**: Minimize memory overhead and computational complexity
3. **Composability**: Support method chaining and LINQ integration
4. **Extensibility**: Allow users to extend functionality through interfaces and extension methods
5. **Zero-Cost Abstractions**: Features that aren't used shouldn't impact performance or memory

### Code Organization Suggestions

```
GraphLib/
├── Core/
│   ├── NodeId.cs
│   ├── Edge.cs
│   └── WeightedEdge.cs
├── Interfaces/
│   ├── IGraph.cs
│   ├── IDirectedGraph.cs
│   ├── IUndirectedGraph.cs
│   └── ...
├── Implementations/
│   ├── DirectedGraph.cs
│   ├── UndirectedGraph.cs
│   └── ...
├── Algorithms/
│   ├── ShortestPath.cs
│   ├── SpanningTree.cs
│   ├── NetworkFlow.cs
│   └── ...
├── Builders/
│   ├── GraphBuilder.cs
│   └── GraphFactory.cs
├── Extensions/
│   ├── GraphLinqExtensions.cs
│   └── SubgraphExtensions.cs
└── Serialization/
    └── GraphSerializers.cs
```

### Naming Conventions
- Use PascalCase for types and public members
- Use camelCase for parameters and local variables
- Prefix interfaces with 'I'
- Use descriptive names that match the specification

### Performance Considerations
- Use `struct` for `NodeId`, `Edge`, and `WeightedEdge` for value semantics
- Implement adjacency lists for sparse graphs (`DirectedGraph`, `UndirectedGraph`)
- Implement adjacency matrix for dense graphs (`DenseDirectedGraph`)
- Use sequential integer node IDs for cache efficiency
- Share data structures in subgraph views to avoid copying

## Current Status

The project is in initial setup phase with:
- Solution and project files created
- Targeting .NET 9.0
- Nullable reference types enabled
- Implicit usings enabled
- No implementation yet (only placeholder Class1.cs)

## Next Steps

1. Remove the placeholder `Class1.cs` file
2. Create the core types (`NodeId`, `Edge`, `WeightedEdge`)
3. Define the interface hierarchy
4. Implement basic graph types
5. Add unit tests project
6. Implement algorithms incrementally
7. Add documentation and examples

## Testing Strategy

When implementing tests:
- Use xUnit or NUnit for unit testing
- Create test fixtures for each graph implementation
- Test edge cases (empty graphs, single node, disconnected components)
- Benchmark performance-critical algorithms
- Ensure algorithm correctness with known test cases
- Test LINQ integration and builder patterns

## Documentation

Consider adding:
- XML documentation comments for all public APIs
- Usage examples in the documentation
- Performance characteristics for each algorithm
- Migration guide for different graph representations

## Important Notes from Specification

- The library uses generic programming extensively - all graph types support custom node and edge data types
- Algorithms are implemented as extension methods in the `GraphLib.Algorithms` namespace
- The specification emphasizes immutable graph support alongside mutable variants
- Pay special attention to the visitor pattern implementation for graph traversal
- The library should support both sequential (array-based) and hash-based node ID storage