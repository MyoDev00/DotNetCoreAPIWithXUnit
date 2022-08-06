
#nullable disable

namespace WorldBank.Entities.DataModel
{
    public partial class Currency
    {
        public Currency()
        {
            BankAccount = new HashSet<BankAccount>();
        }

        public Guid CurrencyId { get; set; }
        public string Currency1 { get; set; }
        public string CurrencySymbol { get; set; }
        public string Description { get; set; }

        public virtual ICollection<BankAccount> BankAccount { get; set; }
    }
}