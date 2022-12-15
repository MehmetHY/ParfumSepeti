using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ParfumSepeti.Controllers;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    public IActionResult Panel()
    {
        return View();
    }
}
