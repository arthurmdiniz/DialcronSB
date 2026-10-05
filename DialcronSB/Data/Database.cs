using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using SQLite;

namespace DialcronSB.Data;

public class Database
{
	public const string SeedFileName = "DialcronSB_PADRAO.db";
	public const string DbFileName = "DialcronSBDB.db";

	private const string DbFolder = "DialcronAgendamentos";

	private SQLiteAsyncConnection? _connection;

	public static Database Instance { get; } = new();

	public static string DbPath { get; } = BuildDbPath();

	public bool DatabaseExists() => File.Exists(DbPath);

	public async Task<SQLiteAsyncConnection> InitializeAsync()
	{
		if (_connection is not null)
			return _connection;

		_connection = DatabaseExists()
			? await OpenConnectionAsync()
			: await CreateDatabaseAsync();

		return _connection;
	}

	public async Task<SQLiteAsyncConnection> CreateDatabaseAsync()
	{
		var folder = Path.GetDirectoryName(DbPath);

		if (!string.IsNullOrEmpty(folder))
			Directory.CreateDirectory(folder);

		using (var seed = await FileSystem.OpenAppPackageFileAsync(SeedFileName))
		using (var target = File.Create(DbPath))
			await seed.CopyToAsync(target);

		return await OpenConnectionAsync();
	}

	public async Task<SQLiteAsyncConnection> OpenConnectionAsync()
	{
		var connection = new SQLiteAsyncConnection(DbPath);
		await connection.ExecuteAsync("PRAGMA foreign_keys = ON;");
		return connection;
	}

	private static string BuildDbPath()
	{
		if (DeviceInfo.Platform == DevicePlatform.Android)
			return Path.Combine(FileSystem.AppDataDirectory, DbFolder, DbFileName);

		return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFileName);
	}
}