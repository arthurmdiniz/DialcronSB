using DialcronSB.Data;
using DialcronSB.Views;

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

	private async void OnConfiguracoesClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("configuracoes");
	}
}
