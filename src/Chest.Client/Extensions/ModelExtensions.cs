using System.Collections.Generic;
using System.Linq;
using Chest.Client.AutorestClient.Models;
using Common;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Chest.Client.Extensions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ModelExtensions
    {
        public static T Get<T>(this MetadataModelContract metadata)
            => JsonConvert.DeserializeObject<T>(metadata.Data);

        public static (T metadata, IList<string> keywords) GetWithKeywords<T>(this MetadataModelContract metadata)
            => (JsonConvert.DeserializeObject<T>(metadata.Data),
                JsonConvert.DeserializeObject<IList<string>>(metadata.Keywords));

        public static Dictionary<string, T> Get<T>(this Dictionary<string, string> data)
            => data.Select(x => x).ToDictionary(x => x.Key, x => JsonConvert.DeserializeObject<T>(x.Value));

        public static MetadataModelContract ToChestContract(this MetadataModel model)
            => new MetadataModelContract
            {
                Data = model.Data.ToJson(),
                Keywords = model.Keywords == null
                    ? string.Empty
                    : model.Keywords.ToJson()
            };

        public static MetadataModelContract ToChestContract<T>(this T obj, List<string> keywords = null)
            where T : class
            => new MetadataModelContract
            {
                Data = obj.ToJson(),
                Keywords = keywords == null
                    ? string.Empty
                    : keywords.ToJson()
            };
    }
}