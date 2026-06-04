using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.Models;

public class Combustivel
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Nome { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public ICollection<Moto> Motos { get; set; } = new List<Moto>();
}