using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Services;
using ParfumSepeti.ViewModels;

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

    [HttpGet]
    public async Task<IActionResult> Sepet()
    {
        var vm = await _magazaManager.GetSepetVMAsync(HttpContext.Session);

        return View(vm);
    }

    [HttpPost]
    public IActionResult SepettenKaldir(int urunId)
    {
        _magazaManager.SepettenKaldir(HttpContext.Session, urunId);

        return RedirectToAction(nameof(Sepet));
    }

    [Authorize]
    [HttpGet]
    public IActionResult OdemeBilgisi()
    {
        var sepetValid = _magazaManager.SepetValid(HttpContext.Session);

        if (sepetValid)
            return View();

        return BadRequest("Geçersiz Sepet");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SiparisOlustur(SiparisBilgiVM vm)
    {
        if (ModelState.IsValid)
        {
            var result = await _magazaManager.SiparisOlusturAsync(HttpContext.Session,
                                                                  vm,
                                                                  User.Identity?.Name);

            if (result.Success)
                return Ok(result.Object!.Id);

            if (result.Fatal)
                return BadRequest(result.ToString());

            ModelState.AddResultErrors(result);
        }

        return View(nameof(OdemeBilgisi), vm);
    }

    #region API
    [HttpPost]
    public async Task<IActionResult> SepeteEkle([FromBody] SepetEkleVM vm)
    {
        var result = await _magazaManager.SepeteEkleAsync(HttpContext.Session,
                                                          vm.UrunId,
                                                          vm.Adet);

        if (result.Success)
            return NoContent();

        return BadRequest();
    }

    [HttpGet]
    public ActionResult<bool> SepetteMi(int id)
    {
        var result = _magazaManager.SepetteMi(HttpContext.Session, id);

        return result;
    }
    #endregion
}
