using ApartmentsProject.Data;
using ApartmentsProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Controllers;

[Authorize]
public class AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment environment) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IWebHostEnvironment _environment = environment;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<Apartment> apartments = await _context.Apartments
            .Include(a => a.Images.OrderBy(i => i.UploadedAt))
            .OrderBy(a => a.Id)
            .ToListAsync();
        return View(apartments);
    }

    [HttpGet]
    public IActionResult GetApartmentModal(int id = 0)
    {
        if (id == 0)
        {
            // Add new apartment
            var apartment = new Apartment();
            return PartialView("_AddApartment", apartment);
        }
        else
        {
            // Edit existing apartment
            var apartment = _context.Apartments
                .Include(a => a.Images.OrderBy(i => i.UploadedAt))
                .FirstOrDefault(a => a.Id == id);

            if (apartment == null)
                return NotFound();

            return PartialView("_EditApartment", apartment);
        }
    }

    [HttpGet]
    public IActionResult GetDeleteApartmentModal(int id)
    {
        var apartment = _context.Apartments.FirstOrDefault(a => a.Id == id);
        if (apartment == null)
            return NotFound();

        return PartialView("_DeleteApartment", apartment);
    }

    [HttpPost]
    public async Task<IActionResult> AddApartment(Apartment apartment, List<IFormFile> imageFiles)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { isValid = false, html = await this.RenderViewAsync("_AddApartment", apartment, true) });
        }

        try
        {
            apartment.CreatedAt = DateTime.Now;
            _context.Apartments.Add(apartment);
            await _context.SaveChangesAsync();

            // Handle image uploads
            if (imageFiles?.Any() == true)
            {
                await SaveImages(apartment.Id, imageFiles);
            }

            return Json(new { isValid = true, message = "Stan je uspješno dodan!" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Došlo je do greške: " + ex.Message);
            return Json(new { isValid = false, html = await this.RenderViewAsync("_AddApartment", apartment, true) });
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditApartment(Apartment apartment, List<IFormFile> imageFiles)
    {
        if (!ModelState.IsValid)
        {
            // Reload images for validation display
            apartment.Images = _context.ApartmentImages
                .Where(i => i.ApartmentId == apartment.Id)
                .OrderBy(i => i.UploadedAt)
                .ToList();
            return Json(new { isValid = false, html = await this.RenderViewAsync("_EditApartment", apartment, true) });
        }

        try
        {
            _context.Apartments.Update(apartment);
            await _context.SaveChangesAsync();

            // Handle new image uploads
            if (imageFiles?.Any() == true)
            {
                await SaveImages(apartment.Id, imageFiles);
            }

            return Json(new { isValid = true, message = "Stan je uspješno ažuriran!" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Došlo je do greške: " + ex.Message);
            apartment.Images = _context.ApartmentImages
                .Where(i => i.ApartmentId == apartment.Id)
                .OrderBy(i => i.UploadedAt)
                .ToList();
            return Json(new { isValid = false, html = await this.RenderViewAsync("_EditApartment", apartment, true) });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteApartment(int id)
    {
        try
        {
            var apartment = await _context.Apartments
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
                return Json(new { isValid = false, message = "Stan nije pronađen." });

            // Delete physical files
            foreach (var image in apartment.Images)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "assets", "images", image.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();

            return Json(new { isValid = true, message = "Stan je uspješno obrisan!" });
        }
        catch (Exception ex)
        {
            return Json(new { isValid = false, message = "Došlo je do greške: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        try
        {
            var image = _context.ApartmentImages.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                return Json(new { success = false, message = "Slika nije pronađena." });

            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath, "assets", "images", image.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.ApartmentImages.Remove(image);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Slika je obrisana!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Došlo je do greške: " + ex.Message });
        }
    }

    private async Task SaveImages(int apartmentId, List<IFormFile> imageFiles)
    {
        var uploadsPath = Path.Combine(_environment.WebRootPath, "assets", "images");
        Directory.CreateDirectory(uploadsPath);

        foreach (var file in imageFiles)
        {
            if (file.Length > 0)
            {
                var fileName = $"{apartmentId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var apartmentImage = new ApartmentImage
                {
                    ApartmentId = apartmentId,
                    FileName = fileName,
                    UploadedAt = DateTime.Now
                };

                _context.ApartmentImages.Add(apartmentImage);
            }
        }

        await _context.SaveChangesAsync();
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }
}

public static class ControllerExtensions
{
    public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = controller.ControllerContext.ActionDescriptor.ActionName;

        controller.ViewData.Model = model;

        using var writer = new StringWriter();
        var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine)) as Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine;
        var viewContext = new ViewContext(controller.ControllerContext, viewEngine.FindView(controller.ControllerContext, viewName, !partial).View, controller.ViewData, controller.TempData, writer, new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions());

        await viewContext.View.RenderAsync(viewContext);
        return writer.GetStringBuilder().ToString();
    }
}