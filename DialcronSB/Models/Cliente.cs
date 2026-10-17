using SQLite;

namespace DialcronSB.Models;

[Table("cad_cliente")]
public class Cliente
{
	[PrimaryKey, AutoIncrement, Column("cliente")]
	public int IdCliente { get; set; }

	[Column("nome")]
	public string Nome { get; set; } = string.Empty;

	[Column("logradouro")]
	public string? Logradouro { get; set; }

	[Column("numero")]
	public int? Numero { get; set; }

	[Column("cpf_cnpj")]
	public string? CpfCnpj { get; set; }

	[Column("dta_nascimento")]
	public string? DtaNascimento { get; set; }

	[Column("dta_criacao")]
	public string? DtaCriacao { get; set; }

	[Column("ddd_telefone")]
	public int? DddTelefone { get; set; }

	[Column("telefone")]
	public int? Telefone { get; set; }

	[Column("ddd_celular")]
	public int? DddCelular { get; set; }

	[Column("celular")]
	public int? Celular { get; set; }

	[Column("email")]
	public string? Email { get; set; }

	[Column("sexo")]
	public string? Sexo { get; set; }

	[Ignore]
	public string Exibicao => Nome;
}
