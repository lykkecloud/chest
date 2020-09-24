using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Models.v2;
using Chest.Models.v2.LocalizedValues;
using Lykke.Snow.Common.Model;

namespace Chest.Services
{
    public interface ILocalizedValuesService
    {
        Task<Result<LocalizedValuesErrorCodes>> AddAsync(LocalizedValue value, string userName, string correlationId);
        Task<Result<LocalizedValuesErrorCodes>> UpdateAsync(LocalizedValue value, string userName, string correlationId);
        Task<Result<LocalizedValuesErrorCodes>> DeleteAsync(string locale, string key, string userName,
            string correlationId);
        Task<Result<LocalizedValue, LocalizedValuesErrorCodes>> GetAsync(string locale, string key);

        Task<List<LocalizedValue>> GetByLocaleAsync(string locale);
        Task<List<string>> GetMissingKeysAsync(Locale locale);

        /// <summary>
        /// Returns all localized values for a specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<LocalizedValue>> GetAllByKey(string key);

        Task<PaginatedResponse<LocalizedValueByKey>> GetAllAsync(int skip = 0, int take = 0);
        Task<Result<LocalizedValuesErrorCodes>> UpsertByKey(string key, Dictionary<string, string> valuesByLocale,
            string userName, string correlationId);
    }
}