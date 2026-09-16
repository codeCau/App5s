using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App5s.Models;

[Table("system_logs")]
public class LogSistema
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("level")]
    public string Nivel { get; set; } = "INFO";

    [Column("message")]
    public string Mensagem { get; set; } = string.Empty;

    [Column("source")]
    public string? Origem { get; set; }

    [Column("user_email")]
    public string? UsuarioEmail { get; set; }

    [Column("details")]
    public string? Detalhes { get; set; }

    [Column("created_at")]
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
}