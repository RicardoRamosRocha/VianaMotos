using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;

namespace VianaMotos.Web.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var motos = await _context.Motos
            .Include(x => x.Marca)
            .Include(x => x.Categoria)
            .Include(x => x.Fotos)
            .Where(x => x.Disponivel)
            .OrderByDescending(x => x.DataCadastro)
            .ToListAsync();

        return View(motos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var moto = await _context.Motos
            .Include(x => x.Marca)
            .Include(x => x.Categoria)
            .Include(x => x.Combustivel)
            .Include(x => x.Fotos)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (moto == null)
            return NotFound();

        return View(moto);
    }
}