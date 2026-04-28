using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class Araclar
{
    public int AracID { get; set; }

    public string Plaka { get; set; } = null!;

    public string Marka { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Yil { get; set; }

    public string Renk { get; set; } = null!;

    public int KategoriID { get; set; }

    public int SubeID { get; set; }

    public string Durum { get; set; } = null!;

    public decimal GunlukUcret { get; set; }

    public int AnlikKM { get; set; }

    public string? ResimUrl { get; set; }

    public virtual ICollection<AracBakimlari> AracBakimlari { get; set; } = new List<AracBakimlari>();

    public virtual ICollection<Degerlendirmeler> Degerlendirmeler { get; set; } = new List<Degerlendirmeler>();

    public virtual AracKategorileri Kategori { get; set; } = null!;

    public virtual ICollection<Kiralamalar> Kiralamalar { get; set; } = new List<Kiralamalar>();

    public virtual ICollection<Rezervasyonlar> Rezervasyonlar { get; set; } = new List<Rezervasyonlar>();

    public virtual Subeler Sube { get; set; } = null!;
}
