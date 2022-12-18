using Microsoft.AspNetCore.Identity;

namespace ParfumSepeti.Models;

#pragma warning disable CS8618

public class Kullanici : IdentityUser
{
    public List<Urun> IstekListesi { get; set; }

    public List<Siparis> Siparisler { get; set; }
}
