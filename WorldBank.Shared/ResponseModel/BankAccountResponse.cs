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
        public Guid BankAccountTypeId { get; set; }
        public string BankAccountType { get; set; }
        public string BankAccountTypeDescription { get; set; }
        public decimal ClosingBalance { get; set; }
        public Guid CurrencyId { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyDescryption { get; set; }
        public int Status { get; set; }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null)
            {
                return false;
            }
            else
            {
                var data = (BankAccountResponse)obj;

                var isEqual = (BankAccountTypeId==data.BankAccountTypeId) && (BankAccountType == data.BankAccountType) 
                            && (BankAccountTypeDescription == data.BankAccountTypeDescription)
                            && (CurrencyId == data.CurrencyId) && (CurrencySymbol == data.CurrencySymbol)
                            && (Status == data.Status);

                return isEqual;
            }
        }
    }
}
