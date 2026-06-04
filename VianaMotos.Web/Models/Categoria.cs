using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Nome { get; set; } = string.Empty;

    public bool Ativa { get; set; } = true;

    public ICollection<Moto> Motos { get; set; } = new List<Moto>();
}