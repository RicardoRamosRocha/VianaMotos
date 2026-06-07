using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalMotos =
            await _context.Motos.CountAsync();

        ViewBag.Disponiveis =
            await _context.Motos
                .CountAsync(x => x.Disponivel);

        ViewBag.Vendidas =
            await _context.Motos
                .CountAsync(x => !x.Disponivel);

        ViewBag.Marcas =
            await _context.Marcas.CountAsync();

        ViewBag.Faturamento =
            await _context.Vendas
           .SumAsync(v => v.ValorVenda);
       
        
        return View();

       
    }
}