using System.ComponentModel.DataAnnotations;

namespace RiyazDentalClinicAPI.Models.Dto
{
    public class PatientsTestimonialUpdateDto
    {
        [Key]
        public int Id { get; set; }
        public string Heading { get; set; }
        public IFormFile File { get; set; }
    }
}
