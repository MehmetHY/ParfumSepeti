using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Services;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Controllers;

[Authorize(Roles = "admin")]
public class UrunController : Controller
{
    private readonly UrunManager _urunManager;

    public UrunController(UrunManager urunManager)
    {
        _urunManager = urunManager;
    }

    [HttpGet]
    public async Task<IActionResult> Listele(int page = 1, int pageSize = 20)
    {
        var result = await _urunManager.GetListeleVMAsync(page, pageSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Olustur()
    {
        var vm = await _urunManager.GetOlusturVMAsync();

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
    public async Task<IActionResult> Olustur(UrunOlusturVM vm, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            var result = await _urunManager.CreateAsync(vm, file);

            if (result.Success)
            {
                await _urunManager.SaveAsync();

                return RedirectToAction(nameof(Listele));
            }

            ModelState.AddResultErrors(result);
        }

        vm.Kategoriler = await _urunManager.GetKategoriSelectListAsync();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Sil(int id)
    {
        var result = await _urunManager.GetSilVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(UrunSilVM vm)
    {
        var result = await _urunManager.RemoveAsync(vm);

        if (result.Success)
        {
            await _urunManager.SaveAsync();

            return RedirectToAction(nameof(Listele));
        }

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Duzenle(int id)
    {
        var result = await _urunManager.GetDuzenleVM(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
    public async Task<IActionResult> Duzenle(UrunDuzenleVM vm, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            var result = await _urunManager.UpdateAsync(vm, file);

            if (result.Success)
            {
                await _urunManager.SaveAsync();

                return RedirectToAction(nameof(Listele));
            }

            ModelState.AddResultErrors(result);
        }

        vm.Kategoriler = await _urunManager.GetKategoriSelectListAsync();

        return View(vm);
    }
}
