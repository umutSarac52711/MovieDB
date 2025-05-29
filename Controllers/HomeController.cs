using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieDB.Models;
using MovieDB.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MovieDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, MovieDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
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
    
    // Methods to fetch data for charts
    [HttpGet]
    public async Task<IActionResult> GetMoviesPerGenreData()
    {
        var genreData = await _dbContext.MovieGenres
            .GroupBy(mg => mg.Genre)
            .Select(g => new 
            {
                Genre = g.Key,
                Count = g.Count()
            })
            .OrderBy(g => g.Genre)
            .ToListAsync();
            
        return Json(new 
        {
            labels = genreData.Select(g => g.Genre).ToArray(),
            data = genreData.Select(g => g.Count).ToArray()
        });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMovieRevenueBudgetData()
    {
        var movieData = await _dbContext.Movies
            .Where(m => m.Budget.HasValue && m.Revenue.HasValue) // Assuming these properties exist
            .Select(m => new 
            { 
                x = m.Budget.Value, 
                y = m.Revenue.Value 
            })
            .ToListAsync();
            
        return Json(movieData);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAwardsByTypeData()
    {
        // Count awards grouped by the type of recipient (Movie, Actor, Director)
        var movieAwards = await _dbContext.Awards
            .Where(a => a.Nominee.Movie != null)
            .CountAsync();
            
        var actorAwards = await _dbContext.Awards
            .Where(a => a.Nominee.Actor != null)
            .CountAsync();
            
        var directorAwards = await _dbContext.Awards
            .Where(a => a.Nominee.Director != null)
            .CountAsync();
            
        var labels = new[] { "Movies", "Actors", "Directors" };
        var data = new[] { movieAwards, actorAwards, directorAwards };
        
        return Json(new { labels, data });
    }
}
