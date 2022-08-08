
#nullable disable
using System;
using System.Collections.Generic;

namespace WorldBank.Entities.DataModel
{
    public partial class Customer
    {
        public Customer()
        {
            BankAccount = new List<BankAccount>();
            Transaction = new List<Transaction>();
        }

        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileCode { get; set; }
        public string Mobile { get; set; }
        public string IdentityCardNo { get; set; }
        public string Password { get; set; }
        public string SaltPassword { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public virtual IList<BankAccount> BankAccount { get; set; }
        public virtual IList<Transaction> Transaction { get; set; }
    }
}