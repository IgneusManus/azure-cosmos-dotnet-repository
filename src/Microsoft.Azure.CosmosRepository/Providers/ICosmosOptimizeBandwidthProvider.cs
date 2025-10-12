// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

interface ICosmosOptimizeBandwidthProvider
{
    bool OptimizeBandwidth<TItem>()
        where TItem : IItem;

    bool OptimizeBandwidth(Type itemType);
}