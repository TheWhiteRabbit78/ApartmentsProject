using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApartmentsProject.Models;

public class Apartment
{
    public Apartment()
    {
        Title = string.Empty;
        Rooms = string.Empty;
        Floor = string.Empty;
        SurfaceArea = string.Empty;
        IsAvailable = true;
        CreatedAt = DateTime.Now.ToLocalTime();
    }

    public int Id { get; set; }

    [Required(ErrorMessage = "Naslov je obavezan")]
    [StringLength(50, ErrorMessage = "Naslov ne smije biti duži od 50 znakova")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Broj soba je obavezan")]
    [StringLength(20, ErrorMessage = "Opis soba ne smije biti duži od 20 znakova")]
    public string Rooms { get; set; }

    [Required(ErrorMessage = "Površina je obavezna")]
    [StringLength(10, ErrorMessage = "Opis površine ne smije biti duži od 10 znakova")]
    public string SurfaceArea { get; set; }

    [Required(ErrorMessage = "Kat je obavezan")]
    [StringLength(25, ErrorMessage = "Opis kata ne smije biti duži od 10 znakova")]
    public string Floor { get; set; }

    [StringLength(500, ErrorMessage = "Opis ne smije biti duži od 500 znakova")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Cijena je obavezna")]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<ApartmentImage> Images { get; set; } = [];

    [NotMapped]
    public string FormattedPrice { get { return $"{Price:N0} €"; } }

    [NotMapped]
    public string Status { get { return IsAvailable ? "Dostupan" : "Prodan"; } }

    [NotMapped]
    public string? HeroImageUrl
    {
        get
        {
            var heroImage = Images?.FirstOrDefault(i => i.ImageType == "hero");
            if (heroImage?.FileName != null)
            {
                return $"/uploads/{heroImage.FileName}";
            }
            var firstImage = Images?.FirstOrDefault();
            if (firstImage?.FileName != null)
            {
                return $"/uploads/{firstImage.FileName}";
            }
            return null;
        }
    }

    [NotMapped]
    public bool HasImages { get { return Images?.Count > 0; } }

    [NotMapped]
    public List<ApartmentImage> SortedImages { get { return Images.OrderBy(i => i.ImageType == "hero" ? 0 : 1).ThenBy(i => i.DisplayOrder).ToList(); } }
}