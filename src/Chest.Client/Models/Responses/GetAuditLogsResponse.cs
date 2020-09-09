using System.Collections.Generic;

namespace Chest.Client.Models.Responses
{
    /// <summary>
    /// Response model for audit logs
    /// </summary>
    public class GetAuditLogsResponse
    {
        /// <summary>
        /// collection of audit logs
        /// </summary>
        public IReadOnlyList<AuditContract> AuditLogs { get; set; }
    }
}