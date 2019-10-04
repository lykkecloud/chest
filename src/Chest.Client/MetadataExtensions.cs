// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Chest.Client
{
    public static class MetadataExtensions
    {
        public static T Get<T>(this MetadataModelContract metadata)
            => JsonConvert.DeserializeObject<T>(metadata.Data);
    }
}