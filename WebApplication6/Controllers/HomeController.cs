using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Models.ViewModels;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        private readonly CarRentalContext _db;
        public HomeController(CarRentalContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var subeler = await _db.Subeler
                .OrderBy(x => x.Sehir).ThenBy(x => x.SubeAdi)
                .ToListAsync();

            var vm = new HomeIndexVm
            {
                Subeler = subeler
                    .Select(s => new SelectListItem($"{s.Sehir} - {s.SubeAdi}", s.SubeID.ToString()))
                    .ToList(),
                Baslangic = DateTime.Today.AddDays(1).Date.AddHours(10),
                Bitis = DateTime.Today.AddDays(3).Date.AddHours(10)
            };

            return View(vm);
        }
    }
}
