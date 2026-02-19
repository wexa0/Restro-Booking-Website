using Microsoft.AspNetCore.Mvc;

namespace Resort.Controllers;

public class LandingController : Controller
{
    [HttpGet]
    public IActionResult Welcome()
    {
        return View();
    }
}
