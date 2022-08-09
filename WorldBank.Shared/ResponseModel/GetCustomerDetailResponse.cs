
namespace WorldBank.Shared.ResponseModel
{
    public class GetCustomerDetailResponse
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileCode { get; set; }
        public string Mobile { get; set; }
        public string IdentityCardNo { get; set; }
        public int Status { get; set; }
        public List<BankAccountResponse> BankAccounts { get; set; }
    }
}
