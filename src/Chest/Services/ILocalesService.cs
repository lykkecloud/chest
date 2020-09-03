using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Models.v2.Locales;
using Lykke.Snow.Common.Model;

namespace Chest.Services
{
    public interface ILocalesService
    {
        Task<Result<LocalesErrorCodes>> UpsertAsync(Locale locale);
        Task<Result<List<Locale>, LocalesErrorCodes>> GetAllAsync();
        Task<Result<LocalesErrorCodes>> DeleteAsync(string id);
    }
}