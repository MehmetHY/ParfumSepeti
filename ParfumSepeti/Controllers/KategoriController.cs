using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Services;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Controllers;

[Authorize(Roles = "admin")]
public class KategoriController : Controller
{
    private readonly KategoriManager _kategoriManager;

    public KategoriController(KategoriManager kategoriManager)
    {
        _kategoriManager = kategoriManager;
    }

    public async Task<IActionResult> Listele(int page = 1, int pageSize = 20)
    {
        var result = await _kategoriManager.GetListeleVMAsync(page, pageSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public IActionResult Olustur() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Olustur(KategoriOlusturVM vm)
    {
        if (ModelState.IsValid)
        {
            await _kategoriManager.CreateAsync(vm);
            await _kategoriManager.SaveAsync();

            return RedirectToAction(nameof(Listele));
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Sil(int id)
    {
        var result = await _kategoriManager.GetSilVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(KategoriSilVM vm)
    {
        var result = await _kategoriManager.RemoveAsync(vm);

        if (result.Success)
        {
            await _kategoriManager.SaveAsync();

            return RedirectToAction(nameof(Listele));
        }

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Duzenle(int id)
    {
        var result = await _kategoriManager.GetDuzenleVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(KategoriDuzenleVM vm)
    {
        if (ModelState.IsValid)
        {
            var result = await _kategoriManager.UpdateAsync(vm);

            if (result.Success)
            {
                await _kategoriManager.SaveAsync();

                return RedirectToAction(nameof(Listele));
            }

            ModelState.AddResultErrors(result);
        }

        return View(vm);
    }

    #region API
    [AcceptVerbs("GET", "POST")]
    public async Task<JsonResult> KategoriAvailable(string isim)
        => Json(!await _kategoriManager.AnyAsync(k => k.Isim == isim));
    #endregion
}
