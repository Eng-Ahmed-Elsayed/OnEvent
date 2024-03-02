using Microsoft.AspNetCore.Mvc;

namespace OnEvent.Areas.InvitationManagement.Controllers
{
    public class InvitationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
