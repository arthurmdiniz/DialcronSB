using DialcronSB.Views;

namespace DialcronSB;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("agenda", typeof(AgendaPage));
		Routing.RegisterRoute("cadastros", typeof(CadastrosPage));
		Routing.RegisterRoute("cliente", typeof(ClientePage));
		Routing.RegisterRoute("servico", typeof(ServicoPage));
		Routing.RegisterRoute("profissional", typeof(ProfissionalPage));
	}
}
