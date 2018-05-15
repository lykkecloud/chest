// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Dto
{
    /// <summary>
    /// Represents the metadata interface that can be implemented by consumers
    /// </summary>
    public interface IMetadataDto
    {
        /// <summary>
        /// Gets or sets the Key that will be used to fill Metadata.Key
        /// This is the only required field, all additional fields implemented by 
        /// consumer class will be converted to Dictionary and fill Metadata.Data
        /// </summary>
        string Key { get; set; }
    }
}