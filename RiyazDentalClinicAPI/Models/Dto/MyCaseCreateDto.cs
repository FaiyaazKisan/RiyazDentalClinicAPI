using System.ComponentModel.DataAnnotations;

namespace RiyazDentalClinicAPI.Models.Dto
{
    public class MyCaseCreateDto
    {
        public string Heading { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
