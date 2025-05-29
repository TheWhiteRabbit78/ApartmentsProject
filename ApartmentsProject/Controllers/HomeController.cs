using ApartmentsProject.Data;
using ApartmentsProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ApartmentsProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, IEmailService emailService, ILogger<HomeController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var apartments = await _context.Apartments
                .Include(a => a.Images)
                .Where(a => a.IsAvailable)
                .OrderBy(a => a.Id)
                .ToListAsync();

            return View(apartments);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact(string fullName, string email, string phone, string message, int? apartmentId)
        {
            try
            {
                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
                {
                    return Json(new { success = false, message = "Molimo unesite sva obavezna polja." });
                }

                // Get apartment info if provided
                string apartmentInfo = "";
                if (apartmentId.HasValue)
                {
                    var apartment = await _context.Apartments.FindAsync(apartmentId.Value);
                    if (apartment != null)
                    {
                        apartmentInfo = $"Stan: {apartment.Title} - {apartment.Rooms} - €{apartment.Price:N0}";
                    }
                }

                // Prepare email content
                var emailBody = $@"
                    <h2>Nova poruka s web stranice</h2>
                    <p><strong>Ime:</strong> {fullName}</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Telefon:</strong> {phone ?? "Nije naveden"}</p>
                    {(!string.IsNullOrEmpty(apartmentInfo) ? $"<p><strong>Interes za:</strong> {apartmentInfo}</p>" : "")}
                    <p><strong>Poruka:</strong></p>
                    <p>{message.Replace("\n", "<br>")}</p>
                ";

                // Send email
                await _emailService.SendEmailAsync("info@residencezg.hr", "Nova poruka s web stranice", emailBody);

                return Json(new { success = true, message = "Vaša poruka je uspješno poslana. Kontaktirat æemo Vas uskoro!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contact form");
                return Json(new { success = false, message = "Došlo je do greške prilikom slanja poruke. Molimo pokušajte ponovo." });
            }
        }       
    }
}