using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models.ViewModels
{
    public class LoginVm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Sifre { get; set; } = "";
    }
}
