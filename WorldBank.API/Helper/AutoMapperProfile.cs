using AutoMapper;
using WorldBank.Entities.DataModel;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;

namespace WorldBank.API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PostCustomerRequest, PostCustomerResponse>().ReverseMap();
            CreateMap<Customer, PostCustomerResponse>().ReverseMap();
            CreateMap<BankAccount, BankAccountResponse>();
            CreateMap<Customer, PutCustomerResponse>();
            CreateMap<PutCustomerRequest, PutCustomerResponse>();
            CreateMap<TransactionTypes, TransactionTypeResponse>().ReverseMap();
            CreateMap<BankAccountTypes, BankAccountTypeResponse>().ReverseMap();
            CreateMap<Currency,CurrencyResponse>().ReverseMap();
            CreateMap<AuditTypes,AuditTypeResponse>().ReverseMap();
        }
    }
}
