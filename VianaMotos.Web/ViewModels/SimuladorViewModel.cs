using Microsoft.AspNetCore.Mvc.Rendering;

namespace VianaMotos.Web.ViewModels;

public class SimuladorViewModel
{
    public int? MotoId { get; set; }

    public decimal ValorMoto { get; set; }

    public decimal Entrada { get; set; }

    public int Parcelas { get; set; }

    public decimal TaxaMensal { get; set; }

    public decimal ValorFinanciado { get; set; }

    public decimal ValorParcela { get; set; }

    public decimal TotalPago { get; set; }

    public decimal TotalJuros { get; set; }

    public IEnumerable<SelectListItem> Motos { get; set; } = new List<SelectListItem>();
}
