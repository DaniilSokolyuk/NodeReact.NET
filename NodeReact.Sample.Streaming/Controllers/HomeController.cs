using Microsoft.AspNetCore.Mvc;

namespace NodeReact.Sample.Streaming.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/{**all}")]
        public IActionResult Index()
        {
            return View("$Components.App", new {title = "Hello from controller"});
        }
    }
}