
#nullable disable
using System;
using System.Collections.Generic;

namespace WorldBank.Entities.DataModel
{
    public partial class BankAccountTypes
    {
        public BankAccountTypes()
        {
            BankAccount = new HashSet<BankAccount>();
        }

        public Guid BankAccountTypeId { get; set; }
        public string BankAccountType { get; set; }
        public string Description { get; set; }

        public virtual ICollection<BankAccount> BankAccount { get; set; }
    }
}