using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;

namespace MovieDB.Controllers;

public class MoviesController : Controller
{
    private readonly MovieDbContext dbContext;
    public MoviesController(MovieDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(MovieViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create a new Awardable entity
            var awardableEntity = new Awardable();
            awardableEntity.Kind = "Movie";

            dbContext.Awardables.Add(awardableEntity);
            await dbContext.SaveChangesAsync();

            // Now create the Movie entity with the Awardable_ID from the newly created Awardable
            
            var movie = new Movie
                {
                    Awardable_ID = awardableEntity.Awardable_ID,
                    Awardable = awardableEntity,
                    Title = model.Title,
                    Release_Date = model.Release_Date,
                    Language = model.Language,
                    Duration = model.Duration,
                    Budget = model.Budget,
                    Revenue = model.Revenue,
                    Rating = model.Rating,
                    PosterUrl = model.PosterUrl,
                    MovieGenres = new List<MovieGenre>() // Initialize MovieGenres collection
                };
            
            /* Add genres if any
            if (model.Genres != null && model.Genres.Count > 0)
            {
                foreach (var genreName in model.Genres)
                {
                    if (!string.IsNullOrWhiteSpace(genreName))
                    {
                        var movieGenre = new MovieGenre
                        {
                            // Movie_ID will be set by EF relationship when added to movie.MovieGenres
                            Genre = genreName 
                        };
                        movie.MovieGenres.Add(movieGenre);
                    }
                }
            }
            */
            
            
            await dbContext.Movies.AddAsync(movie);
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = "Movie successfully added!";
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var movies = await dbContext.Movies.ToListAsync();

        return View(movies);

    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var movie = await dbContext.Movies
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre) 
            .FirstOrDefaultAsync(m => m.Awardable_ID == id);

        if (movie == null)
        {
            return NotFound();
        }

        var model = new MovieViewModel
        {
            Awardable_ID = movie.Awardable_ID,
            Title = movie.Title,
            Release_Date = movie.Release_Date,
            Language = movie.Language,
            Duration = movie.Duration,
            Budget = movie.Budget,
            Revenue = movie.Revenue,
            Rating = movie.Rating,
            PosterUrl = movie.PosterUrl,
            Genres = movie.MovieGenres.Select(mg => mg.Genre).ToList()
        };

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await dbContext.Movies
            .Include(m => m.MovieGenres)
            .FirstOrDefaultAsync(m => m.Awardable_ID == id);

        if (movie == null)
        {
            return NotFound();
        }

        var viewModel = new MovieViewModel
        {
            Awardable_ID = movie.Awardable_ID,
            Title = movie.Title,
            Release_Date = movie.Release_Date,
            Language = movie.Language,
            Duration = movie.Duration,
            Budget = movie.Budget,
            Revenue = movie.Revenue,
            Rating = movie.Rating,
            PosterUrl = movie.PosterUrl,
            Genres = movie.MovieGenres?.Select(mg => mg.Genre).ToList() ?? new List<string>()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MovieViewModel model)
    {
        if (ModelState.IsValid)
        {
            var movie = await dbContext.Movies
                .Include(m => m.MovieGenres)
                .FirstOrDefaultAsync(m => m.Awardable_ID == model.Awardable_ID);

            if (movie == null)
            {
                return NotFound();
            }

            movie.Title = model.Title;
            movie.Release_Date = model.Release_Date;
            movie.Language = model.Language;
            movie.Duration = model.Duration;
            movie.Budget = model.Budget;
            movie.Revenue = model.Revenue;
            movie.Rating = model.Rating;
            movie.PosterUrl = model.PosterUrl;
            
            // Clear existing genres and add new ones
            var existingMovieGenres = dbContext.MovieGenres.Where(mg => mg.Movie_ID == movie.Awardable_ID);
            dbContext.MovieGenres.RemoveRange(existingMovieGenres);
            
            if (model.Genres != null)
            {
                foreach (var genreName in model.Genres)
                {
                    if (!string.IsNullOrWhiteSpace(genreName))
                    {
                        movie.MovieGenres.Add(new MovieGenre { Genre = genreName });
                    }
                }
            }
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie successfully updated!";
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await dbContext.Movies
            .Include(m => m.MovieGenres)
            .FirstOrDefaultAsync(m => m.Awardable_ID == id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await dbContext.Movies
            .Include(m => m.MovieGenres)
            .FirstOrDefaultAsync(m => m.Awardable_ID == id);

        if (movie == null)
        {
            return NotFound();
        }

        // Remove related MovieGenres
        dbContext.MovieGenres.RemoveRange(movie.MovieGenres);
        
        // Remove the movie itself
        dbContext.Movies.Remove(movie);
        
        // Remove the Awardable entity
        var awardable = await dbContext.Awardables.FindAsync(movie.Awardable_ID);
        if (awardable != null)
        {
            dbContext.Awardables.Remove(awardable);
        }

        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Movie successfully deleted!";
        return RedirectToAction(nameof(List));
    }
    
}
