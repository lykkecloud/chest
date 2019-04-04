// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

namespace Chest.Tests.Sdk
{
    using Xunit;

    // LINK (Cameron): https://xunit.github.io/docs/shared-context.html
    [CollectionDefinition("Chest")]
    public class ChestCollection : ICollectionFixture<ChestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
