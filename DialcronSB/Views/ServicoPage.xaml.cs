using System.Collections.ObjectModel;
using DialcronSB.Data;
using DialcronSB.Data.DAO;
using DialcronSB.Models;

namespace DialcronSB.Views;

public partial class ServicoPage : ContentPage
{
	private readonly ObservableCollection<Servico> _servicos = new();
	private Servico? _selecionado;

	public ServicoPage()
	{
		InitializeComponent();

		ServicosCollection.ItemsSource = _servicos;
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
			var servicos = await new ServicoDAO(connection).GetAllAsync();

			var unidadeSelecionada = UnidadePicker.SelectedItem as Unidade;
			UnidadePicker.ItemsSource = unidades;

			if (unidadeSelecionada is not null)
				UnidadePicker.SelectedItem = unidades.FirstOrDefault(u => u.IdUnidade == unidadeSelecionada.IdUnidade);

			_servicos.Clear();
			foreach (var servico in servicos)
				_servicos.Add(servico);
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Serviços", $"Não foi possível carregar: {ex.Message}", "OK");
		}
	}

	private void OnAtivoToggled(object? sender, ToggledEventArgs e)
	{
		AtivoStatusLabel.Text = e.Value ? "Ativo" : "Inativo";
	}

	private void OnSelecaoChanged(object? sender, SelectionChangedEventArgs e)
	{
		_selecionado = e.CurrentSelection.FirstOrDefault() as Servico;

		if (_selecionado is null)
		{
			LimparFormulario();
			return;
		}

		PreencherFormulario(_selecionado);
	}

	private void OnIncluirClicked(object? sender, EventArgs e)
	{
		ServicosCollection.SelectedItem = null;
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
				await DisplayAlertAsync("Serviços", "Selecione a unidade. Se não houver nenhuma, cadastre em Unidades.", "OK");
				return;
			}

			var nome = NomeEntry.Text?.Trim() ?? string.Empty;

			if (string.IsNullOrWhiteSpace(nome))
			{
				await DisplayAlertAsync("Serviços", "Informe o nome do serviço.", "OK");
				return;
			}

			var duracao = Conversor.ParaInt(DuracaoEntry.Text) ?? 0;

			if (duracao <= 0)
			{
				await DisplayAlertAsync("Serviços", "Informe a duração em minutos.", "OK");
				return;
			}

			var dao = new ServicoDAO(await Database.Instance.InitializeAsync());
			var servico = _selecionado ?? new Servico();

			servico.Unidade = unidade.IdUnidade;
			servico.Nome = nome;
			servico.Preco = Conversor.ParaDouble(PrecoEntry.Text);
			servico.DuracaoMinutos = duracao;
			servico.Ativo = Conversor.ParaSimNao(AtivoSwitch.IsToggled);

			if (servico.Id == 0)
				await dao.InsertAsync(servico);
			else
				await dao.UpdateAsync(servico);

			await CarregarAsync();
			ServicosCollection.SelectedItem = _servicos.FirstOrDefault(s => s.Id == servico.Id);
			await DisplayAlertAsync("Serviços", "Serviço salvo com sucesso.", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Serviços", $"Não foi possível salvar: {ex.Message}", "OK");
		}
	}

	private async void OnExcluirClicked(object? sender, EventArgs e)
	{
		if (_selecionado is null)
			return;

		var confirmar = await DisplayAlertAsync("Excluir", $"Excluir o serviço {_selecionado.Nome}?", "Sim", "Não");

		if (!confirmar)
			return;

		try
		{
			var dao = new ServicoDAO(await Database.Instance.InitializeAsync());
			await dao.DeleteAsync(_selecionado);

			_selecionado = null;
			LimparFormulario();
			await CarregarAsync();
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Serviços", $"Não foi possível excluir: {ex.Message}", "OK");
		}
	}

	private void PreencherFormulario(Servico servico)
	{
		UnidadePicker.SelectedItem = (UnidadePicker.ItemsSource as IEnumerable<Unidade>)?
			.FirstOrDefault(u => u.IdUnidade == servico.Unidade);
		NomeEntry.Text = servico.Nome;
		PrecoEntry.Text = servico.Preco.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
		DuracaoEntry.Text = servico.DuracaoMinutos.ToString(System.Globalization.CultureInfo.InvariantCulture);
		AtivoSwitch.IsToggled = servico.EstaAtivo;
		ExcluirBtn.IsEnabled = true;
	}

	private void LimparFormulario()
	{
		NomeEntry.Text = string.Empty;
		PrecoEntry.Text = string.Empty;
		DuracaoEntry.Text = "30";
		AtivoSwitch.IsToggled = true;
		ExcluirBtn.IsEnabled = false;
	}
}
