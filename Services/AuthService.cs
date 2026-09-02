using App5s.Data;
using App5s.Models;
using Microsoft.EntityFrameworkCore;

namespace App5s.Services;

public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AuthService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Usuario?> ValidarCredenciaisAsync(string email, string senha)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var usuario = await context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower() && u.Ativo);

        if (usuario is null)
            return null;

        bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);
        return senhaValida ? usuario : null;
    }

    public string GerarHashSenha(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);
}