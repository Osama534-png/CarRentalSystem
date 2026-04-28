using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class AdliSicilKayitlari
{
    public int KayitID { get; set; }

    public string TCKimlikNo { get; set; } = null!;

    public int SucKodu { get; set; }

    public DateOnly Tarih { get; set; }

    public virtual YasakliSuclar SucKoduNavigation { get; set; } = null!;
}
