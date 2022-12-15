using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Services;

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
        var model = await _kategoriManager.GetListeleVM(page, pageSize);

        return View(model);
    }
}
