using Chest.Client.Api;

namespace Chest.Client.AutorestClient
{
    public class Locales : ILocales
    {
        public ILocalesApi RefitClient { get; }

        public Locales(ILocalesApi refitClient)
        {
            RefitClient = refitClient;
        }
    }
}