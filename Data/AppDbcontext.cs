using App5s.Models;
using Microsoft.EntityFrameworkCore;

namespace App5s.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<LogSistema> Logs => Set<LogSistema>(); // <-- Altere de SystemLogs para Logs aqui
    
    public DbSet<Setor> Setores => Set<Setor>();
    public DbSet<ItemChecklist> ItensChecklist => Set<ItemChecklist>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<RespostaAuditoria> RespostasAuditoria => Set<RespostaAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeia para a tabela existente system_logs
        modelBuilder.Entity<LogSistema>().ToTable("system_logs");

        // Setores
        modelBuilder.Entity<Setor>(entity =>
        {
            entity.ToTable("setores");
            entity.HasOne(s => s.Responsavel)
                  .WithMany()
                  .HasForeignKey(s => s.ResponsavelId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Checklist
        modelBuilder.Entity<ItemChecklist>(entity =>
        {
            entity.ToTable("itens_checklist");
        });

        // Auditorias
        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.ToTable("auditorias");

            entity.HasOne(a => a.Setor)
                  .WithMany(s => s.Auditorias)
                  .HasForeignKey(a => a.SetorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Auditor)
                  .WithMany()
                  .HasForeignKey(a => a.AuditorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Respostas
        modelBuilder.Entity<RespostaAuditoria>(entity =>
        {
            entity.ToTable("respostas_auditoria");

            entity.HasOne(r => r.Auditoria)
                  .WithMany(a => a.Respostas)
                  .HasForeignKey(r => r.AuditoriaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.ItemChecklist)
                  .WithMany()
                  .HasForeignKey(r => r.ItemChecklistId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
        });
    }
    
}
