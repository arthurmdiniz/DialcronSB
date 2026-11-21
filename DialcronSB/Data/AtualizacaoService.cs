using System.Net;
using System.Text.Json;
using Microsoft.Maui.ApplicationModel;

namespace DialcronSB.Data;

public partial class AtualizacaoService
{
	private const string UserAgent = "DialcronSB/1.0";
	private const int TimeoutSegundos = 15;

	private static readonly HttpClient Http = CriarHttpClient();

	private readonly string _owner;
	private readonly string _repo;

	public AtualizacaoService(string owner = "arthurmdiniz", string repo = "DialcronSB")
	{
		_owner = owner;
		_repo = repo;
	}

	private static HttpClient CriarHttpClient()
	{
		var client = new HttpClient
		{
			Timeout = TimeSpan.FromSeconds(TimeoutSegundos)
		};
		client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
		client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
		return client;
	}

	public async Task<UpdateInfo?> VerificarAsync()
	{
		var url = $"https://api.github.com/repos/{_owner}/{_repo}/releases/latest";

		using var response = await Http.GetAsync(url);

		if (response.StatusCode == HttpStatusCode.NotFound)
			return null;

		if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
			throw new InvalidOperationException("Limite de consultas a API do GitHub atingido. Tente novamente mais tarde.");

		response.EnsureSuccessStatusCode();

		using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
		var root = json.RootElement;

		var tag = root.GetProperty("tag_name").GetString();
		if (string.IsNullOrWhiteSpace(tag))
			throw new InvalidOperationException("Release sem identificador de versao.");

		return new UpdateInfo(
			tag,
			NormalizarVersao(tag),
			root.GetProperty("html_url").GetString() ?? string.Empty,
			ObterUrlApk(root));
	}

	public bool EhMaisNova(UpdateInfo info)
	{
		return CompareVersions(info.Versao, AppInfo.VersionString) > 0;
	}

	public partial bool PrecisaPermissaoInstalacao();

	public partial void AbrirConfiguracoesInstalacao();

	public partial Task InstalarAsync(string urlApk, string urlRelease);

	internal static string NormalizarVersao(string versao)
	{
		return versao.Trim().TrimStart('v', 'V');
	}

	internal static int CompareVersions(string a, string b)
	{
		var partesA = NormalizarVersao(a).Split('.');
		var partesB = NormalizarVersao(b).Split('.');
		var tamanho = Math.Max(partesA.Length, partesB.Length);

		for (var i = 0; i < tamanho; i++)
		{
			var diferenca = ParseParte(i < partesA.Length ? partesA[i] : "0")
				- ParseParte(i < partesB.Length ? partesB[i] : "0");

			if (diferenca != 0)
				return diferenca;
		}

		return 0;
	}

	private static int ParseParte(string parte)
	{
		var digitos = new string(parte.Where(char.IsDigit).ToArray());
		return int.TryParse(digitos, out var valor) ? valor : 0;
	}

	private static string? ObterUrlApk(JsonElement root)
	{
		if (!root.TryGetProperty("assets", out var assets))
			return null;

		foreach (var asset in assets.EnumerateArray())
		{
			var nome = asset.GetProperty("name").GetString();
			if (nome?.EndsWith(".apk", StringComparison.OrdinalIgnoreCase) == true)
				return asset.GetProperty("browser_download_url").GetString();
		}

		return null;
	}
}

public sealed record UpdateInfo(string Tag, string Versao, string UrlRelease, string? UrlApk);
