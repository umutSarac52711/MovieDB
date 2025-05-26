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
            .ToListAsync();

        var selectList = awardables.Select(a => new SelectListItem
        {
            Value = a.Awardable_ID.ToString(),
            Text = a.Name
        }).ToList();

        selectList.Insert(0, new SelectListItem { Value = "", Text = "None (General Award)" });
        return selectList;
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var viewModel = new AwardViewModel
        {
            Awardables = await GetAwardablesSelectList()
            
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
                Name = model.Name,
                Award_Year = model.Year,
                Category = model.Category,
                Awardable_ID = model.Awardable_ID // Handle 0 as null
            };

            await dbContext.Awards.AddAsync(award);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Award successfully added!";
            return RedirectToAction(nameof(List));
        }
        model.Awardables = await GetAwardablesSelectList();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var awards = await dbContext.Awards
            .Include(a => a.Awardable)
                .ThenInclude(aw => aw.Movie) // Include Movie for Awardable
            .Include(a => a.Awardable)
                .ThenInclude(aw => aw.Actor) // Include Actor for Awardable
            .Include(a => a.Awardable)
                .ThenInclude(aw => aw.Director) // Include Director for Awardable
            .ToListAsync();

        return View(awards);
    }
    
    // Details action can be added if needed, similar to Edit (GET)

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var award = await dbContext.Awards.FindAsync(id);

        if (award == null)
        {
            return NotFound();
        }

        var viewModel = new AwardViewModel
        {
            Award_ID = award.Award_ID,
            Name = award.Name,
            Year = award.Award_Year,
            Category = award.Category,
            Awardable_ID = award.Awardable_ID,
            Awardables = await GetAwardablesSelectList()
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

            award.Name = model.Name;
            award.Award_Year = model.Year;
            award.Category = model.Category;
            award.Awardable_ID = model.Awardable_ID;
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Award successfully updated!";
            return RedirectToAction(nameof(List));
        }
        model.Awardables = await GetAwardablesSelectList();
        return View(model);
    }
    
    // Delete GET can remain simple, just fetching the award by ID to show a confirmation
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var award = await dbContext.Awards
            .Include(a => a.Awardable)
                .ThenInclude(aw => aw.Movie)
            .Include(a => a.Awardable)
                .ThenInclude(aw => aw.Actor)
            .Include(a => a.Awardable)
                .ThenInclude(aw => aw.Director)
            .FirstOrDefaultAsync(a => a.Award_ID == id);

        if (award == null)
        {
            return NotFound();
        }
        // For display purposes in the delete confirmation view
        var awardableName = "N/A";
        if (award.Awardable != null)
        {
            awardableName = award.Awardable.Kind switch
            {
                "Movie" => award.Awardable.Movie?.Title,
                "Actor" => award.Awardable.Actor?.Name,
                "Director" => award.Awardable.Director?.Name,
                _ => "Unknown Awardable"
            };
        }
        ViewBag.AwardableName = awardableName;


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
