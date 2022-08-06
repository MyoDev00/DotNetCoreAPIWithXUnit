using System.ComponentModel.DataAnnotations;

namespace WorldBank.Shared.RequestModel.CommonRequest
{
    public class GenericPagingRequest
    {
        /// <summary>
        /// Page Index
        /// Range: inclusive between 1 and 2147483647
        /// </summary>
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Page index must be larger than 0")]
        public int PageIndex { get; set; }

        /// <summary>
        /// Page Size
        /// Range: inclusive between 1 and 2147483647
        /// </summary>
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessage = "Page size must be larger than 0")]
        public int PageSize { get; set; }

        public string? OrderBy { get; set; }

        public string? Order { get; set; }
    }
}
