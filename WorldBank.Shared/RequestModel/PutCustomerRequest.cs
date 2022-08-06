using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.RequestModel
{
    public class PutCustomerRequest
    {
        [Required]
        public Guid CustomerId { get; set; }
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
        [Required(ErrorMessage = "Identity Card number is required")]
        [MaxLength(20)]
        public string IdentityCardNo { get; set; }
       
        [Required]
        public int Status { get; set; }
    }
}
