using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class AracKategorileri
{
    public int KategoriID { get; set; }

    public string KategoriAdi { get; set; } = null!;

    public byte EhliyetYiliSiniri { get; set; }

    public virtual ICollection<Araclar> Araclar { get; set; } = new List<Araclar>();
}
