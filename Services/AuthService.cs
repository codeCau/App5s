using App5s.Data;
using App5s.Models;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;

namespace App5s.Services;

public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly LogService _logService;

    public AuthService(IDbContextFactory<AppDbContext> contextFactory, LogService logService)
    {
        _contextFactory = contextFactory;
        _logService = logService;
    }

    public async Task<Usuario?> ValidarCredenciaisAsync(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            return null;

        await using var context = await _contextFactory.CreateDbContextAsync();

        var emailNormalizado = email.Trim().ToLower();

        var usuario = await context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == emailNormalizado && u.Ativo);

        if (usuario is null || string.IsNullOrEmpty(usuario.SenhaHash))
        {
        await _logService.RegistrarAsync(
                "WARNING", 
                "Tentativa de login com usuário inexistente ou inativo", 
                origem: "AuthService", 
                usuarioEmail: emailNormalizado
            );
            return null;
        }

        try
        {
            bool senhaValida = Argon2.Verify(usuario.SenhaHash, senha);
            return senhaValida ? usuario : null;
        }
        catch
        {
            return null;
        }
    }

    public string GerarHashSenha(string senha)
    {
        return Argon2.Hash(senha);
    }
}