using ApartmentsProject.Data;
using ApartmentsProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
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
        List<Apartment> apartments = await _context.Apartments.Include(a => a.Images.OrderBy(i => i.UploadedAt))
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
            var apartment = _context.Apartments.Include(a => a.Images.OrderBy(i => i.UploadedAt)).FirstOrDefault(a => a.Id == id);
            return PartialView("_EditApartment", apartment);
        }
    }

    [HttpGet]
    public IActionResult GetDeleteApartmentModal(int id)
    {
        var apartment = _context.Apartments.FirstOrDefault(a => a.Id == id);                
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
            if (imageFiles.Any())
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
            apartment.Images = _context.ApartmentImages.Where(i => i.ApartmentId == apartment.Id)
                                                       .OrderBy(i => i.UploadedAt)
                                                       .ToList();
            return Json(new { isValid = false, html = await this.RenderViewAsync("_EditApartment", apartment, true) });
        }

        try
        {
            _context.Apartments.Update(apartment);
            await _context.SaveChangesAsync();

            if (imageFiles.Any())
            {
                await SaveImages(apartment.Id, imageFiles);
            }

            return Json(new { isValid = true, message = "Stan je uspješno ažuriran!" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Došlo je do greške: " + ex.Message);
            apartment.Images = _context.ApartmentImages.Where(i => i.ApartmentId == apartment.Id).OrderBy(i => i.UploadedAt).ToList();
            return Json(new { isValid = false, html = await this.RenderViewAsync("_EditApartment", apartment, true) });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteApartment(int id)
    {
        try
        {
            Apartment apartment = await _context.Apartments.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);
            
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

            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath, "assets", "images", image.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.ApartmentImages.Remove(image);
            await _context.SaveChangesAsync();

            return Json(new { isValid = true, message = "Slika je uspješno obrisana!" });
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

                string filePath = Path.Combine(uploadsPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                ApartmentImage apartmentImage = new ApartmentImage
                {
                    ApartmentId = apartmentId,
                    FileName = file.FileName,
                    UploadedAt = DateTime.Now
                };

                _context.ApartmentImages.Add(apartmentImage);
            }
        }

        await _context.SaveChangesAsync();
    }

    [HttpGet]
    public IActionResult Users()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    [HttpGet]
    public IActionResult GetCreateUserModal()
    {
        var model = new CreateUserViewModel();
        return PartialView("_CreateUser", model);
    }

    [HttpGet]
    public IActionResult GetDeleteUserModal(string id)
    {
        var user = _userManager.FindByIdAsync(id).Result;
        if (user == null)
            return NotFound();

        return PartialView("_DeleteUser", user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { isValid = false, html = await this.RenderViewAsync("_CreateUser", model, true) });
        }

        try
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Json(new { isValid = true, message = "Korisnik je uspješno kreiran!" });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Json(new { isValid = false, html = await this.RenderViewAsync("_CreateUser", model, true) });
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Došlo je do greške: " + ex.Message);
            return Json(new { isValid = false, html = await this.RenderViewAsync("_CreateUser", model, true) });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { isValid = false, message = "Korisnik nije pronađen." });

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Json(new { isValid = true, message = "Korisnik je uspješno obrisan!" });
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Json(new { isValid = false, message = "Greška pri brisanju: " + errors });
            }
        }
        catch (Exception ex)
        {
            return Json(new { isValid = false, message = "Došlo je do greške: " + ex.Message });
        }
    }
}

public static class ControllerExtensions
{
    public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel? model, bool partial = false)
    {
        if (string.IsNullOrEmpty(viewName)) 
        {
            viewName = controller.ControllerContext.ActionDescriptor.ActionName;
        }   

        controller.ViewData.Model = model;

        using var writer = new StringWriter();
        var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        var viewContext = new ViewContext(controller.ControllerContext, 
                                          viewEngine.FindView(controller.ControllerContext, viewName, !partial).View, 
                                          controller.ViewData, 
                                          controller.TempData, 
                                          writer, 
                                          new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions());

        await viewContext.View.RenderAsync(viewContext);
        return writer.GetStringBuilder().ToString();
    }
}