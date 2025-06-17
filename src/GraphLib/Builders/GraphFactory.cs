using GraphLib.Core;
using GraphLib.Interfaces;

namespace GraphLib.Builders;

public static class GraphFactory
{
    public static TGraph Complete<TGraph>(int nodeCount) where TGraph : IMutableGraph, new()
    {
        if (nodeCount < 0)
            throw new ArgumentException("Node count must be non-negative", nameof(nodeCount));

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(nodeCount);

        var nodes = Enumerable.Range(0, nodeCount).ToList();
        builder.AddClique(nodes);

        return builder.Build();
    }

    public static TGraph Cycle<TGraph>(int nodeCount) where TGraph : IMutableGraph, new()
    {
        if (nodeCount < 3)
            throw new ArgumentException("A cycle requires at least 3 nodes", nameof(nodeCount));

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(nodeCount);

        var nodes = Enumerable.Range(0, nodeCount).ToArray();
        builder.AddCycle(nodes);

        return builder.Build();
    }

    public static TGraph Path<TGraph>(int nodeCount) where TGraph : IMutableGraph, new()
    {
        if (nodeCount < 1)
            throw new ArgumentException("A path requires at least 1 node", nameof(nodeCount));

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(nodeCount);

        if (nodeCount > 1)
        {
            var nodes = Enumerable.Range(0, nodeCount).ToArray();
            builder.AddPath(nodes);
        }

        return builder.Build();
    }

    public static TGraph Star<TGraph>(int nodeCount) where TGraph : IMutableGraph, new()
    {
        if (nodeCount < 1)
            throw new ArgumentException("A star requires at least 1 node", nameof(nodeCount));

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(nodeCount);

        if (nodeCount > 1)
        {
            var leaves = Enumerable.Range(1, nodeCount - 1).ToList();
            builder.AddStar(0, leaves);
        }

        return builder.Build();
    }

    public static TGraph Grid<TGraph>(int rows, int cols) where TGraph : IMutableGraph, new()
    {
        if (rows < 1 || cols < 1)
            throw new ArgumentException("Grid dimensions must be positive", $"{nameof(rows)}, {nameof(cols)}");

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(rows * cols);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                var currentIndex = i * cols + j;

                if (j < cols - 1)
                {
                    var rightIndex = i * cols + (j + 1);
                    builder.AddEdge(currentIndex, rightIndex);
                }

                if (i < rows - 1)
                {
                    var belowIndex = (i + 1) * cols + j;
                    builder.AddEdge(currentIndex, belowIndex);
                }
            }
        }

        return builder.Build();
    }

    public static TGraph Random<TGraph>(int nodeCount, double edgeProbability, int seed = 0) 
        where TGraph : IMutableGraph, new()
    {
        if (nodeCount < 0)
            throw new ArgumentException("Node count must be non-negative", nameof(nodeCount));
        if (edgeProbability < 0.0 || edgeProbability > 1.0)
            throw new ArgumentException("Edge probability must be between 0 and 1", nameof(edgeProbability));

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(nodeCount);

        var random = new Random(seed);
        var graph = builder.Build();

        if (graph is IMutableUndirectedGraph)
        {
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = i + 1; j < nodeCount; j++)
                {
                    if (random.NextDouble() < edgeProbability)
                    {
                        builder.AddEdge(i, j);
                    }
                }
            }
        }
        else if (graph is IMutableDirectedGraph)
        {
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = 0; j < nodeCount; j++)
                {
                    if (i != j && random.NextDouble() < edgeProbability)
                    {
                        builder.AddEdge(i, j);
                    }
                }
            }
        }

        return builder.Build();
    }

    public static TGraph Tree<TGraph>(int nodeCount, int seed = 0) where TGraph : IMutableGraph, new()
    {
        if (nodeCount < 1)
            throw new ArgumentException("A tree requires at least 1 node", nameof(nodeCount));

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(nodeCount);

        if (nodeCount == 1)
            return builder.Build();

        var random = new Random(seed);
        var connectedNodes = new List<int> { 0 };
        var unconnectedNodes = Enumerable.Range(1, nodeCount - 1).ToList();

        while (unconnectedNodes.Count > 0)
        {
            var newNodeIndex = random.Next(unconnectedNodes.Count);
            var newNode = unconnectedNodes[newNodeIndex];
            unconnectedNodes.RemoveAt(newNodeIndex);

            var parentIndex = random.Next(connectedNodes.Count);
            var parent = connectedNodes[parentIndex];

            builder.AddEdge(parent, newNode);
            connectedNodes.Add(newNode);
        }

        return builder.Build();
    }

    public static TGraph Wheel<TGraph>(int nodeCount) where TGraph : IMutableGraph, new()
    {
        if (nodeCount < 4)
            throw new ArgumentException("A wheel requires at least 4 nodes (center + 3 rim nodes)", nameof(nodeCount));

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(nodeCount);

        var rimNodes = Enumerable.Range(1, nodeCount - 1).ToArray();
        builder.AddCycle(rimNodes);

        builder.AddStar(0, rimNodes);

        return builder.Build();
    }

    public static TGraph Bipartite<TGraph>(int leftSize, int rightSize, double edgeProbability = 1.0, int seed = 0)
        where TGraph : IMutableGraph, new()
    {
        if (leftSize < 0 || rightSize < 0)
            throw new ArgumentException("Partition sizes must be non-negative");
        if (edgeProbability < 0.0 || edgeProbability > 1.0)
            throw new ArgumentException("Edge probability must be between 0 and 1", nameof(edgeProbability));

        var builder = new GraphBuilder<TGraph>();
        builder.AddNodes(leftSize + rightSize);

        if (edgeProbability >= 1.0)
        {
            for (int i = 0; i < leftSize; i++)
            {
                for (int j = leftSize; j < leftSize + rightSize; j++)
                {
                    builder.AddEdge(i, j);
                }
            }
        }
        else if (edgeProbability > 0.0)
        {
            var random = new Random(seed);
            for (int i = 0; i < leftSize; i++)
            {
                for (int j = leftSize; j < leftSize + rightSize; j++)
                {
                    if (random.NextDouble() < edgeProbability)
                    {
                        builder.AddEdge(i, j);
                    }
                }
            }
        }

        return builder.Build();
    }
}