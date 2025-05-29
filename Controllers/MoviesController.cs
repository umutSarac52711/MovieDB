using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList

namespace MovieDB.Controllers;

public class MoviesController : Controller
{
    private readonly MovieDbContext dbContext;
    public MoviesController(MovieDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    private async Task PopulateDropdownsAsync(MovieViewModel model = null)
    {
        ViewBag.Genres = new SelectList(await dbContext.Genres.OrderBy(g => g.Name).ToListAsync(), "Genre_ID", "Name", model?.Genres);
        ViewBag.ActorsList = new SelectList(await dbContext.Actors.OrderBy(a => a.Name).ToListAsync(), "Awardable_ID", "Name", model?.SelectedActorIds);
        ViewBag.DirectorsList = new SelectList(await dbContext.Directors.OrderBy(d => d.Name).ToListAsync(), "Awardable_ID", "Name", model?.SelectedDirectorIds); 
        ViewBag.CompaniesList = new SelectList(await dbContext.Companies.OrderBy(c => c.Name).ToListAsync(), "Company_ID", "Name", model?.SelectedCompanyId); // Corrected to ProductionCompanies
    }
    
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        await PopulateDropdownsAsync();
        return View(new MovieViewModel()); // Pass an empty model
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(MovieViewModel model)
    {
        if (ModelState.IsValid)
        {
            var awardableEntity = new Awardable { Kind = "Movie" };
            dbContext.Awardables.Add(awardableEntity);
            await dbContext.SaveChangesAsync(); // Save to get Awardable_ID

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
                MovieGenres = new List<MovieGenre>(),
                MovieActors = new List<MovieActor>(),
                MovieDirectors = new List<MovieDirector>(),
                MovieCompanies = new List<MovieCompany>()
            };

            // Add selected genres
            if (model.Genres != null)
            {
                foreach (var genreId in model.Genres)
                {
                    movie.MovieGenres.Add(new MovieGenre { Genre_ID = genreId });
                }
            }

            // Add selected actors
            if (model.SelectedActorIds != null)
            {
                foreach (var actorId in model.SelectedActorIds)
                {
                    movie.MovieActors.Add(new MovieActor { Actor_ID = actorId });
                }
            }
            
            // Add selected directors
            if (model.SelectedDirectorIds != null)
            {
                foreach (var directorId in model.SelectedDirectorIds)
                {
                    movie.MovieDirectors.Add(new MovieDirector { Director_ID = directorId });
                }
            }
            
            // Add selected company
            if (model.SelectedCompanyId != null)
            {
                movie.MovieCompanies.Add(new MovieCompany { Company_ID = model.SelectedCompanyId.Value });
            }
            
            await dbContext.Movies.AddAsync(movie);
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = "Movie successfully added!";
            return RedirectToAction(nameof(List));
        }
        await PopulateDropdownsAsync(model); // Repopulate dropdowns if model state is invalid
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var movies = await dbContext.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .Include(m => m.MovieDirectors).ThenInclude(md => md.Director)
            .Include(m => m.MovieCompanies).ThenInclude(mc => mc.Company) // For company name
            .OrderBy(m => m.Title)
            .ToListAsync();
        return View(movies);
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var movie = await dbContext.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre) 
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .Include(m => m.MovieDirectors).ThenInclude(md => md.Director)
            .Include(m => m.MovieCompanies).ThenInclude(mc => mc.Company)
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
            Genres = movie.MovieGenres.Select(mg => mg.Genre_ID).ToList(),
            SelectedActorIds = movie.MovieActors.Select(ma => ma.Actor_ID).ToList(),
            SelectedDirectorIds = movie.MovieDirectors.Select(md => md.Director_ID).ToList(),
            SelectedCompanyId = movie.MovieCompanies.FirstOrDefault()?.Company_ID 
        };
        
        // For display purposes in Details view, pass names via ViewBag or extend ViewModel
        ViewBag.GenreNames = movie.MovieGenres.Select(mg => mg.Genre?.Name).ToList();
        ViewBag.ActorNames = movie.MovieActors.Select(ma => ma.Actor?.Name).ToList();
        ViewBag.DirectorName = movie.MovieDirectors.FirstOrDefault()?.Director?.Name;
        ViewBag.CompanyName = movie.MovieCompanies.FirstOrDefault()?.Company?.Name;

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await dbContext.Movies
            .Include(m => m.MovieGenres)
            .Include(m => m.MovieActors)
            .Include(m => m.MovieDirectors)
            .Include(m => m.MovieCompanies)
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
            Genres = movie.MovieGenres.Select(mg => mg.Genre_ID).ToList(),
            SelectedActorIds = movie.MovieActors.Select(ma => ma.Actor_ID).ToList(),
            SelectedDirectorIds = movie.MovieDirectors.Select(md => md.Director_ID).ToList(),
            SelectedCompanyId = movie.MovieCompanies.FirstOrDefault()?.Company_ID 
        };

        await PopulateDropdownsAsync(viewModel);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MovieViewModel model)
    {
        if (ModelState.IsValid)
        {
            var movie = await dbContext.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MovieActors)
                .Include(m => m.MovieDirectors)
                .Include(m => m.MovieCompanies)
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
            
            // Update Genres
            movie.MovieGenres.Clear();
            if (model.Genres != null)
            {
                foreach (var genreId in model.Genres)
                {
                    movie.MovieGenres.Add(new MovieGenre { Movie_ID = movie.Awardable_ID, Genre_ID = genreId });
                }
            }

            // Update Actors
            movie.MovieActors.Clear();
            if (model.SelectedActorIds != null)
            {
                foreach (var actorId in model.SelectedActorIds)
                {
                    movie.MovieActors.Add(new MovieActor { Movie_ID = movie.Awardable_ID, Actor_ID = actorId });
                }
            }
            
            // Update Directors
            movie.MovieDirectors.Clear();
            if (model.SelectedDirectorIds != null)
            {
                foreach (var directorId in model.SelectedDirectorIds)
                {
                    movie.MovieDirectors.Add(new MovieDirector { Movie_ID = movie.Awardable_ID, Director_ID = directorId });
                }
            }

            // Update Company
            movie.MovieCompanies.Clear();
            if (model.SelectedCompanyId != null)
            {
                movie.MovieCompanies.Add(new MovieCompany { Movie_ID = movie.Awardable_ID, Company_ID = model.SelectedCompanyId.Value });
            }
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Movie successfully updated!";
            return RedirectToAction(nameof(List));
        }
        await PopulateDropdownsAsync(model); // Repopulate dropdowns if model state is invalid
        return View(model);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await dbContext.Movies
            .Include(m => m.MovieGenres)
            .Include(m => m.MovieActors)
            .Include(m => m.MovieDirectors)
            .Include(m => m.MovieCompanies)
            .FirstOrDefaultAsync(m => m.Awardable_ID == id);

        if (movie == null)
        {
            return NotFound();
        }

        // Explicitly remove related entities from join tables
        // EF Core might handle this with cascade delete if configured, but explicit is safer.
        dbContext.MovieGenres.RemoveRange(movie.MovieGenres);
        dbContext.MovieActors.RemoveRange(movie.MovieActors);
        dbContext.MovieDirectors.RemoveRange(movie.MovieDirectors);
        dbContext.MovieCompanies.RemoveRange(movie.MovieCompanies);
        
        dbContext.Movies.Remove(movie);
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
