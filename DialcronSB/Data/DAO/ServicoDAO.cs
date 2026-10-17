using DialcronSB.Models;
using SQLite;

namespace DialcronSB.Data.DAO;

public class ServicoDAO
{
	private readonly SQLiteAsyncConnection _connection;

	public ServicoDAO(SQLiteAsyncConnection connection)
	{
		_connection = connection;
	}

	public Task<List<Servico>> GetAllAsync()
	{
		return _connection.Table<Servico>().OrderBy(s => s.Nome).ToListAsync();
	}

	public async Task<Servico?> GetByIdAsync(int id)
	{
		return await _connection.Table<Servico>().Where(s => s.Id == id).FirstOrDefaultAsync();
	}

	public Task<int> InsertAsync(Servico servico)
	{
		return _connection.InsertAsync(servico);
	}

	public Task<int> UpdateAsync(Servico servico)
	{
		return _connection.UpdateAsync(servico);
	}

	public Task<int> DeleteAsync(Servico servico)
	{
		return _connection.DeleteAsync(servico);
	}
}
