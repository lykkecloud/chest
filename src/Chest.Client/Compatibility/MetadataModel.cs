// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

#pragma warning disable CA2227
#pragma warning disable SA1300 // Element must begin with upper-case letter

namespace Chest.Client.AutorestClient.Models 
{
    /// <summary>
    /// Represents the data model
    /// </summary>
    public class MetadataModel
    {
        public MetadataModel()
        {
        }

        public MetadataModel(IDictionary<string, string> data = null, IList<string> keywords = null)
        {
            Data = data;
            Keywords = keywords;
        }

        public IDictionary<string, string> Data { get; set; }

        public IList<string> Keywords { get; set; }
    }
}
