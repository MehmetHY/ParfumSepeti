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

    [HttpGet]
    public async Task<IActionResult> Anasayfa()
    {
        var vm = await _magazaManager.GetAnasayfaVMAsync();

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Kategori(int id, int page = 1, int pagaSize = 20)
    {
        var result = await _magazaManager.GetKategoriVMAsync(id, page, pagaSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Indirim(int page = 1, int pageSize = 20)
    {
        var result = await _magazaManager.GetIndirimVMAsync(page, pageSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Yeni(int page = 1, int pageSize = 20)
    {
        var result = await _magazaManager.GetYeniVMAsync(page, pageSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Arama(string metin,
                                           int page = 1,
                                           int pageSize = 20)
    {
        var result = await _magazaManager.GetAramaVMAsync(metin, page, pageSize);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }

    [HttpGet]
    public async Task<IActionResult> Urun(int id)
    {
        var result = await _magazaManager.GetUrunVMAsync(id);

        if (result.Success)
            return View(result.Object);

        return BadRequest(result.ToString());
    }
}
