// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Common;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Chest.Client
{
    [UsedImplicitly]
    public static class Extensions
    {
        [UsedImplicitly]
        public static T Get<T>(this MetadataModelContract metadata)
            => JsonConvert.DeserializeObject<T>(metadata.Data);

        [UsedImplicitly]
        public static Dictionary<string, T> Get<T>(this Dictionary<string, string> data)
            => data.Select(x => x).ToDictionary(x => x.Key, x => JsonConvert.DeserializeObject<T>(x.Value));

        [UsedImplicitly]
        public static MetadataModelContract ToChestContract<T>(this T obj, List<string> keywords = null)
            where T : class
            => new MetadataModelContract {Data = obj.ToJson(), Keywords = keywords.ToJson()};
    }
}