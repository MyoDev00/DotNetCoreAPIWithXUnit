using AutoMapper;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.ResponseModel;
using WorldBank.Shared.ResponseModel.CommonResponse;
using WorldBank.UnitOfWork;

namespace WorldBank.API.Business
{
    public interface IMasterDataBL
    {
        public Task<BaseResponse<GetMasterDataResponse>> GetMasterData();
    }
    public class MasterDataBL : IMasterDataBL
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public MasterDataBL(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<BaseResponse<GetMasterDataResponse>> GetMasterData()
        {
            var transactionTypes = unitOfWork.GetRepository<TransactionTypes>().GetAll().ToList();
            var bankAccountTypes = unitOfWork.GetRepository<BankAccountTypes>().GetAll().ToList();
            var currencies = unitOfWork.GetRepository<Currency>().GetAll().ToList();
            var auditTypes = unitOfWork.GetRepository<AuditTypes>().GetAll().ToList();

            var response = new BaseResponse<GetMasterDataResponse>()
            {
                Responsedata= new GetMasterDataResponse()
                {
                    TransactionTypes = mapper.Map<List<TransactionTypeResponse>>(transactionTypes),
                    BankAccountTypes = mapper.Map<List<BankAccountTypeResponse>>(bankAccountTypes),
                    Currencies = mapper.Map<List<CurrencyResponse>>(currencies),
                    AuditTypes = mapper.Map<List<AuditTypeResponse>>(auditTypes),
                }
            };

            return response;
        }
    }
}
