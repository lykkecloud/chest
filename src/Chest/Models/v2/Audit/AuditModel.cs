using System;

namespace Chest.Models.v2.Audit
{
    public class AuditModel : IAuditModel
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string CorrelationId { get; set; }

        public string UserName { get; set; }

        public AuditEventType Type { get; set; }

        public AuditDataType DataType { get; set; }

        public string DataReference { get; set; }

        public string DataDiff { get; set; }
    }
}