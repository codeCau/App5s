using App5s.Models;
using Microsoft.EntityFrameworkCore;

namespace App5s.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(120);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(180);
            entity.Property(e => e.SenhaHash).IsRequired();
            entity.Property(e => e.Perfil).IsRequired().HasMaxLength(50);
        });
    }
}