using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class Odemeler
{
    public int OdemeID { get; set; }

    public int KiralamaID { get; set; }

    public DateTime OdemeTarihi { get; set; }

    public decimal Tutar { get; set; }

    public string OdemeYontemi { get; set; } = null!;

    public string OdemeDurumu { get; set; } = null!;

    public string IslemNo { get; set; } = null!;

    public virtual Kiralamalar Kiralama { get; set; } = null!;
}
