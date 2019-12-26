using Chest.Client.Models;
using Chest.Models.v2;

namespace Chest.Mappers
{
    public static class MetadataModelMapper
    {
        public static MetadataModelContract Map(MetadataModel model)
            => new MetadataModelContract
            {
                Data = model.Data,
                Keywords = model.Keywords,
            };
        
        public static MetadataModel Map(MetadataModelContract model)
            => new MetadataModel
            {
                Data = model.Data,
                Keywords = model.Keywords,
            };
    }
}