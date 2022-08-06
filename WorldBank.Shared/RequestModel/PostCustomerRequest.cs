using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.RequestModel
{
    public class PostCustomerRequest
    {
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [MaxLength(200)]
        public string Email { get; set; }
        [Required]
        public string MobileCode { get; set; }
        [Required(ErrorMessage = "Mobile number is required")]
        [MaxLength(20)]
        public string Mobile { get; set; }
        [Required(ErrorMessage ="Identity Card number is required")]
        [MaxLength(20)]
        public string IdentityCardNo { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, ErrorMessage = "Must be between 5 and 20 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public int Status { get; set; }

        public string BankAccountType { get; set; }
        public Guid CurrencyId { get; set; }
    }
}
