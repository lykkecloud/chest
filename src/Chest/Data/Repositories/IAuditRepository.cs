using System.Linq;
using System.Threading.Tasks;
using Chest.Data.Entities;
using Chest.Models.v2;
using Chest.Models.v2.Audit;
using Microsoft.EntityFrameworkCore;

namespace Chest.Data.Repositories
{
    public interface IAuditRepository
    {
        Task InsertAsync(IAuditModel model);

        Task<PaginatedResponse<IAuditModel>> GetAll(AuditLogsFilterDto filter, int? skip, int? take);
    }
    
    public class AuditRepository : IAuditRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InsertAsync(IAuditModel model)
        {
            var entity = AuditEntity.Create(model);

            _context.AuditTrail.Add(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResponse<IAuditModel>> GetAll(AuditLogsFilterDto filter, int? skip, int? take)
        {
            var query = _context.AuditTrail.AsNoTracking();

            if (!string.IsNullOrEmpty(filter.UserName))
                query = query.Where(x => x.UserName.ToLower().Contains(filter.UserName.ToLower()));

            if (!string.IsNullOrEmpty(filter.CorrelationId))
                query = query.Where(x => x.CorrelationId == filter.CorrelationId);

            if (!string.IsNullOrEmpty(filter.ReferenceId))
                query = query.Where(x => x.DataReference.ToLower().Contains(filter.ReferenceId.ToLower()));

            if (filter.DataType.HasValue)
                query = query.Where(x => x.DataType == filter.DataType.Value);

            if (filter.ActionType.HasValue)
                query = query.Where(x => x.Type == filter.ActionType.Value);

            if (filter.StartDateTime.HasValue)
                query = query.Where(x => x.Timestamp >= filter.StartDateTime.Value);

            if (filter.EndDateTime.HasValue)
                query = query.Where(x => x.Timestamp <= filter.EndDateTime.Value);

            var total = await query.CountAsync();

            if (skip.HasValue && take.HasValue)
                query = query.Skip(skip.Value).Take(take.Value);

            var contents = await query.ToListAsync();

            var result = new PaginatedResponse<IAuditModel>(
                contents: contents,
                start: skip ?? 0,
                size: contents.Count,
                totalSize: total
            );

            return result;
        }
    }
}