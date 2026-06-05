using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;
using VianaMotos.Web.ViewModels;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class MotosController : Controller
{
    private readonly AppDbContext _context;

    public MotosController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var motos = await _context.Motos
            .Include(x => x.Marca)
            .Include(x => x.Categoria)
            .Include(x => x.Combustivel)
            .OrderByDescending(x => x.DataCadastro)
            .ToListAsync();

        return View(motos);
    }

    public async Task<IActionResult> Create()
    {
        var vm = await CarregarViewModelAsync();

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MotoViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Marcas = (await CarregarViewModelAsync()).Marcas;
            vm.Categorias = (await CarregarViewModelAsync()).Categorias;
            vm.Combustiveis = (await CarregarViewModelAsync()).Combustiveis;

            return View(vm);
        }

        vm.Moto.DataCadastro = DateTime.Now;

        _context.Motos.Add(vm.Moto);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task<MotoViewModel> CarregarViewModelAsync()
    {
        return new MotoViewModel
        {
            Marcas = await _context.Marcas
                .Where(x => x.Ativa)
                .OrderBy(x => x.Nome)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nome
                })
                .ToListAsync(),

            Categorias = await _context.Categorias
                .Where(x => x.Ativa)
                .OrderBy(x => x.Nome)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nome
                })
                .ToListAsync(),

            Combustiveis = await _context.Combustiveis
                .Where(x => x.Ativo)
                .OrderBy(x => x.Nome)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nome
                })
                .ToListAsync()
        };
    }
}