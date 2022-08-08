using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.RequestModel
{
    public class PostDepositRequest
    {
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public Guid BankAccountId { get; set; }

        [Required]
        //[RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid price")]
        public decimal Amount { get; set; }
        public string Note { get; set; }
    }
}
