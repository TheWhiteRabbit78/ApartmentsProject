using ApartmentsProject.Data;
using ApartmentsProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ApartmentsProject.Controllers
{
    [Authorize] // This requires user to be logged in
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var apartments = await _context.Apartments
                .Include(a => a.Images)
                .OrderBy(a => a.Id)
                .ToListAsync();

            return View(apartments);
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Images.OrderBy(img => img.DisplayOrder))
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                apartment.CreatedAt = DateTime.Now;
                _context.Apartments.Add(apartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(apartment);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            return View(apartment);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Apartment apartment)
        {
            if (id != apartment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Keep the original CreatedAt date
                    var originalApartment = await _context.Apartments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
                    apartment.CreatedAt = originalApartment.CreatedAt;

                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApartmentExists(apartment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(apartment);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartments
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apartment != null)
            {
                // Delete all image files from disk
                foreach (var image in apartment.Images)
                {
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", image.FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Apartments.Remove(apartment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.Id == id);
        }

        // GET: Admin/AddImage/5
        public async Task<IActionResult> AddImage(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }

            ViewBag.ApartmentTitle = apartment.Title;
            ViewBag.ApartmentId = id;
            return View();
        }

        // POST: Admin/AddImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(int apartmentId, IFormFile imageFile, string imageType, string caption, int displayOrder = 0)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Check file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                    ViewBag.ApartmentId = apartmentId;
                    return View();
                }

                // Generate unique filename
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");

                // Create uploads directory if it doesn't exist
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Save to database
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

        // POST: Admin/DeleteImage/5
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.ApartmentImages.FindAsync(id);
            if (image != null)
            {
                // Delete file from disk
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", image.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Delete from database
                _context.ApartmentImages.Remove(image);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = image.ApartmentId });
            }

            return NotFound();
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // GET: Admin/CreateUser
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Users));
        }
    }
}