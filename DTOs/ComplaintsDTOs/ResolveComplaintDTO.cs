using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Application.DTOs.ComplaintsDTOs
{
    public class ResolveComplaintDTO
    {
        [Required]
        public bool IsResolved { get; set; }
    }
}
