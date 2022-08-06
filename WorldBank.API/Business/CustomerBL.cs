using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;

namespace WorldBank.API.Business
{
    public interface ICustomerBL
    {
        public Task<BaseResponse<PostCustomerResponse>> PostCustomer(PostCustomerRequest request);
        public bool CheckEmailExist(string email);
        public bool CheckMobileExist( string mobileCode,string mobile);
        public bool CheckIdentityNumberExist(string identityNo);
        public Task<BaseResponse<PutCustomerResponse>> PutCustomer(PutCustomerRequest request);
    }
    public class CustomerBL : ICustomerBL
    {
        private readonly IUnitOfWork unitOfWork;

        public CustomerBL(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public bool CheckEmailExist(string email)
        {
            throw new NotImplementedException();
        }

        public bool CheckIdentityNumberExist(string identityNo)
        {
            throw new NotImplementedException();
        }

        public bool CheckMobileExist(string mobileCode, string mobile)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<PostCustomerResponse>> PostCustomer(PostCustomerRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<PutCustomerResponse>> PutCustomer(PutCustomerRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
