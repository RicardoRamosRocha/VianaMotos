using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdministratorRole)]
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

        cliente.DataCadastro = DateTime.UtcNow;

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

        var clienteBanco = await _context.Clientes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (clienteBanco == null)
            return NotFound();

        clienteBanco.Nome = cliente.Nome;
        clienteBanco.Cpf = cliente.Cpf;
        clienteBanco.Telefone = cliente.Telefone;
        clienteBanco.Email = cliente.Email;
        clienteBanco.Endereco = cliente.Endereco;
        clienteBanco.Cidade = cliente.Cidade;
        clienteBanco.Estado = cliente.Estado;
        clienteBanco.Ativo = cliente.Ativo;

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
