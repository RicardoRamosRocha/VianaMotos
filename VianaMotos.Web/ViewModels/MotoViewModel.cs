using Microsoft.AspNetCore.Mvc.Rendering;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.ViewModels;

public class MotoViewModel
{
    public Moto Moto { get; set; } = new();

    public IEnumerable<SelectListItem> Marcas { get; set; }
        = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> Categorias { get; set; }
        = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> Combustiveis { get; set; }
        = Enumerable.Empty<SelectListItem>();
}