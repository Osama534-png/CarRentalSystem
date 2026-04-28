using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication6.Models;

public partial class CarRentalContext : DbContext
{
    public CarRentalContext()
    {
    }

    public CarRentalContext(DbContextOptions<CarRentalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdliSicilKayitlari> AdliSicilKayitlari { get; set; }

    public virtual DbSet<AracBakimlari> AracBakimlari { get; set; }

    public virtual DbSet<AracKategorileri> AracKategorileri { get; set; }

    public virtual DbSet<Araclar> Araclar { get; set; }

    public virtual DbSet<Degerlendirmeler> Degerlendirmeler { get; set; }

    public virtual DbSet<HasarKayitlari> HasarKayitlari { get; set; }

    public virtual DbSet<Kiralamalar> Kiralamalar { get; set; }

    public virtual DbSet<Musteriler> Musteriler { get; set; }

    public virtual DbSet<Odemeler> Odemeler { get; set; }

    public virtual DbSet<Rezervasyonlar> Rezervasyonlar { get; set; }

    public virtual DbSet<Subeler> Subeler { get; set; }

    public virtual DbSet<YasakliSuclar> YasakliSuclar { get; set; }

    // ✅ Üyelik tablosu
    public virtual DbSet<Kullanicilar> Kullanicilar { get; set; }

    // ✅ Sicil kontrol view’u (keyless)
    public virtual DbSet<MusteriSicilKontrol> MusteriSicilKontrol { get; set; }

    public virtual DbSet<vw_AktifKiralamalar> vw_AktifKiralamalar { get; set; }

    public virtual DbSet<vw_MusaitAraclar> vw_MusaitAraclar { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // DI üzerinden (Program.cs) connection string veriyorsan burada override etmesin:
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=OSAMA;Database=kiralama_sitesi;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ✅ A) AdliSicilKayitlari keyless (PK istemesin)
        modelBuilder.Entity<AdliSicilKayitlari>(entity =>
        {
            entity.HasNoKey();
            entity.ToTable("AdliSicilKayitlari"); // VIEW ise: entity.ToView("AdliSicilKayitlari");
        });

        modelBuilder.Entity<AracBakimlari>(entity =>
        {
            entity.HasKey(e => e.BakimID);

            entity.ToTable(tb => tb.HasTrigger("trg_AracBakimlari_SetDurum"));

            entity.Property(e => e.Maliyet).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.YapilanIslem)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Arac).WithMany(p => p.AracBakimlari)
                .HasForeignKey(d => d.AracID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AracBakimlari_Arac");
        });

        modelBuilder.Entity<AracKategorileri>(entity =>
        {
            entity.HasKey(e => e.KategoriID);

            entity.HasIndex(e => e.KategoriAdi, "UQ_AracKategorileri_KategoriAdi").IsUnique();

            entity.Property(e => e.KategoriAdi)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Araclar>(entity =>
        {
            entity.HasKey(e => e.AracID);

            entity.HasIndex(e => e.Plaka, "UQ_Araclar_Plaka").IsUnique();

            entity.Property(e => e.Durum)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Müsait");

            entity.Property(e => e.GunlukUcret).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Marka)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Plaka)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Renk)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Kategori).WithMany(p => p.Araclar)
                .HasForeignKey(d => d.KategoriID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Araclar_Kategori");

            entity.HasOne(d => d.Sube).WithMany(p => p.Araclar)
                .HasForeignKey(d => d.SubeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Araclar_Sube");
        });

        modelBuilder.Entity<Degerlendirmeler>(entity =>
        {
            entity.HasKey(e => e.DegerlendirmeID);

            entity.ToTable(tb => tb.HasTrigger("trg_Degerlendirmeler_OnlyAfterReturn"));

            entity.HasIndex(e => e.AracID, "IX_Degerlendirmeler_AracID");
            entity.HasIndex(e => e.MusteriID, "IX_Degerlendirmeler_MusteriID");
            entity.HasIndex(e => e.KiralamaID, "UQ_Degerlendirmeler_Kiralama").IsUnique();

            entity.Property(e => e.OlusturmaTarihi).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Yorum)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Arac).WithMany(p => p.Degerlendirmeler)
                .HasForeignKey(d => d.AracID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Degerlendirmeler_Araclar");

            entity.HasOne(d => d.Kiralama).WithOne(p => p.Degerlendirmeler)
                .HasForeignKey<Degerlendirmeler>(d => d.KiralamaID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Degerlendirmeler_Kiralamalar");

            entity.HasOne(d => d.Musteri).WithMany(p => p.Degerlendirmeler)
                .HasForeignKey(d => d.MusteriID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Degerlendirmeler_Musteriler");
        });

        modelBuilder.Entity<HasarKayitlari>(entity =>
        {
            entity.HasKey(e => e.HasarID);

            entity.HasIndex(e => e.KiralamaID, "IX_HasarKayitlari_KiralamaID");

            entity.Property(e => e.Aciklama)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FotoUrl)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Tarih).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Tutar).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Kiralama).WithMany(p => p.HasarKayitlari)
                .HasForeignKey(d => d.KiralamaID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HasarKayitlari_Kiralamalar");
        });

        modelBuilder.Entity<Kiralamalar>(entity =>
        {
            entity.HasKey(e => e.KiralamaID);

            entity.ToTable(tb =>
            {
                tb.HasTrigger("trg_Kiralamalar_Teslim");
                tb.HasTrigger("trg_Kiralamalar_ValidateAndDiscount");
            });

            entity.Property(e => e.AlisTarihi).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IndirimTutari).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NetTutar)
                .HasComputedColumnSql("([ToplamTutar]-isnull([IndirimTutari],(0)))", true)
                .HasColumnType("decimal(11, 2)");
            entity.Property(e => e.ToplamTutar).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UygulananIndirimOrani).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Arac).WithMany(p => p.Kiralamalar)
                .HasForeignKey(d => d.AracID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kiralamalar_Arac");

            entity.HasOne(d => d.Musteri).WithMany(p => p.Kiralamalar)
                .HasForeignKey(d => d.MusteriID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Kiralamalar_Musteri");
        });

        modelBuilder.Entity<Musteriler>(entity =>
        {
            entity.HasKey(e => e.MusteriID);

            entity.ToTable(tb => tb.HasTrigger("trg_Rezervasyonlar_Cakisma"));

            entity.HasIndex(e => e.EhliyetNo, "UQ_Musteriler_EhliyetNo").IsUnique();
            entity.HasIndex(e => e.TCKimlikNo, "UQ_Musteriler_TCKimlikNo").IsUnique();

            entity.Property(e => e.Ad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Adres)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.EhliyetNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.EhliyetSinifi)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EngelliSaglikRaporuNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IkametIl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IkametIlce)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Soyad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TCKimlikNo)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Telefon)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TercihKiralamaIl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TercihKiralamaIlce)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Odemeler>(entity =>
        {
            entity.HasKey(e => e.OdemeID);

            entity.HasIndex(e => e.KiralamaID, "IX_Odemeler_KiralamaID");
            entity.HasIndex(e => e.IslemNo, "UQ_Odemeler_IslemNo").IsUnique();

            entity.Property(e => e.IslemNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OdemeDurumu)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Basarili");
            entity.Property(e => e.OdemeTarihi).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.OdemeYontemi)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Tutar).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Kiralama).WithMany(p => p.Odemeler)
                .HasForeignKey(d => d.KiralamaID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Odemeler_Kiralamalar");
        });

        modelBuilder.Entity<Rezervasyonlar>(entity =>
        {
            entity.HasKey(e => e.RezervasyonID);

            entity.ToTable(tb =>
            {
                tb.HasTrigger("trg_Rezervasyonlar_Cakisma");
                tb.UseSqlOutputClause(false); // ✅ kritik
            });
        });



        modelBuilder.Entity<Subeler>(entity =>
        {
            entity.HasKey(e => e.SubeID);

            entity.Property(e => e.Adres)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Ilce)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sehir)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubeAdi)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<YasakliSuclar>(entity =>
        {
            entity.HasKey(e => e.SucKodu);

            entity.Property(e => e.SucKodu).ValueGeneratedNever();
            entity.Property(e => e.SucTanimi)
                .HasMaxLength(100)
                .IsUnicode(false);

            // ✅ Keyless tabloya navigation olmaz -> ignore
            entity.Ignore(e => e.AdliSicilKayitlari);
        });

        // ✅ Kullanicilar mapping (üyelik)
        modelBuilder.Entity<Kullanicilar>(entity =>
        {
            entity.HasKey(e => e.KullaniciID);

            entity.ToTable("Kullanicilar");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Ad).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Soyad).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.SifreHash).HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        });

        // ✅ Sicil kontrol view (keyless)
        modelBuilder.Entity<MusteriSicilKontrol>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("MusteriSicilKontrol"); // view adın buysa böyle kalmalı

            entity.Property(e => e.TCKimlikNo)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Durum)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<vw_AktifKiralamalar>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AktifKiralamalar");

            entity.Property(e => e.Ad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ilce)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IndirimTutari).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.KategoriAdi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Marka)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NetTutar).HasColumnType("decimal(11, 2)");
            entity.Property(e => e.Plaka)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Sehir)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Soyad)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubeAdi)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TCKimlikNo)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Telefon)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ToplamTutar).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UygulananIndirimOrani).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<vw_MusaitAraclar>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_MusaitAraclar");

            entity.Property(e => e.Durum)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.GunlukUcret).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Ilce)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.KategoriAdi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Marka)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Plaka)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Renk)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Sehir)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SubeAdi)
                .HasMaxLength(100)
                .IsUnicode(false);

            // vw_MusaitAraclar view'unda ResimUrl kolonun varsa ve modelde property varsa,
            // ayrıca map etmen şart değil; istersen:
            // entity.Property(e => e.ResimUrl).HasMaxLength(300).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
