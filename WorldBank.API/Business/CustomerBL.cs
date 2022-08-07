using WorldBank.Entities.DataModel;
using WorldBank.Shared.Helper;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;
using static WorldBank.Shared.Constant.Constant;

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
        private readonly string encryptionKey;
        public CustomerBL(IUnitOfWork unitOfWork,IConfiguration config)
        {
            this.unitOfWork = unitOfWork;
            encryptionKey = config["EncryptionKey"];
        }
        public CustomerBL(IUnitOfWork unitOfWork,string encryptionKey)
        {

            this.unitOfWork = unitOfWork;
            this.encryptionKey = encryptionKey;
        }
        public bool CheckEmailExist(string email)
        {
            var encryptedEmail = StringHelper.Encrypt(email, encryptionKey);
            var isAny = unitOfWork.GetRepository<Customer>().GetByCondition(c => c.Email == encryptedEmail).Any();
            return isAny;
        }

        public bool CheckIdentityNumberExist(string identityNo)
        {
            var encryptedNo = StringHelper.Encrypt(identityNo, encryptionKey);
            var isAny = unitOfWork.GetRepository<Customer>().GetByCondition(c => c.IdentityCardNo == encryptedNo).Any();
            return isAny;
        }

        public bool CheckMobileExist(string mobileCode, string mobile)
        {
            var encryptedMobile = StringHelper.Encrypt(mobile, encryptionKey);
            var isAny = unitOfWork.GetRepository<Customer>().GetByCondition(c => c.Mobile == encryptedMobile && c.MobileCode==mobileCode).Any();
            return isAny;
        }

        public async Task<BaseResponse<PostCustomerResponse>> PostCustomer(PostCustomerRequest request)
        {
            var response = new BaseResponse<PostCustomerResponse>();

            var isEmailUsed = CheckEmailExist(request.Email);
            var isIdentityNumberUsed = CheckIdentityNumberExist(request.IdentityCardNo);
            var isMobileUsed = CheckMobileExist(request.MobileCode,request.Mobile);

            if(!isEmailUsed && !isIdentityNumberUsed && !isMobileUsed)
            {
                return response;
            }
            else
            {
                List<Error> errors = new List<Error>();
                if (isEmailUsed)
                    errors.Add(new Error()
                    {
                        ErrorCode = ErrorCode.EmailAlreadyUsed,
                        ErrorMessage = ErrorMessage.EmailAlreadyUsed,
                        FieldName = "Email"
                    });
                
                if (isIdentityNumberUsed)
                    errors.Add(new Error()
                    {
                        ErrorCode = ErrorCode.IdentityAlreadyUsed,
                        ErrorMessage = ErrorMessage.IdentityAlreadyUsed,
                        FieldName = "IdentityCardNo"
                    });

                if(isMobileUsed)
                    errors.Add(new Error()
                    {
                        ErrorCode = ErrorCode.MobileAlreadyUsed,
                        ErrorMessage = ErrorMessage.MobileAlreadyUsed,
                        FieldName = "Mobile"
                    });

                response.Error = errors;
                return response;
            }
        }

        public Task<BaseResponse<PutCustomerResponse>> PutCustomer(PutCustomerRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
