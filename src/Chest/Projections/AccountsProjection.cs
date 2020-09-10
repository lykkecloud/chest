using System.Threading.Tasks;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Currencies;

namespace Chest.Projections
{
    public class CurrencyProjection
    {
        [UsedImplicitly]
        public Task Handle(CurrencyChangedEvent e)
        {
            return Task.CompletedTask;
        }
    }
}