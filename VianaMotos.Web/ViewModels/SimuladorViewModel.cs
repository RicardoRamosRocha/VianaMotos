using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.ViewModels;

public class SimuladorViewModel
{
    [Display(Name = "Moto")]
    public int? MotoId { get; set; }

    [Display(Name = "Valor da moto")]
    public decimal ValorMoto { get; set; }

    [Display(Name = "Entrada")]
    public decimal Entrada { get; set; }

    [Display(Name = "Parcelas")]
    public int Parcelas { get; set; }

    [Display(Name = "Taxa mensal (%)")]
    public decimal TaxaMensal { get; set; }

    [Display(Name = "Valor financiado")]
    public decimal ValorFinanciado { get; set; }

    [Display(Name = "Valor da parcela")]
    public decimal ValorParcela { get; set; }

    [Display(Name = "Total pago")]
    public decimal TotalPago { get; set; }

    [Display(Name = "Total de juros")]
    public decimal TotalJuros { get; set; }

    public IEnumerable<SelectListItem> Motos { get; set; } = new List<SelectListItem>();
}
