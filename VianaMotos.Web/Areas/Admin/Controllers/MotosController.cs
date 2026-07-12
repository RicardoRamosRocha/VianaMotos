using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VianaMotos.Web.Data;
using VianaMotos.Web.Models;
using VianaMotos.Web.Services;
using VianaMotos.Web.ViewModels;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdministratorRole)]
public class MotosController : Controller
{
    private readonly AppDbContext _context;
    private readonly IImageStorageService _imageStorage;
    private readonly ILogger<MotosController> _logger;

    private const long TamanhoMaximoImagem = 10 * 1024 * 1024; // 10 MB

    private static readonly string[] ExtensoesPermitidas =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    public MotosController(
        AppDbContext context,
        IImageStorageService imageStorage,
        ILogger<MotosController> logger)
    {
        _context = context;
        _imageStorage = imageStorage;
        _logger = logger;
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
        ValidarArquivos(vm.ArquivosFotos);

        if (!ModelState.IsValid)
        {
            await RecarregarListasAsync(vm);
            return View(vm);
        }

        // PostgreSQL timestamp with time zone exige UTC.
        vm.Moto.DataCadastro = DateTime.UtcNow;

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var imagensEnviadas = new List<string>();

        try
        {
            // Primeiro salva a moto para gerar o ID.
            _context.Motos.Add(vm.Moto);
            await _context.SaveChangesAsync();

            if (vm.ArquivosFotos is { Count: > 0 })
            {
                var ordem = 1;

                foreach (var foto in vm.ArquivosFotos)
                {
                    if (foto.Length == 0)
                        continue;

                    var principal = ordem == 1;
                    var upload = await _imageStorage.UploadAsync(
                        foto,
                        HttpContext.RequestAborted);

                    imagensEnviadas.Add(upload.Url);

                    _context.FotosMoto.Add(new FotoMoto
                    {
                        MotoId = vm.Moto.Id,
                        CaminhoImagem = upload.Url,
                        Principal = principal,
                        Ordem = ordem++
                    });

                    if (principal)
                    {
                        vm.Moto.FotoPrincipal = upload.Url;
                    }
                }

                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();

            TempData["Sucesso"] = "Moto cadastrada com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            foreach (var imagemUrl in imagensEnviadas)
            {
                await ExcluirImagemComSegurancaAsync(imagemUrl);
            }

            _logger.LogError(ex, "Erro ao cadastrar a moto.");

            ModelState.AddModelError(
                string.Empty,
                "Não foi possível cadastrar a moto. Verifique os dados e tente novamente.");

            await RecarregarListasAsync(vm);

            return View(vm);
        }
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
        if (id != vm.Moto.Id)
            return NotFound();

        ValidarArquivos(vm.ArquivosFotos);

        if (!ModelState.IsValid)
        {
            await RecarregarListasAsync(vm);
            return View(vm);
        }

        var motoBanco = await _context.Motos
            .Include(x => x.Fotos)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (motoBanco == null)
            return NotFound();

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var imagensEnviadas = new List<string>();

        try
        {
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

            if (vm.ArquivosFotos is { Count: > 0 })
            {
                var ordem = (motoBanco.Fotos?.Count ?? 0) + 1;

                var definirPrimeiraComoPrincipal =
                    motoBanco.Fotos == null ||
                    !motoBanco.Fotos.Any();

                foreach (var foto in vm.ArquivosFotos)
                {
                    if (foto.Length == 0)
                        continue;

                    var upload = await _imageStorage.UploadAsync(
                        foto,
                        HttpContext.RequestAborted);

                    imagensEnviadas.Add(upload.Url);

                    _context.FotosMoto.Add(new FotoMoto
                    {
                        MotoId = motoBanco.Id,
                        CaminhoImagem = upload.Url,
                        Principal = definirPrimeiraComoPrincipal,
                        Ordem = ordem++
                    });

                    if (definirPrimeiraComoPrincipal)
                    {
                        motoBanco.FotoPrincipal = upload.Url;
                        definirPrimeiraComoPrincipal = false;
                    }
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Sucesso"] = "Moto atualizada com sucesso.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            foreach (var imagemUrl in imagensEnviadas)
            {
                await ExcluirImagemComSegurancaAsync(imagemUrl);
            }

            _logger.LogError(ex, "Erro ao atualizar a moto {MotoId}.", id);

            ModelState.AddModelError(
                string.Empty,
                "Não foi possível atualizar a moto. Tente novamente.");

            await RecarregarListasAsync(vm);

            return View(vm);
        }
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
            .Include(x => x.Fotos)
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
        var moto = await _context.Motos
            .Include(x => x.Fotos)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (moto == null)
            return RedirectToAction(nameof(Index));

        var imagensParaExcluir = moto.Fotos
            .Select(foto => foto.CaminhoImagem)
            .ToList();

        _context.Motos.Remove(moto);
        await _context.SaveChangesAsync();

        foreach (var imagemUrl in imagensParaExcluir)
        {
            await ExcluirImagemComSegurancaAsync(imagemUrl);
        }

        TempData["Sucesso"] = "Moto excluída com sucesso.";

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

    private async Task RecarregarListasAsync(MotoViewModel vm)
    {
        var listas = await CarregarViewModelAsync();

        vm.Marcas = listas.Marcas;
        vm.Categorias = listas.Categorias;
        vm.Combustiveis = listas.Combustiveis;
    }

    // GALERIA DE FOTOS
    public async Task<IActionResult> Fotos(int id)
    {
        var moto = await _context.Motos
            .Include(x => x.Fotos.OrderBy(x => x.Ordem))
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

        ValidarArquivos(arquivos);

        if (!ModelState.IsValid)
        {
            TempData["Erro"] =
                "Uma ou mais imagens são inválidas. Use JPG, JPEG, PNG ou WEBP com até 10 MB.";

            return RedirectToAction(nameof(Fotos), new { id });
        }

        if (arquivos.Count == 0)
        {
            TempData["Erro"] = "Selecione pelo menos uma imagem.";

            return RedirectToAction(nameof(Fotos), new { id });
        }

        var imagensEnviadas = new List<string>();

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var ordem = moto.Fotos.Count + 1;
            var definirPrimeiraComoPrincipal = !moto.Fotos.Any();

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Length == 0)
                    continue;

                var upload = await _imageStorage.UploadAsync(
                    arquivo,
                    HttpContext.RequestAborted);

                imagensEnviadas.Add(upload.Url);

                _context.FotosMoto.Add(new FotoMoto
                {
                    MotoId = moto.Id,
                    CaminhoImagem = upload.Url,
                    Principal = definirPrimeiraComoPrincipal,
                    Ordem = ordem++
                });

                if (definirPrimeiraComoPrincipal)
                {
                    moto.FotoPrincipal = upload.Url;
                    definirPrimeiraComoPrincipal = false;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Sucesso"] = "Fotos adicionadas com sucesso.";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            foreach (var imagemUrl in imagensEnviadas)
            {
                await ExcluirImagemComSegurancaAsync(imagemUrl);
            }

            _logger.LogError(ex, "Erro ao adicionar fotos à moto {MotoId}.", id);

            TempData["Erro"] =
                "Não foi possível adicionar as fotos.";
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

        var fotoPrincipal = fotos
            .FirstOrDefault(x => x.Id == fotoId);

        if (fotoPrincipal == null)
        {
            TempData["Erro"] = "Foto não encontrada.";

            return RedirectToAction(
                nameof(Fotos),
                new { id = motoId });
        }

        foreach (var foto in fotos)
        {
            foto.Principal = foto.Id == fotoId;
        }

        var moto = await _context.Motos
            .FirstOrDefaultAsync(x => x.Id == motoId);

        if (moto != null)
        {
            moto.FotoPrincipal = fotoPrincipal.CaminhoImagem;
        }

        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Foto principal atualizada.";

        return RedirectToAction(
            nameof(Fotos),
            new { id = motoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExcluirFoto(
        int fotoId,
        int motoId)
    {
        var foto = await _context.FotosMoto
            .FirstOrDefaultAsync(x =>
                x.Id == fotoId &&
                x.MotoId == motoId);

        if (foto == null)
        {
            TempData["Erro"] = "Foto não encontrada.";

            return RedirectToAction(
                nameof(Fotos),
                new { id = motoId });
        }

        var eraPrincipal = foto.Principal;
        var imagemParaExcluir = foto.CaminhoImagem;

        _context.FotosMoto.Remove(foto);
        await _context.SaveChangesAsync();

        await ExcluirImagemComSegurancaAsync(imagemParaExcluir);

        if (eraPrincipal)
        {
            var novaPrincipal = await _context.FotosMoto
                .Where(x => x.MotoId == motoId)
                .OrderBy(x => x.Ordem)
                .FirstOrDefaultAsync();

            var moto = await _context.Motos
                .FindAsync(motoId);

            if (moto != null)
            {
                moto.FotoPrincipal =
                    novaPrincipal?.CaminhoImagem;
            }

            if (novaPrincipal != null)
            {
                novaPrincipal.Principal = true;
            }

            await _context.SaveChangesAsync();
        }

        TempData["Sucesso"] = "Foto excluída com sucesso.";

        return RedirectToAction(
            nameof(Fotos),
            new { id = motoId });
    }

    private async Task ExcluirImagemComSegurancaAsync(string? imagemUrl)
    {
        try
        {
            await _imageStorage.DeleteAsync(
                imagemUrl,
                HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Não foi possível excluir a imagem remota {ImageUrl}.",
                imagemUrl);
        }
    }

    private void ValidarArquivos(
        IEnumerable<IFormFile>? arquivos)
    {
        if (arquivos == null)
            return;

        foreach (var arquivo in arquivos)
        {
            if (arquivo.Length == 0)
                continue;

            var extensao = Path
                .GetExtension(arquivo.FileName)
                .ToLowerInvariant();

            if (!ExtensoesPermitidas.Contains(extensao))
            {
                ModelState.AddModelError(
                    nameof(MotoViewModel.ArquivosFotos),
                    $"O arquivo \"{arquivo.FileName}\" possui formato inválido. " +
                    "Envie imagens JPG, JPEG, PNG ou WEBP.");
            }

            if (arquivo.Length > TamanhoMaximoImagem)
            {
                ModelState.AddModelError(
                    nameof(MotoViewModel.ArquivosFotos),
                    $"A imagem \"{arquivo.FileName}\" ultrapassa o limite de 10 MB.");
            }

            if (string.IsNullOrWhiteSpace(arquivo.ContentType) ||
                !arquivo.ContentType.StartsWith(
                    "image/",
                    StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    nameof(MotoViewModel.ArquivosFotos),
                    $"O arquivo \"{arquivo.FileName}\" não foi reconhecido como imagem.");
            }
        }
    }
}
