using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebApplication6.Models;
using WebApplication6.Models.ViewModels;

namespace WebApplication6.Controllers
{
    public class AccountController : Controller
    {
        private readonly CarRentalContext _db;
        public AccountController(CarRentalContext db) => _db = db;

        // REGISTER (GET)
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new RegisterVm());
        }

        // REGISTER (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm vm, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid) return View(vm);

            // ✅ KVKK + Sözleşme zorunlu
            if (!vm.TermsAccepted || !vm.KvkkAccepted)
            {
                ModelState.AddModelError("", "Üyelik Sözleşmesi ve KVKK Metni onaylanmalıdır.");
                return View(vm);
            }

            var email = (vm.Email ?? "").Trim().ToLowerInvariant();
            var exists = await _db.Kullanicilar.AnyAsync(x => x.Email.ToLower() == email);
            if (exists)
            {
                TempData["Error"] = "Bu e-posta zaten kayıtlı.";
                ModelState.AddModelError(nameof(vm.Email), "Bu e-posta zaten kayıtlı.");
                return View(vm);
            }

            var user = new Kullanicilar
            {
                Ad = (vm.Ad ?? "").Trim(),
                Soyad = (vm.Soyad ?? "").Trim(),
                Email = email,
                SifreHash = Hash(vm.Sifre),
                CreatedAt = DateTime.UtcNow
            };

            _db.Kullanicilar.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Üyelik başarılı şekilde oluşturuldu. Giriş yapabilirsiniz.";

            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        // LOGIN (GET)
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginVm());
        }

        // LOGIN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm vm, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid) return View(vm);

            var email = (vm.Email ?? "").Trim().ToLowerInvariant();
            var hash = Hash(vm.Sifre);

            var user = await _db.Kullanicilar
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email && x.SifreHash == hash);

            if (user == null)
            {
                TempData["Error"] = "E-posta veya şifre hatalı.";
                return View(vm);
            }

            HttpContext.Session.SetInt32("KullaniciID", user.KullaniciID);
            HttpContext.Session.SetString("KullaniciAd", user.Ad);

            TempData["Success"] = $"Giriş başarılı. Hoş geldin {user.Ad}!";

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Çıkış başarılı.";
            return RedirectToAction("Index", "Home");
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? ""));
            return Convert.ToHexString(bytes);
        }
    }
}
