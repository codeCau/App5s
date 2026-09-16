using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using App5s.Data;
using App5s.Models;

namespace App5s.Services;

public class UsuarioService
{
    private readonly AppDbContext _db;

    public UsuarioService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Usuario>> ObterTodosAsync()
    {
        return await _db.Usuarios
            .OrderByDescending(u => u.CriadoEm)
            .ToListAsync();
    }

    public async Task<(bool Sucesso, string? Erro)> CriarUsuarioAsync(NovoUsuarioDto dto)
    {
        var emailNormalizado = dto.Email.Trim().ToLowerInvariant();

        // 1. Verifica se o e-mail já existe
        var jaExiste = await _db.Usuarios.AnyAsync(u => u.Email.ToLower() == emailNormalizado);
        if (jaExiste)
        {
            return (false, "Já existe um usuário cadastrado com este e-mail.");
        }

        // 2. Gera o hash Argon2id da senha informada
        var hash = Argon2.Hash(dto.Senha);

        // 3. Monta e persiste a entidade
        var usuario = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Email = emailNormalizado,
            SenhaHash = hash,
            Perfil = dto.Perfil,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return (true, null);
    }
}