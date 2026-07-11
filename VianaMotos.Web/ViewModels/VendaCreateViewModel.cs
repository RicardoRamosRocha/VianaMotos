using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.ViewModels;

public class VendaCreateViewModel
{
    [Display(Name = "Cliente")]
    [Required(ErrorMessage = "Selecione um cliente.")]
    public int ClienteId { get; set; }

    [Display(Name = "Moto")]
    [Required(ErrorMessage = "Selecione uma moto.")]
    public int MotoId { get; set; }

    [Display(Name = "Valor da moto")]
    public decimal ValorMoto { get; set; }

    [Display(Name = "Observações")]
    [StringLength(500, ErrorMessage = "O campo Observações deve ter no máximo 500 caracteres.")]
    public string? Observacoes { get; set; }
}
