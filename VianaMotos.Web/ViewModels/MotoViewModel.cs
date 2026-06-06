using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.ViewModels;

public class MotoViewModel
{
    public Moto Moto { get; set; } = new();

    // 🔥 MÚLTIPLAS FOTOS
    public List<IFormFile>? ArquivosFotos { get; set; }

    public IEnumerable<SelectListItem> Marcas { get; set; }
        = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> Categorias { get; set; }
        = Enumerable.Empty<SelectListItem>();

    public IEnumerable<SelectListItem> Combustiveis { get; set; }
        = Enumerable.Empty<SelectListItem>();
}