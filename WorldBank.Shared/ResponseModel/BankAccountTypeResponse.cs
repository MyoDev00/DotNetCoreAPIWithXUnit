using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.ResponseModel
{
    public class BankAccountTypeResponse
    {
        public Guid BankAccountTypeId { get; set; }
        public string BankAccountType { get; set; }
        public string Description { get; set; }
    }
}
