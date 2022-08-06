
#nullable disable

namespace WorldBank.Entities.DataModel
{
    public partial class BankAccountLedger
    {
        public Guid LedgerId { get; set; }
        public Guid TransactionId { get; set; }
        public string TransactionType { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual TransactionTypes TransactionTypeNavigation { get; set; }
    }
}