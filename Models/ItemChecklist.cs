using System.ComponentModel.DataAnnotations;

namespace App5s.Models;

public class ItemChecklist
{
    public int Id { get; set; }

    public Senso5S Senso { get; set; }

    [Required(ErrorMessage = "A pergunta é obrigatória.")]
    [MaxLength(300)]
    public string Pergunta { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? GuiaAvaliacao { get; set; } // O que inspecionar em campo

    public int Peso { get; set; } = 1;

    public int Ordem { get; set; }

    public bool Ativo { get; set; } = true;
}