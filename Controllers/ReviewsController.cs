using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;

namespace MovieDB.Controllers;

public class ReviewsController : Controller
{
    private readonly MovieDbContext dbContext;
    public ReviewsController(MovieDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    private async Task PopulateMoviesDropDownList(object? selectedMovie = null)
    {
        var moviesQuery = from m in dbContext.Movies
                          orderby m.Title
                          select m;
        ViewBag.Movies = new SelectList(await moviesQuery.AsNoTracking().ToListAsync(), "Awardable_ID", "Title", selectedMovie);
    }
    
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var viewModel = new ReviewViewModel();
        await PopulateMoviesDropDownList();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(ReviewViewModel model)
    {
        if (ModelState.IsValid)
        {
            var movie = await dbContext.Movies.FindAsync(model.Movie_ID);
            if (movie == null)
            {
                ModelState.AddModelError("Movie_ID", "Selected movie not found.");
                await PopulateMoviesDropDownList(model.Movie_ID);
                return View(model);
            }

            var review = new Review
            {
                Movie_ID = model.Movie_ID,
                Reviewer = model.Reviewer,
                Rating = model.Rating,
                Comment_Text = model.Comment_Text,
                Date_Posted = DateTime.UtcNow 
            };
            
            await dbContext.Reviews.AddAsync(review);
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = $"Review for '{movie.Title}' successfully added!";
            return RedirectToAction(nameof(List));
        }
        await PopulateMoviesDropDownList(model.Movie_ID);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var reviews = await dbContext.Reviews
            .Include(r => r.Movie) // Include Movie to access its Title
            .Select(r => new ReviewViewModel
            {
                Review_ID = r.Review_ID,
                Movie_ID = r.Movie_ID,
                MovieTitle = r.Movie.Title, // Get movie title
                Reviewer = r.Reviewer,
                Rating = r.Rating,
                Comment_Text = r.Comment_Text,
                Date_Posted = r.Date_Posted
            })
            .ToListAsync();

        return View(reviews);
    }
    
    // Details method can be added if needed, similar to Edit (GET) but read-only
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var review = await dbContext.Reviews
            .Include(r => r.Movie)
            .FirstOrDefaultAsync(r => r.Review_ID == id);

        if (review == null)
        {
            return NotFound();
        }

        var viewModel = new ReviewViewModel
        {
            Review_ID = review.Review_ID,
            Movie_ID = review.Movie_ID,
            Reviewer = review.Reviewer,
            Rating = review.Rating,
            Comment_Text = review.Comment_Text,
            Date_Posted = review.Date_Posted
        };
        await PopulateMoviesDropDownList(review.Movie_ID);
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ReviewViewModel model) 
    {
        if (ModelState.IsValid)
        {
            var review = await dbContext.Reviews
                .FirstOrDefaultAsync(r => r.Review_ID == model.Review_ID);

            if (review == null)
            {
                return NotFound();
            }

            var movie = await dbContext.Movies.FindAsync(model.Movie_ID);
            if (movie == null)
            {
                ModelState.AddModelError("Movie_ID", "Selected movie not found.");
                await PopulateMoviesDropDownList(model.Movie_ID);
                return View(model);
            }

            review.Movie_ID = model.Movie_ID;
            review.Reviewer = model.Reviewer;
            review.Rating = model.Rating;
            review.Comment_Text = model.Comment_Text;
            // Date_Posted is generally not updated, or handled differently if needed
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Review for '{movie.Title}' successfully updated!";
            return RedirectToAction(nameof(List));
        }
        await PopulateMoviesDropDownList(model.Movie_ID);
        return View(model);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken] // Added for security
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var review = await dbContext.Reviews.FindAsync(id);

        if (review == null)
        {
            return NotFound();
        }
        
        dbContext.Reviews.Remove(review);
        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Review successfully deleted!";
        return RedirectToAction(nameof(List));
    }
}
