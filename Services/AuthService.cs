using App5s.Data;
using App5s.Models;
using Microsoft.EntityFrameworkCore;
using Isopoh.Cryptography.Argon2;

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

        bool senhaValida = Argon2.Verify(senha, usuario.SenhaHash);
        return senhaValida ? usuario : null;}


    public string GerarHashSenha(string senha)
{
    var config = new Argon2Config
    {   
        Type = Argon2Type.DataIndependentAddressing,
        Version = Argon2Version.Nineteen,
        TimeCost = 3,          
        MemoryCost = 65536,    
        Lanes = 4,            
        Threads = Environment.ProcessorCount,
        Password = System.Text.Encoding.UTF8.GetBytes(senha),
        Salt = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16)
    };

    using var argon2 = new Argon2(config);
    using var hash = argon2.Hash();
    return config.EncodeString(hash.Buffer);
}
}