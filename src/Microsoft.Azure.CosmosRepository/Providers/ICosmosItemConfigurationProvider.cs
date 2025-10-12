// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Providers;

/// <summary>
/// Holds all of the configuration information for an item.
/// </summary>
internal interface ICosmosItemConfigurationProvider
{
    IItemConfiguration GetItemConfiguration<TItem>() where TItem : IItem;

    IItemConfiguration GetItemConfiguration(Type itemType);

    List<IItemConfiguration> GetAllItemConfigurations(params Assembly[]? assemblies);
}