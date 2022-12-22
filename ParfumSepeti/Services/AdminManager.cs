using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Const;
using ParfumSepeti.Data;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Services;

public class AdminManager
{
    private readonly UserManager<Kullanici> _userManager;
    private readonly UrunManager _urunManager;
    private readonly KategoriManager _kategoriManager;
    private readonly KullaniciManager _kullaniciManager;
    private readonly AppDbContext _db;

    public AdminManager(UserManager<Kullanici> userManager,
                        UrunManager urunManager,
                        KategoriManager kategoriManager,
                        KullaniciManager kullaniciManager,
                        AppDbContext db)
    {
        _userManager = userManager;
        _urunManager = urunManager;
        _kategoriManager = kategoriManager;
        _kullaniciManager = kullaniciManager;
        _db = db;
    }

    public async Task<AdminPanelVM> GetPanelVM()
    {
        var kategoriSayisi = await _kategoriManager.CountAsync();
        var urunSayisi = await _urunManager.CountAsync();
        var adminSayisi = (await _userManager.GetUsersInRoleAsync("admin")).Count;
        var kullaniciSayisi = (await _kullaniciManager.CountAsync()) - adminSayisi;
        var siparisSayisi = await _db.Siparis.CountAsync();

        var onemliSiparisSayisi = await _db.Siparis.CountAsync(s =>
            s.OdemeDurumu == OdemeDurumu.ODENDI &&
            s.KargoDurumu == KargoDurumu.GONDERILMEDI
        );

        return new()
        {
            KategoriSayisi = kategoriSayisi,
            UrunSayisi = urunSayisi,
            AdminSayisi = adminSayisi,
            KullaniciSayisi = kullaniciSayisi,
            SiparisSayisi = siparisSayisi,
            OnemliSiparisSayisi = onemliSiparisSayisi
        };
    }
}
