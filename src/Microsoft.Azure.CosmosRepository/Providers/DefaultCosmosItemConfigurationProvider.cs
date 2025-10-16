// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

class DefaultCosmosItemConfigurationProvider(
    ICosmosContainerNameProvider containerNameProvider,
    ICosmosPartitionKeyPathProvider cosmosPartitionKeyPathProvider,
    ICosmosUniqueKeyPolicyProvider cosmosUniqueKeyPolicyProvider,
    ICosmosContainerDefaultTimeToLiveProvider containerDefaultTimeToLiveProvider,
    ICosmosContainerSyncContainerPropertiesProvider syncContainerPropertiesProvider,
    ICosmosThroughputProvider cosmosThroughputProvider,
    ICosmosStrictTypeCheckingProvider cosmosStrictTypeCheckingProvider,
    IItemRequestOptionsProvider itemRequestOptionsProvider) : ICosmosItemConfigurationProvider
{
    private static readonly ConcurrentDictionary<Type, IItemConfiguration> _itemOptionsMap = new();

    public IItemConfiguration GetItemConfiguration<TItem>() where TItem : IItem =>
        GetItemConfiguration(typeof(TItem));

    public IItemConfiguration GetItemConfiguration(Type itemType) =>
        _itemOptionsMap.GetOrAdd(itemType, AddOptions(itemType));

    public List<IItemConfiguration> GetAllItemConfigurations(params Assembly[]? assemblies)
    {
        IEnumerable<Type> itemTypes = (assemblies ?? AppDomain.CurrentDomain.GetAssemblies())
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IItem).IsAssignableFrom(p) && p is { IsInterface: false, IsAbstract: false });

        foreach (Type itemType in itemTypes)
        {
            _itemOptionsMap.GetOrAdd(itemType, AddOptions(itemType));
        }

        return _itemOptionsMap.Select(i => i.Value).ToList();
    }

    private ItemConfiguration AddOptions(Type itemType)
    {
        itemType.IsItem();

        var containerName = containerNameProvider.GetContainerName(itemType);
        var partitionKeyPath = cosmosPartitionKeyPathProvider.GetPartitionKeyPath(itemType);
        UniqueKeyPolicy? uniqueKeyPolicy = cosmosUniqueKeyPolicyProvider.GetUniqueKeyPolicy(itemType);
        var timeToLive = containerDefaultTimeToLiveProvider.GetDefaultTimeToLive(itemType);
        var sync = syncContainerPropertiesProvider.GetWhetherToSyncContainerProperties(itemType);
        ThroughputProperties? throughputProperties = cosmosThroughputProvider.GetThroughputProperties(itemType);
        var useStrictTypeChecking = cosmosStrictTypeCheckingProvider.UseStrictTypeChecking(itemType);
        var createItemRequestOptions = itemRequestOptionsProvider.GetCreateItemRequestOptions(itemType, OperationType.Create);
        var updateItemRequestOptions = itemRequestOptionsProvider.GetCreateItemRequestOptions(itemType, OperationType.Upsert);
        var patchItemRequestOptions = itemRequestOptionsProvider.GetCreateItemRequestOptions(itemType, OperationType.Patch) as PatchItemRequestOptions ?? new PatchItemRequestOptions();
        var deleteItemRequestOptions = itemRequestOptionsProvider.GetCreateItemRequestOptions(itemType, OperationType.Delete);

        return new(
            itemType,
            containerName,
            partitionKeyPath,
            uniqueKeyPolicy,
            throughputProperties,
            createItemRequestOptions,
            updateItemRequestOptions,
            patchItemRequestOptions,
            deleteItemRequestOptions,
            timeToLive,
            sync,
            useStrictTypeChecking: useStrictTypeChecking);
    }
}