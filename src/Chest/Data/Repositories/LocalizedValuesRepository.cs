using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chest.Client.Models;
using Chest.Data.Entities;
using Chest.Exceptions;
using Chest.Extensions;
using Chest.Models.v2;
using Chest.Models.v2.LocalizedValues;
using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using Microsoft.EntityFrameworkCore;

namespace Chest.Data.Repositories
{
    public class LocalizedValuesRepository : ILocalizedValuesRepository
    {
        private readonly MsSqlContextFactory<ChestDbContext> _contextFactory;
        private readonly IEFCacheServiceProvider _cacheProvider;

        private const string DoesNotExistException =
            "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

        public LocalizedValuesRepository(MsSqlContextFactory<ChestDbContext> contextFactory,
            IEFCacheServiceProvider cacheProvider)
        {
            _contextFactory = contextFactory;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<LocalizedValuesErrorCodes>> AddAsync(LocalizedValue value)
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
                    return new Result<LocalizedValuesErrorCodes>(LocalizedValuesErrorCodes.AlreadyExists);
                }

                throw;
            }

            _cacheProvider.ClearAllCachedEntries();
            return new Result<LocalizedValuesErrorCodes>();
        }

        public async Task<Result<LocalizedValuesErrorCodes>> UpdateAsync(LocalizedValue value)
        {
            await using var context = _contextFactory.CreateDataContext();

            context.Update(value);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (e.Message.Contains(DoesNotExistException))
                    return new Result<LocalizedValuesErrorCodes>(LocalizedValuesErrorCodes.DoesNotExist);

                throw;
            }

            _cacheProvider.ClearAllCachedEntries();
            return new Result<LocalizedValuesErrorCodes>();
        }

        public async Task<Result<LocalizedValuesErrorCodes>> DeleteAsync(string locale, string key)
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
            catch (DbUpdateConcurrencyException e)
            {
                if (e.Message.Contains(DoesNotExistException))
                    return new Result<LocalizedValuesErrorCodes>(LocalizedValuesErrorCodes.DoesNotExist);

                throw;
            }

            _cacheProvider.ClearAllCachedEntries();
            return new Result<LocalizedValuesErrorCodes>();
        }


        public async Task<Result<LocalizedValue, LocalizedValuesErrorCodes>> GetAsync(string locale, string key)
        {
            await using var context = _contextFactory.CreateDataContext();

            var existingValue = await context
                .LocalizedValues
                .AsNoTracking()
                .AsQueryable()
                .FirstOrDefaultAsync(v => v.Key == key && v.Locale == locale);

            if (existingValue == null)
                return new Result<LocalizedValue, LocalizedValuesErrorCodes>(LocalizedValuesErrorCodes.DoesNotExist);

            return new Result<LocalizedValue, LocalizedValuesErrorCodes>(existingValue);
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

        public async Task<PaginatedResponse<LocalizedValue>> GetAllAsync(int skip = 0, int take = 0)
        {
            await using var context = _contextFactory.CreateDataContext();

            var distinctKeysQuery = context.LocalizedValues
                .AsNoTracking()
                .AsQueryable()
                .Select(v => v.Key)
                .Distinct();

            var query = context.LocalizedValues.AsNoTracking().AsQueryable();
            
            var totalSize = await distinctKeysQuery.CountAsync();

            if (take > 0)
            {
                var innerQuery = distinctKeysQuery
                    .OrderBy(v => v)
                    .Skip(skip)
                    .Take(take);

                query = query.Where(v => innerQuery.Contains(v.Key));
            }

            var values = await query.ToListAsync();
            return new PaginatedResponse<LocalizedValue>(values, skip, values.Count, totalSize);
        }
    }
}