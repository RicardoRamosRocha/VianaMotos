using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.ViewModels;

public class VendaCreateViewModel
{
    [Required(ErrorMessage = "Selecione um cliente.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Selecione uma moto.")]
    public int MotoId { get; set; }

    public decimal ValorMoto { get; set; }

    [StringLength(500)]
    public string? Observacoes { get; set; }
}
