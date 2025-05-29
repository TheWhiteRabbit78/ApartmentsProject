using ApartmentsProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public HomeController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var apartments = await _context.Apartments
                .Include(a => a.Images.OrderBy(img => img.DisplayOrder))
                .Where(a => a.IsAvailable)
                .OrderBy(a => a.Id)
                .ToListAsync();

            return View(apartments);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitContact(string fullName, string email,
            string phone, string message, int? apartmentId)
        {
            try
            {
                var apartmentInfo = "";
                if (apartmentId.HasValue)
                {
                    var apartment = await _context.Apartments.FindAsync(apartmentId);
                    apartmentInfo = $"Apartment: {apartment?.Title}\n\n";
                }

                var emailBody = $@"
                    <h3>New Contact Form Submission</h3>
                    <p><strong>Name:</strong> {fullName}</p>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Phone:</strong> {phone}</p>
                    {(apartmentId.HasValue ? $"<p><strong>Interested in:</strong> {apartmentInfo}</p>" : "")}
                    <p><strong>Message:</strong><br/>{message}</p>
                ";

                await _emailService.SendEmailAsync("info@yoursite.com",
                    "New Contact Form Submission", emailBody);

                return Json(new { success = true, message = "Poruka je uspješno poslana!" });
            }
            catch
            {
                return Json(new { success = false, message = "Greška pri slanju poruke." });
            }
        }
    }
}