using Microsoft.AspNetCore.Mvc;

namespace WebApplication6.Models.ViewModels
{
    public class ReservationListItemVm
    {
        public int RezervasyonID { get; set; }
        public string? Durum { get; set; }
        public DateTime Baslangic { get; set; }
        public DateTime Bitis { get; set; }

        public string? AracMarka { get; set; }
        public string? AracModel { get; set; }
        public string? Plaka { get; set; }
        public string? ResimUrl { get; set; }
    }
}
