// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Options;

internal class ItemConfiguration(
    Type type,
    string containerName,
    string partitionKeyPath,
    UniqueKeyPolicy? uniqueKeyPolicy,
    ThroughputProperties? throughputProperties,
    ItemRequestOptions createItemRequestOptions,
    ItemRequestOptions updateItemRequestOptions,
    PatchItemRequestOptions patchItemRequestOptions,
    ItemRequestOptions deleteItemRequestOptions,
    int defaultTimeToLive = -1,
    bool syncContainerProperties = false,
    ChangeFeedOptions? changeFeedOptions = null,
    bool useStrictTypeChecking = true) : IItemConfiguration
{
    // Keep original (template) options private and immutable to callers.
    private ItemRequestOptions _createTemplate { get; } = createItemRequestOptions ?? new ItemRequestOptions();
    private ItemRequestOptions _updateTemplate { get; } = updateItemRequestOptions ?? new ItemRequestOptions();
    private PatchItemRequestOptions _patchTemplate { get; } = patchItemRequestOptions ?? new PatchItemRequestOptions();
    private ItemRequestOptions _deleteTemplate { get; } = deleteItemRequestOptions ?? new ItemRequestOptions();

    public Type Type { get; } = type;

    public string ContainerName { get; } = containerName;

    public string PartitionKeyPath { get; } = partitionKeyPath;

    public UniqueKeyPolicy? UniqueKeyPolicy { get; } = uniqueKeyPolicy;

    public ThroughputProperties? ThroughputProperties { get; } = throughputProperties;

    public int DefaultTimeToLive { get; } = defaultTimeToLive;

    public bool SyncContainerProperties { get; } = syncContainerProperties;

    public ChangeFeedOptions? ChangeFeedOptions { get; } = changeFeedOptions;

    public bool UseStrictTypeChecking { get; } = useStrictTypeChecking;

    public ItemRequestOptions CreateItemRequestOptions => DeepCloneRequestOptions(_createTemplate);

    public ItemRequestOptions UpdateItemRequestOptions => DeepCloneRequestOptions(_updateTemplate);

    public PatchItemRequestOptions PatchItemRequestOptions => DeepCloneRequestOptions(_patchTemplate);

    public ItemRequestOptions DeleteItemRequestOptions => DeepCloneRequestOptions(_deleteTemplate);

    private static T DeepCloneRequestOptions<T>(T template) where T : ItemRequestOptions
    {
        var clone = (T)template.ShallowCopy();

        if (clone.PreTriggers is not null)
        {
            clone.PreTriggers = [.. clone.PreTriggers];
        }

        if (clone.PostTriggers is not null)
        {
            clone.PostTriggers = [.. clone.PostTriggers];
        }

        if (clone.Properties is not null)
        {
            var props = new Dictionary<string, object>(clone.Properties.Count);
            foreach (var kvp in clone.Properties)
            {
                props[kvp.Key] = kvp.Value;
            }
            clone.Properties = props;
        }

        if (clone.DedicatedGatewayRequestOptions is not null)
        {
            clone.DedicatedGatewayRequestOptions = new DedicatedGatewayRequestOptions
            {
                MaxIntegratedCacheStaleness = clone.DedicatedGatewayRequestOptions.MaxIntegratedCacheStaleness
            };
        }

        return clone;
    }
}