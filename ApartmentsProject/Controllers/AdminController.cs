// Controllers/AdminController.cs
using ApartmentsProject.Data;
using ApartmentsProject.Models;
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

    public async Task<IActionResult> Details(int id)
    {
        var apartment = await _context.Apartments
            .Include(a => a.Images.OrderBy(img => img.DisplayOrder))
            .FirstOrDefaultAsync(a => a.Id == id);
        return apartment == null ? NotFound() : View(apartment);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Apartment apartment)
    {
        if (!ModelState.IsValid) return View(apartment);
        
        apartment.CreatedAt = DateTime.Now;
        _context.Apartments.Add(apartment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var apartment = await _context.Apartments.FindAsync(id);
        return apartment == null ? NotFound() : View(apartment);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Apartment apartment)
    {
        if (id != apartment.Id) return NotFound();
        if (!ModelState.IsValid) return View(apartment);

        try
        {
            var original = await _context.Apartments.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
            apartment.CreatedAt = original!.CreatedAt;
            
            _context.Update(apartment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            return ApartmentExists(apartment.Id) ? throw null : NotFound();
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var apartment = await _context.Apartments.Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.Id == id);
        return apartment == null ? NotFound() : View(apartment);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var apartment = await _context.Apartments.Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (apartment != null)
        {
            foreach (var image in apartment.Images)
            {
                var filePath = Path.Combine(_environment.WebRootPath, 
                    "uploads", image.FileName);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddImage(int id)
    {
        var apartment = await _context.Apartments.FindAsync(id);
        if (apartment == null) return NotFound();
        
        ViewBag.ApartmentTitle = apartment.Title;
        ViewBag.ApartmentId = id;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(int apartmentId, IFormFile imageFile,
        string imageType, string? caption, int displayOrder = 0)
    {
        if (imageFile?.Length > 0)
        {
            var allowedExts = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExt = Path.GetExtension(imageFile.FileName).ToLower();

            if (!allowedExts.Contains(fileExt))
            {
                ModelState.AddModelError("", "Only .jpg, .jpeg, .png, .gif allowed.");
                ViewBag.ApartmentId = apartmentId;
                return View();
            }

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
            return RedirectToAction("Details", new { id = apartmentId });
        }

        ModelState.AddModelError("", "Please select an image file.");
        ViewBag.ApartmentId = apartmentId;
        return View();
    }

    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await _context.ApartmentImages.FindAsync(id);
        if (image != null)
        {
            var filePath = Path.Combine(_environment.WebRootPath, 
                "uploads", image.FileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.ApartmentImages.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = image.ApartmentId });
        }
        return NotFound();
    }

    public IActionResult Users() => View(_userManager.Users.ToList());

    public IActionResult CreateUser() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Email and password are required.");
            return View();
        }

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded) return RedirectToAction(nameof(Users));

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Users));
    }

    private bool ApartmentExists(int id) => _context.Apartments.Any(e => e.Id == id);
}