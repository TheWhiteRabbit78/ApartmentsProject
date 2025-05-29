using ApartmentsProject.Data;
using ApartmentsProject.Models;
using ApartmentsProject.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Controllers;

[Authorize]
public class AdminController(
    ApplicationDbContext context,
    IWebHostEnvironment environment,
    UserManager<IdentityUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<IActionResult> Index() =>
        View(await _context.Apartments.Include(a => a.Images)
            .OrderBy(a => a.Id).ToListAsync());

    // Modal Actions for Apartments
    public async Task<IActionResult> GetApartmentModal(int id = 0)
    {
        var apartment = id == 0 ? new Apartment() : await _context.Apartments.FindAsync(id);
        if (id != 0 && apartment == null) return NotFound();

        ViewBag.IsEdit = id != 0;
        return PartialView("_ApartmentModal", apartment);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveApartment(Apartment apartment)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = apartment.Id != 0;
                var html = await this.RenderViewAsync("_ApartmentModal", apartment, true);
                return Json(new { isValid = false, html = html });
            }

            if (apartment.Id == 0)
            {
                apartment.CreatedAt = DateTime.Now;
                _context.Apartments.Add(apartment);
            }
            else
            {
                var original = await _context.Apartments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == apartment.Id);
                if (original != null)
                {
                    apartment.CreatedAt = original.CreatedAt;
                }
                _context.Update(apartment);
            }

            await _context.SaveChangesAsync();
            return Json(new { isValid = true, message = "Stan je uspješno spremljen!" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Greška pri spremanju: " + ex.Message);
            ViewBag.IsEdit = apartment.Id != 0;
            var html = await this.RenderViewAsync("_ApartmentModal", apartment, true);
            return Json(new { isValid = false, html = html });
        }
    }

    public async Task<IActionResult> GetDeleteApartmentModal(int id)
    {
        var apartment = await _context.Apartments.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);
        return apartment == null ? NotFound() : PartialView("_DeleteApartmentModal", apartment);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteApartment(int id)
    {
        try
        {
            var apartment = await _context.Apartments.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);

            if (apartment != null)
            {
                foreach (var image in apartment.Images)
                {
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", image.FileName);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();
                return Json(new { isValid = true, message = "Stan je uspješno obrisan!" });
            }

            return Json(new { isValid = false, message = "Stan nije pronađen!" });
        }
        catch (Exception ex)
        {
            return Json(new { isValid = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

    public async Task<IActionResult> GetApartmentDetailsModal(int id)
    {
        var apartment = await _context.Apartments
            .Include(a => a.Images.OrderBy(img => img.DisplayOrder))
            .FirstOrDefaultAsync(a => a.Id == id);
        return apartment == null ? NotFound() : PartialView("_ApartmentDetailsModal", apartment);
    }

    public async Task<IActionResult> GetAddImageModal(int id)
    {
        var apartment = await _context.Apartments.FindAsync(id);
        if (apartment == null) return NotFound();

        ViewBag.ApartmentTitle = apartment.Title;
        ViewBag.ApartmentId = id;
        return PartialView("_AddImageModal");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(int apartmentId, IFormFile imageFile, string imageType, string? caption, int displayOrder = 0)
    {
        try
        {
            if (imageFile?.Length > 0)
            {
                var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExt = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExts.Contains(fileExt))
                {
                    ModelState.AddModelError("", "Dozvoljena su samo .jpg, .jpeg, .png, .gif proširenja.");
                }
                else
                {
                    var fileName = Guid.NewGuid() + fileExt;
                    var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsPath);

                    var filePath = Path.Combine(uploadsPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await imageFile.CopyToAsync(stream);

                    var apartmentImage = new ApartmentImage
                    {
                        ApartmentId = apartmentId,
                        FileName = fileName,
                        ImageType = imageType,
                        Caption = caption,
                        DisplayOrder = displayOrder,
                        UploadedAt = DateTime.Now
                    };

                    _context.ApartmentImages.Add(apartmentImage);
                    await _context.SaveChangesAsync();
                    return Json(new { isValid = true, message = "Slika je uspješno dodana!" });
                }
            }
            else
            {
                ModelState.AddModelError("", "Molimo odaberite datoteku slike.");
            }

            ViewBag.ApartmentId = apartmentId;
            var html = await this.RenderViewAsync("_AddImageModal", (object)null, true);
            return Json(new { isValid = false, html = html });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Greška pri dodavanju slike: " + ex.Message);
            ViewBag.ApartmentId = apartmentId;
            var html = await this.RenderViewAsync("_AddImageModal", (object)null, true);
            return Json(new { isValid = false, html = html });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteImage(int id)
    {
        try
        {
            var image = await _context.ApartmentImages.FindAsync(id);
            if (image != null)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", image.FileName);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _context.ApartmentImages.Remove(image);
                await _context.SaveChangesAsync();
                return Json(new { isValid = true, message = "Slika je uspješno obrisana!" });
            }
            return Json(new { isValid = false, message = "Slika nije pronađena!" });
        }
        catch (Exception ex)
        {
            return Json(new { isValid = false, message = "Greška pri brisanju slike: " + ex.Message });
        }
    }

    // User Management Modal Actions
    public IActionResult Users() => View(_userManager.Users.ToList());

    public IActionResult GetCreateUserModal()
    {
        return PartialView("_CreateUserModal");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email i lozinka su obavezni.");
                var html = await this.RenderViewAsync("_CreateUserModal", (object)null, true);
                return Json(new { isValid = false, html = html });
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return Json(new { isValid = true, message = "Korisnik je uspješno stvoren!" });
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            var errorHtml = await this.RenderViewAsync("_CreateUserModal", (object)null, true);
            return Json(new { isValid = false, html = errorHtml });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Greška pri stvaranju korisnika: " + ex.Message);
            var html = await this.RenderViewAsync("_CreateUserModal", (object)null, true);
            return Json(new { isValid = false, html = html });
        }
    }

    public async Task<IActionResult> GetDeleteUserModal(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user == null ? NotFound() : PartialView("_DeleteUserModal", user);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return Json(new { isValid = true, message = "Korisnik je uspješno obrisan!" });
            }
            return Json(new { isValid = false, message = "Korisnik nije pronađen!" });
        }
        catch (Exception ex)
        {
            return Json(new { isValid = false, message = "Greška pri brisanju korisnika: " + ex.Message });
        }
    }

    private bool ApartmentExists(int id) => _context.Apartments.Any(e => e.Id == id);
}