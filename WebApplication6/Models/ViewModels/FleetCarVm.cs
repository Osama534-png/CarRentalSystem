namespace WebApplication6.Models.ViewModels
{
    public class FleetCarVm
    {
        public int AracID { get; set; }
        public string Marka { get; set; } = "";
        public string Model { get; set; } = "";
        public int Yil { get; set; }
        public string? Renk { get; set; }
        public decimal GunlukUcret { get; set; }
        public string Durum { get; set; } = "";
        public string KategoriAdi { get; set; } = "";
        public string SubeAdi { get; set; } = "";
        public string Sehir { get; set; } = "";
        public int SubeID { get; set; }
    }

    public class FleetIndexVm
    {
        public List<FleetCarVm> Cars { get; set; } = new();
        public int? AlisSubeId { get; set; }
        public int? IadeSubeId { get; set; }
        public DateTime? Baslangic { get; set; }
        public DateTime? Bitis { get; set; }
    }
}
