using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class vw_MusaitAraclar
{
    public int AracID { get; set; }

    public string Plaka { get; set; } = null!;

    public string Marka { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Yil { get; set; }

    public string Renk { get; set; } = null!;

    public decimal GunlukUcret { get; set; }

    public int AnlikKM { get; set; }

    public string Durum { get; set; } = null!;

    public string? ResimUrl { get; set; }

    public string KategoriAdi { get; set; } = null!;

    public string SubeAdi { get; set; } = null!;

    public string Sehir { get; set; } = null!;

    public string Ilce { get; set; } = null!;
}
