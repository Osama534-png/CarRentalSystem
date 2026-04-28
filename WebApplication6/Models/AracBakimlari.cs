using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class AracBakimlari
{
    public int BakimID { get; set; }

    public int AracID { get; set; }

    public DateOnly IslemTarihi { get; set; }

    public string YapilanIslem { get; set; } = null!;

    public decimal Maliyet { get; set; }

    public virtual Araclar Arac { get; set; } = null!;
}
