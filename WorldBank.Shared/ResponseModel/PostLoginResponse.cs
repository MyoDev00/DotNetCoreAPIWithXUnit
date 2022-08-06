
namespace WorldBank.Shared.ResponseModel
{
    public class PostLoginResponse
    {
        public string AuthorizeToken { get; set; }
        public DateTime AuthorizeTokenExpireDate { get; set; }
        public string StaffName { get; set; }
    }
}
