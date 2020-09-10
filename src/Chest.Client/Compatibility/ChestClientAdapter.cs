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
        
        public IAudit AuditApi { get; }
        
        public ILocales LocalesApi { get; }
        
        public ILocalizedValues LocalizedValuesApi { get; }

        public ChestClientAdapter(IHttpClientGenerator metadataGenerator, IHttpClientGenerator rootGenerator = null)
        {
            Metadata = new Metadata(metadataGenerator.Generate<Api.IMetadata>());
            if (rootGenerator != null)
            {
                Root = new Root(rootGenerator.Generate<IIsAlive>());
            }
            
            AuditApi = new Audit(metadataGenerator.Generate<IAuditApi>());
            LocalesApi = new Locales(metadataGenerator.Generate<ILocalesApi>());
            LocalizedValuesApi = new LocalizedValues(metadataGenerator.Generate<ILocalizedValuesApi>());
        }
    }
}