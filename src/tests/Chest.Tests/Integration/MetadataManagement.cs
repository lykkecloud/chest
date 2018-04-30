// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Tests.Integration
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using Chest.Client;
    using Chest.Dto;
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
            var client = new MetadataClient(this.ServiceUrl);
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
                        await client.GetMetadata(key).ConfigureAwait(false);
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
            var client = new MetadataClient(this.ServiceUrl);
            var key = "456987";

            var expected = new Metadata
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

            Metadata actual = null;

            $"Given the metadata for key: {key}"
                .x(async () =>
                {
                    await client.Add(expected).ConfigureAwait(false);
                });

            $"When try to get metadata for the key: {key}"
                .x(async () =>
                {
                    actual = await client.GetMetadata(key).ConfigureAwait(false);
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
            var client = new MetadataClient(this.ServiceUrl);
            var key = "Some-Unique-Key";

            var expected = new Metadata
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
                    await client.Add(expected).ConfigureAwait(false);
                });

            $"When try to add metadata again for key: {key}"
                .x(async () =>
                {
                    try
                    {
                        await client.Add(expected).ConfigureAwait(false);
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
            var client = new MetadataClient(this.ServiceUrl);
            var key = "Case-Sensitive-Key";

            var expected = new Metadata
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
                    await client.Add(expected).ConfigureAwait(false);
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

                        await client.Add(expected).ConfigureAwait(false);
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
    }
}
