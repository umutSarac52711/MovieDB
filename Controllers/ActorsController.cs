using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieDB.Controllers;

public class ActorsController : Controller
{
    private readonly MovieDbContext dbContext;
    public ActorsController(MovieDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    private async Task PopulateMovieDropdownsAsync()
    {
        ViewBag.Movies = new SelectList(await dbContext.Movies.OrderBy(m => m.Title).ToListAsync(), "Awardable_ID", "Title");
    }
    
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(ActorViewModel model) // Parametre ActorViewModel olarak değiştirildi
    {
        if (ModelState.IsValid)
        {
            // Create a new Awardable entity
            var awardableEntity = new Awardable();
            awardableEntity.Kind = "Actor"; // Kind "Actor" olarak ayarlandı

            dbContext.Awardables.Add(awardableEntity);
            await dbContext.SaveChangesAsync();

            // Now create the Actor entity with the Awardable_ID from the newly created Awardable
            
            var actor = new Actor // Movie yerine Actor oluşturuldu
                {
                    Awardable_ID = awardableEntity.Awardable_ID,
                    Awardable = awardableEntity,
                    Name = model.Name, // ActorViewModel'den özellikler eşlendi
                    Birth_Date = model.Birth_Date,
                    Nationality = model.Nationality
                };
            
            // Genre ile ilgili mantık kaldırıldı
            
            
            await dbContext.Actors.AddAsync(actor); // dbContext.Actors'a eklendi
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = "Actor successfully added!"; // Mesaj güncellendi
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var actors = await dbContext.Actors.ToListAsync(); // dbContext.Actors'tan çekildi

        return View(actors);

    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var actor = await dbContext.Actors
            .Include(a => a.MovieActors)
                .ThenInclude(ma => ma.Movie)
            .FirstOrDefaultAsync(a => a.Awardable_ID == id);

        if (actor == null)
        {
            return NotFound();
        }

        var model = new ActorViewModel
        {
            Awardable_ID = actor.Awardable_ID,
            Name = actor.Name,
            Birth_Date = actor.Birth_Date,
            Nationality = actor.Nationality,
            AssociatedMovies = actor.MovieActors?.Select(ma => ma.Movie).ToList() ?? new List<Movie>()
        };

        await PopulateMovieDropdownsAsync();
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var actor = await dbContext.Actors // dbContext.Actors'tan çekildi
            // MovieGenres ile ilgili Include kaldırıldı
            .FirstOrDefaultAsync(a => a.Awardable_ID == id);

        if (actor == null)
        {
            return NotFound();
        }

        var viewModel = new ActorViewModel // ActorViewModel oluşturuldu
        {
            Awardable_ID = actor.Awardable_ID,
            Name = actor.Name, // Actor özelliklerinden eşlendi
            Birth_Date = actor.Birth_Date,
            Nationality = actor.Nationality
            // Genre eşlemesi kaldırıldı
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ActorViewModel model) // Parametre ActorViewModel olarak değiştirildi
    {
        if (ModelState.IsValid)
        {
            var actor = await dbContext.Actors // dbContext.Actors'tan çekildi
                // MovieGenres ile ilgili Include kaldırıldı
                .FirstOrDefaultAsync(a => a.Awardable_ID == model.Awardable_ID);

            if (actor == null)
            {
                return NotFound();
            }

            actor.Name = model.Name; // Actor özellikleri güncellendi
            actor.Birth_Date = model.Birth_Date;
            actor.Nationality = model.Nationality;
            
            // Genre güncelleme mantığı kaldırıldı
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Actor successfully updated!"; // Mesaj güncellendi
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var actor = await dbContext.Actors // dbContext.Actors'tan çekildi
             // MovieGenres ile ilgili Include kaldırıldı
            .FirstOrDefaultAsync(a => a.Awardable_ID == id);

        if (actor == null)
        {
            return NotFound();
        }

        return View(actor);
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var actor = await dbContext.Actors
            .Include(a => a.MovieActors)
            .FirstOrDefaultAsync(a => a.Awardable_ID == id);

        if (actor == null)
        {
            return NotFound();
        }

        // Remove all movie-actor associations
        if (actor.MovieActors != null && actor.MovieActors.Any())
        {
            dbContext.MovieActors.RemoveRange(actor.MovieActors);
        }

        // Remove the actor itself
        dbContext.Actors.Remove(actor);

        // Remove the Awardable entity
        var awardable = await dbContext.Awardables.FindAsync(actor.Awardable_ID);
        if (awardable != null)
        {
            dbContext.Awardables.Remove(awardable);
        }

        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Actor successfully deleted!";
        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMovieAssociation(int actorId, int movieId)
    {
        if (actorId <= 0 || movieId <= 0)
        {
            TempData["ErrorMessage"] = "Invalid actor or movie ID.";
            return RedirectToAction(nameof(Details), new { id = actorId });
        }

        // Check if the association already exists
        var existingAssociation = await dbContext.MovieActors
            .AnyAsync(ma => ma.Actor_ID == actorId && ma.Movie_ID == movieId);

        if (existingAssociation)
        {
            TempData["InfoMessage"] = "This movie is already associated with this actor.";
            return RedirectToAction(nameof(Details), new { id = actorId });
        }

        // Create new association
        var newAssociation = new MovieActor
        {
            Actor_ID = actorId,
            Movie_ID = movieId
        };

        await dbContext.MovieActors.AddAsync(newAssociation);
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Movie association added successfully.";
        return RedirectToAction(nameof(Details), new { id = actorId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveMovieAssociation(int actorId, int movieId)
    {
        var association = await dbContext.MovieActors
            .FirstOrDefaultAsync(ma => ma.Actor_ID == actorId && ma.Movie_ID == movieId);

        if (association == null)
        {
            TempData["ErrorMessage"] = "Association not found.";
            return RedirectToAction(nameof(Details), new { id = actorId });
        }

        dbContext.MovieActors.Remove(association);
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Movie association removed successfully.";
        return RedirectToAction(nameof(Details), new { id = actorId });
    }
}
