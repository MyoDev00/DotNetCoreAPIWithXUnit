using WorldBank.Shared.ResponseModel.CommonResponse;

namespace WorldBank.Shared.ResponseModel
{
    public class GetCustomersResponse:GenericPagingResponse
    {
        public List<CustomerModel> Customers { get; set; }
        public class CustomerModel
        {
            public Guid CustomerId { get; set; }
            public string FullName { get; set; }
            public DateTime CreatedOn { get; set; }
            public DateTime UpdatedOn { get; set; }
        }
    }
}
