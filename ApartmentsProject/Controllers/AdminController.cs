using ApartmentsProject.Data;
using ApartmentsProject.Models;
using ApartmentsProject.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Controllers;

[Authorize]
public class AdminController(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<IdentityUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<Apartment> apartments = await _context.Apartments.Include(a => a.Images).OrderBy(a => a.Id).ToListAsync();
        return View(apartments);
    }

    [HttpGet]
    public async Task<IActionResult> GetApartmentModal(int id = 0)
    {
        Apartment? apartment = new Apartment();
        ViewBag.IsEdit = false;

        if (id != 0)
        {
            apartment = await _context.Apartments.FindAsync(id);
            ViewBag.IsEdit = true;
        }

        return PartialView("_ApartmentModal", apartment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveApartment(Apartment apartment)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                if (apartment.Id != 0)
                {
                    ViewBag.IsEdit = true;
                }

                string html = await this.RenderViewAsync("_ApartmentModal", apartment, true);
                return Json(new { isValid = false, html });
            }

            if (apartment.Id == 0)
            {
                _context.Apartments.Add(apartment);
            }
            else
            {
                Apartment? original = await _context.Apartments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == apartment.Id);
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
            string html = await this.RenderViewAsync("_ApartmentModal", apartment, true);
            return Json(new { isValid = false, html });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDeleteApartmentModal(int id)
    {
        Apartment? apartment = await _context.Apartments.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);
        return PartialView("_DeleteApartmentModal", apartment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteApartment(int id)
    {
        try
        {
            Apartment? apartment = await _context.Apartments.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);

            if (apartment != null)
            {
                foreach (ApartmentImage image in apartment.Images)
                {
                    string filePath = Path.Combine(_environment.WebRootPath, "uploads", image.FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
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

    [HttpGet]
    public async Task<IActionResult> GetApartmentDetailsModal(int id)
    {
        Apartment? apartment = await _context.Apartments.Include(a => a.Images.OrderBy(img => img.DisplayOrder)).FirstOrDefaultAsync(a => a.Id == id);
        return PartialView("_ApartmentDetailsModal", apartment);
    }

    [HttpGet]
    public async Task<IActionResult> GetAddImageModal(int id)
    {
        Apartment? apartment = await _context.Apartments.FindAsync(id);
        if (apartment == null) return NotFound();

        ViewBag.ApartmentTitle = apartment.Title;
        ViewBag.ApartmentId = id;
        return PartialView("_AddImageModal");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(int apartmentId, IFormFile imageFile, string imageType, string? caption, int displayOrder = 0)
    {
        try
        {
            if (imageFile?.Length > 0)
            {
                string[] allowedExts = [".jpg", ".jpeg", ".png"];
                string fileExt = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExts.Contains(fileExt))
                {
                    ModelState.AddModelError("", "Dozvoljena su samo .jpg, .jpeg, .png, proširenja.");
                }
                else
                {
                    // Check if apartment exists
                    var apartment = await _context.Apartments.FindAsync(apartmentId);
                    if (apartment == null)
                    {
                        ModelState.AddModelError("", "Stan nije pronađen.");
                        ViewBag.ApartmentId = apartmentId;
                        string html = await this.RenderViewAsync("_AddImageModal", null!, true);
                        return Json(new { isValid = false, html });
                    }

                    string fileName = Guid.NewGuid() + fileExt;
                    string uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsPath);

                    string filePath = Path.Combine(uploadsPath, fileName);

                    // Check if we already have an image with the same original filename to prevent accidental duplicates
                    var existingImage = await _context.ApartmentImages
                        .Where(ai => ai.ApartmentId == apartmentId &&
                                    ai.Caption == caption &&
                                    ai.ImageType == imageType)
                        .FirstOrDefaultAsync();

                    if (existingImage != null)
                    {
                        ModelState.AddModelError("", "Slika s istim opisom i tipom već postoji za ovaj stan.");
                        ViewBag.ApartmentId = apartmentId;
                        string duplicateHtml = await this.RenderViewAsync("_AddImageModal", null!, true);
                        return Json(new { isValid = false, html = duplicateHtml });
                    }

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    var apartmentImage = new ApartmentImage
                    {
                        ApartmentId = apartmentId,
                        FileName = fileName,
                        ImageType = imageType,
                        Caption = caption,
                        DisplayOrder = displayOrder,
                        UploadedAt = DateTime.Now.ToLocalTime()
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
            string errorHtml = await this.RenderViewAsync("_AddImageModal", null!, true);
            return Json(new { isValid = false, html = errorHtml });
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error in AddImage: {ex.Message}");

            ModelState.AddModelError("", "Greška pri dodavanju slike: " + ex.Message);
            ViewBag.ApartmentId = apartmentId;
            string errorHtml = await this.RenderViewAsync("_AddImageModal", null!, true);
            return Json(new { isValid = false, html = errorHtml });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id)
    {
        try
        {
            ApartmentImage? image = await _context.ApartmentImages.FindAsync(id);
            if (image != null)
            {
                string filePath = Path.Combine(_environment.WebRootPath, "uploads", image.FileName);
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

    [HttpGet]
    public IActionResult Users() { 
        return View(_userManager.Users.ToList());
    }

    [HttpGet]
    public IActionResult GetCreateUserModal()
    {
        return PartialView("_CreateUserModal");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email i lozinka su obavezni.");
                string html = await this.RenderViewAsync("_CreateUserModal", null!, true);
                return Json(new { isValid = false, html });
            }

            IdentityUser user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            IdentityResult result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return Json(new { isValid = true, message = "Korisnik je uspješno dodan!" });
            }

            foreach (IdentityError error in result.Errors) 
            {
                ModelState.AddModelError("", error.Description);
            }            

            string errorHtml = await this.RenderViewAsync("_CreateUserModal", null, true);
            return Json(new { isValid = false, html = errorHtml });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Greška pri dodavanju korisnika: " + ex.Message);
            string html = await this.RenderViewAsync("_CreateUserModal", null, true);
            return Json(new { isValid = false, html });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDeleteUserModal(string id)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return PartialView("_DeleteUserModal", user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            IdentityUser? user = await _userManager.FindByIdAsync(id);
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
}