using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;
using WebApplication6.Models.ViewModels;
using System.Text.Json;


namespace WebApplication6.Controllers
{
    public class ReservationController : Controller
    {
        private const string SessionKey_ReservationIds = "MyReservationIds";

        private readonly CarRentalContext _db;
        public ReservationController(CarRentalContext db) => _db = db;

        private bool IsLoggedIn() => HttpContext.Session.GetInt32("KullaniciID").HasValue;

        [HttpGet]
        public async Task<IActionResult> Create(int aracId, int? alisSubeId, int? iadeSubeId, DateTime? baslangic, DateTime? bitis)
        {
            if (!IsLoggedIn())
            {
                var returnUrl = Url.Action("Create", "Reservation", new { aracId, alisSubeId, iadeSubeId, baslangic, bitis });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            var arac = await _db.Araclar.AsNoTracking().FirstOrDefaultAsync(a => a.AracID == aracId);
            if (arac == null) return NotFound();

            var iadeId = iadeSubeId ?? alisSubeId;

            var vm = new ReservationCreateVm
            {
                AracId = aracId,
                AlisSubeId = alisSubeId,
                IadeSubeId = iadeId,
                Baslangic = baslangic ?? DateTime.Today.AddDays(1).Date.AddHours(10),
                Bitis = bitis ?? DateTime.Today.AddDays(3).Date.AddHours(10),
                DogumTarihi = DateOnly.FromDateTime(DateTime.Today.AddYears(-21)),
                EhliyetSinifi = "B" // default
            };

            await FillSubeTextsAsync(alisSubeId, iadeId);
            ViewBag.Arac = arac;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationCreateVm vm)
        {
            if (!IsLoggedIn())
            {
                var returnUrl = Url.Action("Create", "Reservation", new { aracId = vm.AracId });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            var arac = await _db.Araclar.FirstOrDefaultAsync(a => a.AracID == vm.AracId);
            if (arac == null) return NotFound();

            ViewBag.Arac = arac;
            await FillSubeTextsAsync(vm.AlisSubeId, vm.IadeSubeId);

            // ✅ Hidden + checkbox aynı isimle gelebilir (ör: "false,true")
            vm.SicilDogrulandi = ReadBoolAnyTrue(nameof(vm.SicilDogrulandi), vm.SicilDogrulandi);
            vm.SicilTemizOnay = ReadBoolAnyTrue(nameof(vm.SicilTemizOnay), vm.SicilTemizOnay);
            vm.SicilBarkodNo = (Request.Form[nameof(vm.SicilBarkodNo)].FirstOrDefault() ?? vm.SicilBarkodNo)?.Trim();

            // PDF doğrulandıysa checkbox şartını kaldır
            if (vm.SicilDogrulandi)
            {
                vm.SicilTemizOnay = true;
                ModelState.Remove(nameof(vm.SicilTemizOnay));
            }

            // ✅ DB zorunlu alanlara default
            if (string.IsNullOrWhiteSpace(vm.EhliyetSinifi))
            {
                vm.EhliyetSinifi = "B";
                ModelState.Remove(nameof(vm.EhliyetSinifi));
            }

            if (string.IsNullOrWhiteSpace(vm.Adres))
            {
                vm.Adres = "Belirtilmedi";
                ModelState.Remove(nameof(vm.Adres));
            }

            if (!ModelState.IsValid)
                return View(vm);

            // Tarih kontrolü
            if (vm.Baslangic >= vm.Bitis)
            {
                ModelState.AddModelError("", "Başlangıç tarihi bitiş tarihinden küçük olmalıdır.");
                return View(vm);
            }

            // 21+ yaş kontrolü
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = CalcAge(vm.DogumTarihi, today);
            if (age < 21)
            {
                ModelState.AddModelError("", "Kayıt reddedildi: Müşteri yaşı en az 21 olmalıdır.");
                return View(vm);
            }

            // Adli sicil şartı
            if (!vm.SicilDogrulandi && !vm.SicilTemizOnay)
            {
                ModelState.AddModelError("", "Devam etmek için adli sicil onayını işaretlemelisiniz.");
                return View(vm);
            }

            // Araç durum kontrolü (istersen)
            if (!string.IsNullOrWhiteSpace(arac.Durum) &&
                !string.Equals(arac.Durum, "Müsait", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Seçilen araç şu anda müsait değil.");
                return View(vm);
            }

            // Müşteri var mı? (TCKimlikNo ile)
            var tckn = (vm.Tckn ?? "").Trim();
            var musteri = await _db.Musteriler.FirstOrDefaultAsync(m => m.TCKimlikNo == tckn);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var sinif = string.IsNullOrWhiteSpace(vm.EhliyetSinifi) ? "B" : vm.EhliyetSinifi.Trim();
                var adres = string.IsNullOrWhiteSpace(vm.Adres) ? "Belirtilmedi" : vm.Adres.Trim();

                if (musteri == null)
                {
                    musteri = new Musteriler
                    {
                        TCKimlikNo = tckn,
                        Ad = (vm.Ad ?? "").Trim(),
                        Soyad = (vm.Soyad ?? "").Trim(),
                        Telefon = (vm.Telefon ?? "").Trim(),
                        Email = (vm.Email ?? "").Trim(),
                        DogumTarihi = vm.DogumTarihi,
                        EhliyetNo = (vm.EhliyetNo ?? "").Trim(),
                        EhliyetSinifi = sinif,     // ✅
                        Adres = adres              // ✅
                    };

                    _db.Musteriler.Add(musteri);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    musteri.Ad = (vm.Ad ?? "").Trim();
                    musteri.Soyad = (vm.Soyad ?? "").Trim();
                    musteri.Telefon = (vm.Telefon ?? "").Trim();
                    musteri.Email = (vm.Email ?? "").Trim();
                    musteri.DogumTarihi = vm.DogumTarihi;
                    musteri.EhliyetNo = (vm.EhliyetNo ?? "").Trim();
                    musteri.EhliyetSinifi = sinif; // ✅
                    musteri.Adres = adres;         // ✅

                    await _db.SaveChangesAsync();
                }

                // ✅ Rezervasyon: DOĞRU ALANLAR (BaslangicTarihi/BitisTarihi)
                var rezerv = new Rezervasyonlar
                {
                    MusteriID = musteri.MusteriID,
                    AracID = vm.AracId,
                    BaslangicTarihi = vm.Baslangic,
                    BitisTarihi = vm.Bitis,
                    Durum = "Aktif",
                    OlusturmaTarihi = DateTime.Now
                };

                _db.Rezervasyonlar.Add(rezerv);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                AddMyReservationId(rezerv.RezervasyonID);


                // ✅ Son rezervasyon ID'yi session'a yaz
                HttpContext.Session.SetInt32("LastReservationID", rezerv.RezervasyonID);

                return RedirectToAction("Success", new { id = rezerv.RezervasyonID });

            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                var root = ex.GetBaseException().Message;
                ModelState.AddModelError("", $"Rezervasyon kaydedilemedi. DB: {root}");
                return View(vm);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", $"Rezervasyon kaydedilemedi: {ex.Message}");
                return View(vm);
            }
        }
        [HttpGet]
        public IActionResult Last()
        {
            var lastId = HttpContext.Session.GetInt32("LastReservationID");
            if (lastId.HasValue)
                return RedirectToAction("Success", new { id = lastId.Value });

            // Hiç rezervasyon yoksa Create sayfasına gönder
            return RedirectToAction("Create");
        }


        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            var rezerv = await _db.Rezervasyonlar.AsNoTracking().FirstOrDefaultAsync(r => r.RezervasyonID == id);
            if (rezerv == null) return NotFound();

            var arac = await _db.Araclar.AsNoTracking().FirstOrDefaultAsync(a => a.AracID == rezerv.AracID);
            var musteri = await _db.Musteriler.AsNoTracking().FirstOrDefaultAsync(m => m.MusteriID == rezerv.MusteriID);

            // ✅ Kullanıcı adı (session -> Kullanicilar)
            string? kullaniciAdi = null;
            var kid = HttpContext.Session.GetInt32("KullaniciID");
            if (kid.HasValue)
            {
                var kul = await _db.Kullanicilar.AsNoTracking().FirstOrDefaultAsync(x => x.KullaniciID == kid.Value);
                if (kul != null) kullaniciAdi = $"{kul.Ad} {kul.Soyad}";
            }

            var toplamGun = (int)Math.Ceiling((rezerv.BitisTarihi - rezerv.BaslangicTarihi).TotalDays);
            if (toplamGun < 1) toplamGun = 1;

            var kalanGun = (int)Math.Ceiling((rezerv.BitisTarihi - DateTime.Now).TotalDays);
            if (kalanGun < 0) kalanGun = 0;

            var vm = new ReservationSuccessVm
            {
                RezervasyonID = rezerv.RezervasyonID,
                Baslangic = rezerv.BaslangicTarihi,
                Bitis = rezerv.BitisTarihi,
                ToplamGun = toplamGun,
                KalanGun = kalanGun,
                Arac = arac,
                Durum = rezerv.Durum,
                MusteriAdSoyad = musteri == null ? null : $"{musteri.Ad} {musteri.Soyad}",
                KullaniciAdSoyad = kullaniciAdi
            };

            return View(vm);
        }
        private List<int> GetMyReservationIds()
        {
            var json = HttpContext.Session.GetString(SessionKey_ReservationIds);
            if (string.IsNullOrWhiteSpace(json)) return new List<int>();
            return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
        }

        private void AddMyReservationId(int id)
        {
            var list = GetMyReservationIds();
            if (!list.Contains(id)) list.Add(id);
            HttpContext.Session.SetString(SessionKey_ReservationIds, JsonSerializer.Serialize(list));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
            {
                var returnUrl = Url.Action("Index", "Reservation");
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            var ids = GetMyReservationIds();
            if (ids.Count == 0)
            {
                return View(new List<ReservationListItemVm>());
            }

            var rezervler = await _db.Rezervasyonlar
                .AsNoTracking()
                .Where(r => ids.Contains(r.RezervasyonID))
                .OrderByDescending(r => r.OlusturmaTarihi)
                .Select(r => new ReservationListItemVm
                {
                    RezervasyonID = r.RezervasyonID,
                    Durum = r.Durum,
                    Baslangic = r.BaslangicTarihi,
                    Bitis = r.BitisTarihi,
                    AracMarka = r.Arac.Marka,
                    AracModel = r.Arac.Model,
                    Plaka = r.Arac.Plaka,
                    ResimUrl = r.Arac.ResimUrl
                })
                .ToListAsync();

            return View(rezervler);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Success", "Reservation", new { id }) });

            var rezerv = await _db.Rezervasyonlar.FirstOrDefaultAsync(r => r.RezervasyonID == id);
            if (rezerv == null) return NotFound();

            // Zaten iptal ise tekrar iptal etme
            if (string.Equals(rezerv.Durum, "İptal", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(rezerv.Durum, "Iptal", StringComparison.OrdinalIgnoreCase))
            {
                TempData["CancelMsg"] = "Rezervasyon zaten iptal edilmiş.";
                return RedirectToAction("Success", new { id });
            }

            // ✅ 2 saat kuralı: OlusturmaTarihi'nden itibaren
            var limit = rezerv.OlusturmaTarihi.AddHours(2);
            if (DateTime.Now > limit)
            {
                TempData["CancelErr"] = "Rezervasyon sadece oluşturulduktan sonraki 2 saat içinde iptal edilebilir.";
                return RedirectToAction("Success", new { id });
            }

            rezerv.Durum = "İptal";
            await _db.SaveChangesAsync();

            // İstersen aracı müsait yap (deneme ortamı için)
            var arac = await _db.Araclar.FirstOrDefaultAsync(a => a.AracID == rezerv.AracID);
            if (arac != null) arac.Durum = "Müsait";
            await _db.SaveChangesAsync();

            TempData["CancelMsg"] = "Rezervasyon iptal edildi.";
            return RedirectToAction("Success", new { id });
        }


        // ---------- Helpers ----------

        private bool ReadBoolAnyTrue(string key, bool fallback)
        {
            var values = Request.Form[key];
            if (values.Count == 0) return fallback;

            foreach (var v in values)
            {
                if (string.Equals(v, "true", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(v, "on", StringComparison.OrdinalIgnoreCase) ||
                    v == "1")
                    return true;
            }
            return false;
        }

        private int CalcAge(DateOnly birth, DateOnly today)
        {
            var age = today.Year - birth.Year;
            if (today < birth.AddYears(age)) age--;
            return age;
        }

        private async Task FillSubeTextsAsync(int? alisSubeId, int? iadeSubeId)
        {
            string? alisText = null;
            string? iadeText = null;

            if (alisSubeId.HasValue)
            {
                var s = await _db.Subeler.AsNoTracking().FirstOrDefaultAsync(x => x.SubeID == alisSubeId.Value);
                if (s != null) alisText = $"{s.Sehir} / {s.Ilce} - {s.SubeAdi}";
            }

            if (iadeSubeId.HasValue)
            {
                var s = await _db.Subeler.AsNoTracking().FirstOrDefaultAsync(x => x.SubeID == iadeSubeId.Value);
                if (s != null) iadeText = $"{s.Sehir} / {s.Ilce} - {s.SubeAdi}";
            }

            ViewBag.AlisSubeText = alisText;
            ViewBag.IadeSubeText = iadeText;
        }
    }
}
