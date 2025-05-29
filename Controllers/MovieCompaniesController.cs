using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers;

public class MovieCompaniesController : Controller
{
    private readonly MovieDbContext dbContext;

    public MovieCompaniesController(MovieDbContext context)
    {
        dbContext = context;
    }

    private async Task PopulateDropdownsAsync()
    {
        ViewBag.Movies = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title");
        ViewBag.Companies = new SelectList(await dbContext.Companies.OrderBy(c => c.Name).ToListAsync(), "Company_ID", "Name");
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        await PopulateDropdownsAsync();
        return View(new MovieCompanyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(MovieCompanyViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingAssociation = await dbContext.MovieCompanies
                .FirstOrDefaultAsync(mc => mc.Movie_ID == model.Movie_ID && mc.Company_ID == model.Company_ID);

            if (existingAssociation != null)
            {
                ModelState.AddModelError(string.Empty, "This movie is already associated with this company.");
                await PopulateDropdownsAsync();
                return View(model);
            }

            var movieCompany = new MovieCompany
            {
                Movie_ID = model.Movie_ID,
                Company_ID = model.Company_ID
            };

            dbContext.MovieCompanies.Add(movieCompany);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie-Company association added successfully.";
            return RedirectToAction(nameof(List));
        }
        await PopulateDropdownsAsync();
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var associations = await dbContext.MovieCompanies
            .Include(mc => mc.Movie)
            .Include(mc => mc.Company)
            .OrderBy(mc => mc.Movie.Title)
            .ThenBy(mc => mc.Company.Name)
            .ToListAsync();
        return View(associations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int movieId, int companyId)
    {
        var association = await dbContext.MovieCompanies
            .FirstOrDefaultAsync(mc => mc.Movie_ID == movieId && mc.Company_ID == companyId);

        if (association == null)
        {
            TempData["ErrorMessage"] = "Association not found.";
            return RedirectToAction(nameof(List));
        }

        dbContext.MovieCompanies.Remove(association);
        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Movie-Company association deleted successfully.";
        return RedirectToAction(nameof(List));
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int movieId, int companyId)
    {
        var association = await dbContext.MovieCompanies
            .FirstOrDefaultAsync(mc => mc.Movie_ID == movieId && mc.Company_ID == companyId);

        if (association == null)
        {
            return NotFound();
        }
        
        var viewModel = new MovieCompanyViewModel
        {
            Movie_ID = association.Movie_ID,
            Company_ID = association.Company_ID
        };
        await PopulateDropdownsAsync(); 
        ViewBag.CurrentMovieId = association.Movie_ID;
        ViewBag.CurrentCompanyId = association.Company_ID;
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int originalMovieId, int originalCompanyId, MovieCompanyViewModel model)
    {
        if (ModelState.IsValid)
        {
            // If the association hasn't changed, no action needed.
            if (originalMovieId == model.Movie_ID && originalCompanyId == model.Company_ID)
            {
                TempData["InfoMessage"] = "No changes detected in the association.";
                return RedirectToAction(nameof(List));
            }

            // Check if the new association already exists
            var newAssociationExists = await dbContext.MovieCompanies
                .AnyAsync(mc => mc.Movie_ID == model.Movie_ID && mc.Company_ID == model.Company_ID);

            if (newAssociationExists)
            {
                ModelState.AddModelError(string.Empty, "This movie-company association already exists.");
                await PopulateDropdownsAsync();
                ViewBag.CurrentMovieId = originalMovieId; // Keep original IDs for the view
                ViewBag.CurrentCompanyId = originalCompanyId;
                return View(model);
            }

            // Find and remove the old association
            var oldAssociation = await dbContext.MovieCompanies
                .FirstOrDefaultAsync(mc => mc.Movie_ID == originalMovieId && mc.Company_ID == originalCompanyId);

            if (oldAssociation != null)
            {
                dbContext.MovieCompanies.Remove(oldAssociation);
            }
            else
            {
                TempData["ErrorMessage"] = "Original association not found. Cannot update.";
                return RedirectToAction(nameof(List));
            }

            // Create the new association
            var newMovieCompany = new MovieCompany
            {
                Movie_ID = model.Movie_ID,
                Company_ID = model.Company_ID
            };
            dbContext.MovieCompanies.Add(newMovieCompany);

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie-Company association updated successfully.";
            return RedirectToAction(nameof(List));
        }

        // If model state is invalid, repopulate and return to view
        await PopulateDropdownsAsync();
        ViewBag.CurrentMovieId = originalMovieId; // Keep original IDs for the view
        ViewBag.CurrentCompanyId = originalCompanyId;
        return View(model);
    }
}
