using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chest.Core;
using Chest.Data.Entities;
using Chest.Data.Repositories;
using Chest.Models.v2.Locales;
using Lykke.Snow.Common.Model;

namespace Chest.Services
{
    public class LocalesService : ILocalesService
    {
        private readonly ILocalesRepository _localesRepository;
        private readonly ILocalizedValuesService _localizedValuesService;

        public LocalesService(ILocalesRepository localesRepository, ILocalizedValuesService localizedValuesService)
        {
            _localesRepository = localesRepository;
            _localizedValuesService = localizedValuesService;
        }

        public async Task<Result<LocalesErrorCodes>> UpsertAsync(Locale locale)
        {
            var existing = await _localesRepository.GetById(locale.Id);

            var defaultLocale = (await _localesRepository.GetAllAsync()).Value.FirstOrDefault(x => x.IsDefault);

            // create if not exists
            if (existing.IsFailed && defaultLocale == null)
            {
                return await _localesRepository.AddAsync(locale);
            }

            if (existing.IsSuccess && existing.Value.IsDefault)
            {
                // cannot transit into a state without any default locales
                if (!locale.IsDefault)
                    return new Result<LocalesErrorCodes>(LocalesErrorCodes.CannotDeleteDefaultLocale);

                // nothing to update
                return new Result<LocalesErrorCodes>();
            }

            if (locale.IsDefault)
            {
                var keys = await _localizedValuesService.GetMissingKeysAsync(locale);
                if (keys.Count > 0)
                    return new ErrorResult<LocalesErrorCodes>(LocalesErrorCodes.CannotSetLocaleAsDefault, $"Must provide localized value for keys: {string.Join(", ", keys)}");

                // remove default from the old default locale
                if (defaultLocale != null)
                {
                    defaultLocale.IsDefault = false;
                    await _localesRepository.UpdateAsync(defaultLocale);
                }
            }

            // set new locale as default
            if (existing.IsSuccess)
            {
                return await _localesRepository.UpdateAsync(locale);
            }
            else
            {
                return await _localesRepository.AddAsync(locale);
            }
        }

        public Task<Result<List<Locale>, LocalesErrorCodes>> GetAllAsync()
            => _localesRepository.GetAllAsync();

        public async Task<Result<LocalesErrorCodes>> DeleteAsync(string id)
        {
            var existing = await _localesRepository.GetById(id);

            if (existing.IsFailed) return existing.ToResultWithoutValue();

            if (existing.Value.IsDefault)
                return new Result<LocalesErrorCodes>(LocalesErrorCodes.CannotDeleteDefaultLocale);

            return await _localesRepository.DeleteAsync(id);
        }
    }
}