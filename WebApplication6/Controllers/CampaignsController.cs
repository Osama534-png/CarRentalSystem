using Microsoft.AspNetCore.Mvc;

namespace WebApplication6.Controllers
{
    public class CampaignController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();
    }
}
