using ApartmentsProject.Data;
using ApartmentsProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApartmentsProject.Controllers;

[Authorize]
public class AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : Controller
{
    private readonly ApplicationDbContext _context = context;    
    private readonly UserManager<IdentityUser> _userManager = userManager;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<Apartment> apartments = await _context.Apartments.Include(a => a.Images).OrderBy(a => a.Id).ToListAsync();
        return View(apartments);
    }

    [HttpGet]
    public IActionResult AddApartmentModal()
    {
        ApartmentViewModel viewModel = new ApartmentViewModel();
        return PartialView("_AddApartment", viewModel);
    }

    [HttpGet]
    public IActionResult EditApartmentModal(int id)     
    {
        ApartmentViewModel viewModel = new ApartmentViewModel();
        viewModel.Apartment = _context.Apartments.Where(a => a.Id == id).FirstOrDefault();
        viewModel.ApartmentImages =  _context.ApartmentImages.Where(ai => ai.ApartmentId == id).ToList();
        return PartialView("_EditApartment", viewModel);
    }

    [HttpGet]
    public IActionResult DeleteApartmentModal(int id)
    {
        int apartmentId = _context.Apartments.Where(a => a.Id == id).Select(a => a.Id).FirstOrDefault();
        return PartialView("_DeleteApartment", apartmentId);
    }

    [HttpPost]
    public IActionResult AddApartment(ApartmentViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            //return a toast that says Greška kod dodavanja novog stana (error message)
        }

        try
        {
            if (viewModel.ApartmentImages.Any())
            {
                _context.ApartmentImages.AddRange(viewModel.ApartmentImages);
            }

            _context.Apartments.Add(viewModel.Apartment);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            //return a toast that says Greška kod dodavanja novog stana (ex)
        }

        return View(Index());
    }

    [HttpPost]
    public async Task<IActionResult> EditApartment(ApartmentViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            //return a toast that says Greška kod dodavanja novog stana (error message)
        }

        try
        {
            if (viewModel.ApartmentImages.Any())
            {
                
            }

            _context.Apartments.Update(viewModel.Apartment);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            //return a toast that says Greška kod dodavanja novog stana (ex)
        }

        return View(Index());
    }

    [HttpPost]
    public async Task<IActionResult> DeleteApartment(int id)
    {
        if (!ModelState.IsValid)
        {
            //return a toast that says Greška kod dodavanja novog stana (error message)
        }

        try
        {
            Apartment apartment = _context.Apartments.Where(a => a.Id == id).FirstOrDefault();
            List<ApartmentImage> apartmentImages =  _context.ApartmentImages.Where(ai => ai.ApartmentId == id).ToList();

            _context.ApartmentImages.RemoveRange(apartmentImages);
            _context.Apartments.Remove(apartment);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            //return a toast that says Greška kod dodavanja novog stana (ex)
        }

        return View(Index());
    }

}