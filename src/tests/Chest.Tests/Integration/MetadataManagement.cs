// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Tests.Integration
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using Chest.Client;
    using Chest.Client.AutorestClient;
    using Chest.Client.AutorestClient.Models;
    using Chest.Tests.Dto;
    using Chest.Tests.Sdk;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public class MetadataManagement : IntegrationTest
    {
        public MetadataManagement(ChestFixture fixture)
            : base(fixture)
        {
        }

        [Scenario]
        public void GetShouldReturnNotFound()
        {
            // arrange
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "UnknownCategory";
            var collection = "UnknownCollection";
            var key = "Unknown";

            HttpException exception = null;

            $"Given a category: {category} collection: {collection} key: {key} that was never added to the metadata service before"
                .x(() =>
                {
                });

            $"When I query the metadata for that category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    try
                    {
                        await client.Metadata.GetAsync(category, collection, key).ConfigureAwait(false);
                    }
                    catch (HttpException exp)
                    {
                        exception = exp;
                    }
                });

            $"Then I get a HTTP 404 Not Found response"
                .x(() =>
                {
                    Assert.NotNull(exception);
                    Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
                    Assert.NotNull(exception.ContentMessage);
                });
        }

        [Scenario]
        public void CanAddMetadata()
        {
            // arrange
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "456987";

            var expected = new MetadataModel
            {
                Data = new Dictionary<string, string>
                {
                    { "accountNumber", key },
                    { "marginAccount", "MA01" },
                    { "referenceAccount", "RF11" },
                    { "bankIdentificationReference", "BIR11" },
                }
            };

            MetadataModel actual = null;

            $"Given the metadata for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.AddAsync(category, collection, key, expected).ConfigureAwait(false);
                });

            $"When try to get metadata for the category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    actual = await client.Metadata.GetAsync(category, collection, key).ConfigureAwait(false);
                });

            "Then the fetched metadata should be same"
                .x(() =>
                {
                    Assert.NotNull(actual);
                    actual.Should().BeEquivalentTo(expected);
                });
        }

        [Scenario]
        public void CanUpdateMetadata()
        {
            // arrange
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "456988";

            var expected = new MetadataModel
            {
                Data = new Dictionary<string, string>
                {
                    { "accountNumber", key },
                    { "marginAccount", "MA02" },
                    { "referenceAccount", "RF12" },
                    { "bankIdentificationReference", "BIR12" },
                }
            };

            MetadataModel actual = null;

            $"Given the metadata for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.AddAsync(category, collection, key, expected).ConfigureAwait(false);
                });

            $"When try to update metadata for the category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    expected.Data = new Dictionary<string, string>
                    {
                        { "accountNumber", key },
                        { "referenceAccount", "SomeNewRef" }
                    };

                    await client.Metadata.UpdateAsync(category, collection, key, expected).ConfigureAwait(false);
                });

            $"And try to get metadata for the category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    actual = await client.Metadata.GetAsync(category, collection, key).ConfigureAwait(false);
                });

            "Then the fetched metadata should be same as updated metadata"
                .x(() =>
                {
                    Assert.NotNull(actual);
                    actual.Should().BeEquivalentTo(expected);
                });
        }

        [Scenario]
        public void CanDeleteMetadata()
        {
            // arrange
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "486987";

            var expected = new MetadataModel
            {
                Data = new Dictionary<string, string>
                {
                    { "accountNumber", key },
                    { "marginAccount", "MA01" },
                    { "referenceAccount", "RF11" },
                    { "bankIdentificationReference", "BIR11" },
                }
            };

            MetadataModel actual = null;
            HttpException httpException = null;

            $"Given the metadata for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.AddAsync(category, collection, key, expected).ConfigureAwait(false);
                });

            $"When try to delete metadata for the category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.RemoveAsync(category, collection, key).ConfigureAwait(false);
                });

            $"And try to get the metadata for the category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    try
                    {
                        actual = await client.Metadata.GetAsync(category, collection, key);
                    }
                    catch( HttpException exp)
                    {
                        httpException = exp;
                    }
                });

            "Then the fetched metadata should be null"
                .x(() =>
                {
                    Assert.Null(actual);
                    Assert.NotNull(httpException);
                });
        }

        [Scenario]
        public void ShouldNotAddKeyMultipleTimes()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "Some-Unique-Key";

            var expected = new MetadataModel
            {
                Data = new Dictionary<string, string>
                {
                    { "accountNumber", key },
                    { "marginAccount", "MA02" },
                    { "referenceAccount", "RF12" },
                    { "bankIdentificationReference", "BIR12" },
                }
            };

            HttpException httpException = null;

            $"Given the metadata for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.AddAsync(category, collection, key, expected).ConfigureAwait(false);
                });

            $"When try to add metadata again for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    try
                    {
                        await client.Metadata.AddAsync(category, collection, key, expected).ConfigureAwait(false);
                    }
                    catch (HttpException exp)
                    {
                        httpException = exp;
                    }
                });

            $"Then system should return 409 Conflict"
                .x(() =>
                {
                    Assert.NotNull(httpException);
                    Assert.Equal(HttpStatusCode.Conflict, httpException.StatusCode);
                    Assert.NotNull(httpException.ContentMessage);
                });
        }

        [Scenario]
        public void ShouldNotAddKeyMultipleTimesEvenInDifferentCase()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "Case-Sensitive-Key";

            var expected = new MetadataModel
            {
                Data = new Dictionary<string, string>
                {
                    { "accountNumber", key },
                    { "marginAccount", "MA03" },
                    { "referenceAccount", "RF13" },
                    { "bankIdentificationReference", "BIR13" },
                }
            };

            HttpException httpException = null;

            $"Given the metadata for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.AddAsync(category, collection, key, expected).ConfigureAwait(false);
                });

#pragma warning disable CA1308 // Normalize strings to uppercase
            key = key.ToLower(CultureInfo.InvariantCulture);
#pragma warning restore CA1308 // Normalize strings to uppercase

            $"When try to add metadata again for same category: {category} collection: {collection} key: {key} but in different case"
                .x(async () =>
                {
                    try
                    {
                        await client.Metadata.AddAsync(category, collection, key, expected).ConfigureAwait(false);
                    }
                    catch (HttpException exp)
                    {
                        httpException = exp;
                    }
                });

            $"Then system should return 409 Conflict"
                .x(() =>
                {
                    Assert.NotNull(httpException);
                    Assert.Equal(HttpStatusCode.Conflict, httpException.StatusCode);
                    Assert.NotNull(httpException.ContentMessage);
                });
        }

        [Scenario]
        public void CanAddMetadataUsingDto()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "556988";

            var expected = new AssetAccountMetadata
            {
                AccountNumber = key,
                MarginAccount = "MA01",
                ReferenceAccount = "RF11",
                BankIdentificationReference = "BIR11",
            };

            AssetAccountMetadata actual = null;

            $"Given the AssetAccountMetadata for key: {key}"
                .x(async () =>
                {
                    await client.Metadata.Add<AssetAccountMetadata>(category, collection, key, expected).ConfigureAwait(false);
                });

            $"When try to get AssetAccountMetadata for the key: {key}"
                .x(async () =>
                {
                    actual = await client.Metadata.Get<AssetAccountMetadata>(category, collection, key).ConfigureAwait(false);
                });

            "Then the fetched AssetAccountMetadata should be same"
                .x(() =>
                {
                    Assert.NotNull(actual);
                    actual.Should().BeEquivalentTo(expected);
                });
        }

        [Scenario]
        public void CanGetKeysWithData()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "556985";

            var expected = new AssetAccountMetadata
            {
                AccountNumber = key,
                MarginAccount = "MA07",
                ReferenceAccount = "RF17",
                BankIdentificationReference = "BIR17",
            };

            IDictionary<string, AssetAccountMetadata> actualKeysWithData = null;

            $"Given the AssetAccountMetadata for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.Add<AssetAccountMetadata>(category, collection, key, expected).ConfigureAwait(false);
                });

            $"When try to get all keys with data for category: {category} collection: {collection}"
                .x(async () =>
                {
                    actualKeysWithData = await client.Metadata.GetKeysWithData<AssetAccountMetadata>(category, collection).ConfigureAwait(false);
                });

            "Then the fetched keys with data should contain the given key and data should be same agains the key"
                .x(() =>
                {
                    Assert.NotNull(actualKeysWithData);
                    actualKeysWithData.Should().ContainKey(key);
                    actualKeysWithData.TryGetValue(key, out var actual);
                    actual.Should().BeEquivalentTo(expected);
                });
        }

        [Scenario]
        public void CanGetCollections()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "556989";

            var expected = new AssetAccountMetadata
            {
                AccountNumber = key,
                MarginAccount = "MA05",
                ReferenceAccount = "RF15",
                BankIdentificationReference = "BIR15",
            };

            IList<string> actualCollections = null;

            $"Given the AssetAccountMetadata for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.Add<AssetAccountMetadata>(category, collection, key, expected).ConfigureAwait(false);
                });

            $"When try to get all collections for category: {category}"
                .x(async () =>
                {
                    actualCollections = await client.Metadata.GetCollectionsAsync(category).ConfigureAwait(false);
                });

            "Then the fetched collections should contain the added collection"
                .x(() =>
                {
                    Assert.NotNull(actualCollections);
                    actualCollections.Should().Contain(collection);
                });
        }

        [Scenario]
        public void CanGetCategories()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var category = "Integration";
            var collection = "Tests";
            var key = "556990";

            var expected = new AssetAccountMetadata
            {
                AccountNumber = key,
                MarginAccount = "MA06",
                ReferenceAccount = "RF16",
                BankIdentificationReference = "BIR16",
            };

            IList<string> actualCategories = null;

            $"Given the AssetAccountMetadata for category: {category} collection: {collection} key: {key}"
                .x(async () =>
                {
                    await client.Metadata.Add<AssetAccountMetadata>(category, collection, key, expected).ConfigureAwait(false);
                });

            $"When try to get all categories"
                .x(async () =>
                {
                    actualCategories = await client.Metadata.GetCategoriesAsync().ConfigureAwait(false);
                });

            "Then the fetched categories should contain the added category"
                .x(() =>
                {
                    Assert.NotNull(actualCategories);
                    actualCategories.Should().Contain(category);
                });
        }
    }
}
