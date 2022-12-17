using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Services;

namespace ParfumSepeti.Controllers;
public class MagazaController : Controller
{
    private readonly MagazaManager _magazaManager;

    public MagazaController(MagazaManager magazaManager)
    {
        _magazaManager = magazaManager;
    }

    public async Task<IActionResult> Anasayfa()
    {
        var vm = await _magazaManager.GetAnasayfaVMAsync();

        return View(vm);
    }
}
