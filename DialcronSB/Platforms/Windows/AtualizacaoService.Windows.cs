using Microsoft.Maui.ApplicationModel;

namespace DialcronSB.Data;

public partial class AtualizacaoService
{
	public partial bool PrecisaPermissaoInstalacao()
	{
		return false;
	}

	public partial void AbrirConfiguracoesInstalacao()
	{
	}

	public partial async Task InstalarAsync(string urlApk, string urlRelease)
	{
		if (string.IsNullOrWhiteSpace(urlRelease))
			throw new InvalidOperationException("Link da release indisponivel.");

		await Browser.Default.OpenAsync(new Uri(urlRelease), BrowserLaunchMode.SystemPreferred);
	}
}
