using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.ResponseModel
{
    public class CurrencyResponse
    {
        public Guid CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string Description { get; set; }
    }
}
