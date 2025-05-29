using ApartmentsProject.Data;
using ApartmentsProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    ApplicationDbContext context,
    IEmailService emailService) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly ApplicationDbContext _context = context;
    private readonly IEmailService _emailService = emailService;

    public async Task<IActionResult> Index()
    {
        var apartments = await _context.Apartments
            .Include(a => a.Images)
            .Where(a => a.IsAvailable)
            .OrderBy(a => a.Title)
            .ToListAsync();

        return View(apartments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitContact(string fullName, string email,
        string phone, string message, int? apartmentId)
    {
        try
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, message = "Molimo unesite sva obavezna polja." });
            }

            var apartment = apartmentId.HasValue ?
                await _context.Apartments.FindAsync(apartmentId.Value) : null;

            var subject = apartment != null ?
                $"Upit za stan: {apartment.Title}" :
                "Novi kontakt upit - FABRIKON projekt";

            var htmlBody = $@"
                <h3>Novi kontakt upit</h3>
                <p><strong>Ime:</strong> {fullName}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Telefon:</strong> {phone ?? "Nije uneseno"}</p>
                {(apartment != null ? $"<p><strong>Zanima stan:</strong> {apartment.Title}</p>" : "")}
                <p><strong>Poruka:</strong></p>
                <p>{message.Replace("\n", "<br>")}</p>
                <hr>
                <p><small>Poslano preko web forme - FABRIKON projekt</small></p>
            ";

            await _emailService.SendEmailAsync("info@fabrikon.hr", subject, htmlBody);

            return Json(new
            {
                success = true,
                message = "Vaš upit je uspješno poslan. Kontaktirat æemo vas uskoro!"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending contact email");
            return Json(new
            {
                success = false,
                message = "Došlo je do greške prilikom slanja upita. Molimo pokušajte ponovo."
            });
        }
    }
}