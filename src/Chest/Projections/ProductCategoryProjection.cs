using System;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Exceptions;
using Chest.Services;
using MarginTrading.AssetService.Contracts.Enums;
using MarginTrading.AssetService.Contracts.ProductCategories;

namespace Chest.Projections
{
    public class ProductCategoryProjection
    {
        private readonly ILocalizedValuesService _localizedValuesService;
        private readonly ILocalesService _localesService;

        public ProductCategoryProjection(ILocalizedValuesService localizedValuesService, ILocalesService localesService)
        {
            _localizedValuesService = localizedValuesService;
            _localesService = localesService;
        }
        
        public async Task Handle(ProductCategoryChangedEvent e)
        {
            switch (@e.ChangeType)
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

        private async Task CreateDefaultTranslation(ProductCategoryChangedEvent e)
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
                Key = e.NewValue.LocalizationToken,
                Value = e.OriginalCategoryName ?? e.NewValue.Id,
            };

            await _localizedValuesService.AddAsync(value, e.Username, e.CorrelationId);
        }

        private async Task DeleteAllTranslations(ProductCategoryChangedEvent e)
        {
            var key = e.OldValue.LocalizationToken;
            var values = await _localizedValuesService.GetAllByKey(key);

            foreach (var value in values)
            {
                await _localizedValuesService.DeleteAsync(value.Locale, value.Key, e.Username, e.CorrelationId);
            }
        }
    }
}