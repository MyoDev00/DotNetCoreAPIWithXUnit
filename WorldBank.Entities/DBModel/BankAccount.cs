
#nullable disable
using System;
using System.Collections.Generic;

namespace WorldBank.Entities.DataModel
{
    public partial class BankAccount
    {
        public BankAccount()
        {
            Transaction = new HashSet<Transaction>();
        }

        public Guid BankAccountId { get; set; }
        public Guid CustomerId { get; set; }
        public string IbanNumber { get; set; }
        public Guid BankAccountTypeId { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public Guid CurrencyId { get; set; }
        public int Status { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual BankAccountTypes BankAccountType { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}