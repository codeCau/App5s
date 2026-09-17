using System.ComponentModel.DataAnnotations;

namespace App5s.Models;

public class RespostaAuditoria
{
    public int Id { get; set; }

    public int AuditoriaId { get; set; }
    public Auditoria Auditoria { get; set; } = null!;

    public int ItemChecklistId { get; set; }
    public ItemChecklist ItemChecklist { get; set; } = null!;

    [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
    public int Nota { get; set; }

    [MaxLength(500)]
    public string? ObservacaoNaoConformidade { get; set; }

    [MaxLength(2000)]
    public string? EvidenciaFotoUrl { get; set; }
}