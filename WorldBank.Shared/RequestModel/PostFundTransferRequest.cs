
namespace WorldBank.Shared.RequestModel
{
    public class PostFundTransferRequest
    {
        public Guid CustomerId { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid ReceiverCustomerId { get; set; }
        public Guid ReceiverBankAccountId { get; set; }
        public string Note { get; set; }
        public decimal Amount { get; set; }
    }
}
