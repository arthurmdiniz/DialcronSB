using System.Collections.ObjectModel;
using DialcronSB.Data;
using DialcronSB.Data.DAO;
using DialcronSB.Models;

namespace DialcronSB.Views;

public partial class UnidadePage : ContentPage
{
	private readonly ObservableCollection<Unidade> _unidades = new();
	private Unidade? _selecionado;

	public UnidadePage()
	{
		InitializeComponent();

		UnidadesCollection.ItemsSource = _unidades;
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
			var dao = new UnidadeDAO(await Database.Instance.InitializeAsync());
			var unidades = await dao.GetAllAsync();

			_unidades.Clear();
			foreach (var unidade in unidades)
				_unidades.Add(unidade);
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Unidades", $"Não foi possível carregar: {ex.Message}", "OK");
		}
	}

	private void OnSelecaoChanged(object? sender, SelectionChangedEventArgs e)
	{
		_selecionado = e.CurrentSelection.FirstOrDefault() as Unidade;

		if (_selecionado is null)
		{
			LimparFormulario();
			return;
		}

		PreencherFormulario(_selecionado);
	}

	private void OnIncluirClicked(object? sender, EventArgs e)
	{
		UnidadesCollection.SelectedItem = null;
		_selecionado = null;
		LimparFormulario();
		NomeFantasiaEntry.Focus();
	}

	private async void OnSalvarClicked(object? sender, EventArgs e)
	{
		try
		{
			var nomeFantasia = NomeFantasiaEntry.Text?.Trim() ?? string.Empty;

			if (string.IsNullOrWhiteSpace(nomeFantasia))
			{
				await DisplayAlertAsync("Unidades", "Informe o nome fantasia da unidade.", "OK");
				return;
			}

			var dao = new UnidadeDAO(await Database.Instance.InitializeAsync());
			var unidade = _selecionado ?? new Unidade();

			LerFormulario(unidade);

			if (unidade.IdUnidade == 0)
				await dao.InsertAsync(unidade);
			else
				await dao.UpdateAsync(unidade);

			await CarregarAsync();
			UnidadesCollection.SelectedItem = _unidades.FirstOrDefault(u => u.IdUnidade == unidade.IdUnidade);
			await DisplayAlertAsync("Unidades", "Unidade salva com sucesso.", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Unidades", $"Não foi possível salvar: {ex.Message}", "OK");
		}
	}

	private async void OnExcluirClicked(object? sender, EventArgs e)
	{
		if (_selecionado is null)
			return;

		var confirmar = await DisplayAlertAsync("Excluir", $"Excluir a unidade {_selecionado.Exibicao}?", "Sim", "Não");

		if (!confirmar)
			return;

		try
		{
			var dao = new UnidadeDAO(await Database.Instance.InitializeAsync());
			await dao.DeleteAsync(_selecionado);

			_selecionado = null;
			LimparFormulario();
			await CarregarAsync();
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Unidades", $"Não foi possível excluir: {ex.Message}", "OK");
		}
	}

	private void PreencherFormulario(Unidade unidade)
	{
		NomeFantasiaEntry.Text = unidade.NomeFantasia;
		RazaoSocialEntry.Text = unidade.RazaoSocial;
		CnpjEntry.Text = unidade.Cnpj;
		InscEstadualEntry.Text = unidade.InscEstadual;
		InscMunicipalEntry.Text = unidade.InscMunicipal;
		CepEntry.Text = Conversor.ParaTexto(unidade.Cep);
		LogradouroEntry.Text = unidade.Logradouro;
		NumeroEntry.Text = Conversor.ParaTexto(unidade.Numero);
		BairroEntry.Text = unidade.Bairro;
		ComplementoEntry.Text = unidade.Complemento;
		UfEntry.Text = unidade.Uf;
		MunicipioEntry.Text = unidade.Municipio;
		PaisEntry.Text = unidade.Pais;
		EmailEntry.Text = unidade.Email;
		DddEntry.Text = Conversor.ParaTexto(unidade.Ddd);
		TelefoneEntry.Text = Conversor.ParaTexto(unidade.Telefone);
		ExcluirBtn.IsEnabled = true;
	}

	private void LerFormulario(Unidade unidade)
	{
		unidade.NomeFantasia = Conversor.ParaTextoOuNulo(NomeFantasiaEntry.Text);
		unidade.RazaoSocial = Conversor.ParaTextoOuNulo(RazaoSocialEntry.Text);
		unidade.Cnpj = Conversor.ParaTextoOuNulo(CnpjEntry.Text);
		unidade.InscEstadual = Conversor.ParaTextoOuNulo(InscEstadualEntry.Text);
		unidade.InscMunicipal = Conversor.ParaTextoOuNulo(InscMunicipalEntry.Text);
		unidade.Cep = Conversor.ParaInt(CepEntry.Text);
		unidade.Logradouro = Conversor.ParaTextoOuNulo(LogradouroEntry.Text);
		unidade.Numero = Conversor.ParaInt(NumeroEntry.Text);
		unidade.Bairro = Conversor.ParaTextoOuNulo(BairroEntry.Text);
		unidade.Complemento = Conversor.ParaTextoOuNulo(ComplementoEntry.Text);
		unidade.Uf = Conversor.ParaTextoOuNulo(UfEntry.Text)?.ToUpperInvariant();
		unidade.Municipio = Conversor.ParaTextoOuNulo(MunicipioEntry.Text);
		unidade.Pais = Conversor.ParaTextoOuNulo(PaisEntry.Text);
		unidade.Email = Conversor.ParaTextoOuNulo(EmailEntry.Text);
		unidade.Ddd = Conversor.ParaInt(DddEntry.Text);
		unidade.Telefone = Conversor.ParaInt(TelefoneEntry.Text);
	}

	private void LimparFormulario()
	{
		NomeFantasiaEntry.Text = string.Empty;
		RazaoSocialEntry.Text = string.Empty;
		CnpjEntry.Text = string.Empty;
		InscEstadualEntry.Text = string.Empty;
		InscMunicipalEntry.Text = string.Empty;
		CepEntry.Text = string.Empty;
		LogradouroEntry.Text = string.Empty;
		NumeroEntry.Text = string.Empty;
		BairroEntry.Text = string.Empty;
		ComplementoEntry.Text = string.Empty;
		UfEntry.Text = string.Empty;
		MunicipioEntry.Text = string.Empty;
		PaisEntry.Text = string.Empty;
		EmailEntry.Text = string.Empty;
		DddEntry.Text = string.Empty;
		TelefoneEntry.Text = string.Empty;
		ExcluirBtn.IsEnabled = false;
	}
}
