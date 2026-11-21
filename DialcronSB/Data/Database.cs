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

	public async Task ImportarBackupAsync(string caminhoOrigem)
	{
		if (!File.Exists(caminhoOrigem))
			throw new FileNotFoundException("Arquivo de backup nao encontrado.", caminhoOrigem);

		ValidarArquivoSQLite(caminhoOrigem);

		if (_connection is not null)
		{
			await _connection.CloseAsync();
			_connection = null;
		}

		var temporario = DbPath + ".import";
		File.Copy(caminhoOrigem, temporario, true);
		File.Move(temporario, DbPath, true);

		_connection = await OpenConnectionAsync();
	}

	private static void ValidarArquivoSQLite(string caminhoOrigem)
	{
		using var stream = File.OpenRead(caminhoOrigem);
		var cabecalho = new byte[16];
		var lidos = stream.Read(cabecalho, 0, cabecalho.Length);
		var assinatura = System.Text.Encoding.ASCII.GetString(cabecalho, 0, lidos);

		if (!assinatura.StartsWith("SQLite format 3", StringComparison.Ordinal))
			throw new InvalidOperationException("O arquivo selecionado nao e um banco de dados SQLite valido.");
	}

	private static string BuildDbPath()
	{
		if (DeviceInfo.Platform == DevicePlatform.Android)
			return Path.Combine(FileSystem.AppDataDirectory, DbFolder, DbFileName);

		return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFileName);
	}
}