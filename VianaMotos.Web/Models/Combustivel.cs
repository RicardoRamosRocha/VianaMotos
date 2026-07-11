using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.Models;

public class Combustivel
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    [StringLength(30, ErrorMessage = "O campo Nome deve ter no máximo 30 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    public ICollection<Moto> Motos { get; set; } = new List<Moto>();
}
