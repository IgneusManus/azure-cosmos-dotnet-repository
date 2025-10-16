// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

// ReSharper disable once CheckNamespace
namespace Microsoft.Azure.CosmosRepository;

internal partial class DefaultRepository<TItem>
{
    /// <inheritdoc />
    public async ValueTask UpdateAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        var partitionKey = GetPartitionKeyValue(list);

        Container container = await containerProvider.GetContainerAsync();

        TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(partitionKey));
        var updateItemOptions = cosmosItemConfigurationProvider.GetItemConfiguration<TItem>().UpdateItemRequestOptions;

        foreach (TItem item in list)
        {
            TransactionalBatchItemRequestOptions options = new()
            {
                IndexingDirective = updateItemOptions.IndexingDirective,
                Properties = updateItemOptions.Properties,
                EnableContentResponseOnWrite = updateItemOptions.EnableContentResponseOnWrite
            };

            if (item is IItemWithEtag itemWithEtag)
            {
                options.IfMatchEtag = itemWithEtag.Etag;
            }

            batch.UpsertItem(item, options);
        }

        using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new BatchOperationException<TItem>(response);
        }
    }

    /// <inheritdoc />
    public async ValueTask CreateAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        var partitionKey = GetPartitionKeyValue(list);

        Container container = await containerProvider.GetContainerAsync();

        TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(partitionKey));
        var createItemOptions = cosmosItemConfigurationProvider.GetItemConfiguration<TItem>().CreateItemRequestOptions;

        foreach (TItem item in list)
        {
            TransactionalBatchItemRequestOptions options = new()
            {
                IndexingDirective = createItemOptions.IndexingDirective,
                Properties = createItemOptions.Properties,
                EnableContentResponseOnWrite = createItemOptions.EnableContentResponseOnWrite
            };

            batch.CreateItem(item, options);
        }

        using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new BatchOperationException<TItem>(response);
        }
    }

    public async ValueTask DeleteAsBatchAsync(
        IEnumerable<TItem> items,
        CancellationToken cancellationToken = default)
    {
        var list = items.ToList();

        var partitionKey = GetPartitionKeyValue(list);

        Container container = await containerProvider.GetContainerAsync();

        TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(partitionKey));
        var deleteItemOptions = cosmosItemConfigurationProvider.GetItemConfiguration<TItem>().DeleteItemRequestOptions;

        foreach (TItem item in list)
        {
            TransactionalBatchItemRequestOptions options = new()
            {
                IndexingDirective = deleteItemOptions.IndexingDirective,
                Properties = deleteItemOptions.Properties,
                EnableContentResponseOnWrite = deleteItemOptions.EnableContentResponseOnWrite
            };

            batch.DeleteItem(item.Id, options);
        }

        using TransactionalBatchResponse response = await batch.ExecuteAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new BatchOperationException<TItem>(response);
        }
    }

    private static string GetPartitionKeyValue(List<TItem> items)
    {
        if (!items.Any())
        {
            throw new ArgumentException(
                "Unable to perform batch operation with no items",
                nameof(items));
        }

        return items[0].PartitionKey;
    }
}