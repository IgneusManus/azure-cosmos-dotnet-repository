// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepositoryTests.Builders;

public class Item1 : Item
{
    [JsonProperty("thisIsTheName")]
    public string TestProperty { get; set; } = null!;

    public int TestIntProperty { get; set; }
}

public class RequiredItem : Item
{
    [Required]
    public string TestProperty { get; set; } = null!;
}

public class RequiredAndJsonItem : Item
{
    [Required]
    [JsonProperty("testProperty")]
    public string TestProperty { get; set; } = null!;
}

/// <summary>
/// An item stored by a client configured with <see cref="CosmosClientOptions.UseSystemTextJsonSerializerWithOptions"/>:
/// only System.Text.Json attributes and its naming policy decide the stored property names.
/// </summary>
public class SystemTextJsonItem : Item
{
    [System.Text.Json.Serialization.JsonPropertyName("renamed")]
    public string TestProperty { get; set; } = null!;

    [JsonProperty("newtonsoftName")]
    public int TestIntProperty { get; set; }

    public bool IsDeleted { get; set; }

    public SystemTextJsonChild Child { get; set; } = new();
}

public class SystemTextJsonChild
{
    public DateTime LastReadAt { get; set; }
}

public class PatchOperationBuilderTests
{
    private static readonly System.Text.Json.JsonSerializerOptions CamelCaseSystemTextJson = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    [Theory]
    [InlineData(CosmosPropertyNamingPolicy.Default)]
    [InlineData(CosmosPropertyNamingPolicy.CamelCase)]
    [InlineData(null)]
    public void ReplaceWithSystemTextJsonOptionsFollowsItsNamingPolicyRegardlessOfCosmosPolicy(CosmosPropertyNamingPolicy? cosmosPolicy)
    {
        //Arrange
        IPatchOperationBuilder<SystemTextJsonItem> builder = new PatchOperationBuilder<SystemTextJsonItem>(cosmosPolicy, CamelCaseSystemTextJson);

        //Act
        builder.Replace(x => x.IsDeleted, true);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/isDeleted", operation.Path);
    }

    [Fact]
    public void ReplaceWithSystemTextJsonOptionsUsesJsonPropertyNameAttribute()
    {
        //Arrange
        IPatchOperationBuilder<SystemTextJsonItem> builder = new PatchOperationBuilder<SystemTextJsonItem>(CosmosPropertyNamingPolicy.Default, CamelCaseSystemTextJson);

        //Act
        builder.Replace(x => x.TestProperty, "100");

        //Assert
        Assert.Equal("/renamed", builder.PatchOperations[0].Path);
    }

    [Fact]
    public void ReplaceWithSystemTextJsonOptionsIgnoresNewtonsoftJsonPropertyAttribute()
    {
        //Arrange
        IPatchOperationBuilder<SystemTextJsonItem> builder = new PatchOperationBuilder<SystemTextJsonItem>(CosmosPropertyNamingPolicy.Default, CamelCaseSystemTextJson);

        //Act
        builder.Replace(x => x.TestIntProperty, 50);

        //Assert
        Assert.Equal("/testIntProperty", builder.PatchOperations[0].Path);
    }

    [Fact]
    public void ReplaceWithSystemTextJsonOptionsWithoutNamingPolicyKeepsPropertyName()
    {
        //Arrange
        IPatchOperationBuilder<SystemTextJsonItem> builder = new PatchOperationBuilder<SystemTextJsonItem>(CosmosPropertyNamingPolicy.CamelCase, new System.Text.Json.JsonSerializerOptions());

        //Act
        builder.Replace(x => x.IsDeleted, true);

        //Assert
        Assert.Equal("/IsDeleted", builder.PatchOperations[0].Path);
    }

    [Fact]
    public void ReplaceWithSystemTextJsonOptionsAppliesNamingPolicyToEveryPathSegment()
    {
        //Arrange
        IPatchOperationBuilder<SystemTextJsonItem> builder = new PatchOperationBuilder<SystemTextJsonItem>(CosmosPropertyNamingPolicy.Default, CamelCaseSystemTextJson);

        //Act
        builder.Replace(x => x.Child.LastReadAt, DateTime.UnixEpoch);

        //Assert
        Assert.Equal("/child/lastReadAt", builder.PatchOperations[0].Path);
    }

    [Fact]
    public void ReplaceWithoutSystemTextJsonOptionsStillHonoursNewtonsoftJsonPropertyAttribute()
    {
        //Arrange
        IPatchOperationBuilder<SystemTextJsonItem> builder = new PatchOperationBuilder<SystemTextJsonItem>(CosmosPropertyNamingPolicy.Default, null);

        //Act
        builder.Replace(x => x.TestIntProperty, 50);

        //Assert
        Assert.Equal("/newtonsoftName", builder.PatchOperations[0].Path);
    }

    [Fact]
    public void ReplaceGivenPropertyValueWithJsonAttributeSetsCorrectReplaceValue()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Replace(x => x.TestProperty, "100");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/thisIsTheName", operation.Path);
    }

    [Fact]
    public void ReplaceGivenPropertyWithNoAttributesSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>();

        //Act
        builder.Replace(x => x.TestIntProperty, 50);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/testIntProperty", operation.Path);
    }

    [Fact]
    public void ReplaceGivenPropertyWithRequiredAttributeSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<RequiredItem> builder = new PatchOperationBuilder<RequiredItem>();

        //Act
        builder.Replace(x => x.TestProperty, "Test Value");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/testProperty", operation.Path);
    }

    [Fact]
    public void ReplaceGivenPropertyWithRequiredAndJsonAttributesSetsCorrectPatchOperation()
    {
        //Arrange
        IPatchOperationBuilder<RequiredAndJsonItem> builder = new PatchOperationBuilder<RequiredAndJsonItem>();

        //Act
        builder.Replace(x => x.TestProperty, "Test Value");

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal("/testProperty", operation.Path);
    }

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void AcknowledgeRepositorySerializationSettingForRetrievingPatchOperation(CosmosPropertyNamingPolicy? propertyNamingPolicy,
        string expectedPropertyName)
    {
        //Arrange
        IPatchOperationBuilder<Item1> builder = new PatchOperationBuilder<Item1>(propertyNamingPolicy);

        //Act
        builder.Replace(x => x.TestIntProperty, 1234);

        //Assert
        PatchOperation operation = builder.PatchOperations[0];
        Assert.Equal(PatchOperationType.Replace, operation.OperationType);
        Assert.Equal($"/{expectedPropertyName}", operation.Path);
    }

    public static IEnumerable<object?[]> GetTestCases()
    {
        yield return new object?[]
        {
            CosmosPropertyNamingPolicy.CamelCase,
            new CamelCaseNamingStrategy().GetPropertyName(nameof(Item1.TestIntProperty), false)
        };
        yield return new object?[]
        {
            CosmosPropertyNamingPolicy.Default,
            new DefaultNamingStrategy().GetPropertyName(nameof(Item1.TestIntProperty), false)
        };
        yield return new object?[]
        {
            null,
            new CamelCaseNamingStrategy().GetPropertyName(nameof(Item1.TestIntProperty), false)
        };
    }
}