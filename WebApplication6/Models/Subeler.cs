using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class Subeler
{
    public int SubeID { get; set; }

    public string SubeAdi { get; set; } = null!;

    public string Sehir { get; set; } = null!;

    public string Ilce { get; set; } = null!;

    public string Adres { get; set; } = null!;

    public virtual ICollection<Araclar> Araclar { get; set; } = new List<Araclar>();
}
