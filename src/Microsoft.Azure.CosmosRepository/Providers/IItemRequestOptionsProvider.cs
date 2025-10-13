// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

interface IItemRequestOptionsProvider
{
    ItemRequestOptions GetCreateItemRequestOptions<TItem>(OperationType operationType)
        where TItem : IItem;

    ItemRequestOptions GetCreateItemRequestOptions(Type itemType, OperationType operationType);
}