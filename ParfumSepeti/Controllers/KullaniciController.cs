using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Models;
using ParfumSepeti.Services;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Controllers;
public class KullaniciController : Controller
{
    private readonly SignInManager<Kullanici> _signInManager;
    private readonly UserManager<Kullanici> _userManager;
    private readonly KullaniciManager _kullaniciManager;

    public KullaniciController(SignInManager<Kullanici> signInManager,
                               UserManager<Kullanici> userManager,
                               KullaniciManager kullaniciManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _kullaniciManager = kullaniciManager;
    }

    [HttpGet]
    public IActionResult Kaydol()
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Anasayfa", "Magaza");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kaydol(KaydolVM model)
    {
        if (ModelState.IsValid)
        {
            var result = await _userManager.CreateAsync(model);

            if (result.Success)
            {
                await _signInManager.SignInAsync(result.Object!, false);

                return RedirectToAction("Anasayfa", "Magaza");
            }

            ModelState.AddResultErrors(result);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Giris()
    {
        if (_signInManager.IsSignedIn(User))
            return RedirectToAction("Anasayfa", "Magaza");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Giris(GirisVM model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.SignInAsync(_userManager, model);

            if (result.Success)
                return LocalRedirect(returnUrl ?? "/");

            ModelState.AddResultErrors(result);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cikis()
    {
        if (_signInManager.IsSignedIn(User))
            await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Giris));
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> Listele(int page = 1, int pageSize = 20)
    {
        var result = await _kullaniciManager.GetListeleVMAsync(page, pageSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> Sil(string id)
    {
        var result = await _kullaniciManager.GetSilVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(KullaniciSilVM vm)
    {
        var result = await _kullaniciManager.RemoveAsync(vm);

        if (result.Success)
            return RedirectToAction(nameof(Listele));

        return BadRequest(result.ToString());
    }

    #region API
    [AcceptVerbs("GET", "POST")]
    public async Task<JsonResult> UserNameAvailable(string kullaniciAdi)
    {
        var user = await _userManager.FindByNameAsync(kullaniciAdi ?? string.Empty);

        return Json(user == null);
    }
    #endregion
}
