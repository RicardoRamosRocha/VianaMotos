using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VianaMotos.Web.Models;

public class Moto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [Required]
    public int Ano { get; set; }

    [StringLength(30)]
    public string? Cor { get; set; }

    public int Quilometragem { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    [StringLength(2000)]
    public string? Descricao { get; set; }

    public bool Disponivel { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.Now;

    // Relacionamentos

    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int CombustivelId { get; set; }
    public Combustivel? Combustivel { get; set; }

    public ICollection<FotoMoto> Fotos { get; set; } = new List<FotoMoto>();

    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
}
