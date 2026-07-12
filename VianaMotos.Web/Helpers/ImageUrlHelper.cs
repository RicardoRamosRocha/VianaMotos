namespace VianaMotos.Web.Helpers;

public static class ImageUrlHelper
{
    public static string ObterUrl(string? caminhoImagem)
    {
        if (string.IsNullOrWhiteSpace(caminhoImagem))
            return "/images/sem-imagem.png";

        if (Uri.TryCreate(caminhoImagem, UriKind.Absolute, out _))
            return caminhoImagem;

        return $"/uploads/motos/{caminhoImagem.TrimStart('/')}";
    }
}
