using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers;

public class MovieActorController : Controller
{
    private readonly MovieDbContext dbContext;

    public MovieActorController(MovieDbContext context) // Corrected constructor name
    {
        dbContext = context;
    }

    private async Task PopulateDropdownsAsync()
    {
        ViewBag.Movies = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title");
        ViewBag.Actors = new SelectList(await dbContext.Actors.OrderBy(a => a.Name).ToListAsync(), "Awardable_ID", "Name");
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        await PopulateDropdownsAsync();
        return View(new MovieActorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(MovieActorViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingAssociation = await dbContext.MovieActors
                .FirstOrDefaultAsync(ma => ma.Movie_ID == model.Movie_ID && ma.Actor_ID == model.Actor_ID);

            if (existingAssociation != null)
            {
                ModelState.AddModelError(string.Empty, "This actor is already associated with this movie.");
                await PopulateDropdownsAsync();
                return View(model);
            }

            var movieActor = new MovieActor
            {
                Movie_ID = model.Movie_ID,
                Actor_ID = model.Actor_ID
            };

            dbContext.MovieActors.Add(movieActor);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie-Actor association added successfully.";
            return RedirectToAction(nameof(List));
        }
        await PopulateDropdownsAsync();
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var associations = await dbContext.MovieActors
            .Include(ma => ma.Movie)
            .Include(ma => ma.Actor)
            .OrderBy(ma => ma.Movie.Title)
            .ThenBy(ma => ma.Actor.Name)
            .ToListAsync();
        return View(associations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int movieId, int actorId) // Changed companyId to actorId
    {
        var association = await dbContext.MovieActors
            .FirstOrDefaultAsync(ma => ma.Movie_ID == movieId && ma.Actor_ID == actorId);

        if (association == null)
        {
            TempData["ErrorMessage"] = "Association not found.";
            return RedirectToAction(nameof(List));
        }

        dbContext.MovieActors.Remove(association);
        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Movie-Actor association deleted successfully.";
        return RedirectToAction(nameof(List));
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int movieId, int actorId) // Changed companyId to actorId
    {
        var association = await dbContext.MovieActors
            .FirstOrDefaultAsync(ma => ma.Movie_ID == movieId && ma.Actor_ID == actorId);

        if (association == null)
        {
            return NotFound();
        }
        
        var viewModel = new MovieActorViewModel
        {
            Movie_ID = association.Movie_ID,
            Actor_ID = association.Actor_ID
        };
        await PopulateDropdownsAsync(); 
        ViewBag.CurrentMovieId = association.Movie_ID;
        ViewBag.CurrentActorId = association.Actor_ID; 
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int originalMovieId, int originalActorId, MovieActorViewModel model)
    {
        if (ModelState.IsValid)
        {
            // If the association hasn't changed, no action needed.
            if (originalMovieId == model.Movie_ID && originalActorId == model.Actor_ID)
            {
                TempData["InfoMessage"] = "No changes detected in the association.";
                return RedirectToAction(nameof(List));
            }

            // Check if the new association already exists
            var newAssociationExists = await dbContext.MovieActors
                .AnyAsync(ma => ma.Movie_ID == model.Movie_ID && ma.Actor_ID == model.Actor_ID);

            if (newAssociationExists)
            {
                ModelState.AddModelError(string.Empty, "This movie-actor association already exists.");
                await PopulateDropdownsAsync();
                ViewBag.CurrentMovieId = originalMovieId; // Keep original IDs for the view
                ViewBag.CurrentActorId = originalActorId;
                return View(model);
            }

            // Find and remove the old association
            var oldAssociation = await dbContext.MovieActors
                .FirstOrDefaultAsync(ma => ma.Movie_ID == originalMovieId && ma.Actor_ID == originalActorId);

            if (oldAssociation != null)
            {
                dbContext.MovieActors.Remove(oldAssociation);
            }
            else
            {
                // Should not happen if we are editing an existing record, but good to handle
                TempData["ErrorMessage"] = "Original association not found. Cannot update.";
                return RedirectToAction(nameof(List));
            }

            // Create the new association
            var newMovieActor = new MovieActor
            {
                Movie_ID = model.Movie_ID,
                Actor_ID = model.Actor_ID
            };
            dbContext.MovieActors.Add(newMovieActor);

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie-Actor association updated successfully.";
            return RedirectToAction(nameof(List));
        }

        // If model state is invalid, repopulate and return to view
        await PopulateDropdownsAsync();
        ViewBag.CurrentMovieId = originalMovieId; // Keep original IDs for the view
        ViewBag.CurrentActorId = originalActorId;
        return View(model);
    }
}
