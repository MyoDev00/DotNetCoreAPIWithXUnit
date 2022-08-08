
namespace WorldBank.Entities.DataModel
{
    public partial class TransactionCharges
    {
        public Guid TransactionChargesId { get; set; }
        public string ChargesType { get; set; }
        public decimal Percentage { get; set; }
        public string Description { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}