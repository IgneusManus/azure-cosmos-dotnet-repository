// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

class DefaultCosmosOptimizeBandwidthProvider(IOptions<RepositoryOptions> options) : ICosmosOptimizeBandwidthProvider
{
    public bool OptimizeBandwidth<TItem>() where TItem : IItem =>
        OptimizeBandwidth(typeof(TItem));

    /// <summary>
    /// Indicates whether bandwidth optimization should be applied for the given item type.
    /// </summary>
    /// <param name="itemType">Placeholder to match the interface, in preparation for per-item optimization support</param>
    /// <returns></returns>
    public bool OptimizeBandwidth(Type itemType)
    {
        return options.Value.OptimizeBandwidth;
    }
}