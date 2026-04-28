using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class Rezervasyonlar
{
    public int RezervasyonID { get; set; }

    public int MusteriID { get; set; }

    public int AracID { get; set; }

    public DateTime BaslangicTarihi { get; set; }

    public DateTime BitisTarihi { get; set; }

    public string Durum { get; set; } = null!;

    public DateTime OlusturmaTarihi { get; set; }

    public virtual Araclar Arac { get; set; } = null!;

    public virtual Musteriler Musteri { get; set; } = null!;
}
