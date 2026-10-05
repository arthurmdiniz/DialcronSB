namespace DialcronSB.Views;

public partial class CadastrosPage : ContentPage
{
	public CadastrosPage()
	{
		InitializeComponent();
	}

	private async void OnClientesClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("cliente");
	}

	private async void OnServicosClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("servico");
	}

	private async void OnProfissionaisClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("profissional");
	}
}