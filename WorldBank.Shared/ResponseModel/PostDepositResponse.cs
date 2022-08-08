
namespace WorldBank.Shared.ResponseModel
{
    public class PostDepositResponse
    {
        public Guid CustomerId { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid TransactionId { get; set; }
        public string TransactionNo { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal Charges { get; set; }
        public DateTime UpdatedOn { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                var data = (PostDepositResponse)obj;

                var isEqual = (CustomerId == data.CustomerId) && (BankAccountId == data.BankAccountId)
                            && (ClosingBalance == data.ClosingBalance) && (TotalCredit == data.TotalCredit)
                            && (Charges == data.Charges) ;
                return isEqual;
            }
        }
    }
}
