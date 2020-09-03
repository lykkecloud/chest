using System;
using Chest.Client.Models;

namespace Chest.Models.v2.Audit
{
    public interface IAuditModel
    {
        int Id { get; set; }
        DateTime Timestamp { get; set; }
        string CorrelationId { get; set; }
        string UserName { get; set; }
        AuditEventType Type { get; set; }
        AuditDataType DataType { get; set; }
        string DataReference { get; set; }
        string DataDiff { get; set; }
    }
}