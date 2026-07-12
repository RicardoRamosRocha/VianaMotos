using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;
using VianaMotos.Web.ViewModels;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdministratorRole)]
public class VendasController : Controller
{
    private readonly AppDbContext _context;

    public VendasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vendas = await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Moto)
                .ThenInclude(m => m!.Marca)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();

        return View(vendas);
    }

    public async Task<IActionResult> Create()
    {
        await CarregarCombos();

        return View(new VendaCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VendaCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await CarregarCombos();
            return View(model);
        }

        var moto = await _context.Motos
            .FirstOrDefaultAsync(x => x.Id == model.MotoId);

        if (moto == null)
        {
            ModelState.AddModelError("", "Moto não encontrada.");
            await CarregarCombos();
            return View(model);
        }

        if (!moto.Disponivel)
        {
            ModelState.AddModelError("", "Esta moto já foi vendida.");
            await CarregarCombos();
            return View(model);
        }

        var venda = new Venda
        {
            ClienteId = model.ClienteId,
            MotoId = model.MotoId,
            ValorVenda = moto.Valor,
            DataVenda = DateTime.Now,
            Observacoes = model.Observacoes
        };

        _context.Vendas.Add(venda);

        moto.Disponivel = false;

        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Venda registrada com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CarregarCombos()
    {
        ViewBag.Clientes = new SelectList(
            await _context.Clientes
                .OrderBy(c => c.Nome)
                .ToListAsync(),
            "Id",
            "Nome");

        ViewBag.Motos = new SelectList(
            await _context.Motos
                .Where(m => m.Disponivel)
                .OrderBy(m => m.Modelo)
                .ToListAsync(),
            "Id",
            "Modelo");
    }

    [HttpGet]
    public async Task<IActionResult> ObterMoto(int id)
    {
        var moto = await _context.Motos
            .Include(m => m.Marca)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (moto == null)
            return NotFound();

        return Json(new
        {
            valor = moto.Valor,
            modelo = moto.Modelo,
            marca = moto.Marca!.Nome
        });
    }
}
