using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers;

// Consider renaming the controller file to MovieActorsController.cs
public class MovieActorsController : Controller // Renamed from ActorsController
{
    private readonly MovieDbContext dbContext;
    public MovieActorsController(MovieDbContext dbContext) 
    {
        this.dbContext = dbContext;
    }
    
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var viewModel = new MovieActorViewModel
        {
            MovieList = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title"),
            ActorList = new SelectList(await dbContext.Actors.OrderBy(a => a.Name).ToListAsync(), "Awardable_ID", "Name")
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(MovieActorViewModel model) 
    {
        if (ModelState.IsValid)
        {
            // Check if the relationship already exists
            var existingRelationship = await dbContext.MovieActors
                .FirstOrDefaultAsync(ma => ma.MovieId == model.MovieId && ma.ActorId == model.ActorId);

            if (existingRelationship != null)
            {
                ModelState.AddModelError(string.Empty, "This actor is already assigned to this movie.");
                model.MovieList = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title", model.MovieId);
                model.ActorList = new SelectList(await dbContext.Actors.OrderBy(a => a.Name).ToListAsync(), "Awardable_ID", "Name", model.ActorId);
                return View(model);
            }

            var movieActor = new MovieActor
            {
                MovieId = model.MovieId,
                ActorId = model.ActorId,
                Role = model.Role
            };
            
            await dbContext.MovieActors.AddAsync(movieActor);
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = "Actor successfully assigned to movie!";
            return RedirectToAction(nameof(List));
        }
        model.MovieList = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title", model.MovieId);
        model.ActorList = new SelectList(await dbContext.Actors.OrderBy(a => a.Name).ToListAsync(), "Awardable_ID", "Name", model.ActorId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var movieActors = await dbContext.MovieActors
            .Include(ma => ma.Movie)
            .Include(ma => ma.Actor)
            .OrderBy(ma => ma.Movie.Title)
            .ThenBy(ma => ma.Actor.Name)
            .ToListAsync(); 

        return View(movieActors);
    }
    
    // Details view might not be necessary for a simple join table, or it could show Movie/Actor details.
    // For now, we'll skip implementing Details for MovieActor.

    [HttpGet]
    public async Task<IActionResult> Edit(int movieId, int actorId) // Composite key
    {
        var movieActor = await dbContext.MovieActors
            .Include(ma => ma.Movie)
            .Include(ma => ma.Actor)
            .FirstOrDefaultAsync(ma => ma.MovieId == movieId && ma.ActorId == actorId);

        if (movieActor == null)
        {
            return NotFound();
        }

        var viewModel = new MovieActorViewModel
        {
            MovieId = movieActor.MovieId,
            ActorId = movieActor.ActorId,
            Role = movieActor.Role,
            MovieTitle = movieActor.Movie.Title, // For display purposes
            ActorName = movieActor.Actor.Name,   // For display purposes
            // MovieList and ActorList are not strictly needed if MovieId/ActorId are not editable here
            // If they were editable, it's more like a delete and add new.
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MovieActorViewModel model) 
    {
        if (ModelState.IsValid) // Only Role is typically editable here. MovieId/ActorId form the key.
        {
            var movieActor = await dbContext.MovieActors
                .FirstOrDefaultAsync(ma => ma.MovieId == model.MovieId && ma.ActorId == model.ActorId);

            if (movieActor == null)
            {
                return NotFound();
            }

            movieActor.Role = model.Role;
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie-Actor relationship successfully updated!";
            return RedirectToAction(nameof(List));
        }
        // If model state is invalid, repopulate necessary fields if any (e.g., MovieTitle, ActorName for display)
        if (model.MovieId > 0 && model.ActorId > 0)
        {
            var existing = await dbContext.MovieActors
                .Include(ma => ma.Movie)
                .Include(ma => ma.Actor)
                .AsNoTracking()
                .FirstOrDefaultAsync(ma => ma.MovieId == model.MovieId && ma.ActorId == model.ActorId);
            if (existing != null)
            {
                model.MovieTitle = existing.Movie.Title;
                model.ActorName = existing.Actor.Name;
            }
        }
        return View(model);
    }
    
    // Delete GET view is often skipped for join table entries; deletion is usually a direct POST from the list.
    // However, if you want a confirmation page:
    [HttpGet]
    public async Task<IActionResult> Delete(int movieId, int actorId)
    {
        var movieActor = await dbContext.MovieActors
            .Include(ma => ma.Movie)
            .Include(ma => ma.Actor)
            .FirstOrDefaultAsync(ma => ma.MovieId == movieId && ma.ActorId == actorId);

        if (movieActor == null)
        {
            return NotFound();
        }
        // Pass the entity to a simple confirmation view if desired, or use JavaScript confirm on the List page.
        return View(movieActor); // You'll need a Delete.cshtml view that takes MovieActor
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int movieId, int actorId) // Composite key
    {
        var movieActor = await dbContext.MovieActors
            .FirstOrDefaultAsync(ma => ma.MovieId == movieId && ma.ActorId == actorId);

        if (movieActor == null)
        {
            return NotFound();
        }
        
        dbContext.MovieActors.Remove(movieActor);
        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Movie-Actor relationship successfully deleted!";
        return RedirectToAction(nameof(List));
    }
}
