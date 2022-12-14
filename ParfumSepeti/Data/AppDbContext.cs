using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParfumSepeti.Models;

namespace ParfumSepeti.Data;

#pragma warning disable CS8618

public class AppDbContext : IdentityDbContext<Kullanici>
{
    public DbSet<Kategori> Kategori { get; set; }
    public DbSet<Urun> Urun { get; set; }
    public DbSet<SepetOgesi> SepetOgesi { get; set; }
    public DbSet<Sepet> Sepet { get; set; }
    public DbSet<SiparisOgesi> SiparisOgesi { get; set; }
    public DbSet<Siparis> Siparis { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Siparis>()
            .HasOne(s => s.Kullanici)
            .WithMany(k => k.Siparisler)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.Entity<Kullanici>()
            .HasMany(k => k.IstekListesi)
            .WithMany(u => u.Isteyenler)
            .UsingEntity(t => t.ToTable("IstekUrun"));
    }
}
