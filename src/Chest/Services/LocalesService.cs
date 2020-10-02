using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chest.Core;
using Chest.Data.Entities;
using Chest.Data.Repositories;
using Chest.Models.v2.Audit;
using Chest.Models.v2.Locales;
using Common;
using Lykke.Snow.Common.Model;

namespace Chest.Services
{
    public class LocalesService : ILocalesService
    {
        private readonly ILocalesRepository _localesRepository;
        private readonly ILocalizedValuesService _localizedValuesService;
        private readonly IAuditService _auditService;

        public LocalesService(ILocalesRepository localesRepository,
            ILocalizedValuesService localizedValuesService,
            IAuditService auditService)
        {
            _localesRepository = localesRepository;
            _localizedValuesService = localizedValuesService;
            _auditService = auditService;
        }

        public async Task<Result<LocalesErrorCodes>> UpsertAsync(Locale locale, string userName, string correlationId)
        {
            var existing = await _localesRepository.GetById(locale.Id);

            // todo: should use GetDefaultLocale instead, but it can return an error - should handle it
            var locales = (await _localesRepository.GetAllAsync()).Value;
            var defaultLocale = locales.FirstOrDefault(x => x.IsDefault);

            // create if not exists
            if (existing.IsFailed && defaultLocale == null)
            {
                var result = await _localesRepository.AddAsync(locale);
                if (result.IsSuccess)
                    await _auditService.TryAudit(correlationId, userName, locale.Id, AuditDataType.Locale,
                        locale.ToJson());

                return result;
            }

            if (existing.IsSuccess && existing.Value.IsDefault)
            {
                // cannot transit into a state without any default locales
                if (!locale.IsDefault)
                    return new Result<LocalesErrorCodes>(LocalesErrorCodes.CannotDeleteDefaultLocale);

                // nothing to update
                return new Result<LocalesErrorCodes>();
            }

            if (locale.IsDefault && defaultLocale != null)
            {
                var keys = await _localizedValuesService.GetMissingKeysAsync(locale);
                if (keys.Count > 0)
                    return new ErrorResult<LocalesErrorCodes>(LocalesErrorCodes.CannotSetLocaleAsDefault,
                        $"Must provide localized value for keys: {string.Join(", ", keys)}");

                // remove default from the old default locale
                var old = defaultLocale.ToJson();
                defaultLocale.IsDefault = false;
                var result = await _localesRepository.UpdateAsync(defaultLocale);
                if (result.IsSuccess)
                    await _auditService.TryAudit(correlationId, userName, defaultLocale.Id, AuditDataType.Locale,
                        defaultLocale.ToJson(), old);
            }

            // set new locale as default
            if (existing.IsSuccess)
            {
                var result = await _localesRepository.UpdateAsync(locale);
                if (result.IsSuccess)
                    await _auditService.TryAudit(correlationId, userName, locale.Id, AuditDataType.Locale,
                        locale.ToJson(), locales.First(l => l.Id == locale.Id).ToJson());

                return result;
            }
            else
            {
                var result = await _localesRepository.AddAsync(locale);
                if (result.IsSuccess)
                    await _auditService.TryAudit(correlationId, userName, locale.Id, AuditDataType.Locale,
                        locale.ToJson());

                return result;
            }
        }

        public Task<Result<List<Locale>, LocalesErrorCodes>> GetAllAsync()
            => _localesRepository.GetAllAsync();

        public Task<Result<Locale, LocalesErrorCodes>> GetDefaultLocale()
            => _localesRepository.GetDefaultLocale();

        public async Task<Result<LocalesErrorCodes>> DeleteAsync(string id, string userName, string correlationId)
        {
            var existing = await _localesRepository.GetById(id);

            if (existing.IsFailed) return existing.ToResultWithoutValue();
            
            if(await _localesRepository.HasAnyLocalizedValues(id))
                return new Result<LocalesErrorCodes>(LocalesErrorCodes.CannotDeleteLocaleAssignedToAnyLocalizedValue);

            if (existing.Value.IsDefault)
                return new Result<LocalesErrorCodes>(LocalesErrorCodes.CannotDeleteDefaultLocale);

            var result = await _localesRepository.DeleteAsync(id);
            if(result.IsSuccess) await _auditService.TryAudit(correlationId, userName, id, AuditDataType.Locale,
                oldStateJson: existing.Value.ToJson());

            return result;
        }
    }
}