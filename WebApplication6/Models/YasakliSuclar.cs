using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class YasakliSuclar
{
    public int SucKodu { get; set; }

    public string SucTanimi { get; set; } = null!;

    public bool EngelDurumu { get; set; }

    public virtual ICollection<AdliSicilKayitlari> AdliSicilKayitlari { get; set; } = new List<AdliSicilKayitlari>();
}
