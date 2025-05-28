using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers;

public class FeaturesController : Controller // Renamed from FeaturesRelationshipController
{
    private readonly MovieDbContext dbContext;
    public FeaturesController(MovieDbContext dbContext) // Constructor name updated
    {
        this.dbContext = dbContext;
    }

    private async Task PopulateDropdowns(FeaturesViewModel? model = null)
    {
        var movies = await dbContext.Movies
            .OrderBy(m => m.Title)
            .Select(m => new SelectListItem { Value = m.Awardable_ID.ToString(), Text = m.Title })
            .ToListAsync();

        var directors = await dbContext.Directors
            .OrderBy(d => d.Name)
            .Select(d => new SelectListItem { Value = d.Awardable_ID.ToString(), Text = d.Name })
            .ToListAsync();

        var actors = await dbContext.Actors
            .OrderBy(a => a.Name)
            .Select(a => new SelectListItem { Value = a.Awardable_ID.ToString(), Text = a.Name })
            .ToListAsync();

        if (model != null)
        {
            model.Movies = movies;
            model.Directors = directors;
            model.Actors = actors;
        }
        else
        {
            ViewBag.Movies = movies;
            ViewBag.Directors = directors;
            ViewBag.Actors = actors;
        }
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var featuresList = await dbContext.Features
            .Include(f => f.Movie)
            .Include(f => f.Director)
            .Include(f => f.Actor)
            .Select(f => new FeaturesViewModel
            {
                Movie_ID = f.Movie_ID,
                MovieTitle = f.Movie.Title,
                Director_ID = f.Director_ID,
                DirectorName = f.Director.Name,
                Actor_ID = f.Actor_ID,
                ActorName = f.Actor.Name
            })
            .OrderBy(f => f.MovieTitle).ThenBy(f => f.DirectorName).ThenBy(f => f.ActorName)
            .ToListAsync();

        return View(featuresList);
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var viewModel = new FeaturesViewModel();
        await PopulateDropdowns(viewModel);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(FeaturesViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingFeature = await dbContext.Features
                .FirstOrDefaultAsync(f => f.Movie_ID == model.Movie_ID &&
                                          f.Director_ID == model.Director_ID &&
                                          f.Actor_ID == model.Actor_ID);
            if (existingFeature != null)
            {
                ModelState.AddModelError(string.Empty, "This feature relationship already exists.");
                await PopulateDropdowns(model);
                return View(model);
            }

            var feature = new Features
            {
                Movie_ID = model.Movie_ID,
                Director_ID = model.Director_ID,
                Actor_ID = model.Actor_ID
            };

            dbContext.Features.Add(feature);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Feature relationship successfully added!";
            return RedirectToAction(nameof(List));
        }
        await PopulateDropdowns(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int movieId, int directorId, int actorId)
    {
        var feature = await dbContext.Features
            .FirstOrDefaultAsync(f => f.Movie_ID == movieId &&
                                      f.Director_ID == directorId &&
                                      f.Actor_ID == actorId);

        if (feature == null)
        {
            return NotFound();
        }

        var viewModel = new FeaturesViewModel
        {
            Movie_ID = feature.Movie_ID,
            Director_ID = feature.Director_ID,
            Actor_ID = feature.Actor_ID,
            Original_Movie_ID = feature.Movie_ID,
            Original_Director_ID = feature.Director_ID,
            Original_Actor_ID = feature.Actor_ID
        };

        await PopulateDropdowns(viewModel);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FeaturesViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Check if the new combination is different from the original
            bool isNewCombination = model.Movie_ID != model.Original_Movie_ID ||
                                    model.Director_ID != model.Original_Director_ID ||
                                    model.Actor_ID != model.Original_Actor_ID;

            if (isNewCombination)
            {
                // Check if the new combination already exists
                var newCombinationExists = await dbContext.Features
                    .AnyAsync(f => f.Movie_ID == model.Movie_ID &&
                                   f.Director_ID == model.Director_ID &&
                                   f.Actor_ID == model.Actor_ID);
                if (newCombinationExists)
                {
                    ModelState.AddModelError(string.Empty, "This feature relationship (the new combination) already exists.");
                    await PopulateDropdowns(model);
                    return View(model);
                }
            }

            // Find the original feature to delete it
            var originalFeature = await dbContext.Features
                .FirstOrDefaultAsync(f => f.Movie_ID == model.Original_Movie_ID &&
                                          f.Director_ID == model.Original_Director_ID &&
                                          f.Actor_ID == model.Original_Actor_ID);

            if (originalFeature == null)
            {
                // This case should ideally not happen if the form is correctly submitted from Edit GET
                TempData["ErrorMessage"] = "Original feature relationship not found. Edit failed.";
                return RedirectToAction(nameof(List));
            }
            
            dbContext.Features.Remove(originalFeature);

            // Add the new or updated feature
            var newFeature = new Features
            {
                Movie_ID = model.Movie_ID,
                Director_ID = model.Director_ID,
                Actor_ID = model.Actor_ID
            };
            dbContext.Features.Add(newFeature);
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Feature relationship successfully updated!";
            return RedirectToAction(nameof(List));
        }
        await PopulateDropdowns(model);
        return View(model);
    }
    
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int movieId, int directorId, int actorId)
    {
        var feature = await dbContext.Features
            .FirstOrDefaultAsync(f => f.Movie_ID == movieId &&
                                      f.Director_ID == directorId &&
                                      f.Actor_ID == actorId);

        if (feature == null)
        {
            TempData["ErrorMessage"] = "Feature relationship not found.";
            return RedirectToAction(nameof(List));
        }

        dbContext.Features.Remove(feature);
        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Feature relationship successfully deleted!";
        return RedirectToAction(nameof(List));
    }
}
