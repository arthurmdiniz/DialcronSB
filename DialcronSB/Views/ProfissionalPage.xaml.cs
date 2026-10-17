using System.Collections.ObjectModel;
using DialcronSB.Data;
using DialcronSB.Data.DAO;
using DialcronSB.Models;

namespace DialcronSB.Views;

public partial class ProfissionalPage : ContentPage
{
	private readonly ObservableCollection<Profissional> _profissionais = new();
	private Profissional? _selecionado;

	public ProfissionalPage()
	{
		InitializeComponent();

		ProfissionaisCollection.ItemsSource = _profissionais;
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
			var connection = await Database.Instance.InitializeAsync();
			var unidades = await new UnidadeDAO(connection).GetAllAsync();
			var profissionais = await new ProfissionalDAO(connection).GetAllAsync();

			var unidadeSelecionada = UnidadePicker.SelectedItem as Unidade;
			UnidadePicker.ItemsSource = unidades;

			if (unidadeSelecionada is not null)
				UnidadePicker.SelectedItem = unidades.FirstOrDefault(u => u.IdUnidade == unidadeSelecionada.IdUnidade);

			_profissionais.Clear();
			foreach (var profissional in profissionais)
				_profissionais.Add(profissional);
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Profissionais", $"Não foi possível carregar: {ex.Message}", "OK");
		}
	}

	private void OnAtivoToggled(object? sender, ToggledEventArgs e)
	{
		AtivoStatusLabel.Text = e.Value ? "Ativo" : "Inativo";
	}

	private void OnSelecaoChanged(object? sender, SelectionChangedEventArgs e)
	{
		_selecionado = e.CurrentSelection.FirstOrDefault() as Profissional;

		if (_selecionado is null)
		{
			LimparFormulario();
			return;
		}

		PreencherFormulario(_selecionado);
	}

	private void OnIncluirClicked(object? sender, EventArgs e)
	{
		ProfissionaisCollection.SelectedItem = null;
		_selecionado = null;
		LimparFormulario();
		NomeEntry.Focus();
	}

	private async void OnSalvarClicked(object? sender, EventArgs e)
	{
		try
		{
			if (UnidadePicker.SelectedItem is not Unidade unidade)
			{
				await DisplayAlertAsync("Profissionais", "Selecione a unidade. Se não houver nenhuma, cadastre em Unidades.", "OK");
				return;
			}

			var nome = NomeEntry.Text?.Trim() ?? string.Empty;

			if (string.IsNullOrWhiteSpace(nome))
			{
				await DisplayAlertAsync("Profissionais", "Informe o nome do profissional.", "OK");
				return;
			}

			var dao = new ProfissionalDAO(await Database.Instance.InitializeAsync());
			var profissional = _selecionado ?? new Profissional();

			profissional.Unidade = unidade.IdUnidade;
			profissional.Nome = nome;
			profissional.Especialidade = Conversor.ParaTextoOuNulo(EspecialidadeEntry.Text);
			profissional.DddTelefone = Conversor.ParaInt(DddTelefoneEntry.Text);
			profissional.Telefone = Conversor.ParaInt(TelefoneEntry.Text);
			profissional.DddCelular = Conversor.ParaInt(DddCelularEntry.Text);
			profissional.Celular = Conversor.ParaInt(CelularEntry.Text);
			profissional.DddOutros = Conversor.ParaInt(DddOutrosEntry.Text);
			profissional.TelefoneOutros = Conversor.ParaInt(TelefoneOutrosEntry.Text);
			profissional.Ativo = Conversor.ParaSimNao(AtivoSwitch.IsToggled);

			if (profissional.Id == 0)
				await dao.InsertAsync(profissional);
			else
				await dao.UpdateAsync(profissional);

			await CarregarAsync();
			ProfissionaisCollection.SelectedItem = _profissionais.FirstOrDefault(p => p.Id == profissional.Id);
			await DisplayAlertAsync("Profissionais", "Profissional salvo com sucesso.", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Profissionais", $"Não foi possível salvar: {ex.Message}", "OK");
		}
	}

	private async void OnExcluirClicked(object? sender, EventArgs e)
	{
		if (_selecionado is null)
			return;

		var confirmar = await DisplayAlertAsync("Excluir", $"Excluir o profissional {_selecionado.Nome}?", "Sim", "Não");

		if (!confirmar)
			return;

		try
		{
			var dao = new ProfissionalDAO(await Database.Instance.InitializeAsync());
			await dao.DeleteAsync(_selecionado);

			_selecionado = null;
			LimparFormulario();
			await CarregarAsync();
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Profissionais", $"Não foi possível excluir: {ex.Message}", "OK");
		}
	}

	private void PreencherFormulario(Profissional profissional)
	{
		UnidadePicker.SelectedItem = (UnidadePicker.ItemsSource as IEnumerable<Unidade>)?
			.FirstOrDefault(u => u.IdUnidade == profissional.Unidade);
		NomeEntry.Text = profissional.Nome;
		EspecialidadeEntry.Text = profissional.Especialidade;
		DddTelefoneEntry.Text = Conversor.ParaTexto(profissional.DddTelefone);
		TelefoneEntry.Text = Conversor.ParaTexto(profissional.Telefone);
		DddCelularEntry.Text = Conversor.ParaTexto(profissional.DddCelular);
		CelularEntry.Text = Conversor.ParaTexto(profissional.Celular);
		DddOutrosEntry.Text = Conversor.ParaTexto(profissional.DddOutros);
		TelefoneOutrosEntry.Text = Conversor.ParaTexto(profissional.TelefoneOutros);
		AtivoSwitch.IsToggled = profissional.EstaAtivo;
		ExcluirBtn.IsEnabled = true;
	}

	private void LimparFormulario()
	{
		NomeEntry.Text = string.Empty;
		EspecialidadeEntry.Text = string.Empty;
		DddTelefoneEntry.Text = string.Empty;
		TelefoneEntry.Text = string.Empty;
		DddCelularEntry.Text = string.Empty;
		CelularEntry.Text = string.Empty;
		DddOutrosEntry.Text = string.Empty;
		TelefoneOutrosEntry.Text = string.Empty;
		AtivoSwitch.IsToggled = true;
		ExcluirBtn.IsEnabled = false;
	}
}
