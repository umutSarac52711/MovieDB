using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers;

// Consider renaming the controller file to MovieDirectorsController.cs
public class MovieDirectorsController : Controller // Renamed from ActorsController
{
    private readonly MovieDbContext dbContext;
    public MovieDirectorsController(MovieDbContext dbContext) 
    {
        this.dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var viewModel = new MovieDirectorViewModel
        {
            MovieList = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title"),
            DirectorList = new SelectList(await dbContext.Directors.OrderBy(d => d.Name).ToListAsync(), "Awardable_ID", "Name")
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(MovieDirectorViewModel model) 
    {
        if (ModelState.IsValid)
        {
            var existingRelationship = await dbContext.MovieDirectors
                .FirstOrDefaultAsync(md => md.MovieId == model.MovieId && md.DirectorId == model.DirectorId);

            if (existingRelationship != null)
            {
                ModelState.AddModelError(string.Empty, "This director is already assigned to this movie.");
                model.MovieList = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title", model.MovieId);
                model.DirectorList = new SelectList(await dbContext.Directors.OrderBy(d => d.Name).ToListAsync(), "Awardable_ID", "Name", model.DirectorId);
                return View(model);
            }

            var movieDirector = new MovieDirector
            {
                MovieId = model.MovieId,
                DirectorId = model.DirectorId
                // Assign other properties if MovieDirector entity has them
            };
            
            await dbContext.MovieDirectors.AddAsync(movieDirector);
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = "Director successfully assigned to movie!";
            return RedirectToAction(nameof(List));
        }
        model.MovieList = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title", model.MovieId);
        model.DirectorList = new SelectList(await dbContext.Directors.OrderBy(d => d.Name).ToListAsync(), "Awardable_ID", "Name", model.DirectorId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var movieDirectors = await dbContext.MovieDirectors
            .Include(md => md.Movie)
            .Include(md => md.Director)
            .OrderBy(md => md.Movie.Title)
            .ThenBy(md => md.Director.Name)
            .ToListAsync(); 

        return View(movieDirectors);
    }
    
    // Details view is usually not needed for a simple join table.

    [HttpGet]
    public async Task<IActionResult> Edit(int movieId, int directorId) // Composite key
    {
        var movieDirector = await dbContext.MovieDirectors
            .Include(md => md.Movie)
            .Include(md => md.Director)
            .FirstOrDefaultAsync(md => md.MovieId == movieId && md.DirectorId == directorId);

        if (movieDirector == null)
        {
            return NotFound();
        }

        var viewModel = new MovieDirectorViewModel
        {
            MovieId = movieDirector.MovieId,
            DirectorId = movieDirector.DirectorId,
            MovieTitle = movieDirector.Movie.Title, 
            DirectorName = movieDirector.Director.Name
            // Map other properties if MovieDirector entity has them
        };
        // Since there are no other properties on MovieDirector to edit currently,
        // this view will be mostly informational or a placeholder.
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MovieDirectorViewModel model) 
    {
        // If MovieDirector had other properties (e.g., RoleInMovie), they would be updated here.
        // Currently, only the relationship itself exists. If the intent is to change
        // the movie or director, it's a delete and add new operation.
        // For now, this Edit POST might not do much if no properties are editable.
        if (ModelState.IsValid)
        {
            var movieDirector = await dbContext.MovieDirectors
                .FirstOrDefaultAsync(md => md.MovieId == model.MovieId && md.DirectorId == model.DirectorId);

            if (movieDirector == null)
            {
                return NotFound();
            }
            
            // Example: if MovieDirector had a property like 'AssignmentType'
            // movieDirector.AssignmentType = model.AssignmentType;
            // await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Movie-Director relationship details (if any) updated!"; // Or "No changes to apply."
            return RedirectToAction(nameof(List));
        }
        // Repopulate for display if model state is invalid
        if (model.MovieId > 0 && model.DirectorId > 0)
        {
            var existing = await dbContext.MovieDirectors
                .Include(md => md.Movie)
                .Include(md => md.Director)
                .AsNoTracking()
                .FirstOrDefaultAsync(md => md.MovieId == model.MovieId && md.DirectorId == model.DirectorId);
            if (existing != null)
            {
                model.MovieTitle = existing.Movie.Title;
                model.DirectorName = existing.Director.Name;
            }
        }
        return View(model);
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int movieId, int directorId) // Composite key
    {
        var movieDirector = await dbContext.MovieDirectors
            .FirstOrDefaultAsync(md => md.MovieId == movieId && md.DirectorId == directorId);

        if (movieDirector == null)
        {
            return NotFound();
        }
        
        dbContext.MovieDirectors.Remove(movieDirector);
        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Movie-Director relationship successfully deleted!";
        return RedirectToAction(nameof(List));
    }
}
