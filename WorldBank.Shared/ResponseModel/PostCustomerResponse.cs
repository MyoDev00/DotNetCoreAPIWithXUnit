using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.ResponseModel
{
    public class PostCustomerResponse
    {
        public Guid CustomerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobileCode { get; set; }
        public string Mobile { get; set; }
        public string IdentityCardNo { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<BankAccountResponse> BankAccounts { get; set; }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null)
            {
                return false;
            }
            else
            {
                var data = (PostCustomerResponse)obj;

                var isEqual = (FullName == data.FullName) && (Email == data.Email)
                            && (MobileCode == data.MobileCode) && (Mobile == data.Mobile)
                            && (IdentityCardNo == data.IdentityCardNo) && (Status == data.Status);
                //isEqual = BankAccounts.Equals(data.BankAccounts);
                return isEqual;
            }
        }
    }
   
}
