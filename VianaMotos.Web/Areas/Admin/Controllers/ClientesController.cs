using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ClientesController : Controller
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clientes = await _context.Clientes
            .OrderBy(x => x.Nome)
            .ToListAsync();

        return View(clientes);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        if (!ModelState.IsValid)
            return View(cliente);

        cliente.DataCadastro = DateTime.Now;

        _context.Clientes.Add(cliente);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cliente cliente)
    {
        if (id != cliente.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(cliente);

        _context.Update(cliente);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cliente == null)
            return NotFound();

        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, Cliente cliente)
    {
        var clienteBanco = await _context.Clientes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (clienteBanco == null)
            return NotFound();

        _context.Clientes.Remove(clienteBanco);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}