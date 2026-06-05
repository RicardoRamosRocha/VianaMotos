using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class MarcasController : Controller
{
    private readonly AppDbContext _context;

    public MarcasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var marcas = await _context.Marcas
            .OrderBy(x => x.Nome)
            .ToListAsync();

        return View(marcas);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Marca marca)
    {
        if (!ModelState.IsValid)
        {
            var erros = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Content(string.Join(" | ", erros));
        }

        _context.Marcas.Add(marca);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var marca = await _context.Marcas.FindAsync(id);

        if (marca == null)
            return NotFound();

        return View(marca);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Marca marca)
    {
        if (id != marca.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(marca);

        _context.Update(marca);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var marca = await _context.Marcas.FindAsync(id);

        if (marca == null)
            return NotFound();

        return View(marca);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var marca = await _context.Marcas.FindAsync(id);

        if (marca != null)
        {
            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
