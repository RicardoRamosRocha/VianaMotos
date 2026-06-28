using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using VianaMotos.Web.Data;
using VianaMotos.Web.ViewModels;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class SimuladorController : Controller
{
    private readonly AppDbContext _context;

    public SimuladorController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new SimuladorViewModel
        {
            TaxaMensal = 2,
            Parcelas = 12
        };

        await CarregarMotosAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SimuladorViewModel model)
    {
        if (!model.MotoId.HasValue)
        {
            ModelState.AddModelError(nameof(model.MotoId), "Selecione uma moto.");
        }

        if (model.Parcelas <= 0)
        {
            ModelState.AddModelError(nameof(model.Parcelas), "A quantidade de parcelas deve ser maior que zero.");
        }

        var moto = model.MotoId.HasValue
            ? await _context.Motos
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == model.MotoId.Value && m.Disponivel)
            : null;

        if (model.MotoId.HasValue && moto == null)
        {
            ModelState.AddModelError(nameof(model.MotoId), "Moto não encontrada ou indisponível.");
        }

        model.ValorMoto = moto?.Valor ?? 0;

        if (model.Entrada > model.ValorMoto)
        {
            ModelState.AddModelError(nameof(model.Entrada), "A entrada não pode ser maior que o valor da moto.");
        }

        if (model.Entrada < 0)
        {
            ModelState.AddModelError(nameof(model.Entrada), "A entrada não pode ser negativa.");
        }

        if (model.TaxaMensal < 0)
        {
            ModelState.AddModelError(nameof(model.TaxaMensal), "A taxa mensal não pode ser negativa.");
        }

        if (ModelState.IsValid)
        {
            CalcularSimulacao(model);
        }

        await CarregarMotosAsync(model);

        return View(model);
    }

    private static void CalcularSimulacao(SimuladorViewModel model)
    {
        model.ValorFinanciado = model.ValorMoto - model.Entrada;

        var taxa = model.TaxaMensal / 100;

        if (taxa > 0)
        {
            model.ValorParcela = model.ValorFinanciado * taxa /
                (1 - (decimal)Math.Pow((double)(1 + taxa), -model.Parcelas));
        }
        else
        {
            model.ValorParcela = model.ValorFinanciado / model.Parcelas;
        }

        model.TotalPago = model.ValorParcela * model.Parcelas;
        model.TotalJuros = model.TotalPago - model.ValorFinanciado;
    }

    private async Task CarregarMotosAsync(SimuladorViewModel model)
    {
        var culture = CultureInfo.GetCultureInfo("pt-BR");

        var motos = await _context.Motos
            .AsNoTracking()
            .Include(m => m.Marca)
            .Where(m => m.Disponivel)
            .OrderBy(m => m.Modelo)
            .ToListAsync();

        model.Motos = motos.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.Modelo} - {m.Marca?.Nome} - {m.Ano} - {m.Valor.ToString("C", culture)}",
            Selected = model.MotoId == m.Id
        });
    }
}
