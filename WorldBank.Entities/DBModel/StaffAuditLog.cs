
#nullable disable
using System;
using System.Collections.Generic;

namespace WorldBank.Entities.DataModel
{
    public partial class StaffAuditLog
    {
        public Guid StaffAuditId { get; set; }
        public Guid StaffId { get; set; }
        public Guid AuditTypeId { get; set; }
        public Guid RecordId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual AuditTypes AuditType { get; set; }
        public virtual Staff Staff { get; set; }
    }
}