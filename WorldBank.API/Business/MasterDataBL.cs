using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;

namespace WorldBank.API.Business
{
    public interface IMasterDataBL
    {
        public Task<BaseResponse<GetTransactionTypeResponse>> GetTransactionType();
    }
    public class MasterDataBL : IMasterDataBL
    {
        public Task<BaseResponse<GetTransactionTypeResponse>> GetTransactionType()
        {
            throw new NotImplementedException();
        }
    }
}
