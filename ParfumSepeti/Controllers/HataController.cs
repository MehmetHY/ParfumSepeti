using Microsoft.AspNetCore.Mvc;

namespace ParfumSepeti.Controllers;
public class HataController : Controller
{
    [HttpGet]
    public IActionResult ErisimEngellendi() => View();
}
