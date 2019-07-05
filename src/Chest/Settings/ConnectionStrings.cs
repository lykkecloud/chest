// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Settings
{
    using Lykke.SettingsReader.Attributes;

    public class ConnectionStrings
    {
        [SqlCheck]
        public string Chest { get; set; }
    }
}
