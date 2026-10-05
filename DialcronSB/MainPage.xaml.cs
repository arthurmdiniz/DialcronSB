using CommunityToolkit.Maui.Storage;
using DialcronSB.Data;
using DialcronSB.Data.DAO;
using DialcronSB.Views;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace DialcronSB;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
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
}