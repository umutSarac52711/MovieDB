using Microsoft.AspNetCore.Mvc;

namespace MovieDB.Controllers
{
    public class ReviewController : Controller
    {

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
    }
}
