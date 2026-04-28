using Microsoft.AspNetCore.Mvc;

namespace WebApplication6.Controllers
{
    public class OfficesController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
