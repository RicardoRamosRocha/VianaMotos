using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.Models;

public class Vendedor
{
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O campo Nome é obrigatório.")]
    [StringLength(150, ErrorMessage = "O campo Nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "CPF")]
    [StringLength(14, ErrorMessage = "O campo CPF deve ter no máximo 14 caracteres.")]
    public string? Cpf { get; set; }

    [Display(Name = "Telefone")]
    [StringLength(20, ErrorMessage = "O campo Telefone deve ter no máximo 20 caracteres.")]
    public string? Telefone { get; set; }

    [Display(Name = "E-mail")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(150, ErrorMessage = "O campo E-mail deve ter no máximo 150 caracteres.")]
    public string? Email { get; set; }

    [Display(Name = "Data de cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}
