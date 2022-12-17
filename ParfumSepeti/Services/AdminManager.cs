using Microsoft.AspNetCore.Identity;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Services;

public class AdminManager
{
    private readonly UserManager<Kullanici> _userManager;
    private readonly UrunManager _urunManager;
    private readonly KategoriManager _kategoriManager;
    private readonly KullaniciManager _kullaniciManager;

    public AdminManager(UserManager<Kullanici> userManager,
                        UrunManager urunManager,
                        KategoriManager kategoriManager,
                        KullaniciManager kullaniciManager)
    {
        _userManager= userManager;
        _urunManager = urunManager;
        _kategoriManager = kategoriManager;
        _kullaniciManager = kullaniciManager;
    }

    public async Task<AdminPanelVM> GetPanelVM()
    {
        var kategoriSayisi = await _kategoriManager.CountAsync();
        var urunSayisi = await _urunManager.CountAsync();
        var adminSayisi = (await _userManager.GetUsersInRoleAsync("admin")).Count;
        var kullaniciSayisi = (await _kullaniciManager.CountAsync()) - adminSayisi;

        return new()
        {
            KategoriSayisi = kategoriSayisi,
            UrunSayisi = urunSayisi,
            AdminSayisi = adminSayisi,
            KullaniciSayisi = kullaniciSayisi
        };
    }
}
