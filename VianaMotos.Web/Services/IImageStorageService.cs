namespace VianaMotos.Web.Services;

public sealed record StoredImageResult(string Url, string PublicId);

public interface IImageStorageService
{
    Task<StoredImageResult> UploadAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string? imageUrl,
        CancellationToken cancellationToken = default);
}
