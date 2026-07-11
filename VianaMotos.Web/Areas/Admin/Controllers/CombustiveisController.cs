using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CombustiveisController : Controller
{
    private readonly AppDbContext _context;

    public CombustiveisController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var combustiveis = await _context.Combustiveis
            .OrderBy(x => x.Nome)
            .ToListAsync();

        return View(combustiveis);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Combustivel combustivel)    
    {
        if (!ModelState.IsValid)
        {
            return View(combustivel);
        }

        _context.Combustiveis.Add(combustivel);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var combustivel = await _context.Combustiveis.FindAsync(id);

        if (combustivel == null)
            return NotFound();

        return View(combustivel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Combustivel combustivel)
    {
        if (id != combustivel.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(combustivel);

        _context.Update(combustivel);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var combustivel = await _context.Combustiveis.FindAsync(id);

        if (combustivel == null)
            return NotFound();

        return View(combustivel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var combustivel = await _context.Combustiveis.FindAsync(id);

        if (combustivel != null)
        {
            _context.Combustiveis.Remove(combustivel);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
