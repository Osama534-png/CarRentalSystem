using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class Kiralamalar
{
    public int KiralamaID { get; set; }

    public int MusteriID { get; set; }

    public int AracID { get; set; }

    public DateTime AlisTarihi { get; set; }

    public DateTime DonusTarihi { get; set; }

    public DateTime? TeslimTarihi { get; set; }

    public decimal ToplamTutar { get; set; }

    public decimal? UygulananIndirimOrani { get; set; }

    public decimal? IndirimTutari { get; set; }

    public decimal? NetTutar { get; set; }

    public virtual Araclar Arac { get; set; } = null!;

    public virtual Degerlendirmeler? Degerlendirmeler { get; set; }

    public virtual ICollection<HasarKayitlari> HasarKayitlari { get; set; } = new List<HasarKayitlari>();

    public virtual Musteriler Musteri { get; set; } = null!;

    public virtual ICollection<Odemeler> Odemeler { get; set; } = new List<Odemeler>();
}
