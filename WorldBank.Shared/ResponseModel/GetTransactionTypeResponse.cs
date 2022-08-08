using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.ResponseModel
{
    public class GetTransactionTypeResponse
    {
        public Guid TransactionTypeId { get; set; }
        public int TransactionType { get; set; }
        public string Description { get; set; }
    }
}
