﻿// Copyright (c) 2019 Lykke Corp.
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

        protected string ServiceUrl => fixture.ServiceUrl;
    }
}
