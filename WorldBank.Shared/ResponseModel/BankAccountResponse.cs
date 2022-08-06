using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.ResponseModel
{
    public class BankAccountResponse
    {
        public Guid BankAccountId { get; set; }
        public string IBANNumber { get; set; }
        public string BankAccountType { get; set; }
        public string BankAccountTypeDescription { get; set; }
        public decimal ClosingBalance { get; set; }
        public Guid CurrencyId { get; set; }
        public string CurrencySymbol { get; set; }
        public int Status { get; set; }
    }
}
