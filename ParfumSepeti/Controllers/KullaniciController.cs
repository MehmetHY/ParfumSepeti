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

    public KullaniciController(SignInManager<Kullanici> signInManager,
                               UserManager<Kullanici> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
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

    #region API
    [AcceptVerbs("GET", "POST")]
    public async Task<JsonResult> UserNameAvailable(string kullaniciAdi)
    {
        var user = await _userManager.FindByNameAsync(kullaniciAdi ?? string.Empty);

        return Json(user == null);
    }
    #endregion
}
