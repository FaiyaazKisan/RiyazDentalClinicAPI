﻿using System.ComponentModel.DataAnnotations;

namespace RiyazDentalClinicAPI.Models
{
    public class DoctorsTestimonial
    {
        [Key]
        public int Id { get; set; }
        public string Heading { get; set; }
        [Required]
        public string File { get; set; }
    }
}
