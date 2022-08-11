using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.ResponseModel
{
    public class TransactionTypeResponse
    {
        public Guid TransactionTypeId { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
    }
}
