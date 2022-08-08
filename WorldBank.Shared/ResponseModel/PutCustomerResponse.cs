using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.ResponseModel
{
    public class PutCustomerResponse
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileCode { get; set; }
        public string Mobile { get; set; }
        public string IdentityCardNo { get; set; }
        public int Status { get; set; }

    }

   
}
