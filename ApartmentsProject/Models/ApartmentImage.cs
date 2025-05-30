using System.ComponentModel.DataAnnotations;

namespace ApartmentsProject.Models;

public class ApartmentImage
{
    public int Id { get; set; }
    [Required]
    public string FileName { get; set; }
    public DateTime UploadedAt { get; set; }
    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; }
}