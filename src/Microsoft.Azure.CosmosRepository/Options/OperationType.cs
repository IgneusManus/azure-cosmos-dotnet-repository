// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.Options;

/// <summary>
/// The type of operation being performed, used for applying different <see cref="ItemRequestOptions"/>
/// </summary>
public enum OperationType
{
    None,
    /// <summary>
    /// Used for create operations
    /// </summary>
    Create,
    /// <summary>
    /// Currently used for both update and upsert operations.  At the moment, this library uses the Cosmos
    /// SDK's Upsert methods for update operations, not the Cosmos SDK's Replace methods.
    /// </summary>
    Upsert,
    /// <summary>
    /// Used for patch operations
    /// </summary>
    Patch,
    /// <summary>
    /// Used for delete operations
    /// </summary>
    Delete
}