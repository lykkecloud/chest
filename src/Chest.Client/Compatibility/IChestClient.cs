// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

namespace Chest.Client.AutorestClient
{
    public interface IChestClient
    {
        /// <summary> 
        /// Gets the IMetadata. 
        /// </summary> 
        IMetadata Metadata { get; } 
 
        /// <summary> 
        /// Gets the IRoot. 
        /// </summary> 
        IRoot Root { get; }
    }
}