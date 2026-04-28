using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Models.ViewModels;

namespace WebApplication6.Controllers
{
    public class UserPanelController : Controller
    {
        private readonly CarRentalContext _db;
        public UserPanelController(CarRentalContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciId == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "UserPanel") });

            var user = await _db.Kullanicilar
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.KullaniciID == kullaniciId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var email = (user.Email ?? "").Trim().ToLower();

            var musteri = await _db.Musteriler
                .AsNoTracking()
                .FirstOrDefaultAsync(m => (m.Email ?? "").Trim().ToLower() == email);

            var vm = new UserPanelVm
            {
                AdSoyad = $"{user.Ad} {user.Soyad}",
                Email = user.Email,
                Telefon = musteri?.Telefon,
                Tckn = musteri?.TCKimlikNo,
                DogumTarihi = musteri?.DogumTarihi,  // DateOnly?
                EhliyetNo = musteri?.EhliyetNo
            };

            if (musteri != null)
            {
                vm.KiralamaGecmisi = await _db.Kiralamalar
                    .AsNoTracking()
                    .Where(k => k.MusteriID == musteri.MusteriID)
                    .Include(k => k.Arac)
                        .ThenInclude(a => a.Sube)
                    .OrderByDescending(k => k.AlisTarihi)
                    .Select(k => new UserRentalRowVm
                    {
                        KiralamaID = k.KiralamaID,
                        Arac = k.Arac.Marka + " " + k.Arac.Model,
                        Plaka = k.Arac.Plaka,
                        ResimUrl = k.Arac.ResimUrl,
                        Sube = k.Arac.Sube.SubeAdi + " (" + k.Arac.Sube.Sehir + ")",
                        AlisTarihi = k.AlisTarihi,
                        DonusTarihi = k.DonusTarihi,
                        NetTutar = k.NetTutar, // decimal?
                        Durum = k.TeslimTarihi == null ? "Aktif" : "Tamamlandı"
                    })
                    .ToListAsync();
            }

            return View(vm);
        }
    }
}
