using System.Collections.ObjectModel;
using DialcronSB.Data;
using DialcronSB.Data.DAO;
using DialcronSB.Models;

namespace DialcronSB.Views;

public partial class ClientePage : ContentPage
{
	private readonly ObservableCollection<Cliente> _clientes = new();
	private List<Cliente> _todos = new();
	private Cliente? _selecionado;

	public ClientePage()
	{
		InitializeComponent();

		ClientesCollection.ItemsSource = _clientes;
		SexoPicker.ItemsSource = new[] { "Masculino", "Feminino" };
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await CarregarAsync();
	}

	private async Task CarregarAsync()
	{
		try
		{
			var dao = new ClienteDAO(await Database.Instance.InitializeAsync());
			_todos = await dao.GetAllAsync();
			AplicarFiltro();
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Clientes", $"Não foi possível carregar: {ex.Message}", "OK");
		}
	}

    private void AplicarFiltro()
    {
        var termo = BuscaBar.Text?.Trim() ?? string.Empty;
        var termoDigitos = Formatador.SomenteDigitos(termo);
        var buscarPorDigitos = termoDigitos.Length > 0;

        _clientes.Clear();

        foreach (var cliente in _todos.Where(c =>
            string.IsNullOrEmpty(termo)
            || c.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase)
            || (buscarPorDigitos && (
                Formatador.SomenteDigitos(c.CpfCnpj).Contains(termoDigitos, StringComparison.Ordinal)
                || Formatador.SomenteDigitos($"{c.DddTelefone}{c.Telefone}").Contains(termoDigitos, StringComparison.Ordinal)
                || Formatador.SomenteDigitos($"{c.DddCelular}{c.Celular}").Contains(termoDigitos, StringComparison.Ordinal)))))
        {
            _clientes.Add(cliente);
        }

        ListaVaziaLabel.IsVisible = _clientes.Count == 0;
    }

    private void OnBuscaTextChanged(object? sender, TextChangedEventArgs e)
	{
		AplicarFiltro();
	}

	private void OnSelecaoChanged(object? sender, SelectionChangedEventArgs e)
	{
		_selecionado = e.CurrentSelection.FirstOrDefault() as Cliente;

		if (_selecionado is null)
		{
			LimparFormulario();
			return;
		}

		PreencherFormulario(_selecionado);
	}

	private void OnIncluirClicked(object? sender, EventArgs e)
	{
		ClientesCollection.SelectedItem = null;
		_selecionado = null;
		LimparFormulario();
		NomeEntry.Focus();
	}

	private async void OnSalvarClicked(object? sender, EventArgs e)
	{
		try
		{
			var nome = NomeEntry.Text?.Trim() ?? string.Empty;

			if (string.IsNullOrWhiteSpace(nome))
			{
				await DisplayAlertAsync("Clientes", "Informe o nome do cliente.", "OK");
				return;
			}

			var dao = new ClienteDAO(await Database.Instance.InitializeAsync());
			var cliente = _selecionado ?? new Cliente
			{
				DtaCriacao = AtendimentoDAO.FormatarDataHora(DateTime.Now)
			};

			LerFormulario(cliente);

			if (cliente.IdCliente == 0)
				await dao.InsertAsync(cliente);
			else
				await dao.UpdateAsync(cliente);

			await CarregarAsync();
			SelecionarPorId(cliente.IdCliente);
			await DisplayAlertAsync("Clientes", "Cliente salvo com sucesso.", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Clientes", $"Não foi possível salvar: {ex.Message}", "OK");
		}
	}

	private async void OnExcluirClicked(object? sender, EventArgs e)
	{
		if (_selecionado is null)
			return;

		var confirmar = await DisplayAlertAsync("Excluir", $"Excluir o cliente {_selecionado.Nome}?", "Sim", "Não");

		if (!confirmar)
			return;

		try
		{
			var dao = new ClienteDAO(await Database.Instance.InitializeAsync());
			await dao.DeleteAsync(_selecionado);

			_selecionado = null;
			LimparFormulario();
			await CarregarAsync();
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Clientes", $"Não foi possível excluir: {ex.Message}", "OK");
		}
	}

	private void SelecionarPorId(int id)
	{
		var encontrado = _clientes.FirstOrDefault(c => c.IdCliente == id);
		ClientesCollection.SelectedItem = encontrado;
	}

	private void PreencherFormulario(Cliente cliente)
	{
		TituloFormulario.Text = "Dados do cliente";
		NomeEntry.Text = cliente.Nome;
		CpfCnpjEntry.Text = cliente.CpfCnpj;
		DtaNascimentoEntry.Text = cliente.DtaNascimento;
		SexoPicker.SelectedIndex = cliente.Sexo switch
		{
			"M" => 0,
			"F" => 1,
			_ => -1
		};
		DddTelefoneEntry.Text = Formatador.NumeroTelefone(cliente.DddTelefone);
		TelefoneEntry.Text = Formatador.NumeroTelefone(cliente.Telefone);
		DddCelularEntry.Text = Formatador.NumeroTelefone(cliente.DddCelular);
		CelularEntry.Text = Formatador.NumeroTelefone(cliente.Celular);
		EmailEntry.Text = cliente.Email;
		NumeroEntry.Text = Conversor.ParaTexto(cliente.Numero);
		LogradouroEntry.Text = cliente.Logradouro;
		ExcluirBtn.IsEnabled = true;
	}

	private void LerFormulario(Cliente cliente)
	{
		cliente.Nome = NomeEntry.Text?.Trim() ?? string.Empty;
		cliente.CpfCnpj = Conversor.ParaTextoOuNulo(CpfCnpjEntry.Text);
		cliente.DtaNascimento = Conversor.ParaTextoOuNulo(DtaNascimentoEntry.Text);
		cliente.Sexo = SexoPicker.SelectedIndex switch
		{
			0 => "M",
			1 => "F",
			_ => null
		};
		cliente.DddTelefone = Conversor.ParaInt(Formatador.SomenteDigitos(DddTelefoneEntry.Text));
		cliente.Telefone = Conversor.ParaInt(Formatador.SomenteDigitos(TelefoneEntry.Text));
		cliente.DddCelular = Conversor.ParaInt(Formatador.SomenteDigitos(DddCelularEntry.Text));
		cliente.Celular = Conversor.ParaInt(Formatador.SomenteDigitos(CelularEntry.Text));
		cliente.Email = Conversor.ParaTextoOuNulo(EmailEntry.Text);
		cliente.Numero = Conversor.ParaInt(NumeroEntry.Text);
		cliente.Logradouro = Conversor.ParaTextoOuNulo(LogradouroEntry.Text);
	}

	private void LimparFormulario()
	{
		NomeEntry.Text = string.Empty;
		CpfCnpjEntry.Text = string.Empty;
		DtaNascimentoEntry.Text = string.Empty;
		SexoPicker.SelectedIndex = -1;
		DddTelefoneEntry.Text = string.Empty;
		TelefoneEntry.Text = string.Empty;
		DddCelularEntry.Text = string.Empty;
		CelularEntry.Text = string.Empty;
		EmailEntry.Text = string.Empty;
		NumeroEntry.Text = string.Empty;
		LogradouroEntry.Text = string.Empty;
		ExcluirBtn.IsEnabled = false;
	}

    private void OnMascaraDdd(object? sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
            Mascara.Aplicar(entry, Formatador.MascaraDdd);
    }

    private void OnMascaraTelefone(object? sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
            Mascara.Aplicar(entry, Formatador.MascaraNumeroTelefone);
    }

    private void OnMascaraDocumento(object? sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
            Mascara.Aplicar(entry, Formatador.MascaraCpfCnpj);
    }

    private void OnMascaraCep(object? sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
            Mascara.Aplicar(entry, Formatador.MascaraCep);
    }
}
