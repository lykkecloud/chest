using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Data.Repositories;
using Chest.Models.v2.Audit;
using Chest.Models.v2.LocalizedValues;
using Common;
using Lykke.Snow.Common.Model;

namespace Chest.Services
{
    public class LocalizedValuesService : ILocalizedValuesService
    {
        private readonly ILocalizedValuesRepository _localizedValuesRepository;
        private readonly IAuditService _auditService;

        public LocalizedValuesService(ILocalizedValuesRepository localizedValuesRepository,
            IAuditService auditService)
        {
            _localizedValuesRepository = localizedValuesRepository;
            _auditService = auditService;
        }

        public async Task<Result<LocalizedValuesErrorCodes>> AddAsync(LocalizedValue value, string userName,
            string correlationId)
        {
            var result = await _localizedValuesRepository.AddAsync(value);

            if (result.IsSuccess)
            {
                await _auditService.TryAudit(correlationId, userName, GetId(value), AuditDataType.LocalizedValue,
                    value.ToJson());
            }

            return result;
        }

        public async Task<Result<LocalizedValuesErrorCodes>> UpdateAsync(LocalizedValue value, string userName,
            string correlationId)
        {
            var existing = await _localizedValuesRepository.GetAsync(value.Locale, value.Key);
            if (existing.IsSuccess)
            {
                var result = await _localizedValuesRepository.UpdateAsync(value);

                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, userName, GetId(value), AuditDataType.LocalizedValue,
                        value.ToJson(), existing.Value.ToJson());
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public async Task<Result<LocalizedValuesErrorCodes>> DeleteAsync(string locale, string key, string userName,
            string correlationId)
        {
            var existing = await _localizedValuesRepository.GetAsync(locale, key);
            if (existing.IsSuccess)
            {
                var result = await _localizedValuesRepository.DeleteAsync(locale, key);
                if (result.IsSuccess)
                {
                    await _auditService.TryAudit(correlationId, userName, GetId(locale, key),
                        AuditDataType.LocalizedValue,
                        oldStateJson: existing.Value.ToJson());
                }

                return result;
            }

            return existing.ToResultWithoutValue();
        }

        public Task<Result<LocalizedValue, LocalizedValuesErrorCodes>> GetAsync(string locale, string key)
            => _localizedValuesRepository.GetAsync(locale, key);

        public Task<List<LocalizedValue>> GetByLocaleAsync(string locale)
            => _localizedValuesRepository.GetByLocaleAsync(locale);

        public Task<List<string>> GetMissingKeysAsync(Locale locale)
            => _localizedValuesRepository.GetMissingKeysAsync(locale);

        public Task<List<LocalizedValue>> GetAllByKey(string key)
            => _localizedValuesRepository.GetAllByKey(key);

        private static string GetId(LocalizedValue value)
        {
            return GetId(value.Locale, value.Key);
        }

        private static string GetId(string locale, string key)
        {
            return $"{locale}.{key}";
        }
    }
}