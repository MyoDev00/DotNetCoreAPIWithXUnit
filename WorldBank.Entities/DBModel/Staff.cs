
#nullable disable
using System;
using System.Collections.Generic;

namespace WorldBank.Entities.DataModel
{
    public partial class Staff
    {
        public Staff()
        {
            StaffAuditLog = new HashSet<StaffAuditLog>();
        }

        public Guid StaffId { get; set; }
        public string FullName { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string SaltPassword { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public virtual ICollection<StaffAuditLog> StaffAuditLog { get; set; }
    }
}