using ApartmentsProject.Data;
using ApartmentsProject.Models;
using ApartmentsProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApartmentsProject.Controllers;

public class HomeController(ApplicationDbContext context, IEmailService emailService, IOptions<EmailSettings> emailSettings) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly IEmailService _emailService = emailService;    
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task<IActionResult> Index()
    {
        var apartments = await _context.Apartments
            .Include(a => a.Images)
            .OrderBy(a => a.Id)
            .ToListAsync();

        return View(apartments);
    }

    public IActionResult ApartmentDetails(int id) {

        Apartment apartment = _context.Apartments.Include(a => a.Images).Where(a => a.Id == id).FirstOrDefault();
        if (apartment == null)
        {
            return NotFound();
        }
        return View(apartment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]    
    public async Task<IActionResult> SubmitContact([FromBody] ContactFormModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Message))
            {
                return Json(new { success = false, message = "Molimo unesite sva obavezna polja." });
            }

            string apartmentInfo = "";
            if (model.ApartmentId.HasValue)
            {
                var apartment = await _context.Apartments.FindAsync(model.ApartmentId.Value);
                if (apartment != null)
                {
                    apartmentInfo = $"<br><strong>Stan:</strong> {apartment.Title} - {apartment.Price:N0}€";
                }
            }

            string emailBody = $@"
                <h3>Nova poruka s web stranice</h3>
                <p><strong>Ime:</strong> {model.FullName}</p>
                <p><strong>Email:</strong> {model.Email}</p>
                <p><strong>Telefon:</strong> {model.Phone ?? "Nije navedeno"}</p>
                {apartmentInfo}
                <p><strong>Poruka:</strong></p>
                <p>{model.Message.Replace("\n", "<br>")}</p>
            ";

            await _emailService.SendEmailAsync(_emailSettings.SenderEmail, $"Nova poruka od {model.FullName}", emailBody);


            return Json(new { success = true, message = "Vaša poruka je uspješno poslana!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Došlo je do greške pri slanju poruke." });
        }
    }

    public class ContactFormModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public int? ApartmentId { get; set; }
    }
}