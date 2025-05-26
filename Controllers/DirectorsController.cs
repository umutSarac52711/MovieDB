using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;

namespace MovieDB.Controllers;

public class DirectorsController : Controller // Renamed from ActorsController
{
    private readonly MovieDbContext dbContext;
    public DirectorsController(MovieDbContext dbContext) // Constructor name updated
    {
        this.dbContext = dbContext;
    }
    
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(DirectorViewModel model) // Parameter changed to DirectorViewModel
    {
        if (ModelState.IsValid)
        {
            // Create a new Awardable entity
            var awardableEntity = new Awardable();
            awardableEntity.Kind = "Director"; // Kind "Director" olarak ayarlandı

            dbContext.Awardables.Add(awardableEntity);
            await dbContext.SaveChangesAsync();

            // Now create the Director entity with the Awardable_ID from the newly created Awardable
            
            var director = new Director // Director oluşturuldu
                {
                    Awardable_ID = awardableEntity.Awardable_ID,
                    Awardable = awardableEntity,
                    Name = model.Name, // DirectorViewModel'den özellikler eşlendi
                    Birth_Date = model.Birth_Date,
                    Nationality = model.Nationality
                };
            
            
            await dbContext.Directors.AddAsync(director); // dbContext.Directors'a eklendi
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = "Director successfully added!"; // Mesaj güncellendi
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var directors = await dbContext.Directors.ToListAsync(); // dbContext.Directors'tan çekildi

        return View(directors);

    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var director = await dbContext.Directors // dbContext.Directors'tan çekildi
            .FirstOrDefaultAsync(d => d.Awardable_ID == id);

        if (director == null)
        {
            return NotFound();
        }

        var model = new DirectorViewModel // DirectorViewModel oluşturuldu
        {
            Awardable_ID = director.Awardable_ID,
            Name = director.Name, // Director özelliklerinden eşlendi
            Birth_Date = director.Birth_Date,
            Nationality = director.Nationality
        };

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var director = await dbContext.Directors // dbContext.Directors'tan çekildi
            .FirstOrDefaultAsync(d => d.Awardable_ID == id);

        if (director == null)
        {
            return NotFound();
        }

        var viewModel = new DirectorViewModel // DirectorViewModel oluşturuldu
        {
            Awardable_ID = director.Awardable_ID,
            Name = director.Name, // Director özelliklerinden eşlendi
            Birth_Date = director.Birth_Date,
            Nationality = director.Nationality
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DirectorViewModel model) // Parameter changed to DirectorViewModel
    {
        if (ModelState.IsValid)
        {
            var director = await dbContext.Directors // dbContext.Directors'tan çekildi
                .FirstOrDefaultAsync(d => d.Awardable_ID == model.Awardable_ID);

            if (director == null)
            {
                return NotFound();
            }

            director.Name = model.Name; // Director özellikleri güncellendi
            director.Birth_Date = model.Birth_Date;
            director.Nationality = model.Nationality;
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Director successfully updated!"; // Mesaj güncellendi
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var director = await dbContext.Directors // dbContext.Directors'tan çekildi
            .FirstOrDefaultAsync(d => d.Awardable_ID == id);

        if (director == null)
        {
            return NotFound();
        }

        return View(director);
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var director = await dbContext.Directors // dbContext.Directors'tan çekildi
            .FirstOrDefaultAsync(d => d.Awardable_ID == id);

        if (director == null)
        {
            return NotFound();
        }
        
        // Remove the director itself
        dbContext.Directors.Remove(director); // dbContext.Directors'tan silindi
        
        // Remove the Awardable entity
        var awardable = await dbContext.Awardables.FindAsync(director.Awardable_ID);
        if (awardable != null)
        {
            dbContext.Awardables.Remove(awardable);
        }

        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Director successfully deleted!"; // Mesaj güncellendi
        return RedirectToAction(nameof(List));
    }
    
}
