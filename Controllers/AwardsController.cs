using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models; // Your main entities namespace
using MovieDB.Models.Entities; // Assuming Awardable is here

namespace MovieDB.Controllers
{
    public class AwardsController : Controller
    {
        private readonly MovieDbContext dbContext;

        public AwardsController(MovieDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Helper to get all Awardables (Movies, Actors, Directors) for Nominee dropdown
        private async Task<List<SelectListItem>> GetNomineeAwardablesSelectList()
        {
            var awardables = await dbContext.Awardables
                .Include(a => a.Movie)  // Eager load related entities for name display
                .Include(a => a.Actor)
                .Include(a => a.Director)
                .Select(a => new
                {
                    a.Awardable_ID,
                    Name = a.Kind == "Movie" ? (a.Movie != null ? a.Movie.Title : "N/A") :
                           (a.Kind == "Actor" ? (a.Actor != null ? a.Actor.Name : "N/A") :
                           (a.Kind == "Director" ? (a.Director != null ? a.Director.Name : "N/A") : "Unknown")),
                    Kind = a.Kind
                })
                .OrderBy(a => a.Name)
                .ToListAsync();

            return awardables.Select(a => new SelectListItem
            {
                Value = a.Awardable_ID.ToString(),
                Text = $"{a.Name} ({a.Kind})"
            }).ToList();
        }

        // Helper to get only Movies for Movie Context dropdown
        private async Task<List<SelectListItem>> GetMovieContextSelectList()
        {
            var movies = await dbContext.Movies // Assuming Movies table is directly accessible or via Awardables
                .Select(m => new SelectListItem
                {
                    Value = m.Awardable_ID.ToString(), // Assuming Movie.Awardable_ID is the PK
                    Text = m.Title
                })
                .OrderBy(m => m.Text)
                .ToListAsync();

            // Add an option for "None" or "Not Applicable"
            movies.Insert(0, new SelectListItem { Value = "", Text = "N/A (e.g., Best Picture)" });
            return movies;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var viewModel = new AwardViewModel
            {
                NomineeAwardables = await GetNomineeAwardablesSelectList(),
                MovieContexts = await GetMovieContextSelectList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AwardViewModel model)
        {
            // Server-side validation for nominee kind and movie context based on category
            if (model.Nominee_Awardable_ID > 0) // Check if a nominee is selected
            {
                var nominee = await dbContext.Awardables.FindAsync(model.Nominee_Awardable_ID);
                if (nominee == null)
                {
                    ModelState.AddModelError("Nominee_Awardable_ID", "Selected nominee not found.");
                }
                else
                {
                    // Example: "Best Actor", "Best Actress", "Best Director" typically require a movie context
                    if (model.Specific_Award_Category.Contains("Actor") || model.Specific_Award_Category.Contains("Director"))
                    {
                        if (nominee.Kind != "Actor" && nominee.Kind != "Director")
                        {
                            ModelState.AddModelError("Nominee_Awardable_ID", $"For '{model.Specific_Award_Category}', nominee must be an Actor or Director.");
                        }
                        if (!model.Movie_Context_ID.HasValue || model.Movie_Context_ID == 0)
                        {
                            ModelState.AddModelError("Movie_Context_ID", "Movie context is required for this award category.");
                        }
                        else if (model.Movie_Context_ID.HasValue)
                        {
                            var movieContext = await dbContext.Movies.FindAsync(model.Movie_Context_ID.Value); // Or Awardables if PK is there
                            if (movieContext == null) // Or check awardable.Kind == "Movie"
                            {
                                ModelState.AddModelError("Movie_Context_ID", "Selected movie context not found or is not a movie.");
                            }
                        }
                    }
                    // Example: "Best Picture", "Best Screenplay" - nominee is a Movie, context is null/N.A.
                    else if (model.Specific_Award_Category.Contains("Picture") || model.Specific_Award_Category.Contains("Screenplay") || model.Specific_Award_Category.Contains("Visual Effects"))
                    {
                        if (nominee.Kind != "Movie")
                        {
                            ModelState.AddModelError("Nominee_Awardable_ID", $"For '{model.Specific_Award_Category}', nominee must be a Movie.");
                        }
                        // Optionally enforce Movie_Context_ID to be null or match Nominee_Awardable_ID
                        if (model.Movie_Context_ID.HasValue && model.Movie_Context_ID != 0) {
                             // model.Movie_Context_ID = null; // Or add ModelState error if it should be empty
                             if(model.Movie_Context_ID != model.Nominee_Awardable_ID) { // If you allow it but must match
                                // ModelState.AddModelError("Movie_Context_ID", "Movie context should be N/A or match the nominated movie for this category.");
                             }
                             // For simplicity, if it's a movie award, we usually make context N/A
                             model.Movie_Context_ID = null;
                        }
                    }
                }
            }


            if (ModelState.IsValid)
            {
                var awardNomination = new Award // Your actual entity
                {
                    Award_Event_Name = model.Award_Event_Name,
                    Specific_Award_Category = model.Specific_Award_Category,
                    Award_Year = model.Award_Year,
                    Nominee_Awardable_ID = model.Nominee_Awardable_ID,
                    Movie_Context_ID = (model.Movie_Context_ID == 0 || !model.Movie_Context_ID.HasValue) ? null : model.Movie_Context_ID,
                    Nomination_Status = model.Nomination_Status
                };

                await dbContext.Awards.AddAsync(awardNomination);
                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Award nomination successfully added!";
                return RedirectToAction(nameof(List));
            }

            // If ModelState is invalid, re-populate dropdowns
            model.NomineeAwardables = await GetNomineeAwardablesSelectList();
            model.MovieContexts = await GetMovieContextSelectList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var nominations = await dbContext.Awards
                .Include(a => a.Nominee) // The Awardable entity that was nominated
                    .ThenInclude(n => n.Movie) // If nominee is a Movie
                .Include(a => a.Nominee)
                    .ThenInclude(n => n.Actor) // If nominee is an Actor
                .Include(a => a.Nominee)
                    .ThenInclude(n => n.Director) // If nominee is a Director
                .Include(a => a.MovieContext) // The Movie entity for context (if any)
                .OrderByDescending(a => a.Award_Year)
                .ThenBy(a => a.Award_Event_Name)
                .ThenBy(a => a.Specific_Award_Category)
                .ToListAsync();

            return View(nominations); // Pass List<Award>
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var nomination = await dbContext.Awards.FindAsync(id);

            if (nomination == null)
            {
                return NotFound();
            }

            var viewModel = new AwardViewModel
            {
                Award_ID = nomination.Award_ID,
                Award_Event_Name = nomination.Award_Event_Name,
                Specific_Award_Category = nomination.Specific_Award_Category,
                Award_Year = nomination.Award_Year,
                Nominee_Awardable_ID = nomination.Nominee_Awardable_ID,
                Movie_Context_ID = nomination.Movie_Context_ID,
                Nomination_Status = nomination.Nomination_Status,
                NomineeAwardables = await GetNomineeAwardablesSelectList(),
                MovieContexts = await GetMovieContextSelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AwardViewModel model)
        {
            // Apply the same server-side validation as in Add
            if (model.Nominee_Awardable_ID > 0)
            {
                var nominee = await dbContext.Awardables.FindAsync(model.Nominee_Awardable_ID);
                if (nominee == null) { ModelState.AddModelError("Nominee_Awardable_ID", "Selected nominee not found."); }
                else
                {
                    if (model.Specific_Award_Category.Contains("Actor") || model.Specific_Award_Category.Contains("Director"))
                    {
                        if (nominee.Kind != "Actor" && nominee.Kind != "Director") { ModelState.AddModelError("Nominee_Awardable_ID", "Nominee must be an Actor or Director for this category."); }
                        if (!model.Movie_Context_ID.HasValue || model.Movie_Context_ID == 0) { ModelState.AddModelError("Movie_Context_ID", "Movie context is required."); }
                        else if (model.Movie_Context_ID.HasValue)
                        {
                             var movieContextAwardable = await dbContext.Awardables.FindAsync(model.Movie_Context_ID.Value);
                             if (movieContextAwardable == null || movieContextAwardable.Kind != "Movie") {
                                 ModelState.AddModelError("Movie_Context_ID", "Selected movie context not found or is not a movie.");
                             }
                        }
                    }
                    else if (model.Specific_Award_Category.Contains("Picture") || model.Specific_Award_Category.Contains("Screenplay") || model.Specific_Award_Category.Contains("Visual Effects"))
                    {
                        if (nominee.Kind != "Movie") { ModelState.AddModelError("Nominee_Awardable_ID", "Nominee must be a Movie for this category."); }
                        if (model.Movie_Context_ID.HasValue && model.Movie_Context_ID != 0) { model.Movie_Context_ID = null; } // Enforce null context
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var nomination = await dbContext.Awards.FindAsync(model.Award_ID);
                if (nomination == null)
                {
                    return NotFound();
                }

                nomination.Award_Event_Name = model.Award_Event_Name;
                nomination.Specific_Award_Category = model.Specific_Award_Category;
                nomination.Award_Year = model.Award_Year;
                nomination.Nominee_Awardable_ID = model.Nominee_Awardable_ID;
                nomination.Movie_Context_ID = (model.Movie_Context_ID == 0 || !model.Movie_Context_ID.HasValue) ? null : model.Movie_Context_ID;
                nomination.Nomination_Status = model.Nomination_Status;

                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Award nomination successfully updated!";
                return RedirectToAction(nameof(List));
            }

            model.NomineeAwardables = await GetNomineeAwardablesSelectList();
            model.MovieContexts = await GetMovieContextSelectList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var nomination = await dbContext.Awards
                .Include(a => a.Nominee)
                    .ThenInclude(n => n.Movie)
                .Include(a => a.Nominee)
                    .ThenInclude(n => n.Actor)
                .Include(a => a.Nominee)
                    .ThenInclude(n => n.Director)
                .Include(a => a.MovieContext)
                .FirstOrDefaultAsync(a => a.Award_ID == id);

            if (nomination == null)
            {
                return NotFound();
            }
            return View(nomination); // Pass the Award entity to the view for confirmation
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nomination = await dbContext.Awards.FindAsync(id);
            if (nomination == null)
            {
                return NotFound();
            }

            dbContext.Awards.Remove(nomination);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Award nomination successfully deleted!";
            return RedirectToAction(nameof(List));
        }
    }
}