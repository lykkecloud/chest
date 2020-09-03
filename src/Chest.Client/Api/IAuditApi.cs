using System.Threading.Tasks;
using Chest.Client.Models;
using Chest.Client.Models.Requests;
using Chest.Client.Models.Responses.Chest.Client.Models.Responses;
using Refit;

namespace Chest.Client.Api
{
    public interface IAuditApi
    {
        /// <summary>
        /// Get audit logs
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [Get("/api/v2/audit")]
        Task<PaginatedResponseContract<AuditContract>> GetAuditTrailAsync([Query] GetAuditLogsRequest request,
            int? skip = null, int? take = null);
    }
}