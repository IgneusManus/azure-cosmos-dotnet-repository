// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

/// <summary>
/// A builder to configure a container for an item
/// </summary>
public interface IItemRequestOptionsBuilder
{
    /// <summary>
    /// The list of already configured options for a <see cref="IItem"/>
    /// </summary>
    IReadOnlyList<RequestOptionsBuilder> Options { get; }

    /// <summary>
    /// Provides a <see cref="RequestOptionsBuilder"/> instance to configure a container for an item
    /// </summary>
    /// <typeparam name="TItem">The type of <see cref="IItem"/> to configure.</typeparam>
    /// <returns>Instance of <see cref="IItemRequestOptionsBuilder"/></returns>
    IItemRequestOptionsBuilder Configure<TItem>(Action<RequestOptionsBuilder> requestOptions) where TItem : IItem;
}
