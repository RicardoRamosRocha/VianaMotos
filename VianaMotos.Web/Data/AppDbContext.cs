using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Marca> Marcas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Combustivel> Combustiveis { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Moto> Motos { get; set; }
    public DbSet<FotoMoto> FotosMoto { get; set; }
    public DbSet<Venda> Vendas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Moto>()
            .HasOne(m => m.Marca)
            .WithMany(m => m.Motos)
            .HasForeignKey(m => m.MarcaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Moto>()
            .HasOne(m => m.Categoria)
            .WithMany(c => c.Motos)
            .HasForeignKey(m => m.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Moto>()
            .HasOne(m => m.Combustivel)
            .WithMany(c => c.Motos)
            .HasForeignKey(m => m.CombustivelId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FotoMoto>()
            .HasOne(f => f.Moto)
            .WithMany(m => m.Fotos)
            .HasForeignKey(f => f.MotoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Venda>()
            .HasOne(v => v.Cliente)
            .WithMany(c => c.Vendas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venda>()
            .HasOne(v => v.Moto)
            .WithMany(m => m.Vendas)
            .HasForeignKey(v => v.MotoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
