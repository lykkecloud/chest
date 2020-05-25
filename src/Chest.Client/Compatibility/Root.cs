// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

using Chest.Client.Api;

namespace Chest.Client.AutorestClient 
{
    public class Root: IRoot
    {
        public readonly IIsAlive _refitClient;

        public Root(IIsAlive refitClient)
        {
            _refitClient = refitClient;
        }
        
        
    }
}