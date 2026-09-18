// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Azure.CosmosRepository.Builders;

internal class PatchOperationBuilder<TItem> : IPatchOperationBuilder<TItem> where TItem : IItem
{
    private readonly List<PatchOperation> _patchOperations = [];
    private readonly NamingStrategy _namingStrategy;
    private readonly JsonSerializerOptions? _systemTextJsonOptions;

    internal readonly List<InternalPatchOperation> _rawPatchOperations = [];

    public IReadOnlyList<PatchOperation> PatchOperations => _patchOperations;

    public PatchOperationBuilder() : this(null, null)
    {
    }

    public PatchOperationBuilder(CosmosPropertyNamingPolicy? cosmosPropertyNamingPolicy)
        : this(cosmosPropertyNamingPolicy, null)
    {
    }

    /// <summary>
    /// Creates a builder whose patch paths name properties the way the client's serializer writes them.
    /// </summary>
    /// <param name="cosmosPropertyNamingPolicy">
    /// The <see cref="CosmosSerializationOptions.PropertyNamingPolicy"/> in force when the client serializes
    /// with Newtonsoft.Json. Ignored when <paramref name="systemTextJsonOptions"/> is set.
    /// </param>
    /// <param name="systemTextJsonOptions">
    /// The <see cref="CosmosClientOptions.UseSystemTextJsonSerializerWithOptions"/> value. When set, the client
    /// serializes with System.Text.Json, so paths follow <see cref="JsonPropertyNameAttribute"/> and
    /// <see cref="JsonSerializerOptions.PropertyNamingPolicy"/> instead of the Newtonsoft.Json attributes.
    /// </param>
    public PatchOperationBuilder(
        CosmosPropertyNamingPolicy? cosmosPropertyNamingPolicy,
        JsonSerializerOptions? systemTextJsonOptions)
    {
        _namingStrategy = cosmosPropertyNamingPolicy == CosmosPropertyNamingPolicy.Default
            ? new DefaultNamingStrategy()
            : new CamelCaseNamingStrategy();
        _systemTextJsonOptions = systemTextJsonOptions;
    }

    public IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value)
    {
        IReadOnlyList<PropertyInfo> propertyInfos = expression.GetPropertyInfos();
        var propertyToReplace = GetPropertyToReplace(propertyInfos);

        _rawPatchOperations.Add(new InternalPatchOperation(propertyInfos, value, PatchOperationType.Replace));
        _patchOperations.Add(PatchOperation.Replace($"/{propertyToReplace}", value));

        return this;
    }

    private string GetPropertyToReplace(IEnumerable<MemberInfo> propertyInfos) =>
        string.Join("/", propertyInfos.Cast<PropertyInfo>().Select(GetPropertyName));

    private string GetPropertyName(PropertyInfo propertyInfo)
    {
        if (_systemTextJsonOptions is not null)
        {
            // The same resolution the SDK's System.Text.Json serializer applies to LINQ member names,
            // so a patch path and a query name the same stored property.
            JsonPropertyNameAttribute? attribute =
                propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>(true);

            return attribute?.Name
                ?? _systemTextJsonOptions.PropertyNamingPolicy?.ConvertName(propertyInfo.Name)
                ?? propertyInfo.Name;
        }

        JsonPropertyAttribute[] attributes =
            propertyInfo.GetCustomAttributes<JsonPropertyAttribute>(true).ToArray();

        return attributes.Length is 0
            ? _namingStrategy.GetPropertyName(propertyInfo.Name, false)
            : attributes[0].PropertyName ?? propertyInfo.Name;
    }
}
