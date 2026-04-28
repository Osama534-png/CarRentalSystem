using Microsoft.EntityFrameworkCore;

namespace WebApplication6.Models;

public partial class CarRentalContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MusteriSicilKontrol>(entity =>
        {
            // TABLO ise:
            entity.HasKey(e => e.TCKimlikNo);
            entity.ToTable("MusteriSicilKontrol");

            // VIEW ise aşağıdakileri aç, yukarıdaki HasKey/ToTable satırlarını kapat:
            // entity.HasNoKey();
            // entity.ToView("MusteriSicilKontrol");

            entity.Property(e => e.TCKimlikNo)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();

            entity.Property(e => e.Durum)
                .HasMaxLength(20)
                .IsUnicode(false);
        });
    }
}
