using GraphLib.Core;

namespace GraphLib.Algorithms.Results;

/// <summary>
/// Represents the result of a network flow algorithm.
/// </summary>
/// <typeparam name="TCapacity">The type of the flow capacity.</typeparam>
public class FlowResult<TCapacity>
{
    /// <summary>
    /// Gets the maximum flow value from source to sink.
    /// </summary>
    public TCapacity MaxFlow { get; }

    /// <summary>
    /// Gets the flow values for each edge in the network.
    /// </summary>
    public IReadOnlyDictionary<Edge, TCapacity> EdgeFlows { get; }

    /// <summary>
    /// Initializes a new instance of the FlowResult class.
    /// </summary>
    /// <param name="maxFlow">The maximum flow value.</param>
    /// <param name="edgeFlows">The flow values for each edge.</param>
    public FlowResult(TCapacity maxFlow, IReadOnlyDictionary<Edge, TCapacity> edgeFlows)
    {
        MaxFlow = maxFlow;
        EdgeFlows = edgeFlows ?? throw new ArgumentNullException(nameof(edgeFlows));
    }
}