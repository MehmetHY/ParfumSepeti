using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Services;

namespace ParfumSepeti.Controllers;

[Authorize(Roles = "admin")]
public class SiparisController : Controller
{
    private readonly SiparisManager _siparisManager;

    public SiparisController(SiparisManager siparisManager)
    {
        _siparisManager = siparisManager;
    }

    [HttpGet]
    public async Task<IActionResult> Listele()
    {
        var vm = await _siparisManager.GetSiparisListeleVMAsync();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Detay(int id)
    {
        var result = await _siparisManager.GetSiparisDetayVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }
}
