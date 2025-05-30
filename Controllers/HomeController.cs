using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data; // Your DbContext namespace
using MovieDB.Models.Entities; // Your Entities namespace
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // For Dictionary
using MovieDB.Models; // For ErrorViewModel if you have it

namespace MovieDB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieDbContext _dbContext; // Inject your DbContext

        public HomeController(ILogger<HomeController> logger, MovieDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Movies per Genre Data
            var moviesPerGenre = await _dbContext.MovieGenres
                .Include(mg => mg.Genre)
                .GroupBy(mg => mg.Genre)
                .Select(g => new { Genre = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10) // Take top 10 genres or adjust as needed
                .ToListAsync();

            ViewBag.GenreLabels = moviesPerGenre.Select(x => x.Genre.Name).ToList();
            ViewBag.GenreCounts = moviesPerGenre.Select(x => x.Count).ToList();

            // 2. Movie Revenue vs Budget Data
            var revenueBudget = await _dbContext.Movies
                .Where(m => m.Budget.HasValue && m.Revenue.HasValue && m.Budget > 0 && m.Revenue > 0) // Ensure data exists
                .Select(m => new {
                    Title = m.Title, // For tooltips or labels if needed
                    Budget = m.Budget.Value / 1000000, // Assuming in millions
                    Revenue = m.Revenue.Value / 1000000 // Assuming in millions
                })
                .Take(50) // Limit the number of points for performance/clarity
                .ToListAsync();

            ViewBag.RevenueBudgetData = revenueBudget.Select(rb => new { x = rb.Budget, y = rb.Revenue }).ToList();
            // For richer tooltips, you could pass the full title, budget, revenue
            ViewBag.RevenueBudgetFullData = revenueBudget.Select(rb => new { title = rb.Title, budget = rb.Budget, revenue = rb.Revenue }).ToList();


            // 3. Number of Awards by Candidate Type (using the new Nomination structure)
            // This counts unique WINNERS for simplicity, or you can count all nominations.
            // Let's count unique winners.
            var awardsByCandidateType = await _dbContext.Awards // This is your nominations table
                .Where(a => a.Nomination_Status == "Winner") // Only count wins
                .Include(a => a.Nominee) // The Awardable entity
                .GroupBy(a => a.Nominee.Kind) // Group by the Kind of the Awardable (Movie, Actor, Director)
                .Select(g => new { Kind = g.Key, Count = g.Count() }) // Count distinct nominations per kind
                .ToListAsync();

            // Prepare data for the chart ensuring all expected types are present, even if count is 0
            var candidateTypes = new List<string> { "Movie", "Actor", "Director" };
            var awardsData = new Dictionary<string, int>();
            foreach (var type in candidateTypes)
            {
                awardsData[type] = 0; // Initialize with 0
            }
            foreach (var item in awardsByCandidateType)
            {
                if (awardsData.ContainsKey(item.Kind))
                {
                    awardsData[item.Kind] = item.Count;
                }
            }

            ViewBag.AwardTypeLabels = awardsData.Keys.ToList();
            ViewBag.AwardTypeCounts = awardsData.Values.ToList();


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}