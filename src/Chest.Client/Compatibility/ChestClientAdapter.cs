// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

using Chest.Client.Api;
using Lykke.HttpClientGenerator;

namespace Chest.Client.AutorestClient
{
    public class ChestClientAdapter : IChestClient
    {
        public IMetadata Metadata { get; set; }
        public IRoot Root { get; set; }
        
        public IAuditApi AuditApi { get; }
        
        public ILocalesApi LocalesApi { get; }
        
        public ILocalizedValuesApi LocalizedValuesApi { get; }

        public ChestClientAdapter(IHttpClientGenerator metadataGenerator, IHttpClientGenerator rootGenerator = null)
        {
            Metadata = new Metadata(metadataGenerator.Generate<Api.IMetadata>());
            if (rootGenerator != null)
            {
                Root = new Root(rootGenerator.Generate<IIsAlive>());
            }

            AuditApi = metadataGenerator.Generate<IAuditApi>();
            LocalesApi = metadataGenerator.Generate<ILocalesApi>();
            LocalizedValuesApi = metadataGenerator.Generate<ILocalizedValuesApi>();
        }
    }
}