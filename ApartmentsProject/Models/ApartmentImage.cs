using System.ComponentModel.DataAnnotations;

namespace ApartmentsProject.Models
{
    public class ApartmentImage
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string ImageType { get; set; } // "main", "floorplan", "interior", etc.

        public string? Caption { get; set; }

        public int DisplayOrder { get; set; } // For ordering images

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Foreign key to apartment
        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
    }
}