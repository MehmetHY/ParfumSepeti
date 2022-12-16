using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class UrunOlusturVM
{
    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    public string Baslik { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    public string Model { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    public int KategoriId { get; set; }
    [ValidateNever]
    public List<SelectListItem> Kategoriler { get; set; } = new();

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [RegularExpression("""^\d{1,8}((,|\.)\d{1,2})?$""", ErrorMessage = "Örnek: 45.90")]
    [DataType(DataType.Currency)]
    public decimal Fiyat { get; set; }

    [RegularExpression("""^\d{1,2}$""", ErrorMessage = "0'dan 99'a kadar bir tam sayı")]
    [DataType(DataType.Text)]
    public int? IndirimYuzdesi { get; set; } = 0;

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    public string Aciklama { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    public string KargoBilgisi { get; set; }
}
