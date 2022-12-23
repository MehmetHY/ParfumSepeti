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
    public async Task<IActionResult> Listele(int page = 1, int pageSize = 20)
    {
        var result = await _siparisManager.GetListeleVMAsync();

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Detay(int id)
    {
        var result = await _siparisManager.GetDetayVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Sil(int id)
    {
        var result = await _siparisManager.GetSilVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(AdminSiparisSilVM vm)
    {
        var result = await _siparisManager.SilAsync(vm.Id);

        if (result.Success)
            return RedirectToAction(nameof(Listele));

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Duzenle(int id)
    {
        var result = await _siparisManager.GetDuzenleVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(AdminSiparisDuzenleVM vm)
    {
        if (ModelState.IsValid)
        {
            var result = await _siparisManager.DuzenleAsync(vm);

            if (result.Success)
                return RedirectToAction(nameof(Listele));

            if (result.Fatal)
                return BadRequest(result.ToString());

            ModelState.AddResultErrors(result);
        }

        vm.KargoDurumlari = _siparisManager.GetKargoDurumlari();

        return View(vm);
    }
}
