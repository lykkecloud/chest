using System;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Exceptions;
using Chest.Services;
using JetBrains.Annotations;
using MarginTrading.AssetService.Contracts.Currencies;
using MarginTrading.AssetService.Contracts.Enums;

namespace Chest.Projections
{
    public class CurrencyProjection
    {
        private readonly ILocalizedValuesService _localizedValuesService;
        private readonly ILocalesService _localesService;

        public CurrencyProjection(ILocalizedValuesService localizedValuesService, ILocalesService localesService)
        {
            _localizedValuesService = localizedValuesService;
            _localesService = localesService;
        }
        
        [UsedImplicitly]
        public async Task Handle(CurrencyChangedEvent e)
        {
            switch (e.ChangeType)
            {
                case ChangeType.Creation:
                    // create ONE default translation
                    await CreateDefaultTranslation(e);
                    break;
                case ChangeType.Edition:
                    // do nothing
                    break;
                case ChangeType.Deletion:
                    // delete ALL translations
                    await DeleteAllTranslations(e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task DeleteAllTranslations(CurrencyChangedEvent e)
        {
            var key = GetKey(e.OldCurrency.Id);
            var values = await _localizedValuesService.GetAllByKey(key);

            foreach (var value in values)
            {
                await _localizedValuesService.DeleteAsync(value.Locale, value.Key, e.Username, e.CorrelationId);
            }
        }

        private async Task CreateDefaultTranslation(CurrencyChangedEvent e)
        {
            var defaultLocaleResult = await _localesService.GetDefaultLocale();
            if (defaultLocaleResult.IsFailed)
            {
                // cannot create localized values if default locale does not exist
                throw new DefaultLocaleDoesNotExistException("Default locale must exist before trying to create currency translations");
            }
            
            var value = new LocalizedValue()
            {
                Locale = defaultLocaleResult.Value.Id,
                Key = GetKey(e.NewCurrency.Id),
                Value = e.NewCurrency.InterestRateMdsCode,
            };

            await _localizedValuesService.AddAsync(value, e.Username, e.CorrelationId);
        }

        private string GetKey(string currencyId) => $"currencyInterestRateName.{currencyId}";
    }
}