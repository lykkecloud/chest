using System.Threading.Tasks;
using Chest.Models.v2;
using Chest.Models.v2.Audit;

namespace Chest.Data.Repositories
{
    public interface IAuditRepository
    {
        Task InsertAsync(IAuditModel model);

        Task<PaginatedResponse<IAuditModel>> GetAll(AuditLogsFilterDto filter, int? skip, int? take);
    }
}