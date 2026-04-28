using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models.ViewModels
{
    public class RegisterVm
    {
        [Required, StringLength(50)]
        public string Ad { get; set; } = "";

        [Required, StringLength(50)]
        public string Soyad { get; set; } = "";

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = "";

        [Required, StringLength(100, MinimumLength = 6)]
        public string Sifre { get; set; } = "";

        [Required, Compare(nameof(Sifre), ErrorMessage = "Şifreler aynı değil.")]
        public string SifreTekrar { get; set; } = "";
        public bool TermsAccepted { get; set; }
        public bool KvkkAccepted { get; set; }

    }
}
