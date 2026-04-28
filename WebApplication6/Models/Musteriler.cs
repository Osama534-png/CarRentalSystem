using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class Musteriler
{
    public int MusteriID { get; set; }

    public string TCKimlikNo { get; set; } = null!;

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public string Telefon { get; set; } = null!;

    public string? Email { get; set; }

    public DateOnly DogumTarihi { get; set; }

    public string IkametIl { get; set; } = null!;

    public string IkametIlce { get; set; } = null!;

    public string Adres { get; set; } = null!;

    public string? TercihKiralamaIl { get; set; }

    public string? TercihKiralamaIlce { get; set; }

    public string EhliyetNo { get; set; } = null!;

    public string EhliyetSinifi { get; set; } = null!;

    public DateOnly EhliyetAlisTarihi { get; set; }

    public byte EhliyetCezaPuani { get; set; }

    public bool EngelDurumu { get; set; }

    public byte? EngelOrani { get; set; }

    public string? EngelliSaglikRaporuNo { get; set; }

    public virtual ICollection<Degerlendirmeler> Degerlendirmeler { get; set; } = new List<Degerlendirmeler>();

    public virtual ICollection<Kiralamalar> Kiralamalar { get; set; } = new List<Kiralamalar>();

    public virtual ICollection<Rezervasyonlar> Rezervasyonlar { get; set; } = new List<Rezervasyonlar>();
}
