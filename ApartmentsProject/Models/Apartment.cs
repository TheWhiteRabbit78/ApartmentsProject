using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Computed properties for the view
        [NotMapped]
        public string FormattedPrice => $"€{Price:N0}";

        [NotMapped]
        public string Status => IsAvailable ? "Dostupan" : "Prodan";

        [NotMapped]
        public string? MainImageUrl => Images?.FirstOrDefault(i => i.ImageType == "main")?.FileName != null
            ? $"/uploads/{Images.First(i => i.ImageType == "main").FileName}"
            : null;

        [NotMapped]
        public bool HasImages => Images?.Any() == true;

        [NotMapped]
        public List<ImageViewModel> AllImages => Images?.OrderBy(i => i.DisplayOrder)
            .Select(i => new ImageViewModel
            {
                Url = $"/uploads/{i.FileName}",
                Caption = i.Caption ?? "",
                Type = i.ImageType
            }).ToList() ?? new List<ImageViewModel>();
    }

    // Helper class for image display
    public class ImageViewModel
    {
        public string Url { get; set; } = "";
        public string Caption { get; set; } = "";
        public string Type { get; set; } = "";
    }
}