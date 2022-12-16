using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class GirisVM
{
    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Kullanıcı Adı")]
    public string KullaniciAdi { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Şifre")]
    [DataType(DataType.Password)]
    public string Sifre { get; set; }

    [Display(Name = "Beni Hatırla")]
    public bool BeniHatirla { get; set; } = false;
}
