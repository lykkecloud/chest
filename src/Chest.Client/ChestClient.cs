using Chest.Client.Api;
using Lykke.HttpClientGenerator;

namespace Chest.Client
{
    public class ChestClient : IChestClient
    {
        public ChestClient(IHttpClientGenerator metadataGenerator)
        {
            Metadata = metadataGenerator.Generate<IMetadata>();
        }
        
        public IMetadata Metadata { get; }
    }
}