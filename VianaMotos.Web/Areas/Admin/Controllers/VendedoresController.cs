using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdministratorRole)]
public class VendedoresController : Controller
{
    private readonly AppDbContext _context;

    public VendedoresController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vendedores = await _context.Vendedores
            .OrderBy(x => x.Nome)
            .ToListAsync();

        return View(vendedores);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Vendedor vendedor)
    {
        if (!ModelState.IsValid)
            return View(vendedor);

        vendedor.DataCadastro = DateTime.UtcNow;

        _context.Vendedores.Add(vendedor);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Vendedor cadastrado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var vendedor = await _context.Vendedores.FindAsync(id);

        if (vendedor == null)
            return NotFound();

        return View(vendedor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vendedor vendedor)
    {
        if (id != vendedor.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(vendedor);

        var vendedorBanco = await _context.Vendedores
            .FirstOrDefaultAsync(x => x.Id == id);

        if (vendedorBanco == null)
            return NotFound();

        vendedorBanco.Nome = vendedor.Nome;
        vendedorBanco.Cpf = vendedor.Cpf;
        vendedorBanco.Telefone = vendedor.Telefone;
        vendedorBanco.Email = vendedor.Email;
        vendedorBanco.Ativo = vendedor.Ativo;

        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Vendedor atualizado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var vendedor = await _context.Vendedores
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (vendedor == null)
            return NotFound();

        return View(vendedor);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var vendedor = await _context.Vendedores
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (vendedor == null)
            return NotFound();

        return View(vendedor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vendedor = await _context.Vendedores.FindAsync(id);

        if (vendedor == null)
            return RedirectToAction(nameof(Index));

        _context.Vendedores.Remove(vendedor);
        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Vendedor excluído com sucesso.";
        return RedirectToAction(nameof(Index));
    }
}
