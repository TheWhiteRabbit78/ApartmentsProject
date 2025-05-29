using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApartmentsProject.Models;

public class Apartment
{
    public int Id { get; set; }

    [Required] public string Title { get; set; } = "";
    [Required] public string Rooms { get; set; } = "";
    public string? Description { get; set; }
    [Required] public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<ApartmentImage> Images { get; set; } = [];

    [NotMapped] public string FormattedPrice => $"€{Price:N0}";
    [NotMapped] public string Status => IsAvailable ? "Dostupan" : "Prodan";
    [NotMapped]
    public string? HeroImageUrl =>
        Images?.FirstOrDefault(i => i.ImageType == "hero")?.FileName != null
            ? $"/uploads/{Images.First(i => i.ImageType == "hero").FileName}"
            : Images?.FirstOrDefault()?.FileName != null
                ? $"/uploads/{Images.First().FileName}" : null;
    [NotMapped] public bool HasImages => Images?.Count > 0;
    [NotMapped]
    public List<ApartmentImage> SortedImages =>
        Images?.OrderBy(i => i.ImageType == "hero" ? 0 : 1)
              .ThenBy(i => i.DisplayOrder).ToList() ?? [];
}