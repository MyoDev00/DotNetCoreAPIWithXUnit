
#nullable disable

namespace WorldBank.Entities.DataModel
{
    public partial class AuditTypes
    {
        public AuditTypes()
        {
            StaffAuditLog = new HashSet<StaffAuditLog>();
        }

        public Guid AuditTypeId { get; set; }
        public string AuditType { get; set; }
        public string Description { get; set; }

        public virtual ICollection<StaffAuditLog> StaffAuditLog { get; set; }
    }
}