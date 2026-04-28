using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class Degerlendirmeler
{
    public int DegerlendirmeID { get; set; }

    public int KiralamaID { get; set; }

    public int MusteriID { get; set; }

    public int AracID { get; set; }

    public byte AracPuani { get; set; }

    public byte HizmetPuani { get; set; }

    public string? Yorum { get; set; }

    public DateTime OlusturmaTarihi { get; set; }

    public virtual Araclar Arac { get; set; } = null!;

    public virtual Kiralamalar Kiralama { get; set; } = null!;

    public virtual Musteriler Musteri { get; set; } = null!;
}
