using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VianaMotos.Web.Models;

public class Venda
{
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    [Required]
    public int MotoId { get; set; }

    public Moto? Moto { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorVenda { get; set; }

    public DateTime DataVenda { get; set; } = DateTime.Now;

    [StringLength(500)]
    public string? Observacoes { get; set; }
}