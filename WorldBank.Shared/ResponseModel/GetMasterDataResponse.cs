
namespace WorldBank.Shared.ResponseModel
{
    public class GetMasterDataResponse
    {
        public List<TransactionTypeResponse> TransactionTypes { get; set; }
        public List<BankAccountTypeResponse> BankAccountTypes { get; set; }
        public List<CurrencyResponse> Currencies { get; set; }
        public List<AuditTypeResponse> AuditTypes { get; set; }
    }
}
