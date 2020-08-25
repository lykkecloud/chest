using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data;
using Chest.Data.Entities;
using Chest.Exceptions;
using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using Lykke.Common.MsSql;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Chest.Services
{
    public class LocalizedValuesService : ILocalizedValuesService
    {
        private readonly ApplicationDbContext context;
        private readonly IEFCacheServiceProvider cacheProvider;

        public LocalizedValuesService(ApplicationDbContext context, IEFCacheServiceProvider cacheProvider)
        {
            this.context = context;
            this.cacheProvider = cacheProvider;
        }

        public async Task AddAsync(LocalizedValue value)
        {
            await context.AddAsync(value);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is SqlException sqlException &&
                    sqlException.Number == MsSqlErrorCodes.DuplicateIndex)
                {
                    throw new LocalizedValueAlreadyExistsException(value);
                }

                throw;
            }

            cacheProvider.ClearAllCachedEntries();
        }

        public async Task UpdateAsync(LocalizedValue value)
        {
            context.Update(value);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new LocalizedValueNotFoundException(value);
            }

            cacheProvider.ClearAllCachedEntries();
        }

        public async Task DeleteAsync(string locale, string key)
        {
            var value = new LocalizedValue()
            {
                Locale = locale,
                Key = key,
            };

            context.Attach(value);
            context.LocalizedValues.Remove(value);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new LocalizedValueNotFoundException(locale, key);
            }

            cacheProvider.ClearAllCachedEntries();
        }

        public async Task<LocalizedValue> GetAsync(string locale, string key)
        {
            var existingValue = await context
                .LocalizedValues
                .AsQueryable()
                .FirstOrDefaultAsync(v => v.Key == key && v.Locale == locale);

            return existingValue;
        }

        public async Task<List<LocalizedValue>> GetByLocaleAsync(string locale)
        {
            return await context.LocalizedValues
                .AsQueryable()
                .Where(value => value.Locale == locale)
                .Cacheable()
                .AsQueryable()
                .ToListAsync();
        }
    }
}