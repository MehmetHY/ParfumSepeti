using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Const;
using ParfumSepeti.Data;
using ParfumSepeti.Models;
using ParfumSepeti.ViewModels;

namespace ParfumSepeti.Services;

public class KullaniciManager : Manager<Kullanici>
{
    private readonly UserManager<Kullanici> _userManager;

    public KullaniciManager(AppDbContext db,
                            UserManager<Kullanici> userManager) : base(db)
    {
        _userManager = userManager;
    }

    public async Task<Result<KullaniciListeleVM>> GetListeleVMAsync(int page = 1,
                                                                    int pageSize = 20)
    {
        if (!await ValidPage(page, pageSize))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz sayfa" }
            };

        var admins = await _userManager.GetUsersInRoleAsync("admin");
        var adminIds = admins.Select(a => a.Id);

        var items = await GetQueryable(filter: u => !adminIds.Contains(u.Id),
                                       tracked: false,
                                       include: u => u.Siparisler,
                                       page: page,
                                       pageSize: pageSize)
            .Select(u => new KullaniciListeleItem
            {
                Id = u.Id,
                Isim = u.UserName ?? "-",

                OdenmisSiparis = u.Siparisler
                    .Where(s => s.OdemeDurumu == OdeneDurumu.ODENDI)
                    .Count()
            })
            .ToListAsync();

        return new()
        {
            Object = new()
            {
                Items = items,
                CurrentPage = page,
                PageSize = pageSize
            }
        };
    }

    public async Task<Result<KullaniciSilVM>> GetSilVMAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await GetFirstOrDefaultAsync(filter: k => k.Id == id,
                                                     include: k => k.Siparisler,
                                                     tracked: false);

        if (kullanici == null || await _userManager.IsInRoleAsync(kullanici, "admin"))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kullanıcı" }
            };

        return new()
        {
            Object = new()
            {
                Id = kullanici.Id,
                Isim = kullanici.UserName ?? "-",

                OdenenSiparis = kullanici.Siparisler
                    .Where(s => s.OdemeDurumu == OdeneDurumu.ODENDI)
                    .Count()
            }
        };
    }

    public async Task<Result> RemoveAsync(KullaniciSilVM vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Id))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await _userManager.FindByIdAsync(vm.Id);

        if (kullanici == null || await _userManager.IsInRoleAsync(kullanici, "admin"))
            return new()
            {
                Success = false,
                Errors = { "Geçersiz kullanıcı" }
            };

        var result = await _userManager.DeleteAsync(kullanici);

        if (!result.Succeeded)
            return new()
            {
                Success = false,
                Errors = { "Silinemedi" }
            };

        return new();
    }

    public async Task<Result<KullaniciDuzenleVM>> GetDuzenleVMAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await GetFirstOrDefaultAsync(filter: k => k.Id == id,
                                                     tracked: false);

        if (kullanici == null || await _userManager.IsInRoleAsync(kullanici, "admin"))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        return new()
        {
            Object = new()
            {
                Id = kullanici.Id,
                KullaniciAdi = kullanici.UserName ?? "-"
            }
        };
    }

    public async Task<Result> UpdateAsync(KullaniciDuzenleVM vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Id))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        var kullanici = await GetFirstOrDefaultAsync(filter: k => k.Id == vm.Id);

        if (kullanici == null || await _userManager.IsInRoleAsync(kullanici, "admin"))
            return new()
            {
                Success = false,
                Fatal = true,
                Errors = { "Geçersiz kullanıcı" }
            };

        if (await AnyAsync(k => k.Id != kullanici.Id && k.UserName == vm.KullaniciAdi))
            return new()
            {
                Success = false,
                Errors = { $"Kullanıcı adı '{vm.KullaniciAdi}' zaten mevcut" }
            };

        kullanici.UserName = vm.KullaniciAdi;

        return new();
    }
}
