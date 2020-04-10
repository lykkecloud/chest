// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chest.Client.AutorestClient.Models;
using Microsoft.Rest;

namespace Chest.Client.AutorestClient 
{
    public interface IMetadata
    {
        Api.IMetadata RefitClient { get; }
    } 
}