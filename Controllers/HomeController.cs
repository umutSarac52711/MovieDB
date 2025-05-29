using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieDB.Models;
using MovieDB.Data; // Added for MovieDbContext
using Microsoft.EntityFrameworkCore; // Added for Include, GroupBy, etc.
using System.Linq; // Added for LINQ methods
using System.Threading.Tasks; // Added for async operations

namespace MovieDB.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MovieDbContext _context; // Added DbContext

    public HomeController(ILogger<HomeController> logger, MovieDbContext context) // Injected DbContext
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index() // Made async
    {
        // Data for Movie per Genre chart
        var genreData = await _context.MovieGenres
            .Include(mg => mg.Genre)
            .Where(mg => mg.Genre != null) // Ensure Genre is not null
            .GroupBy(mg => mg.Genre.Name)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderBy(g => g.Name)
            .ToListAsync();
        
        ViewData["GenreChartLabels"] = genreData.Select(g => g.Name).ToList();
        ViewData["GenreChartData"] = genreData.Select(g => g.Count).ToList();

        // Data for Movie Revenue vs Budget chart
        // Scaling to millions for better readability on the chart if values are large
        var revenueBudgetData = await _context.Movies
            .Where(m => m.Budget.HasValue && m.Revenue.HasValue && m.Budget > 0 && m.Revenue > 0) 
            .Select(m => new { x = m.Budget.Value / 1000000.0m, y = m.Revenue.Value / 1000000.0m }) 
            .ToListAsync();
        ViewData["RevenueBudgetChartData"] = revenueBudgetData;
        
        // Data for Number of Awards by Candidate Type chart
        var awardsData = await _context.Awards
            .Include(a => a.Nominee) // Nominee is the Awardable entity
            .Where(a => a.Nominee != null) // Ensure Nominee is not null
            .GroupBy(a => a.Nominee.Kind) // Group by the 'Kind' property of Awardable
            .Select(g => new { Kind = g.Key, Count = g.Count() })
            .OrderBy(g => g.Kind)
            .ToListAsync();

        ViewData["AwardsChartLabels"] = awardsData.Select(a => a.Kind).ToList();
        ViewData["AwardsChartData"] = awardsData.Select(a => a.Count).ToList();

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
