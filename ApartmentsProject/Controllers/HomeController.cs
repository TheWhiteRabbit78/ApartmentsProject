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
            .Where(a => a.IsAvailable)
            .OrderBy(a => a.Id)
            .ToListAsync();

        return View(apartments);
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

            string apartmentInfo = "";
            if (apartmentId.HasValue)
            {
                var apartment = await _context.Apartments.FindAsync(apartmentId.Value);
                if (apartment != null)
                {
                    apartmentInfo = $"<br><strong>Stan:</strong> {apartment.Title} - {apartment.Price:N0}€";
                }
            }

            string emailBody = $@"
                <h3>Nova poruka s web stranice</h3>
                <p><strong>Ime:</strong> {fullName}</p>
                <p><strong>Email:</strong> {email}</p>
                <p><strong>Telefon:</strong> {phone ?? "Nije navedeno"}</p>
                {apartmentInfo}
                <p><strong>Poruka:</strong></p>
                <p>{message.Replace("\n", "<br>")}</p>
            ";

            await _emailService.SendEmailAsync(_emailSettings.SenderEmail, $"Nova poruka od {fullName}", emailBody);


            return Json(new { success = true, message = "Vaša poruka je uspješno poslana!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Došlo je do greške pri slanju poruke." });
        }
    }
}