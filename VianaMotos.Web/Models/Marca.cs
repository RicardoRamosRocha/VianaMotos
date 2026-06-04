using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.Models;

public class Marca
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Nome { get; set; } = string.Empty;

    public bool Ativa { get; set; } = true;
}
