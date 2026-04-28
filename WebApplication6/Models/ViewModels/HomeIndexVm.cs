using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication6.Models.ViewModels
{
    public class HomeIndexVm
    {
        public List<SelectListItem> Subeler { get; set; } = new();

        public int? AlisSubeId { get; set; }
        public int? IadeSubeId { get; set; }

        public DateTime Baslangic { get; set; }
        public DateTime Bitis { get; set; }
    }
}
