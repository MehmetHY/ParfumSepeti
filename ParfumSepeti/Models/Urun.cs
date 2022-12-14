﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ParfumSepeti.Models;

#pragma warning disable CS8618

public class Urun
{
    public int Id { get; set; }

    public string Baslik { get; set; }

    public string Model { get; set; }

    public string KategoriId { get; set; }
    public Kategori Kategori { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Fiyat { get; set; }

    public int IndirimYuzdesi { get; set; }

    public string Aciklama { get; set; }

    public string KargoBilgisi { get; set; }

    public string KapakUrl { get; set; }

    public List<Kullanici> Isteyenler { get; set; }

    public List<Siparis> Siparisler { get; set; }

    public List<SepetOgesi> SepetOgeleri { get; set; }
}
