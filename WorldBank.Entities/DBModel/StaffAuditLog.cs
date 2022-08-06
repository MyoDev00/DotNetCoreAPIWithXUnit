
#nullable disable

namespace WorldBank.Entities.DataModel
{
    public partial class StaffAuditLog
    {
        public Guid StaffAuditId { get; set; }
        public Guid StaffId { get; set; }
        public string AuditType { get; set; }
        public Guid RecordId { get; set; }
        public string Note { get; set; }

        public virtual AuditTypes AuditTypeNavigation { get; set; }
        public virtual Staff Staff { get; set; }
    }
}