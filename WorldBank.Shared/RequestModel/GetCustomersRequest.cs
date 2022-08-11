using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.Shared.RequestModel.CommonRequest;

namespace WorldBank.Shared.RequestModel
{
    public class GetCustomersRequest:GenericPagingRequest
    {
        public string? SearchString { get; set; }

    }
}
