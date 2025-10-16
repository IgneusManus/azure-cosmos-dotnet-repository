// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

/// <summary>
/// Options for a container
/// </summary>
/// <remarks>
/// Creates an instance of <see cref="RequestOptionsBuilder"/>.
/// </remarks>
/// <param name="type">The type of <see cref="IItem"/> the options are for.</param>
public class RequestOptionsBuilder(Type type)
{
    /// <summary>
    /// The <see cref="IItem"/> type the request options are for
    /// </summary>
    internal Type Type { get; } = type;

    /// <summary>
    /// The <see cref="ItemRequestOptions"/> to use when creating an item.
    /// </summary>
    internal ItemRequestOptions? CreateItemRequestOptions { get; private set; }

    /// <summary>
    /// The <see cref="ItemRequestOptions"/> to use when updating or upserting an item.
    /// Currently, this library uses the Cosmos SDK's Upsert methods for update operations,
    /// not the Cosmos SDK's Replace methods.
    /// </summary>
    internal ItemRequestOptions? UpsertItemRequestOptions { get; private set; }

    /// <summary>
    /// The <see cref="ItemRequestOptions"/> to use when patching an item.
    /// </summary>
    internal PatchItemRequestOptions? PatchItemRequestOptions { get; private set; }

    /// <summary>
    /// The <see cref="ItemRequestOptions"/> to use when deleting an item.
    /// </summary>
    internal ItemRequestOptions? DeleteItemRequestOptions { get; private set; }

    /// <summary>
    /// Sets the <see cref="ItemRequestOptions"/> to use when creating all items of this type.
    /// Can be overridden by passing an ItemRequestOptions instance to this library's Create methods.
    /// </summary>
    /// <param name="options">The <see cref="ItemRequestOptions"/> to use.
    /// If the "EnableContentResponseOnWrite" property is set, it will override this library's 
    /// global RepositoryOptions.OptimizeBandwidth setting when creating items of this type
    /// </param>
    /// <returns>The <see cref="RequestOptionsBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public RequestOptionsBuilder WithCreateItemRequestOptions(ItemRequestOptions options)
    {
        CreateItemRequestOptions = options ?? throw new ArgumentNullException(nameof(options));
        return this;
    }

    /// <summary>
    /// Sets the <see cref="ItemRequestOptions"/> to use for non-PATCH update/upsert operations for
    /// all items of this type. Currently, this library's Update methods uses the Cosmos SDK's Upsert
    /// methods so use this to set options for both update and upsert operations.
    /// Can be overridden by passing an ItemRequestOptions instance to this library's Update methods.
    /// </summary>
    /// <param name="options">The <see cref="ItemRequestOptions"/> to use.
    /// If the "EnableContentResponseOnWrite" property is set, it will override this library's 
    /// global RepositoryOptions.OptimizeBandwidth setting when upserting items of this type
    /// </param>
    /// <returns>The <see cref="RequestOptionsBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public RequestOptionsBuilder WithUpsertItemRequestOptions(ItemRequestOptions options)
    {
        UpsertItemRequestOptions = options ?? throw new ArgumentNullException(nameof(options));
        return this;
    }

    /// <summary>
    /// Sets the <see cref="PatchItemRequestOptions"/> to use when patching all items of this type.
    /// Can be overridden by passing an PatchItemRequestOptions instance to this library's Update methods when
    /// using the Patch overloads.
    /// </summary>
    /// <param name="options">The <see cref="PatchItemRequestOptions"/> to use.
    /// If the "EnableContentResponseOnWrite" property is set, it will override this library's 
    /// global RepositoryOptions.OptimizeBandwidth setting when patching items of this type
    /// </param>
    /// <returns>The <see cref="RequestOptionsBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public RequestOptionsBuilder WithPatchItemRequestOptions(PatchItemRequestOptions options)
    {
        PatchItemRequestOptions = options ?? throw new ArgumentNullException(nameof(options));
        return this;
    }

    /// <summary>
    /// Sets the <see cref="ItemRequestOptions"/> to use when deleting all items of this type.
    /// Can be overridden by passing an ItemRequestOptions instance to this library's Delete methods.
    /// </summary>
    /// <param name="options">The <see cref="ItemRequestOptions"/> to use.
    /// If the "EnableContentResponseOnWrite" property is set, it will override this library's 
    /// global RepositoryOptions.OptimizeBandwidth setting when deleting items of this type
    /// </param>
    /// <returns>The <see cref="RequestOptionsBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public RequestOptionsBuilder WithDeleteItemRequestOptions(ItemRequestOptions options)
    {
        DeleteItemRequestOptions = options ?? throw new ArgumentNullException(nameof(options));
        return this;
    }
}
