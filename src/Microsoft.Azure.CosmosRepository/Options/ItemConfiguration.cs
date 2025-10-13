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
    ItemRequestOptions patchItemRequestOptions,
    ItemRequestOptions deleteItemRequestOptions,
    int defaultTimeToLive = -1,
    bool syncContainerProperties = false,
    ChangeFeedOptions? changeFeedOptions = null,
    bool useStrictTypeChecking = true) : IItemConfiguration
{
    // Keep original (template) options private and immutable to callers.
    private ItemRequestOptions _createTemplate { get; } = createItemRequestOptions ?? new ItemRequestOptions();
    private ItemRequestOptions _updateTemplate { get; } = updateItemRequestOptions ?? new ItemRequestOptions();
    private ItemRequestOptions _patchTemplate { get; } = patchItemRequestOptions ?? new ItemRequestOptions();
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

    public ItemRequestOptions CreateItemRequestOptions => CloneItemRequestOptions(_createTemplate);

    public ItemRequestOptions UpdateItemRequestOptions => CloneItemRequestOptions(_updateTemplate);

    public ItemRequestOptions PatchItemRequestOptions => CloneItemRequestOptions(_patchTemplate);

    public ItemRequestOptions DeleteItemRequestOptions => CloneItemRequestOptions(_deleteTemplate);

    private static ItemRequestOptions CloneItemRequestOptions(ItemRequestOptions source)
    {
        var clone = new ItemRequestOptions();

        foreach (var prop in typeof(ItemRequestOptions).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            if (!prop.CanRead || !prop.CanWrite)
            {
                continue;
            }

            var value = prop.GetValue(source);
            prop.SetValue(clone, value);
        }

        return clone;
    }
}