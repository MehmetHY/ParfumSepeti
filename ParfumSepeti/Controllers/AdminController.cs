using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParfumSepeti.Services;

namespace ParfumSepeti.Controllers;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly AdminManager _adminManager;

    public AdminController(AdminManager adminManager)
    {
        _adminManager = adminManager;
    }
    public async Task<IActionResult> Panel()
    {
        var vm = await _adminManager.GetPanelVM();

        return View(vm);
    }
}
