using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KaydolVM
{
    [Required(ErrorMessage = "Gerekli!")]
    [RegularExpression("""^[a-zA-Z0-9-_\.]{3,50}$""",
        ErrorMessage = "En az 3, en fazla 50 karakter olmalı. '-', '_' ve '.' dışında özel karakter bulunduramaz!")]
    [Display(Name = "Kullanıcı Adı")]
    [Remote(action: "UserNameAvailable", controller: "Kullanici", ErrorMessage = "Zaten mevcut!")]
    public string KullaniciAdi { get; set; }

    [Required(ErrorMessage = "Gerekli!")]
    [MinLength(3, ErrorMessage = "En az 3 karakter olmalı!")]
    [Display(Name = "Şifre")]
    [DataType(DataType.Password)]
    public string Sifre { get; set; }

    [Required(ErrorMessage = "Gerekli!")]
    [Compare(nameof(Sifre), ErrorMessage = "Şifreler eşlemedi!")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre Tekrar")]
    public string SifreTekrar { get; set; }
}
