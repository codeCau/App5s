using System.ComponentModel.DataAnnotations;

namespace App5s.Models;

public class Auditoria
{
    public int Id { get; set; }

    public int SetorId { get; set; }
    public Setor Setor { get; set; } = null!;

    public int AuditorId { get; set; }
    public Usuario Auditor { get; set; } = null!;

    public DateTime DataRealizacao { get; set; } = DateTime.UtcNow;

    [MaxLength(30)]
    public string Status { get; set; } = "EmAndamento"; // EmAndamento, Concluida, Cancelada

    public decimal PontuacaoGeral { get; set; } // Nota percentual (0.00% a 100.00%)

    [MaxLength(1000)]
    public string? ObservacoesGerais { get; set; }

    public ICollection<RespostaAuditoria> Respostas { get; set; } = new List<RespostaAuditoria>();
}