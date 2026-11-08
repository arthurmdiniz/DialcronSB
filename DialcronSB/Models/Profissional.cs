using DialcronSB.Data;
using SQLite;

namespace DialcronSB.Models;

[Table("bel_profissional")]
public class Profissional
{
	[PrimaryKey, AutoIncrement, Column("id")]
	public int Id { get; set; }

	[Column("unidade")]
	public int Unidade { get; set; }

	[Column("nome")]
	public string Nome { get; set; } = string.Empty;

	[Column("especialidade")]
	public string? Especialidade { get; set; }

	[Column("ddd_telefone")]
	public int? DddTelefone { get; set; }

	[Column("telefone")]
	public int? Telefone { get; set; }

	[Column("ddd_celular")]
	public int? DddCelular { get; set; }

	[Column("celular")]
	public int? Celular { get; set; }

	[Column("ddd_outros")]
	public int? DddOutros { get; set; }

	[Column("telefone_outros")]
	public int? TelefoneOutros { get; set; }

	[Column("ativo")]
	public string Ativo { get; set; } = "S";

	[Ignore]
	public bool EstaAtivo => Ativo == "S";


	[Ignore]
	public string CelularExibicao => Formatador.Telefone(DddCelular, Celular);

    [Ignore]
    public string TelefoneExibicao => Formatador.Telefone(DddTelefone, Telefone);

	[Ignore]
    public string TelefoneOutrosExibicao => Formatador.Telefone(DddOutros, TelefoneOutros);


}
