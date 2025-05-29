using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers;

public class AwardsController : Controller // Renamed from ActorsController
{
    private readonly MovieDbContext dbContext;
    public AwardsController(MovieDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    private async Task<List<SelectListItem>> GetAwardablesSelectList()
    {
        var awardables = await dbContext.Awardables
            .Select(a => new
            {
                a.Awardable_ID,
                Name = a.Kind == "Movie" ? a.Movie.Title : (a.Kind == "Actor" ? a.Actor.Name : (a.Kind == "Director" ? a.Director.Name : "Unknown"))
            })
            .OrderBy(a => a.Name)
            .ToListAsync();

        var selectList = awardables.Select(a => new SelectListItem
        {
            Value = a.Awardable_ID.ToString(),
            Text = a.Name
        }).ToList();
        // Nominee is required, so no "None" option. A default selection prompt can be added in the view.
        return selectList;
    }

    private async Task<List<SelectListItem>> GetMoviesSelectList()
    {
        var movies = await dbContext.Movies
            .OrderBy(m => m.Title)
            .Select(m => new SelectListItem
            {
                Value = m.Awardable_ID.ToString(), // Movie's PK is Awardable_ID
                Text = m.Title
            })
            .ToListAsync();

        movies.Insert(0, new SelectListItem { Value = "", Text = "N/A (e.g., Best Picture Award)" });
        return movies;
    }

    private List<SelectListItem> GetNominationStatusSelectList()
    {
        return new List<SelectListItem>
        {
            new SelectListItem { Value = "Nominated", Text = "Nominated" },
            new SelectListItem { Value = "Winner", Text = "Winner" }
        };
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var viewModel = new AwardViewModel
        {
            AwardablesList = new SelectList(await GetAwardablesSelectList(), "Value", "Text"),
            MoviesList = new SelectList(await GetMoviesSelectList(), "Value", "Text"),
            NominationStatusList = new SelectList(GetNominationStatusSelectList(), "Value", "Text")
            
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AwardViewModel model)
    {
        if (ModelState.IsValid)
        {
            var award = new Award
            {
                Event_Name = model.Name, // Maps from model.Name
                Award_Year = model.Year,
                Category = model.Category,
                Nominee_Awardable_ID = model.Nominee_Awardable_ID,
                Movie_Context_ID = model.Movie_Context_ID == 0 ? null : model.Movie_Context_ID, // Handle 0 from select list as null
                Nomination_Status = model.Nomination_Status
            };

            await dbContext.Awards.AddAsync(award);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Award successfully added!";
            return RedirectToAction(nameof(List));
        }
        model.AwardablesList = new SelectList(await GetAwardablesSelectList(), "Value", "Text", model.Nominee_Awardable_ID);
        model.MoviesList = new SelectList(await GetMoviesSelectList(), "Value", "Text", model.Movie_Context_ID);
        model.NominationStatusList = new SelectList(GetNominationStatusSelectList(), "Value", "Text", model.Nomination_Status);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var awards = await dbContext.Awards
            .Include(a => a.Nominee) // Eager load the Nominee (Awardable)
                .ThenInclude(n => n.Movie) // If Nominee is a Movie
            .Include(a => a.Nominee)
                .ThenInclude(n => n.Actor) // If Nominee is an Actor
            .Include(a => a.Nominee)
                .ThenInclude(n => n.Director) // If Nominee is a Director
            .Include(a => a.MovieContext) // Eager load the MovieContext
            .OrderBy(a => a.Event_Name).ThenBy(a => a.Award_Year).ThenBy(a => a.Category)
            .ToListAsync();

        return View(awards);
    }
    
    // Details action can be added if needed, similar to Edit (GET)

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        // Replace FindAsync with FirstOrDefaultAsync to include needed related entities
        var award = await dbContext.Awards
            .Include(a => a.Nominee)
            .Include(a => a.MovieContext)
            .FirstOrDefaultAsync(a => a.Award_ID == id);

        if (award == null)
        {
            return NotFound();
        }

        var viewModel = new AwardViewModel
        {
            Award_ID = award.Award_ID,
            Name = award.Event_Name,
            Year = award.Award_Year,
            Category = award.Category,
            Nominee_Awardable_ID = award.Nominee_Awardable_ID,
            Movie_Context_ID = award.Movie_Context_ID,
            Nomination_Status = award.Nomination_Status,
            AwardablesList = new SelectList(await GetAwardablesSelectList(), "Value", "Text", award.Nominee_Awardable_ID),
            MoviesList = new SelectList(await GetMoviesSelectList(), "Value", "Text", award.Movie_Context_ID),
            NominationStatusList = new SelectList(GetNominationStatusSelectList(), "Value", "Text", award.Nomination_Status)
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(AwardViewModel model)
    {
        if (ModelState.IsValid)
        {
            var award = await dbContext.Awards.FindAsync(model.Award_ID);

            if (award == null)
            {
                return NotFound();
            }

            award.Event_Name = model.Name;
            award.Award_Year = model.Year;
            award.Category = model.Category;
            award.Nominee_Awardable_ID = model.Nominee_Awardable_ID;
            award.Movie_Context_ID = model.Movie_Context_ID == 0 ? null : model.Movie_Context_ID;
            award.Nomination_Status = model.Nomination_Status;
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Award successfully updated!";
            return RedirectToAction(nameof(List));
        }
        model.AwardablesList = new SelectList(await GetAwardablesSelectList(), "Value", "Text", model.Nominee_Awardable_ID);
        model.MoviesList = new SelectList(await GetMoviesSelectList(), "Value", "Text", model.Movie_Context_ID);
        model.NominationStatusList = new SelectList(GetNominationStatusSelectList(), "Value", "Text", model.Nomination_Status);
        return View(model);
    }
    
    // Delete GET can remain simple, just fetching the award by ID to show a confirmation
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var award = await dbContext.Awards
            .Include(a => a.Nominee)
                .ThenInclude(n => n.Movie)
            .Include(a => a.Nominee)
                .ThenInclude(n => n.Actor)
            .Include(a => a.Nominee)
                .ThenInclude(n => n.Director)
            .Include(a => a.MovieContext) // Include MovieContext
            .FirstOrDefaultAsync(a => a.Award_ID == id);

        if (award == null)
        {
            return NotFound();
        }
        // For display purposes in the delete confirmation view
        var nomineeName = "N/A";
        if (award.Nominee != null)
        {
            nomineeName = award.Nominee.Kind switch
            {
                "Movie" => award.Nominee.Movie?.Title,
                "Actor" => award.Nominee.Actor?.Name,
                "Director" => award.Nominee.Director?.Name,
                _ => "Unknown Nominee"
            };
        }
        ViewBag.NomineeName = nomineeName;
        ViewBag.MovieContextTitle = award.MovieContext?.Title ?? "N/A";


        return View(award); // Pass the Award entity to the view
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var award = await dbContext.Awards.FindAsync(id);

        if (award == null)
        {
            return NotFound();
        }
        
        dbContext.Awards.Remove(award);
        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Award successfully deleted!";
        return RedirectToAction(nameof(List));
    }
}
