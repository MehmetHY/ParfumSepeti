using System.ComponentModel.DataAnnotations.Schema;

namespace ParfumSepeti.Models;

#pragma warning disable CS8618

public class SiparisOgesi
{
    public int Id { get; set; }

    public string UrunIsmi { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Fiyat { get; set; }

    public int Adet { get; set; }

    public Siparis Siparis { get; set; }
}
