using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Models.v2.Locales;
using Lykke.Snow.Common.Model;

namespace Chest.Services
{
    public interface ILocalesService
    {
        Task<Result<LocalesErrorCodes>> UpsertAsync(Locale locale, string userName, string correlationId);
        Task<Result<List<Locale>, LocalesErrorCodes>> GetAllAsync();
        Task<Result<LocalesErrorCodes>> DeleteAsync(string id, string userName, string correlationId);
        Task<Result<Locale, LocalesErrorCodes>> GetDefaultLocale();
    }
}