using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.ResponseModel
{
    public class AuditTypeResponse
    {
        public Guid AuditTypeId { get; set; }
        public string AuditType { get; set; }
        public string Description { get; set; }
    }
}
