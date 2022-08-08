using System.ComponentModel.DataAnnotations;

namespace WorldBank.Shared.RequestModel
{
    public class PostLoginRequest
    {
        [Required]
        public string? LoginId { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
