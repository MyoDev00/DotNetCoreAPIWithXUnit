
#nullable disable

namespace WorldBank.Entities.DataModel
{
    public partial class Transaction
    {
        public Transaction()
        {
            BankAccountLedger = new HashSet<BankAccountLedger>();
        }

        public Guid TransactionId { get; set; }
        public string TransactionNo { get; set; }
        public string TransactionType { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid ReceiverCustomerId { get; set; }
        public Guid ReceiverBankAccountId { get; set; }
        public decimal Charges { get; set; }
        public decimal ChargesPercentage { get; set; }
        public decimal Amount { get; set; }
        public decimal NetAmount { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual TransactionTypes TransactionTypeNavigation { get; set; }
        public virtual ICollection<BankAccountLedger> BankAccountLedger { get; set; }
    }
}