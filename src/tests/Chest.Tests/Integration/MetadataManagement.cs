// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Tests.Integration
{
    using System.Collections.Generic;
    using System.Globalization;
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
        public void CanGetAndAddMetdata()
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
                    { "reference_account", "RF11" },
                    { "bank_identification_reference", "BIR11" },
                }
            };

            "Given the metadata for key doesn't exist, When try to get metadata for the key, Then system should should return 404 NotFound"
                .x(async () =>
                {
                    try
                    {
                        var response = await client.GetMetadata(key).ConfigureAwait(false);
                        Assert.Null(response);
                    }
                    catch (HttpException exp)
                    {
                        if (exp.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Assert.NotNull(exp.ContentMessage);
                            return;
                        }
                        throw;
                    }
                    catch
                    {
                        throw;
                    }
                });

            "Given the metadata for the key doesn't exist, When try to add metadata for the key, Then system should add data"
                .x(async () =>
                {
                    await client.Add(expected).ConfigureAwait(false);
                });

            "Given the metadata for the key exist, When try to get metadata for the key, Then system should return the same metadata"
                .x(async () =>
                {
                    var response = await client.GetMetadata(key).ConfigureAwait(false);
                    Assert.NotNull(response);

                    response.Should().BeEquivalentTo(expected);
                });

            "Given the metadata for key already exist, When try to add metadata for the key again, Then system should return 409 Conflict"
                .x(async () =>
                {
                    try
                    {
                        await client.Add(expected).ConfigureAwait(false);
                    }
                    catch (HttpException exp)
                    {
                        if (exp.StatusCode == System.Net.HttpStatusCode.Conflict)
                        {
                            Assert.NotNull(exp.ContentMessage);

                            return;
                        }

                        throw;
                    }
                });

            "Given the metadata for key already exist, When try to add metadata for the key but in different casing, Then system should return 409 Conflict"
                .x(async () =>
                {
                    try
                    {
#pragma warning disable CA1308 // Normalize strings to uppercase
                        expected.Key = key.ToLower(CultureInfo.InvariantCulture);
#pragma warning restore CA1308 // Normalize strings to uppercase

                        await client.Add(expected).ConfigureAwait(false);
                    }
                    catch (HttpException exp)
                    {
                        if (exp.StatusCode == System.Net.HttpStatusCode.Conflict)
                        {
                            Assert.NotNull(exp.ContentMessage);

                            return;
                        }

                        throw;
                    }
                });
        }
    }
}
