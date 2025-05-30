using System.ComponentModel.DataAnnotations;

namespace ApartmentsProject.Models;

public class Apartment
{
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
    public string Description { get; set; }

    [Required(ErrorMessage = "Cijena je obavezna")]
    public int Price { get; set; }

    public bool IsAvailable { get; set; }

    public List<ApartmentImage> Images { get; set; }
}