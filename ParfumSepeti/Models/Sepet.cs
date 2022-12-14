namespace ParfumSepeti.Models;

#pragma warning disable CS8618

public class Sepet
{
    public int Id { get; set; }

    public string KullaniciId { get; set; }
    public Kullanici Kullanici { get; set; }

    public List<SepetOgesi> Ogeler { get; set; }
}
