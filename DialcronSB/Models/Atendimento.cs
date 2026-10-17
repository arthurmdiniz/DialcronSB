using SQLite;

namespace DialcronSB.Models;

[Table("cad_atendimento")]
public class CadAtendimento
{
	[PrimaryKey, AutoIncrement, Column("id")]
	public int Id { get; set; }

	[Column("unidade")]
	public int Unidade { get; set; }

	[Column("cliente")]
	public int? Cliente { get; set; }

	[Column("descricao")]
	public string? Descricao { get; set; }

	[Column("situacao")]
	public string? Situacao { get; set; }

	[Column("dta_criacao")]
	public string? DtaCriacao { get; set; }

	[Column("dta_encerramento")]
	public string? DtaEncerramento { get; set; }

	[Column("usuario_criacao")]
	public int? UsuarioCriacao { get; set; }

	[Column("usuario_encerramento")]
	public int? UsuarioEncerramento { get; set; }
}

[Table("bel_atendimento")]
public class BelAtendimento
{
	[PrimaryKey, AutoIncrement, Column("id")]
	public int Id { get; set; }

	[Column("atendimento")]
	public int Atendimento { get; set; }

	[Column("profissional")]
	public int Profissional { get; set; }

	[Column("dta_atendimento")]
	public string DtaAtendimento { get; set; } = string.Empty;

	[Column("dta_criacao")]
	public string? DtaCriacao { get; set; }

	[Column("dta_encerramento")]
	public string? DtaEncerramento { get; set; }

	[Column("situacao")]
	public string Situacao { get; set; } = "a";

	[Column("valor")]
	public double Valor { get; set; }

	[Column("pago")]
	public int Pago { get; set; }

	[Column("observacao")]
	public string? Observacao { get; set; }
}

[Table("bel_atendimento_servico")]
public class AtendimentoServico
{
	[PrimaryKey, AutoIncrement, Column("id")]
	public int Id { get; set; }

	[Column("atendimento")]
	public int Atendimento { get; set; }

	[Column("servico")]
	public int Servico { get; set; }
}
