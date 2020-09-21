using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Models.v2.Locales;
using Lykke.Snow.Common.Model;

namespace Chest.Data.Repositories
{
    public interface ILocalesRepository
    {
        Task<Result<List<Locale>, LocalesErrorCodes>> GetAllAsync();
        Task<Result<LocalesErrorCodes>> DeleteAsync (string id);
        Task<Result<Locale, LocalesErrorCodes>> GetById(string id);
        Task<Result<LocalesErrorCodes>> AddAsync(Locale locale);
        Task<Result<LocalesErrorCodes>> UpdateAsync(Locale locale);
        Task<Result<Locale, LocalesErrorCodes>> GetDefaultLocale();
        Task<bool> ExistsAsync(string id);
        Task<bool> HasAnyLocalizedValues(string id);
    }
}