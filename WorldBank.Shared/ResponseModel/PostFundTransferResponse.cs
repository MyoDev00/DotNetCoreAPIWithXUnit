
namespace WorldBank.Shared.ResponseModel
{
    public class PostFundTransferResponse
    {
        public Guid TransactionId { get;set; }
        public string TransactionNo { get; set; }
        public decimal Amount { get; set; }
        public decimal Charges { get; set; }

    }
}
