using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Services;
using ParfumSepeti.ViewModels;

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

    [HttpGet]
    public async Task<IActionResult> Sil(int id)
    {
        var result = await _siparisManager.GetSiparisSilVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(AdminSiparisSilVM vm)
    {
        var result = await _siparisManager.SiparisSilAsync(vm.Id);

        if (result.Success)
            return RedirectToAction(nameof(Listele));
        
        return BadRequest(result.ToString());
    }
}
