using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.Shared.RequestModel;
using WorldBank.Shared.ResponseModel;

namespace WorldBank.API.Test
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PostCustomerRequest, PostCustomerResponse>();
        }
    }
}
