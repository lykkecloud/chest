// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

using Lykke.HttpClientGenerator;

namespace Chest.Client.AutorestClient
{
    public class ChestClient : IChestClient
    {
        public IMetadata Metadata { get; set; }
        public IRoot Root { get; set; }

        public ChestClient(IHttpClientGenerator metadataGenerator, IHttpClientGenerator rootGenerator = null)
        {
            Metadata = new Metadata(metadataGenerator.Generate<Client.IMetadata>());
            if (rootGenerator != null)
            {
                Root = new Root(rootGenerator.Generate<Client.IIsAlive>());
            }
        }
    }
}