using System.ComponentModel.DataAnnotations;

namespace ApartmentsProject.Models;

public class ApartmentImage
{
    public int Id { get; set; }
    [Required] 
    public string FileName { get; set; } 
    [Required] 
    public string ImageType { get; set; }
    public string Caption { get; set; }
    public DateTime UploadedAt { get; set; }
    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; }
}