using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models.ViewModels
{
    public class ReservationCreateVm
    {
        // Hidden / query'den gelenler
        public int AracId { get; set; }
        public int? AlisSubeId { get; set; }
        public int? IadeSubeId { get; set; }

        [Required]
        public DateTime Baslangic { get; set; }

        [Required]
        public DateTime Bitis { get; set; }

        // Form alanları
        [Required, StringLength(50)]
        public string Ad { get; set; } = "";

        [Required, StringLength(50)]
        public string Soyad { get; set; } = "";

        [Required, StringLength(11, MinimumLength = 11)]
        public string Tckn { get; set; } = "";

        [Required]
        public DateOnly DogumTarihi { get; set; }

        [Required, StringLength(30)]
        public string Telefon { get; set; } = "";

        [Required, EmailAddress, StringLength(120)]
        public string Email { get; set; } = "";

        [Required, StringLength(30)]
        public string EhliyetNo { get; set; } = "";

        // ✅ DB zorunlu alanlar
        [Required, StringLength(250)]
        public string Adres { get; set; } = "";

        // ✅ DB zorunlu alan (default B vereceğiz)
        [StringLength(5)]
        public string? EhliyetSinifi { get; set; }

        // Adli sicil
        public bool SicilTemizOnay { get; set; }     // checkbox
        public bool SicilDogrulandi { get; set; }    // PDF doğrulama sonucu (hidden)
        public string? SicilBarkodNo { get; set; }   // girilen barkod (hidden)
    }
}
