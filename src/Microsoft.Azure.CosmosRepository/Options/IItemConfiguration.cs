// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Options;

/// <summary>
/// Abstraction describing the computed configuration for a repository item type.
/// </summary>
internal interface IItemConfiguration
{
    Type Type { get; }

    string ContainerName { get; }

    string PartitionKeyPath { get; }

    UniqueKeyPolicy? UniqueKeyPolicy { get; }

    ThroughputProperties? ThroughputProperties { get; }

    int DefaultTimeToLive { get; }

    bool SyncContainerProperties { get; }

    ChangeFeedOptions? ChangeFeedOptions { get; }

    bool UseStrictTypeChecking { get; }

    ItemRequestOptions CreateItemRequestOptions { get; }

    ItemRequestOptions UpdateItemRequestOptions { get; }

    ItemRequestOptions PatchItemRequestOptions { get; }

    ItemRequestOptions DeleteItemRequestOptions { get; }
}
