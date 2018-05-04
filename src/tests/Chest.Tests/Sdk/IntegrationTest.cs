// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Tests.Sdk
{
    using Xunit;

    [Collection("Chest")]
    public class IntegrationTest
    {
        private ChestFixture fixture;

        public IntegrationTest(ChestFixture fixture)
        {
            this.fixture = fixture;
        }

#pragma warning disable CA1056 // Uri properties should not be strings
        protected string ServiceUrl => this.fixture.ServiceUrl;
#pragma warning restore CA1056 // Uri properties should not be strings
    }
}
