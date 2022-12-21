using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.ViewModels;

public class SiparisAdresDuzenleVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "İsim")]
    public string? Isim { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Soyisim")]
    public string? SoyIsim { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Telefon Numarası")]
    [DataType(DataType.PhoneNumber)]
    public string? Telefon { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "İl")]
    public string? Il { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "İlçe")]
    public string? Ilce { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Adres")]
    public string? Adres { get; set; }

    [Required(ErrorMessage = "Gerekli", AllowEmptyStrings = false)]
    [Display(Name = "Posta Kodu")]
    [DataType(DataType.PostalCode)]
    public string? PostaKodu { get; set; }
}
