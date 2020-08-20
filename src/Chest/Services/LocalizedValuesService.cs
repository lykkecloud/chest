using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data;
using Chest.Data.Entities;
using Chest.Exceptions;
using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
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
            var existingValue = await context
                .LocalizedValues
                .AsQueryable()
                .FirstOrDefaultAsync(v => v.Key == value.Key && v.Locale == value.Locale);

            if (existingValue != null) throw new LocalizedValueAlreadyExistsException(value);

            await context.AddAsync(value);
            await context.SaveChangesAsync();

            cacheProvider.ClearAllCachedEntries();
        }

        public async Task UpdateAsync(LocalizedValue value)
        {
            var existingValue = await context
                .LocalizedValues
                .AsQueryable()
                .FirstOrDefaultAsync(v => v.Key == value.Key && v.Locale == value.Locale);

            if (existingValue == null)
            {
                throw new LocalizedValueNotFoundException(value);
            }

            existingValue.Value = value.Value;

            await context.SaveChangesAsync();

            cacheProvider.ClearAllCachedEntries();
        }

        public async Task DeleteAsync(string locale, string key)
        {
            var existingValue = await context
                .LocalizedValues
                .AsQueryable()
                .FirstOrDefaultAsync(v => v.Key == key && v.Locale == locale);

            if (existingValue == null) throw new LocalizedValueNotFoundException(locale, key);

            context.LocalizedValues.Remove(existingValue);
            await context.SaveChangesAsync();

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