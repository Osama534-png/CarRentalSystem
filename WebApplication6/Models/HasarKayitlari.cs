using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class HasarKayitlari
{
    public int HasarID { get; set; }

    public int KiralamaID { get; set; }

    public DateTime Tarih { get; set; }

    public string Aciklama { get; set; } = null!;

    public decimal Tutar { get; set; }

    public string? FotoUrl { get; set; }

    public virtual Kiralamalar Kiralama { get; set; } = null!;
}
