    using App5s.Data;
using App5s.Models;
using Microsoft.EntityFrameworkCore;

namespace App5s.Services;

public class LogService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public LogService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task RegistrarAsync(
        string nivel, 
        string mensagem, 
        string? origem = null, 
        string? usuarioEmail = null, 
        string? detalhes = null)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            
            context.Logs.Add(new LogSistema
            {
                Nivel = nivel.ToUpperInvariant(),
                Mensagem = mensagem,
                Origem = origem,
                UsuarioEmail = usuarioEmail,
                Detalhes = detalhes,
                DataHora = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }
        catch
        {
            // Falhas na escrita do log não interrompem a requisição do usuário
        }
    }
}