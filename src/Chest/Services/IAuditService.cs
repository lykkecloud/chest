using System.Threading.Tasks;
using Chest.Models.v2;
using Chest.Models.v2.Audit;

namespace Chest.Services
{
    public interface IAuditService
    {
        Task<PaginatedResponse<IAuditModel>> GetAll(AuditLogsFilterDto filter, int? skip, int? take);

        Task<bool> TryAudit(string correlationId, string userName, string referenceId,
            AuditDataType type, string newStateJson = null, string oldStateJson = null);
    }
}