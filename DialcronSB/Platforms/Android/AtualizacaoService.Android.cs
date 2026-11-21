using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;

namespace DialcronSB.Data;

public partial class AtualizacaoService
{
	private const string PastaAtualizacoes = "updates";
	private const string NomeApk = "DialcronSB-update.apk";
	private static readonly TimeSpan TimeoutDownload = TimeSpan.FromMinutes(10);

	public partial bool PrecisaPermissaoInstalacao()
	{
		return OperatingSystem.IsAndroidVersionAtLeast(26)
			&& Platform.CurrentActivity?.PackageManager?.CanRequestPackageInstalls() == false;
	}

	public partial void AbrirConfiguracoesInstalacao()
	{
		if (!OperatingSystem.IsAndroidVersionAtLeast(26))
			return;

		var activity = Platform.CurrentActivity;
		if (activity is null)
			return;

		var intent = new Intent(Android.Provider.Settings.ActionManageUnknownAppSources);
		intent.SetData(Android.Net.Uri.Parse("package:" + activity.PackageName));
		activity.StartActivity(intent);
	}

	public partial async Task InstalarAsync(string urlApk, string urlRelease)
	{
		var caminhoApk = await BaixarApkAsync(urlApk);

		var activity = Platform.CurrentActivity
			?? throw new InvalidOperationException("Atividade Android indisponivel.");

		var arquivo = new Java.IO.File(caminhoApk);
		var uri = AndroidX.Core.Content.FileProvider.GetUriForFile(activity, activity.PackageName + ".updateFileProvider", arquivo);

		var intent = new Intent(Intent.ActionView);
		intent.SetDataAndType(uri, "application/vnd.android.package-archive");
		intent.AddFlags(ActivityFlags.GrantReadUriPermission | ActivityFlags.NewTask);
		activity.StartActivity(intent);
	}

	private static async Task<string> BaixarApkAsync(string urlApk)
	{
		var diretorio = Path.Combine(FileSystem.CacheDirectory, PastaAtualizacoes);
		Directory.CreateDirectory(diretorio);

		foreach (var arquivoAntigo in Directory.GetFiles(diretorio))
			File.Delete(arquivoAntigo);

		var caminho = Path.Combine(diretorio, NomeApk);

		using var client = new HttpClient
		{
			Timeout = TimeoutDownload
		};
		client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

		using var response = await client.GetAsync(urlApk, HttpCompletionOption.ResponseHeadersRead);
		response.EnsureSuccessStatusCode();

		await using var stream = await response.Content.ReadAsStreamAsync();
		await using var arquivo = File.Create(caminho);
		await stream.CopyToAsync(arquivo);

		return caminho;
	}
}
