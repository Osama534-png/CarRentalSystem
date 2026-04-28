using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class FleetController : Controller
    {
        private readonly CarRentalContext _db;
        public FleetController(CarRentalContext db) => _db = db;

        // /Fleet?alisSubeId=...&iadeSubeId=...&baslangic=...&bitis=...
        [HttpGet]
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Index(string? marka, string? modelVeyaYil)
        {
            var q = _db.vw_MusaitAraclar.AsNoTracking();

            marka = (marka ?? "").Trim();
            modelVeyaYil = (modelVeyaYil ?? "").Trim();

            int yil;
            bool isYear = int.TryParse(modelVeyaYil, out yil) && modelVeyaYil.Length == 4;

            // ✅ Sadece sıralama: filtreleme YOK
            q = q
                // 1) Yıl girildiyse o yıl en üste
                .OrderByDescending(a => isYear && a.Yil == yil)

                // 2) Marka girildiyse o marka en üste
                .ThenByDescending(a =>
                    !string.IsNullOrWhiteSpace(marka) &&
                    a.Marka != null &&
                    a.Marka.Contains(marka))

                // 3) Model (metin) girildiyse o modele uyanlar en üste
                .ThenByDescending(a =>
                    !string.IsNullOrWhiteSpace(modelVeyaYil) &&
                    !isYear &&
                    a.Model != null &&
                    a.Model.Contains(modelVeyaYil))

                // 4) Kalanlar normal sırada
                .ThenBy(a => a.Marka)
                .ThenBy(a => a.Model);

            var list = await q.ToListAsync();

            ViewBag.Marka = marka;
            ViewBag.ModelVeyaYil = modelVeyaYil;

            return View(list);
        }



    }
}
