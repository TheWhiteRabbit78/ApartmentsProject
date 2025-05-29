// Models/ApartmentImage.cs
using System.ComponentModel.DataAnnotations;

namespace ApartmentsProject.Models;

public class ApartmentImage
{
    public int Id { get; set; }
    [Required] public string FileName { get; set; } = "";
    [Required] public string ImageType { get; set; } = "gallery"; // hero/gallery
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; } = null!;
}