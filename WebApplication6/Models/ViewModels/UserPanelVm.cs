namespace WebApplication6.Models.ViewModels
{
    public class UserPanelVm
    {
        public string AdSoyad { get; set; } = "";
        public string Email { get; set; } = "";

        public string? Telefon { get; set; }
        public string? Tckn { get; set; }
        public DateOnly? DogumTarihi { get; set; }   // ✅ DateOnly?
        public string? EhliyetNo { get; set; }

        public List<UserRentalRowVm> KiralamaGecmisi { get; set; } = new();
    }

    public class UserRentalRowVm
    {
        public int KiralamaID { get; set; }
        public string Arac { get; set; } = "";
        public string Plaka { get; set; } = "";
        public string? ResimUrl { get; set; }

        public string Sube { get; set; } = "";
        public DateTime AlisTarihi { get; set; }
        public DateTime DonusTarihi { get; set; }

        public decimal? NetTutar { get; set; }       // ✅ decimal?
        public string Durum { get; set; } = "";
    }
}
