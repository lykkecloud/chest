using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chest.Data;
using Chest.Data.Entities;
using Chest.Exceptions;
using Chest.Extensions;
using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using Lykke.Common.MsSql;
using Microsoft.EntityFrameworkCore;

namespace Chest.Services
{
    public class LocalizedValuesService : ILocalizedValuesService
    {
        private readonly MsSqlContextFactory<ChestDbContext> _contextFactory;
        private readonly IEFCacheServiceProvider _cacheProvider;

        public LocalizedValuesService(MsSqlContextFactory<ChestDbContext> contextFactory, IEFCacheServiceProvider cacheProvider)
        {
            _contextFactory = contextFactory;
            _cacheProvider = cacheProvider;
        }

        public async Task AddAsync(LocalizedValue value)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            await context.AddAsync(value);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.ValueAlreadyExistsException())
                {
                    throw new LocalizedValueAlreadyExistsException(value);
                }

                throw;
            }

            _cacheProvider.ClearAllCachedEntries();
        }

        public async Task UpdateAsync(LocalizedValue value)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            context.Update(value);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new LocalizedValueNotFoundException(value);
            }

            _cacheProvider.ClearAllCachedEntries();
        }

        public async Task DeleteAsync(string locale, string key)
        {
            await using var context = _contextFactory.CreateDataContext();
            
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

            _cacheProvider.ClearAllCachedEntries();
        }

        public async Task<LocalizedValue> GetAsync(string locale, string key)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var existingValue = await context
                .LocalizedValues
                .AsNoTracking()
                .AsQueryable()
                .FirstOrDefaultAsync(v => v.Key == key && v.Locale == locale);

            return existingValue;
        }

        public async Task<List<LocalizedValue>> GetByLocaleAsync(string locale)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            return await context.LocalizedValues
                .AsNoTracking()
                .AsQueryable()
                .Where(value => value.Locale == locale)
                .Cacheable()
                .AsQueryable()
                .ToListAsync();
        }

        public async Task<List<string>> GetMissingKeysAsync(Locale locale)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var innerQuery = from lv in context.LocalizedValues.AsQueryable()
                where lv.Locale == locale.Id
                select lv.Key;

            var query = from lv in context.LocalizedValues
                join defaultLocale in context.Locales.AsQueryable() on lv.Locale equals defaultLocale.Id
                where defaultLocale.IsDefault && !innerQuery.Contains(lv.Key)
                select lv.Key;

            var missingKeys = await query.ToListAsync();

            return missingKeys;
        }


        /// <summary>
        /// Returns all localized values for a specified key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<LocalizedValue>> GetAllByKey(string key)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var values = await context.LocalizedValues
                .AsNoTracking()
                .AsQueryable()
                .Where(lv => lv.Key == key)
                .ToListAsync();

            return values;
        }
    }
}