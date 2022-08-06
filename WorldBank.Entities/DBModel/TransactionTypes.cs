
#nullable disable

namespace WorldBank.Entities.DataModel
{
    public partial class TransactionTypes
    {
        public TransactionTypes()
        {
            BankAccountLedger = new HashSet<BankAccountLedger>();
            Transaction = new HashSet<Transaction>();
        }

        public Guid TransactionTypeId { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }

        public virtual ICollection<BankAccountLedger> BankAccountLedger { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}