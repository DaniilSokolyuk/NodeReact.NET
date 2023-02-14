using Microsoft.AspNetCore.Mvc;

namespace NodeReact.Sample.Webpack.AspNetCore.Streaming.Controllers
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