using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        var model = await _kategoriManager.GetListeleVMAsync(page, pageSize);

        return View(model);
    }

    [HttpGet]
    public IActionResult Olustur() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Olustur(KategoriOlusturVM model)
    {
        if (ModelState.IsValid)
        {
            await _kategoriManager.CreateAsync(model);
            await _kategoriManager.SaveAsync();

            return RedirectToAction(nameof(Listele));
        }

        return View(model);
    }

    #region API
    [AcceptVerbs("GET", "POST")]
    public async Task<JsonResult> KategoriAvailable(string isim)
        => Json(!await _kategoriManager.Any(k => k.Isim == isim));
    #endregion
}
