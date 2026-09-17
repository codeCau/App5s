using System.ComponentModel.DataAnnotations;

namespace App5s.Models;

public class Setor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do setor é obrigatório.")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Descricao { get; set; }

    public int? ResponsavelId { get; set; }
    public Usuario? Responsavel { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
}