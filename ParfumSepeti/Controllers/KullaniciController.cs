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

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> Duzenle(string id)
    {
        var result = await _kullaniciManager.GetDuzenleVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(KullaniciDuzenleVM vm)
    {
        if (ModelState.IsValid)
        {
            var result = await _kullaniciManager.UpdateAsync(vm);

            if (result.Success)
            {
                await _kullaniciManager.SaveAsync();

                return RedirectToAction(nameof(Listele));
            }

            if (result.Fatal)
                return BadRequest(result.ToString());

            ModelState.AddResultErrors(result);
        }

        return View(vm);
    }

    [Authorize]
    public async Task<IActionResult> IstekListesi(int page = 1, int pageSize = 20)
    {
        var result = await _kullaniciManager.GetIstekListesiVMAsync(User.Identity?.Name,
                                                                    page,
                                                                    pageSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Siparisler(int page = 1, int pageSize = 20)
    {
        var result = await _kullaniciManager.GetSiparislerVM(User.Identity?.Name,
                                                             page,
                                                             pageSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> SiparisDetaylari(int id)
    {
        var result = await _kullaniciManager.GetSiparisDetayVMAsync(User.Identity?.Name,
                                                                    id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> SiparisDuzenle(int id)
    {
        var result = await _kullaniciManager.GetSiparisAdresDuzenleVMAsync(
            User.Identity?.Name,
            id
        );

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SiparisDuzenle(SiparisAdresDuzenleVM vm)
    {
        var result = await _kullaniciManager.SiparisAdresGuncelleAsync(
            User.Identity?.Name,
            vm
        );

        if (result.Success)
            return RedirectToAction(nameof(SiparisDetaylari), new { id = vm.Id });

        return BadRequest(result?.ToString());
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> SiparisIptal(int id)
    {
        var result = await _kullaniciManager.GetSiparisIptalVMAsync(User.Identity?.Name,
                                                                    id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SiparisIptal(SiparisIptalVM vm)
    {
        var result = await _kullaniciManager.SiparisIptalEtAsync(User.Identity?.Name, vm);

        if (result.Success)
            return RedirectToAction(nameof(Siparisler));

        return BadRequest(result.ToString());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SiparisOde(int siparisId)
    {
        var result = await _kullaniciManager.GetSiparisOdemeUrlAsync(User.Identity?.Name,
                                                                     siparisId);

        if (result.Success)
        {
            Response.Headers.Location = result.Object;

            return new StatusCodeResult(303);
        }

        return BadRequest(result.ToString());
    }

    #region API
    [AcceptVerbs("GET", "POST")]
    public async Task<JsonResult> UserNameAvailable(string kullaniciAdi)
    {
        var user = await _userManager.FindByNameAsync(kullaniciAdi ?? string.Empty);

        return Json(user == null);
    }

    [HttpGet]
    public ActionResult<bool> GirisYapildiMi()
    {
        return _signInManager.IsSignedIn(User);
    }

    [HttpGet]
    public async Task<ActionResult<bool>> IstekListesindeMi(int id)
    {
        var result = await _kullaniciManager.IstekListesindeMi(User.Identity?.Name, id);

        return result;
    }

    [HttpPut]
    public async Task<IActionResult> IstekListesineEkle([FromBody] IstekListesineEkleVM vm)
    {
        await _kullaniciManager.IstekListesineEkle(User.Identity?.Name, vm.UrunId);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> IstekListesindenKaldir(
        [FromBody] IstekListesindenKaldirVM vm
    )
    {
        await _kullaniciManager.IstekListesindenKaldir(User.Identity?.Name, vm.UrunId);

        return NoContent();
    }
    #endregion
}
