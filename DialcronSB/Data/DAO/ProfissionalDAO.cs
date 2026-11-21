using DialcronSB.Models;
using SQLite;

namespace DialcronSB.Data.DAO;

public class ProfissionalDAO
{
	private readonly SQLiteAsyncConnection _connection;

	public ProfissionalDAO(SQLiteAsyncConnection connection)
	{
		_connection = connection;
	}

	public Task<List<Profissional>> GetAllAsync()
	{
		return _connection.Table<Profissional>().OrderBy(p => p.Nome).ToListAsync();
	}

	public async Task<Profissional?> GetByIdAsync(int id)
	{
		return await _connection.Table<Profissional>().Where(p => p.Id == id).FirstOrDefaultAsync();
	}

	public Task<int> InsertAsync(Profissional profissional)
	{
		return _connection.InsertAsync(profissional);
	}

	public Task<int> UpdateAsync(Profissional profissional)
	{
		return _connection.UpdateAsync(profissional);
	}

	public Task<int> DeleteAsync(Profissional profissional)
	{
		return _connection.DeleteAsync(profissional);
	}
}
