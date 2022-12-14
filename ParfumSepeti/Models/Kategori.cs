using System.ComponentModel.DataAnnotations;

namespace ParfumSepeti.Models;

#pragma warning disable CS8618

public class Kategori
{
    [Key]
    public string Isim { get; set; }

    public List<Urun> Urunler { get; set; }
}
