// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

namespace Chest.Client.AutorestClient 
{
    public class Metadata: IMetadata
    {
        public Api.IMetadata RefitClient { get; private set; }

        public Metadata(Api.IMetadata refitClient)
        {
            RefitClient = refitClient;
        }
        
        
    }
}