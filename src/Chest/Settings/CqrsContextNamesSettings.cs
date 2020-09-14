using Lykke.SettingsReader.Attributes;

namespace Chest.Settings
{
    public class CqrsContextNamesSettings
    {
        [Optional] public string AssetService { get; set; } = "SettingsService";
        
        public string Chest { get; set; } = "Chest";
    }
}