// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

namespace Chest.Settings
{
    using Lykke.SettingsReader.Attributes;

    public class ConnectionStrings
    {
        [SqlCheck]
        public string Chest { get; set; }
    }
}
