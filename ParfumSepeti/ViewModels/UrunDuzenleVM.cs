using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

#pragma warning disable CS8618

public class UrunDuzenleVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Başlık")]
    public string Baslik { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    public string Model { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Kategori")]
    public int KategoriId { get; set; }
    [ValidateNever]
    public List<SelectListItem> Kategoriler { get; set; } = new();

    [ValidateNever]
    public string KapakUrl { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [RegularExpression("""^\d{1,8}((,|\.)\d{1,2})?$""", ErrorMessage = "Örnek: 45.90")]
    [DataType(DataType.Currency)]
    public decimal Fiyat { get; set; }

    [RegularExpression("""^\d{1,2}$""", ErrorMessage = "0'dan 99'a kadar bir tam sayı")]
    [DataType(DataType.Text)]
    [Display(Name = "İndirim Yüzdesi")]
    public int? IndirimYuzdesi { get; set; } = 0;

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Açıklama")]
    public string Aciklama { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Kargo Bilgisi")]
    public string KargoBilgisi { get; set; }

    [Display(Name = "Kapak resmini değiştir")]
    public bool KapakDegistir { get; set; } = false;
}
