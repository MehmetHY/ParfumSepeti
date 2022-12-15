using Microsoft.AspNetCore.Mvc;

namespace ParfumSepeti.Controllers;
public class MagazaController : Controller
{
    public IActionResult Anasayfa()
    {
        return View();
    }
}
