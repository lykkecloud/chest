using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Extensions;
using Chest.Models.v2.Locales;
using EFSecondLevelCache.Core.Contracts;
using Lykke.Snow.Common.Model;
using Microsoft.EntityFrameworkCore;

namespace Chest.Data.Repositories
{
    public class LocalesRepository : ILocalesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IEFCacheServiceProvider _cacheProvider;

        private const string DoesNotExistException =
            "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

        public LocalesRepository(ApplicationDbContext context, IEFCacheServiceProvider cacheProvider)
        {
            _context = context;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<Locale, LocalesErrorCodes>> GetById(string id)
        {
            var value = await _context.Locales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return value == null
                ? new Result<Locale, LocalesErrorCodes>(LocalesErrorCodes.DoesNotExist)
                : new Result<Locale, LocalesErrorCodes>(value);
        }

        public async Task<Result<LocalesErrorCodes>> AddAsync(Locale locale)
        {
            await _context.Locales.AddAsync(locale);

            try
            {
                await _context.SaveChangesAsync();
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
            _context.Update(locale);

            try
            {
                await _context.SaveChangesAsync();
                
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
            var locales = await _context.Locales.AsNoTracking().ToListAsync();

            return new Result<List<Locale>, LocalesErrorCodes>(locales);
        }

        public async Task<Result<LocalesErrorCodes>> DeleteAsync(string id)
        {
            var locale = new Locale() {Id = id};

            _context.Locales.Remove(locale);

            try
            {
                await _context.SaveChangesAsync();
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