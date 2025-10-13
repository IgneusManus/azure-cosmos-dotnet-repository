// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

class DefaultItemRequestOptionsProvider(IOptions<RepositoryOptions> options) : IItemRequestOptionsProvider
{
    public ItemRequestOptions GetCreateItemRequestOptions<TItem>(OperationType operationType) where TItem : IItem =>
        GetCreateItemRequestOptions(typeof(TItem), operationType);

    /// <summary>
    /// Indicates whether bandwidth optimization should be applied for the given item type.
    /// </summary>
    /// <param name="itemType">The type of the item to check bandwidth optimization for.</param>
    /// <returns></returns>
    public ItemRequestOptions GetCreateItemRequestOptions(Type itemType, OperationType operationType)
    {
        var repositoryOptionsOptimizeBandwidthSetting = options.Value.OptimizeBandwidth;
        ItemRequestOptions? retVal = null;
        switch (operationType)
        {
            case OperationType.Create:
                retVal = options.Value.RequestOptionsBuilder.Options.Where(x => x.Type == itemType).SingleOrDefault()?.CreateItemRequestOptions;
                break;
            case OperationType.Upsert:
                retVal = options.Value.RequestOptionsBuilder.Options.Where(x => x.Type == itemType).SingleOrDefault()?.UpsertItemRequestOptions;
                break;
            case OperationType.Patch:
                retVal = options.Value.RequestOptionsBuilder.Options.Where(x => x.Type == itemType).SingleOrDefault()?.PatchItemRequestOptions;
                break;
            case OperationType.Delete:
                retVal = options.Value.RequestOptionsBuilder.Options.Where(x => x.Type == itemType).SingleOrDefault()?.DeleteItemRequestOptions;
                break;
            case OperationType.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
        }

        if (retVal == null)
        {
            retVal = new ItemRequestOptions();
        }

        if (retVal.EnableContentResponseOnWrite == null)
        {
            retVal.EnableContentResponseOnWrite = !repositoryOptionsOptimizeBandwidthSetting;
        }

        return retVal;
    }
}