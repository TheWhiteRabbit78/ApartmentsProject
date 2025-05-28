using System.Diagnostics;
using ApartmentsProject.Data;
using ApartmentsProject.Models;
using ApartmentsProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get apartments with their images
                var apartments = await _context.Apartments
                    .Include(a => a.Images.OrderBy(img => img.DisplayOrder))
                    .Where(a => a.IsAvailable)
                    .OrderBy(a => a.Id)
                    .ToListAsync();

                var viewModel = new HomeViewModel
                {
                    Apartments = apartments.Select(a => new ApartmentViewModel
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Rooms = a.Rooms,
                        Description = a.Description,
                        Price = a.Price,
                        IsAvailable = a.IsAvailable,
                        // Convert images to ViewModel
                        AllImages = a.Images.Select(img => new ApartmentImageViewModel
                        {
                            Id = img.Id,
                            Url = $"/uploads/{img.FileName}",
                            Type = img.ImageType,
                            Caption = img.Caption ?? "",
                            DisplayOrder = img.DisplayOrder
                        }).OrderBy(img => img.DisplayOrder).ToList()
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading apartments");
                return View(new HomeViewModel { Apartments = new List<ApartmentViewModel>() });
            }
        }

        private string? GetImageUrl(List<ApartmentImage> images, string imageType, int index = 0)
        {
            var image = images
                .Where(img => img.ImageType == imageType)
                .Skip(index)
                .FirstOrDefault();

            return image != null ? $"/uploads/{image.FileName}" : null;
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}