using Microsoft.AspNetCore.Mvc;

namespace NodeReact.Sample.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/{**all}")]
        public IActionResult Index()
        {
            return View();
        }
    }
}