using Microsoft.AspNetCore.Mvc;

namespace NodeReact.Sample.Webpack.AspNetCore.Controllers;

public class NodeReactStreamingController : Controller
{
    [HttpGet("/stream")]
    public IActionResult Stream()
    {
        return View("$Components.App", new {title = "Hello from controller"});
    }
}
