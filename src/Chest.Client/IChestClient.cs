// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

using Chest.Client.Api;

namespace Chest.Client
{
    public interface IChestClient
    {
        /// <summary> 
        /// Gets the IMetadata. 
        /// </summary> 
        IMetadata Metadata { get; } 
    }
}