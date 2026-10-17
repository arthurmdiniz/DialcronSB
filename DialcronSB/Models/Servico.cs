using SQLite;

namespace DialcronSB.Models;

[Table("bel_servico")]
public class Servico
{
	[PrimaryKey, AutoIncrement, Column("id")]
	public int Id { get; set; }

	[Column("unidade")]
	public int Unidade { get; set; }

	[Column("nome")]
	public string Nome { get; set; } = string.Empty;

	[Column("preco")]
	public double Preco { get; set; }

	[Column("duracao_minutos")]
	public int DuracaoMinutos { get; set; } = 30;

	[Column("ativo")]
	public string Ativo { get; set; } = "S";

	[Ignore]
	public bool EstaAtivo => Ativo == "S";
}
