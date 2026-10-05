namespace DialcronSB.Views;

public static class Navegacao
{
	public static async Task IrPara(string rota)
	{
		Shell? shell = Shell.Current;

		if (shell is null)
			return;

		try
		{
			await shell.GoToAsync(rota);
		}
		catch (Exception ex)
		{
			await shell.DisplayAlertAsync("Navegação", $"Não foi possível abrir a tela: {ex.Message}", "OK");
		}
	}
}