using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers;

public class MovieDirectorController : Controller
{
    private readonly MovieDbContext dbContext;

    public MovieDirectorController(MovieDbContext context) // Corrected constructor name
    {
        dbContext = context;
    }

    private async Task PopulateDropdownsAsync()
    {
        ViewBag.Movies = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title");
        ViewBag.Directors = new SelectList(await dbContext.Directors.OrderBy(d => d.Name).ToListAsync(), "Awardable_ID", "Name");
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        await PopulateDropdownsAsync();
        return View(new MovieDirectorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(MovieDirectorViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingAssociation = await dbContext.MovieDirectors
                .FirstOrDefaultAsync(md => md.Movie_ID == model.Movie_ID && md.Director_ID == model.Director_ID);

            if (existingAssociation != null)
            {
                ModelState.AddModelError(string.Empty, "This director is already associated with this movie.");
                await PopulateDropdownsAsync();
                return View(model);
            }

            var movieDirector = new MovieDirector
            {
                Movie_ID = model.Movie_ID,
                Director_ID = model.Director_ID
            };

            dbContext.MovieDirectors.Add(movieDirector);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie-Director association added successfully.";
            return RedirectToAction(nameof(List));
        }

        await PopulateDropdownsAsync();
        return View(model);
        
    }
    
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var associations = await dbContext.MovieDirectors
            .Include(md => md.Movie)
            .Include(md => md.Director)
            .OrderBy(md => md.Movie.Title) // Added ordering
            .ThenBy(md => md.Director.Name) // Added ordering
            .ToListAsync();

        return View(associations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int movieId, int directorId) // Changed actorId to directorId
    {
        var association = await dbContext.MovieDirectors
            .FirstOrDefaultAsync(md => md.Movie_ID == movieId && md.Director_ID == directorId); 
        if (association == null)
        {
            TempData["ErrorMessage"] = "Association not found.";
            return RedirectToAction(nameof(List));
        }
        
        dbContext.MovieDirectors.Remove(association);
        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Movie-Director association deleted successfully.";
        return RedirectToAction(nameof(List));
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int movieId, int directorId) // Changed actorId to directorId
    {
        var association = await dbContext.MovieDirectors
            .FirstOrDefaultAsync(md => md.Movie_ID == movieId && md.Director_ID == directorId); 

        if (association == null)
        {
            return NotFound();
        }

        var model = new MovieDirectorViewModel
        {
            Movie_ID = association.Movie_ID,
            Director_ID = association.Director_ID
        };

        await PopulateDropdownsAsync();
        ViewBag.CurrentMovieId = movieId; 
        ViewBag.CurrentDirectorId = directorId; // Changed CurrentActorId to CurrentDirectorId
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int originalMovieId, int originalDirectorId, MovieDirectorViewModel model) // Changed originalActorId
    {
        if (ModelState.IsValid)
        {
            // If the association hasn't changed, no action needed.
            if (originalMovieId == model.Movie_ID && originalDirectorId == model.Director_ID)
            {
                TempData["InfoMessage"] = "No changes detected in the association.";
                return RedirectToAction(nameof(List));
            }
            
            // Check if the new association already exists
            var newAssociationExists = await dbContext.MovieDirectors
                .AnyAsync(md => md.Movie_ID == model.Movie_ID && md.Director_ID == model.Director_ID);

            if (newAssociationExists)
            {
                ModelState.AddModelError(string.Empty, "This movie-director association already exists.");
                await PopulateDropdownsAsync();
                ViewBag.CurrentMovieId = originalMovieId;
                ViewBag.CurrentDirectorId = originalDirectorId;
                return View(model);
            }

            // Find and remove the old association
            var oldAssociation = await dbContext.MovieDirectors
                .FirstOrDefaultAsync(md => md.Movie_ID == originalMovieId && md.Director_ID == originalDirectorId);

            if (oldAssociation != null)
            {
                dbContext.MovieDirectors.Remove(oldAssociation);
            }
            else
            {
                TempData["ErrorMessage"] = "Original association not found. Cannot update.";
                return RedirectToAction(nameof(List));
            }
            
            // Create the new association
            var newMovieDirector = new MovieDirector
            {
                Movie_ID = model.Movie_ID,
                Director_ID = model.Director_ID
            };
            dbContext.MovieDirectors.Add(newMovieDirector);

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie-Director association updated successfully.";
            return RedirectToAction(nameof(List));
        }

        // If model state is invalid, repopulate and return to view
        await PopulateDropdownsAsync();
        ViewBag.CurrentMovieId = originalMovieId; 
        ViewBag.CurrentDirectorId = originalDirectorId; // Changed CurrentActorId
        return View(model);
    }
}
