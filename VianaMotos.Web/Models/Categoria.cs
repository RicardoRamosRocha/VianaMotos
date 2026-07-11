using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.Models;

public class Categoria
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    [StringLength(50, ErrorMessage = "O campo Nome deve ter no máximo 50 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Ativa")]
    public bool Ativa { get; set; } = true;

    public ICollection<Moto> Motos { get; set; } = new List<Moto>();
}
