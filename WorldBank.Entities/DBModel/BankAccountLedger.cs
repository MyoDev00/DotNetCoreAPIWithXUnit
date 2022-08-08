
#nullable disable
using System;
using System.Collections.Generic;

namespace WorldBank.Entities.DataModel
{
    public partial class BankAccountLedger
    {
        public Guid LedgerId { get; set; }
        public Guid TransactionId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public virtual TransactionTypes TransactionType { get; set; }
    }
}