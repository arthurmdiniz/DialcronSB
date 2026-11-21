using DialcronSB.Models;
using SQLite;

namespace DialcronSB.Data.DAO;

public class ClienteDAO
{
	private readonly SQLiteAsyncConnection _connection;

	public ClienteDAO(SQLiteAsyncConnection connection)
	{
		_connection = connection;
	}

	public Task<List<Cliente>> GetAllAsync()
	{
		return _connection.Table<Cliente>().OrderBy(c => c.Nome).ToListAsync();
	}

	public async Task<Cliente?> GetByIdAsync(int id)
	{
		return await _connection.Table<Cliente>().Where(c => c.IdCliente == id).FirstOrDefaultAsync();
	}

	public Task<int> InsertAsync(Cliente cliente)
	{
		return _connection.InsertAsync(cliente);
	}

	public Task<int> UpdateAsync(Cliente cliente)
	{
		return _connection.UpdateAsync(cliente);
	}

	public Task<int> DeleteAsync(Cliente cliente)
	{
		return _connection.DeleteAsync(cliente);
	}
}
