using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VianaMotos.Web.Models;

public class Moto
{
    public int Id { get; set; }

    [Display(Name = "Modelo")]
    [Required(ErrorMessage = "O campo Modelo é obrigatório.")]
    [StringLength(100, ErrorMessage = "O campo Modelo deve ter no máximo 100 caracteres.")]
    public string Modelo { get; set; } = string.Empty;

    [Display(Name = "Ano")]
    [Required(ErrorMessage = "O campo Ano é obrigatório.")]
    public int Ano { get; set; }

    [Display(Name = "Cor")]
    [StringLength(30, ErrorMessage = "O campo Cor deve ter no máximo 30 caracteres.")]
    public string? Cor { get; set; }

    [Display(Name = "Quilometragem")]
    public int Quilometragem { get; set; }

    [Display(Name = "Valor")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    [Display(Name = "Descrição")]
    [StringLength(2000, ErrorMessage = "O campo Descrição deve ter no máximo 2000 caracteres.")]
    public string? Descricao { get; set; }

    [Display(Name = "Foto principal")]
    [StringLength(255, ErrorMessage = "O campo Foto principal deve ter no máximo 255 caracteres.")]
    public string? FotoPrincipal { get; set; }

    [Display(Name = "Disponível")]
    public bool Disponivel { get; set; } = true;

    [Display(Name = "Data de cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    // Relacionamentos

    [Display(Name = "Marca")]
    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }

    [Display(Name = "Categoria")]
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    [Display(Name = "Combustível")]
    public int CombustivelId { get; set; }
    public Combustivel? Combustivel { get; set; }

    public ICollection<FotoMoto> Fotos { get; set; } = new List<FotoMoto>();

    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
}
