using DialcronSB.Models;
using SQLite;

namespace DialcronSB.Data.DAO;

public class UnidadeDAO
{
	private readonly SQLiteAsyncConnection _connection;

	public UnidadeDAO(SQLiteAsyncConnection connection)
	{
		_connection = connection;
	}

	public Task<List<Unidade>> GetAllAsync()
	{
		return _connection.Table<Unidade>().OrderBy(u => u.NomeFantasia).ToListAsync();
	}

	public async Task<Unidade?> GetByIdAsync(int id)
	{
		return await _connection.Table<Unidade>().Where(u => u.IdUnidade == id).FirstOrDefaultAsync();
	}

	public Task<int> InsertAsync(Unidade unidade)
	{
		return _connection.InsertAsync(unidade);
	}

	public Task<int> UpdateAsync(Unidade unidade)
	{
		return _connection.UpdateAsync(unidade);
	}

	public Task<int> DeleteAsync(Unidade unidade)
	{
		return _connection.DeleteAsync(unidade);
	}
}
