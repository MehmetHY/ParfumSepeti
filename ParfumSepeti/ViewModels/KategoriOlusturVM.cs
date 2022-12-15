using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class KategoriOlusturVM
{
    [Required(ErrorMessage = "Gerekli!")]
    [Remote(action: "KategoriAvailable", controller: "Kategori", ErrorMessage = "Zaten mevcut!")]
    public string Isim { get; set; }
}
