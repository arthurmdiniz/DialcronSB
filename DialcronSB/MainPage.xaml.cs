using CommunityToolkit.Maui.Storage;
using DialcronSB.Data;
using DialcronSB.Data.DAO;
using DialcronSB.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace DialcronSB;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		VersaoLabel.Text = $"Versão {AppInfo.VersionString}";
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		try
		{
			await Database.Instance.InitializeAsync();
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Banco de dados", ex.Message, "OK");
		}
	}

	private async void OnAgendaClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("agenda");
	}

	private async void OnCadastrosClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("cadastros");
	}

	private async void OnBackupClicked(object? sender, EventArgs e)
	{
		try
		{
			var fileName = BackupDAO.BuildBackupFileName();
			var backupDAO = new BackupDAO(await Database.Instance.InitializeAsync());

			if (DeviceInfo.Platform == DevicePlatform.Android)
			{
				var shareFile = Path.Combine(FileSystem.CacheDirectory, "sharing-root", fileName);
				await backupDAO.CreateBackupAsync(shareFile);

				await Share.Default.RequestAsync(new ShareFileRequest
				{
					Title = "Backup do banco Dialcron",
					File = new ShareFile(shareFile)
				});
			}
			else
			{
				var tempFile = Path.Combine(FileSystem.CacheDirectory, fileName);
				await backupDAO.CreateBackupAsync(tempFile);

				using var stream = File.OpenRead(tempFile);
				var result = await FileSaver.Default.SaveAsync(fileName, stream);
				File.Delete(tempFile);

				await DisplayAlertAsync(
					"Backup",
					result.IsSuccessful
						? $"Arquivo salvo em:\n{result.FilePath}"
						: "Nao foi possivel salvar o backup.",
					"OK");
			}
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Backup", ex.Message, "OK");
		}
	}

	private async void OnVerificarAtualizacoesClicked(object? sender, EventArgs e)
	{
		AtualizacoesBtn.IsEnabled = false;
		AtualizacoesBtn.Text = "Verificando...";

		try
		{
			var servico = new AtualizacaoService();
			var info = await servico.VerificarAsync();

			if (info is null)
			{
				await DisplayAlertAsync("Atualização", "Nenhuma release publicada para este aplicativo.", "OK");
				return;
			}

			if (!servico.EhMaisNova(info))
			{
				await DisplayAlertAsync("Atualização", $"Você já está na versão mais recente ({AppInfo.VersionString}).", "OK");
				return;
			}

			if (string.IsNullOrWhiteSpace(info.UrlApk))
			{
				var abrir = await DisplayAlertAsync(
					"Atualização",
					$"Versão {info.Tag} disponível, mas sem arquivo de instalação anexado. Deseja abrir a página da release?",
					"Abrir",
					"Agora não");

				if (abrir)
					await Browser.Default.OpenAsync(new Uri(info.UrlRelease), BrowserLaunchMode.SystemPreferred);

				return;
			}

			if (servico.PrecisaPermissaoInstalacao())
			{
				var abrir = await DisplayAlertAsync(
					"Permissão necessária",
					"Permita a instalação de aplicativos desta fonte nas configurações do Android e toque novamente em Verificar atualização.",
					"Abrir configurações",
					"Cancelar");

				if (abrir)
					servico.AbrirConfiguracoesInstalacao();

				return;
			}

			var mensagem = DeviceInfo.Platform == DevicePlatform.Android
				? $"Versão {info.Tag} disponível. A atualização é opcional. Deseja atualizar agora?"
				: $"Versão {info.Tag} disponível. A página da release será aberta no navegador para download manual. Continuar?";

			var atualizar = await DisplayAlertAsync("Atualização", mensagem, "Atualizar", "Agora não");

			if (atualizar)
				await servico.InstalarAsync(info.UrlApk, info.UrlRelease);
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Atualização", $"Não foi possível verificar a atualização: {ex.Message}", "OK");
		}
		finally
		{
			AtualizacoesBtn.Text = "Verificar atualização";
			AtualizacoesBtn.IsEnabled = true;
		}
	}
}