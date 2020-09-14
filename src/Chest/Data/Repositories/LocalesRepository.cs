using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Extensions;
using Chest.Models.v2.Locales;
using EFSecondLevelCache.Core.Contracts;
using Lykke.Common.MsSql;
using Lykke.Snow.Common.Model;
using Microsoft.EntityFrameworkCore;

namespace Chest.Data.Repositories
{
    public class LocalesRepository : ILocalesRepository
    {
        private readonly MsSqlContextFactory<ChestDbContext> _contextFactory;
        private readonly IEFCacheServiceProvider _cacheProvider;

        private const string DoesNotExistException =
            "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

        public LocalesRepository(MsSqlContextFactory<ChestDbContext> contextFactory,
            IEFCacheServiceProvider cacheProvider)
        {
            _contextFactory = contextFactory;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<Locale, LocalesErrorCodes>> GetById(string id)
        {
            await using var context = _contextFactory.CreateDataContext();

            var value = await context.Locales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return value == null
                ? new Result<Locale, LocalesErrorCodes>(LocalesErrorCodes.DoesNotExist)
                : new Result<Locale, LocalesErrorCodes>(value);
        }

        public async Task<Result<Locale, LocalesErrorCodes>> GetDefaultLocale()
        {
            await using var context = _contextFactory.CreateDataContext();

            var value = await context.Locales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsDefault);

            return value == null
                ? new Result<Locale, LocalesErrorCodes>(LocalesErrorCodes.DoesNotExist)
                : new Result<Locale, LocalesErrorCodes>(value);
        }

        public async Task<Result<LocalesErrorCodes>> AddAsync(Locale locale)
        {
            await using var context = _contextFactory.CreateDataContext();

            await context.Locales.AddAsync(locale);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.ValueAlreadyExistsException())
                {
                    return new Result<LocalesErrorCodes>(LocalesErrorCodes.AlreadyExists);
                }

                throw;
            }

            _cacheProvider.ClearAllCachedEntries();
            return new Result<LocalesErrorCodes>();
        }

        public async Task<Result<LocalesErrorCodes>> UpdateAsync(Locale locale)
        {
            await using var context = _contextFactory.CreateDataContext();

            context.Update(locale);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (e.Message.Contains(DoesNotExistException))
                    return new Result<LocalesErrorCodes>(LocalesErrorCodes.DoesNotExist);

                throw;
            }

            _cacheProvider.ClearAllCachedEntries();
            return new Result<LocalesErrorCodes>();
        }

        public async Task<Result<List<Locale>, LocalesErrorCodes>> GetAllAsync()
        {
            await using var context = _contextFactory.CreateDataContext();

            var locales = await context.Locales.AsNoTracking().ToListAsync();

            return new Result<List<Locale>, LocalesErrorCodes>(locales);
        }

        public async Task<Result<LocalesErrorCodes>> DeleteAsync(string id)
        {
            await using var context = _contextFactory.CreateDataContext();
            
            var locale = new Locale() {Id = id};

            context.Locales.Remove(locale);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return new Result<LocalesErrorCodes>(LocalesErrorCodes.DoesNotExist);
            }

            _cacheProvider.ClearAllCachedEntries();
            return new Result<LocalesErrorCodes>();
        }
    }
}