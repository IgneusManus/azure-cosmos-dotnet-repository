// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Builders;

/// <summary>
/// Allows a collection of <see cref="PatchOperation"/>'s to built./>
/// </summary>
public interface IPatchOperationBuilder<TItem> where TItem : IItem
{
    /// <summary>
    /// The currently built <see cref="PatchOperation"/>'s
    /// </summary>
    IReadOnlyList<PatchOperation> PatchOperations { get; }

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be replaced with the value provided
    /// </summary>
    /// <param name="expression">The expression to define which property to operate on.</param>
    /// <param name="value">The value to replace the property defined with.</param>
    /// <typeparam name="TValue">The type of the property that is been replaced.</typeparam>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    /// <remarks>Nested properties are supported by chaining member access in the expression,
    /// for example <c>x => x.Child.Name</c> produces the path <c>/child/name</c>.</remarks>
    IPatchOperationBuilder<TItem> Replace<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value);

    /// <summary>
    /// Allows a property of an <see cref="IItem"/> to be set to the value provided. Unlike <see cref="Replace{TValue}"/>,
    /// the property is created when it is absent on the stored document and overwritten when it is present.
    /// </summary>
    /// <param name="expression">The expression to define which property to operate on.</param>
    /// <param name="value">The value to set the property defined to.</param>
    /// <typeparam name="TValue">The type of the property that is been set.</typeparam>
    /// <returns>The same instance of <see cref="IPatchOperationBuilder{TItem}"/></returns>
    /// <remarks>Nested properties are supported by chaining member access in the expression,
    /// for example <c>x => x.Child.Name</c> produces the path <c>/child/name</c>.</remarks>
    IPatchOperationBuilder<TItem> Set<TValue>(Expression<Func<TItem, TValue>> expression, TValue? value);
}