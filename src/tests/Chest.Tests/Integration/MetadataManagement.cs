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
    using Chest.Tests;
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
            var key = "Unknown";

            HttpException exception = null;

            $"Given a key: {key} that was never added to the metadata service before"
                .x(() =>
                {
                });

            $"When I query the metadata for that key: {key}"
                .x(async () =>
                {
                    // arrange
                    try
                    {
                        await client.Metadata.GetAsync(key).ConfigureAwait(false);
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
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var key = "456987";

            var expected = new MetadataModel
            {
                Key = key,
                Data = new Dictionary<string, string>
                {
                    { "account_number", key },
                    { "margin_account", "MA01" },
                    { "reference_account", "RF11" },
                    { "bank_identification_reference", "BIR11" },
                }
            };

            MetadataModel actual = null;

            $"Given the metadata for key: {key}"
                .x(async () =>
                {
                    await client.Metadata.AddAsync(expected).ConfigureAwait(false);
                });

            $"When try to get metadata for the key: {key}"
                .x(async () =>
                {
                    actual = await client.Metadata.GetAsync(key).ConfigureAwait(false);
                });

            "Then the fetched metadata should be same"
                .x(() =>
                {
                    Assert.NotNull(actual);
                    actual.Should().BeEquivalentTo(expected);
                });
        }

        [Scenario]
        public void ShouldNotAddKeyMultipleTimes()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var key = "Some-Unique-Key";

            var expected = new MetadataModel
            {
                Key = key,
                Data = new Dictionary<string, string>
                {
                    { "account_number", key },
                    { "margin_account", "MA02" },
                    { "reference_account", "RF12" },
                    { "bank_identification_reference", "BIR12" },
                }
            };

            HttpException httpException = null;

            $"Given the metadata for key: {key}"
                .x(async () =>
                {
                    await client.Metadata.AddAsync(expected).ConfigureAwait(false);
                });

            $"When try to add metadata again for key: {key}"
                .x(async () =>
                {
                    try
                    {
                        await client.Metadata.AddAsync(expected).ConfigureAwait(false);
                    }
                    catch (HttpException exp)
                    {
                        httpException = exp;
                    }
                });

            $"Then system should return 409 Conflict"
                .x(() =>
                {
                    Assert.Equal(HttpStatusCode.Conflict, httpException.StatusCode);
                    Assert.NotNull(httpException.ContentMessage);
                });
        }

        [Scenario]
        public void ShouldNotAddKeyMultipleTimesEvenInDifferentCase()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
            var key = "Case-Sensitive-Key";

            var expected = new MetadataModel
            {
                Key = key,
                Data = new Dictionary<string, string>
                {
                    { "account_number", key },
                    { "margin_account", "MA03" },
                    { "reference_account", "RF13" },
                    { "bank_identification_reference", "BIR13" },
                }
            };

            HttpException httpException = null;

            $"Given the metadata for key: {key}"
                .x(async () =>
                {
                    await client.Metadata.AddAsync(expected).ConfigureAwait(false);
                });

#pragma warning disable CA1308 // Normalize strings to uppercase
            key = key.ToLower(CultureInfo.InvariantCulture);
#pragma warning restore CA1308 // Normalize strings to uppercase

            $"When try to add metadata again for same key: {key} but in different case"
                .x(async () =>
                {
                    try
                    {
                        expected.Key = key;

                        await client.Metadata.AddAsync(expected).ConfigureAwait(false);
                    }
                    catch (HttpException exp)
                    {
                        httpException = exp;
                    }
                });

            $"Then system should return 409 Conflict"
                .x(() =>
                {
                    Assert.Equal(HttpStatusCode.Conflict, httpException.StatusCode);
                    Assert.NotNull(httpException.ContentMessage);
                });
        }

        [Scenario]
        public void CanAddMetadataUsingDto()
        {
            var client = new ChestClient(this.ServiceUrl, new[] { new SuccessHandler() });
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
                    await client.Metadata.AddAsync(key, expected).ConfigureAwait(false);
                });

            $"When try to get AssetAccountMetadata for the key: {key}"
                .x(async () =>
                {
                    actual = await client.Metadata.GetAsync<AssetAccountMetadata>(key).ConfigureAwait(false);
                });

            "Then the fetched AssetAccountMetadata should be same"
                .x(() =>
                {
                    Assert.NotNull(actual);
                    actual.Should().BeEquivalentTo(expected);
                });
        }
    }
}
