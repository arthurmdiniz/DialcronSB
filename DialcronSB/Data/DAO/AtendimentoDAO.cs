using System.Globalization;
using DialcronSB.Models;
using SQLite;

namespace DialcronSB.Data.DAO;

public class AtendimentoDAO
{
	public const string FormatoDataHora = "yyyy-MM-dd HH:mm:ss";

	private readonly SQLiteAsyncConnection _connection;

	public AtendimentoDAO(SQLiteAsyncConnection connection)
	{
		_connection = connection;
	}

	public static string FormatarDataHora(DateTime valor)
	{
		return valor.ToString(FormatoDataHora, CultureInfo.InvariantCulture);
	}

	public Task<List<BelAtendimento>> GetByDiaAsync(DateTime dia)
	{
		var inicio = FormatarDataHora(dia.Date);
		var fim = FormatarDataHora(dia.Date.AddDays(1));

		return _connection.QueryAsync<BelAtendimento>(
			"SELECT * FROM bel_atendimento WHERE dta_atendimento >= ? AND dta_atendimento < ? ORDER BY dta_atendimento",
			inicio,
			fim);
	}

	public Task<List<CadAtendimento>> GetCabecalhosAsync()
	{
		return _connection.Table<CadAtendimento>().ToListAsync();
	}

	public Task<List<AtendimentoServico>> GetServicosAsync(int atendimentoId)
	{
		return _connection.Table<AtendimentoServico>()
			.Where(s => s.Atendimento == atendimentoId)
			.ToListAsync();
	}

	public async Task InsertAsync(CadAtendimento cabecalho, BelAtendimento atendimento, IEnumerable<int> servicos)
	{
		await _connection.RunInTransactionAsync(conn =>
		{
			conn.Insert(cabecalho);
			atendimento.Atendimento = cabecalho.Id;
			conn.Insert(atendimento);

			foreach (var servicoId in servicos)
				conn.Insert(new AtendimentoServico { Atendimento = atendimento.Id, Servico = servicoId });
		});
	}

	public async Task UpdateAsync(CadAtendimento cabecalho, BelAtendimento atendimento, IEnumerable<int> servicos)
	{
		await _connection.RunInTransactionAsync(conn =>
		{
			conn.Update(cabecalho);
			conn.Update(atendimento);
			conn.Execute("DELETE FROM bel_atendimento_servico WHERE atendimento = ?", atendimento.Id);

			foreach (var servicoId in servicos)
				conn.Insert(new AtendimentoServico { Atendimento = atendimento.Id, Servico = servicoId });
		});
	}

	public async Task DeleteAsync(CadAtendimento cabecalho, BelAtendimento atendimento)
	{
		await _connection.RunInTransactionAsync(conn =>
		{
			conn.Execute("DELETE FROM bel_atendimento_servico WHERE atendimento = ?", atendimento.Id);
			conn.Delete(atendimento);
			conn.Delete(cabecalho);
		});
	}
}
