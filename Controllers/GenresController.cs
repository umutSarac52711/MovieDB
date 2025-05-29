using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;

namespace MovieDB.Controllers;

public class GenresController : Controller // Renamed from ActorsController
{
    private readonly MovieDbContext dbContext;
    public GenresController(MovieDbContext dbContext) // Constructor name updated
    {
        this.dbContext = dbContext;
    }
    
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(GenreViewModel model) // Parameter type changed to GenreViewModel
    {
        if (ModelState.IsValid)
        {
            // Check if genre with the same name already exists
            if (await dbContext.Genres.AnyAsync(g => g.Name == model.Name))
            {
                ModelState.AddModelError("Name", "A genre with this name already exists.");
                return View(model);
            }

            var genre = new Genre // Creating Genre entity
                {
                    Name = model.Name
                };
            
            await dbContext.Genres.AddAsync(genre); // Adding to dbContext.Genres
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = "Genre successfully added!"; // Message updated
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var genres = await dbContext.Genres.ToListAsync(); // Fetching from dbContext.Genres
        return View(genres);
    }
    
    // Details action might be overly simple for Genre, but kept for consistency if needed.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var genre = await dbContext.Genres
            .FirstOrDefaultAsync(g => g.Genre_ID == id);

        if (genre == null)
        {
            return NotFound();
        }

        var model = new GenreViewModel // Using GenreViewModel
        {
            Genre_ID = genre.Genre_ID,
            Name = genre.Name
        };

        return View(model); // Consider if a separate Details view is needed or if List is sufficient.
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var genre = await dbContext.Genres
            .FirstOrDefaultAsync(g => g.Genre_ID == id);

        if (genre == null)
        {
            return NotFound();
        }

        var viewModel = new GenreViewModel // Using GenreViewModel
        {
            Genre_ID = genre.Genre_ID,
            Name = genre.Name
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(GenreViewModel model) // Parameter type changed to GenreViewModel
    {
        if (ModelState.IsValid)
        {
            // Check if another genre with the same name already exists (excluding the current one)
            if (await dbContext.Genres.AnyAsync(g => g.Name == model.Name && g.Genre_ID != model.Genre_ID))
            {
                ModelState.AddModelError("Name", "Another genre with this name already exists.");
                return View(model);
            }

            var genre = await dbContext.Genres
                .FirstOrDefaultAsync(g => g.Genre_ID == model.Genre_ID);

            if (genre == null)
            {
                return NotFound();
            }

            genre.Name = model.Name;
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Genre successfully updated!"; // Message updated
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int id) // This action shows the confirmation view
    {
        var genre = await dbContext.Genres
            .FirstOrDefaultAsync(g => g.Genre_ID == id);

        if (genre == null)
        {
            return NotFound();
        }
        // Pass the Genre entity to the view for display
        return View(genre); 
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var genre = await dbContext.Genres
            .Include(g => g.MovieGenres) // Include MovieGenres to check for associations
            .FirstOrDefaultAsync(g => g.Genre_ID == id);

        if (genre == null)
        {
            return NotFound();
        }

        // Check if the genre is associated with any movies
        if (genre.MovieGenres.Any())
        {
            // If it is, add a model error and return to a view (e.g., the List view or a specific error view)
            // Or, more simply, set TempData and redirect.
            TempData["ErrorMessage"] = $"Cannot delete genre '{genre.Name}' because it is associated with one or more movies.";
            return RedirectToAction(nameof(List));
        }
        
        dbContext.Genres.Remove(genre); // Removing from dbContext.Genres
        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Genre successfully deleted!"; // Message updated
        return RedirectToAction(nameof(List));
    }
}
