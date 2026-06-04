using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.Models;

public class FotoMoto
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string CaminhoImagem { get; set; } = string.Empty;

    public bool Principal { get; set; }

    public int Ordem { get; set; }

    public int MotoId { get; set; }

    public Moto? Moto { get; set; }
}