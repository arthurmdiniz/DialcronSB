using DialcronSB.Views;

namespace DialcronSB;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnAgendaClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("agenda");
	}

	private async void OnCadastrosClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("cadastros");
	}
}