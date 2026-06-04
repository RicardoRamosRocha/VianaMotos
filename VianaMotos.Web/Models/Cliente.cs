using System.ComponentModel.DataAnnotations;

namespace VianaMotos.Web.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(14)]
    public string? Cpf { get; set; }

    [StringLength(20)]
    public string? Telefone { get; set; }

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(200)]
    public string? Endereco { get; set; }

    [StringLength(100)]
    public string? Cidade { get; set; }

    [StringLength(2)]
    public string? Estado { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.Now;
}