
#nullable disable

namespace WorldBank.Entities.DataModel
{
    public partial class Currency
    {
        public Guid CurrencyId { get; set; }
        public string CurrencySymbol { get; set; }
        public string Description { get; set; }

    }
}