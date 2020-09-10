using Chest.Client.Api;

namespace Chest.Client.AutorestClient
{
    public class LocalizedValues : ILocalizedValues
    {
        public ILocalizedValuesApi RefitClient { get; }

        public LocalizedValues(ILocalizedValuesApi refitClient)
        {
            RefitClient = refitClient;
        }
    }
}