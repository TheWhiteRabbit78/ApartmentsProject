// Models/ApartmentImage.cs
using System.ComponentModel.DataAnnotations;

namespace ApartmentsProject.Models;

public class ApartmentImage
{
    public ApartmentImage()
    {
        FileName = string.Empty;
        ImageType = "gallery";
        Apartment = null!;
    }

    public int Id { get; set; }
    [Required] public string FileName { get; set; } 
    [Required] public string ImageType { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.Now.ToLocalTime();
    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; }
}