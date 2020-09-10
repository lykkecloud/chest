// Copyright (c) 2019 Lykke Corp. 
// See the LICENSE file in the project root for more information. 

namespace Chest.Client.AutorestClient 
{
    public interface IAudit
    {
        Api.IAuditApi RefitClient { get; }
    }
}