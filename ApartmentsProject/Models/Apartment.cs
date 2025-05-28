using System.ComponentModel.DataAnnotations;

namespace ApartmentsProject.Models
{
    public class Apartment
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Rooms { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property - this apartment can have many images
        public List<ApartmentImage> Images { get; set; } = new List<ApartmentImage>();
    }
}