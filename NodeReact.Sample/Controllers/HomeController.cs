using Microsoft.AspNetCore.Mvc;

namespace NodeReact.Sample.AspNetCore.Controllers
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