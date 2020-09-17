using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chest.Client.Models;
using Chest.Core;
using Chest.Data.Entities;
using Chest.Data.Repositories;
using Chest.Models.v2;
using Chest.Models.v2.LocalizedValues;
using Common;
using Lykke.Snow.Common.Model;
using AuditDataType = Chest.Models.v2.Audit.AuditDataType;

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

        public async Task<PaginatedResponse<LocalizedValueByKey>> GetAllAsync(int skip = 0,
            int take = 0)
        {
            var paginatedResponse = await _localizedValuesRepository.GetAllAsync(skip, take);

            var valuesByKey = paginatedResponse.Contents.GroupBy(v => v.Key);
            var result = valuesByKey
                .Select(v => new LocalizedValueByKey(v.Key, v.ToList()))
                .ToList();

            return new PaginatedResponse<LocalizedValueByKey>(result, paginatedResponse.Start, result.Count, paginatedResponse.TotalSize);
        }

        public async Task<Result<LocalizedValuesErrorCodes>> UpsertByKey(string key,
            Dictionary<string, string> valuesByLocale, string userName, string correlationId)
        {
            var existing = await _localizedValuesRepository.GetAllByKey(key);

            var results = new List<Result<LocalizedValuesErrorCodes>>();
            var localesWithError = new List<string>();

            foreach (var valueByLocale in valuesByLocale)
            {
                var localizedValue = new LocalizedValue()
                {
                    Key = key,
                    Locale = valueByLocale.Key,
                    Value = valueByLocale.Value,
                };

                var result = existing.Exists(v => v.Locale == valueByLocale.Key)
                    ? await UpdateAsync(localizedValue, userName, correlationId)
                    : await AddAsync(localizedValue, userName, correlationId);

                results.Add(result);
                if (result.IsFailed)
                {
                    localesWithError.Add(localizedValue.Locale);
                }
            }

            if (results.All(r => r.IsFailed))
                return new Result<LocalizedValuesErrorCodes>(LocalizedValuesErrorCodes.UpsertFailed);

            if (results.Any(r => r.IsFailed))
            {
                return new ErrorResult<LocalizedValuesErrorCodes>(LocalizedValuesErrorCodes.UpsertPartiallyFailed,
                    $"Upsert partially failed for locales: {string.Join(", ", localesWithError)}");
            }

            return new Result<LocalizedValuesErrorCodes>();
        }

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