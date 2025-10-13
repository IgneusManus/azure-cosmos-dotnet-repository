// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

/// <inheritdoc/>
internal class DefaultItemRequestOptionsBuilder : IItemRequestOptionsBuilder
{
    private readonly List<RequestOptionsBuilder> _options = [];

    public IReadOnlyList<RequestOptionsBuilder> Options => _options;

    public IItemRequestOptionsBuilder Configure<TItem>(Action<RequestOptionsBuilder> requestOptions) where TItem : IItem
    {
        if (requestOptions is null) throw new ArgumentNullException(nameof(requestOptions));

        RequestOptionsBuilder optionsBuilder = new(typeof(TItem));

        requestOptions(optionsBuilder);

        _options.Add(optionsBuilder);

        return this;
    }
}