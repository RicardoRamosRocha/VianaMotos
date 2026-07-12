using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VianaMotos.Web.Models;

public class Venda
{
    public int Id { get; set; }

    [Display(Name = "Cliente")]
    [Required(ErrorMessage = "Selecione um cliente.")]
    public int ClienteId { get; set; }

    public Cliente? Cliente { get; set; }

    [Display(Name = "Moto")]
    [Required(ErrorMessage = "Selecione uma moto.")]
    public int MotoId { get; set; }

    public Moto? Moto { get; set; }

    [Display(Name = "Valor da venda")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorVenda { get; set; }

    [Display(Name = "Data da venda")]
    public DateTime DataVenda { get; set; } = DateTime.UtcNow;

    [Display(Name = "Observações")]
    [StringLength(500, ErrorMessage = "O campo Observações deve ter no máximo 500 caracteres.")]
    public string? Observacoes { get; set; }
}
