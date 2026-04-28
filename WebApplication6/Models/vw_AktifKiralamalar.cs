using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class vw_AktifKiralamalar
{
    public int KiralamaID { get; set; }

    public DateTime AlisTarihi { get; set; }

    public DateTime DonusTarihi { get; set; }

    public DateTime? TeslimTarihi { get; set; }

    public decimal ToplamTutar { get; set; }

    public decimal? UygulananIndirimOrani { get; set; }

    public decimal? IndirimTutari { get; set; }

    public decimal? NetTutar { get; set; }

    public int MusteriID { get; set; }

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public string Telefon { get; set; } = null!;

    public string TCKimlikNo { get; set; } = null!;

    public int AracID { get; set; }

    public string Plaka { get; set; } = null!;

    public string Marka { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string SubeAdi { get; set; } = null!;

    public string Sehir { get; set; } = null!;

    public string Ilce { get; set; } = null!;

    public string KategoriAdi { get; set; } = null!;
}
