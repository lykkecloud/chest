// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

namespace Chest.Tests.Sdk
{
    using System;
    using Xunit;

    [Collection("Chest")]
    public class IntegrationTest
    {
        private ChestFixture fixture;

        public IntegrationTest(ChestFixture fixture)
        {
            this.fixture = fixture;
        }

        protected Uri ServiceUrl => this.fixture.ServiceUrl;
    }
}
