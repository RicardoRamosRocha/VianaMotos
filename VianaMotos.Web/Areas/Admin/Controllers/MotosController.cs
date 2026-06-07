using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;
using VianaMotos.Web.ViewModels;
using System.IO;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class MotosController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _context;

    public MotosController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // LISTA
    public async Task<IActionResult> Index()
    {
        var motos = await _context.Motos
          .Include(x => x.Marca)
          .Include(x => x.Categoria)
          .Include(x => x.Combustivel)
          .Include(x => x.Fotos)
          .OrderByDescending(x => x.DataCadastro)
          .ToListAsync();
        return View(motos);
    }

    // CREATE GET
    public async Task<IActionResult> Create()
    {
        var vm = await CarregarViewModelAsync();
        return View(vm);
    }

    // CREATE POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MotoViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var listas = await CarregarViewModelAsync();

            vm.Marcas = listas.Marcas;
            vm.Categorias = listas.Categorias;
            vm.Combustiveis = listas.Combustiveis;

            return View(vm);
        }

        vm.Moto.DataCadastro = DateTime.Now;

        // 1. SALVA MOTO PRIMEIRO (para gerar ID)
        _context.Motos.Add(vm.Moto);
        await _context.SaveChangesAsync();

        // 2. UPLOAD MÚLTIPLO DE FOTOS
        if (vm.ArquivosFotos != null && vm.ArquivosFotos.Count > 0)
        {
            var pastaUploads = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "motos");

            Directory.CreateDirectory(pastaUploads);

            int ordem = 1;

            foreach (var foto in vm.ArquivosFotos)
            {
                var nomeArquivo =
                    $"{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}";

                var caminhoArquivo = Path.Combine(pastaUploads, nomeArquivo);

                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                _context.FotosMoto.Add(new FotoMoto
                {
                    MotoId = vm.Moto.Id,
                    CaminhoImagem = nomeArquivo,
                    Principal = ordem == 1,
                    Ordem = ordem++
                });
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // EDIT GET
    public async Task<IActionResult> Edit(int id)
    {
        var moto = await _context.Motos
          .Include(x => x.Fotos)
          .FirstOrDefaultAsync(x => x.Id == id);

        if (moto == null)
            return NotFound();

        var vm = await CarregarViewModelAsync();
        vm.Moto = moto;

        return View(vm);
    }

    // EDIT POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MotoViewModel vm)
    {
        Console.WriteLine("EDIT EXECUTOU");
        if (id != vm.Moto.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            var listas = await CarregarViewModelAsync();

            vm.Marcas = listas.Marcas;
            vm.Categorias = listas.Categorias;
            vm.Combustiveis = listas.Combustiveis;

            return View(vm);
        }

        var motoBanco = await _context.Motos
            .Include(x => x.Fotos)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (motoBanco == null)
            return NotFound();

        // Atualiza dados
        motoBanco.Modelo = vm.Moto.Modelo;
        motoBanco.MarcaId = vm.Moto.MarcaId;
        motoBanco.CategoriaId = vm.Moto.CategoriaId;
        motoBanco.CombustivelId = vm.Moto.CombustivelId;
        motoBanco.Ano = vm.Moto.Ano;
        motoBanco.Cor = vm.Moto.Cor;
        motoBanco.Quilometragem = vm.Moto.Quilometragem;
        motoBanco.Valor = vm.Moto.Valor;
        motoBanco.Descricao = vm.Moto.Descricao;
        motoBanco.Disponivel = vm.Moto.Disponivel;

        Console.WriteLine("EDIT EXECUTOU");

        Console.WriteLine(
            vm.ArquivosFotos == null
                ? "ArquivosFotos = NULL"
                : $"ArquivosFotos = {vm.ArquivosFotos.Count}");


            // Upload múltiplo
            if (vm.ArquivosFotos != null && vm.ArquivosFotos.Count > 0)
        {
            var pastaUploads = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "motos");

            Directory.CreateDirectory(pastaUploads);

            int ordem = (motoBanco.Fotos?.Count ?? 0) + 1;

            foreach (var foto in vm.ArquivosFotos)
            {
                var nomeArquivo =
                    $"{Guid.NewGuid()}{Path.GetExtension(foto.FileName)}";

                var caminhoArquivo = Path.Combine(pastaUploads, nomeArquivo);

                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                _context.FotosMoto.Add(new FotoMoto
                {
                    MotoId = motoBanco.Id,
                    CaminhoImagem = nomeArquivo,
                    Principal = false,
                    Ordem = ordem++
                });
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // DETAILS
    public async Task<IActionResult> Details(int id)
    {
        var moto = await _context.Motos
            .Include(x => x.Marca)
            .Include(x => x.Categoria)
            .Include(x => x.Combustivel)
            .Include(x => x.Fotos.OrderBy(f => f.Ordem))
            .FirstOrDefaultAsync(x => x.Id == id);

        if (moto == null)
            return NotFound();

        return View(moto);
    }

    // DELETE GET
    public async Task<IActionResult> Delete(int id)
    {
        var moto = await _context.Motos
            .Include(x => x.Marca)
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (moto == null)
            return NotFound();

        return View(moto);
    }

    // DELETE POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var moto = await _context.Motos.FindAsync(id);

        if (moto != null)
        {
            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // CARREGAR DROPDOWNS
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

    public async Task<IActionResult> Fotos(int id)
    {
        var moto = await _context.Motos
            .Include(x => x.Fotos)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (moto == null)
            return NotFound();

        return View(moto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdicionarFotos(
    int id,
    List<IFormFile> arquivos)
    {
        var moto = await _context.Motos
            .Include(x => x.Fotos)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (moto == null)
            return NotFound();

        if (arquivos != null && arquivos.Count > 0)
        {
            var pastaUploads = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "motos");

            Console.WriteLine("WEBROOT:");
            Console.WriteLine(_environment.WebRootPath);

            Console.WriteLine("UPLOAD:");
            Console.WriteLine(pastaUploads);

            Directory.CreateDirectory(pastaUploads);

            int ordem = moto.Fotos.Count + 1;

            foreach (var arquivo in arquivos)
            {
                var nomeArquivo =
                    $"{Guid.NewGuid()}{Path.GetExtension(arquivo.FileName)}";

                var caminhoArquivo =
                    Path.Combine(pastaUploads, nomeArquivo);

                using (var stream = new FileStream(
                    caminhoArquivo,
                    FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                _context.FotosMoto.Add(new FotoMoto
                {
                    MotoId = moto.Id,
                    CaminhoImagem = nomeArquivo,
                    Principal = false,
                    Ordem = ordem++
                });
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Fotos), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DefinirPrincipal(
    int fotoId,
    int motoId)
    {
        var fotos = await _context.FotosMoto
            .Where(x => x.MotoId == motoId)
            .ToListAsync();

        foreach (var foto in fotos)
        {
            foto.Principal = false;
        }

        var fotoPrincipal = fotos
            .FirstOrDefault(x => x.Id == fotoId);

        if (fotoPrincipal != null)
        {
            fotoPrincipal.Principal = true;

            var moto = await _context.Motos
                .FirstOrDefaultAsync(x => x.Id == motoId);

            if (moto != null)
            {
                moto.FotoPrincipal =
                    fotoPrincipal.CaminhoImagem;
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Fotos),
            new { id = motoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirFoto(
    int fotoId,
    int motoId)
    {
        var foto = await _context.FotosMoto
            .FirstOrDefaultAsync(x => x.Id == fotoId);

        if (foto == null)
            return RedirectToAction(nameof(Fotos),
                new { id = motoId });

        // Apagar arquivo físico
        var caminhoArquivo = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "motos",
            foto.CaminhoImagem);

        if (System.IO.File.Exists(caminhoArquivo))
            System.IO.File.Delete(caminhoArquivo);

        bool eraPrincipal = foto.Principal;

        _context.FotosMoto.Remove(foto);

        await _context.SaveChangesAsync();

        // Se excluiu a principal, escolhe outra
        if (eraPrincipal)
        {
            var novaPrincipal = await _context.FotosMoto
                .Where(x => x.MotoId == motoId)
                .OrderBy(x => x.Ordem)
                .FirstOrDefaultAsync();

            if (novaPrincipal != null)
            {
                novaPrincipal.Principal = true;

                var moto = await _context.Motos
                    .FindAsync(motoId);

                if (moto != null)
                {
                    moto.FotoPrincipal =
                        novaPrincipal.CaminhoImagem;
                }

                await _context.SaveChangesAsync();
            }
        }

        return RedirectToAction(nameof(Fotos),
            new { id = motoId });
    }
}