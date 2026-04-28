using System;
using System.Collections.Generic;

namespace WebApplication6.Models;

public partial class Kullanicilar
{
    public int KullaniciID { get; set; }

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string SifreHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
