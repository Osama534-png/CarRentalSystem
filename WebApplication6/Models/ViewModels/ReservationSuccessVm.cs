using WebApplication6.Models;

namespace WebApplication6.Models.ViewModels
{
    public class ReservationSuccessVm
    {
        public int RezervasyonID { get; set; }
        public DateTime Baslangic { get; set; }
        public DateTime Bitis { get; set; }
        public int ToplamGun { get; set; }
        public int KalanGun { get; set; }
        public string? Durum { get; set; }

        public Araclar? Arac { get; set; }
        public string? MusteriAdSoyad { get; set; }

        // ✅ istenen: kullanici adi
        public string? KullaniciAdSoyad { get; set; }
    }
}
