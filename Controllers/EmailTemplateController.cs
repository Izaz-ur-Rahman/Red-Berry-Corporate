using Microsoft.AspNetCore.Mvc;

namespace RedBerryCorporate.Controllers
{
    public class EmailTemplateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
