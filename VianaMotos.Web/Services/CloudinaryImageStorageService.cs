using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace VianaMotos.Web.Services;

public sealed class CloudinaryImageStorageService : IImageStorageService
{
    private const string UploadFolder = "viana-motos/motos";

    private readonly Cloudinary? _cloudinary;
    private readonly ILogger<CloudinaryImageStorageService> _logger;

    public CloudinaryImageStorageService(
        IConfiguration configuration,
        ILogger<CloudinaryImageStorageService> logger)
    {
        _logger = logger;

        var cloudinaryUrl = configuration["CLOUDINARY_URL"];

        if (!string.IsNullOrWhiteSpace(cloudinaryUrl))
        {
            _cloudinary = new Cloudinary(cloudinaryUrl)
            {
                Api = { Secure = true }
            };
        }
    }

    public async Task<StoredImageResult> UploadAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var cloudinary = ObterCloudinaryConfigurado();

        await using var stream = file.OpenReadStream();

        var publicId = $"{UploadFolder}/{Guid.NewGuid():N}";

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = publicId,
            Overwrite = false,
            UniqueFilename = false,
            UseFilename = false
        };

        var result = await cloudinary.UploadAsync(
            uploadParams,
            cancellationToken);

        if (result.Error is not null || result.SecureUrl is null)
        {
            throw new InvalidOperationException(
                result.Error?.Message ??
                "O Cloudinary não retornou a URL da imagem.");
        }

        return new StoredImageResult(
            result.SecureUrl.AbsoluteUri,
            result.PublicId);
    }

    public async Task DeleteAsync(
        string? imageUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageUrl) ||
            !Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) ||
            !uri.Host.EndsWith(
                "cloudinary.com",
                StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var publicId = ExtrairPublicId(uri);

        if (string.IsNullOrWhiteSpace(publicId))
        {
            _logger.LogWarning(
                "Não foi possível identificar o PublicId da imagem {ImageUrl}.",
                imageUrl);
            return;
        }

        var cloudinary = ObterCloudinaryConfigurado();

        var result = await cloudinary.DestroyAsync(new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image,
            Invalidate = true
        });

        if (result.Result is not ("ok" or "not found"))
        {
            _logger.LogWarning(
                "Cloudinary não confirmou a exclusão de {PublicId}. Resultado: {Result}.",
                publicId,
                result.Result);
        }
    }

    private Cloudinary ObterCloudinaryConfigurado()
    {
        return _cloudinary ?? throw new InvalidOperationException(
            "O armazenamento de imagens não está configurado. " +
            "Cadastre a variável CLOUDINARY_URL no ambiente da aplicação.");
    }

    private static string? ExtrairPublicId(Uri uri)
    {
        var partes = uri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        var indiceUpload = Array.FindIndex(
            partes,
            parte => parte.Equals(
                "upload",
                StringComparison.OrdinalIgnoreCase));

        if (indiceUpload < 0 || indiceUpload + 1 >= partes.Length)
            return null;

        var inicio = indiceUpload + 1;

        if (partes[inicio].Length > 1 &&
            partes[inicio][0] == 'v' &&
            long.TryParse(partes[inicio][1..], out _))
        {
            inicio++;
        }

        if (inicio >= partes.Length)
            return null;

        var publicId = string.Join('/', partes[inicio..]);
        var extensao = Path.GetExtension(publicId);

        return string.IsNullOrWhiteSpace(extensao)
            ? publicId
            : publicId[..^extensao.Length];
    }
}
