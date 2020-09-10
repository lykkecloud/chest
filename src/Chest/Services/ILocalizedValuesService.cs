using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data.Entities;

namespace Chest.Services
{
    public interface ILocalizedValuesService
    {
        Task AddAsync(LocalizedValue value);
        Task UpdateAsync(LocalizedValue value);
        Task DeleteAsync(string locale, string key);
        Task<LocalizedValue> GetAsync(string locale, string key);

        Task<List<LocalizedValue>> GetByLocaleAsync(string locale);
        Task<List<string>> GetMissingKeysAsync(Locale locale);

        /// <summary>
        /// Returns all localized values for a specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<LocalizedValue>> GetAllByKey(string key);
    }
}