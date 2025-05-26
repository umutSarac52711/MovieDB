using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieDB.Data;
using MovieDB.Models;
using MovieDB.Models.Entities;

namespace MovieDB.Controllers;

public class ActorsController : Controller
{
    private readonly MovieDbContext dbContext;
    public ActorsController(MovieDbContext dbContext) // Constructor adı ActorsController olarak düzeltildi
    {
        this.dbContext = dbContext;
    }
    
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(ActorViewModel model) // Parametre ActorViewModel olarak değiştirildi
    {
        if (ModelState.IsValid)
        {
            // Create a new Awardable entity
            var awardableEntity = new Awardable();
            awardableEntity.Kind = "Actor"; // Kind "Actor" olarak ayarlandı

            dbContext.Awardables.Add(awardableEntity);
            await dbContext.SaveChangesAsync();

            // Now create the Actor entity with the Awardable_ID from the newly created Awardable
            
            var actor = new Actor // Movie yerine Actor oluşturuldu
                {
                    Awardable_ID = awardableEntity.Awardable_ID,
                    Awardable = awardableEntity,
                    Name = model.Name, // ActorViewModel'den özellikler eşlendi
                    Birth_Date = model.Birth_Date,
                    Nationality = model.Nationality
                };
            
            // Genre ile ilgili mantık kaldırıldı
            
            
            await dbContext.Actors.AddAsync(actor); // dbContext.Actors'a eklendi
            await dbContext.SaveChangesAsync(); 
            TempData["SuccessMessage"] = "Actor successfully added!"; // Mesaj güncellendi
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var actors = await dbContext.Actors.ToListAsync(); // dbContext.Actors'tan çekildi

        return View(actors);

    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var actor = await dbContext.Actors // dbContext.Actors'tan çekildi
            // MovieGenres ile ilgili Include kaldırıldı
            .FirstOrDefaultAsync(a => a.Awardable_ID == id);

        if (actor == null)
        {
            return NotFound();
        }

        var model = new ActorViewModel // ActorViewModel oluşturuldu
        {
            Awardable_ID = actor.Awardable_ID,
            Name = actor.Name, // Actor özelliklerinden eşlendi
            Birth_Date = actor.Birth_Date,
            Nationality = actor.Nationality
            // Genre eşlemesi kaldırıldı
        };

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var actor = await dbContext.Actors // dbContext.Actors'tan çekildi
            // MovieGenres ile ilgili Include kaldırıldı
            .FirstOrDefaultAsync(a => a.Awardable_ID == id);

        if (actor == null)
        {
            return NotFound();
        }

        var viewModel = new ActorViewModel // ActorViewModel oluşturuldu
        {
            Awardable_ID = actor.Awardable_ID,
            Name = actor.Name, // Actor özelliklerinden eşlendi
            Birth_Date = actor.Birth_Date,
            Nationality = actor.Nationality
            // Genre eşlemesi kaldırıldı
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ActorViewModel model) // Parametre ActorViewModel olarak değiştirildi
    {
        if (ModelState.IsValid)
        {
            var actor = await dbContext.Actors // dbContext.Actors'tan çekildi
                // MovieGenres ile ilgili Include kaldırıldı
                .FirstOrDefaultAsync(a => a.Awardable_ID == model.Awardable_ID);

            if (actor == null)
            {
                return NotFound();
            }

            actor.Name = model.Name; // Actor özellikleri güncellendi
            actor.Birth_Date = model.Birth_Date;
            actor.Nationality = model.Nationality;
            
            // Genre güncelleme mantığı kaldırıldı
            
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Actor successfully updated!"; // Mesaj güncellendi
            return RedirectToAction(nameof(List));
        }
        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var actor = await dbContext.Actors // dbContext.Actors'tan çekildi
             // MovieGenres ile ilgili Include kaldırıldı
            .FirstOrDefaultAsync(a => a.Awardable_ID == id);

        if (actor == null)
        {
            return NotFound();
        }

        return View(actor);
    }
    
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var actor = await dbContext.Actors // dbContext.Actors'tan çekildi
            // MovieGenres ile ilgili Include kaldırıldı
            .FirstOrDefaultAsync(a => a.Awardable_ID == id);

        if (actor == null)
        {
            return NotFound();
        }

        // MovieGenres silme mantığı kaldırıldı
        
        // Remove the actor itself
        dbContext.Actors.Remove(actor); // dbContext.Actors'tan silindi
        
        // Remove the Awardable entity
        var awardable = await dbContext.Awardables.FindAsync(actor.Awardable_ID);
        if (awardable != null)
        {
            dbContext.Awardables.Remove(awardable);
        }

        await dbContext.SaveChangesAsync();
        
        TempData["SuccessMessage"] = "Actor successfully deleted!"; // Mesaj güncellendi
        return RedirectToAction(nameof(List));
    }
    
}
