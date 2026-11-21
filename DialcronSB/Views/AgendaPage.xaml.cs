using System.Collections.ObjectModel;
using DialcronSB.Data;
using DialcronSB.Data.DAO;
using DialcronSB.Models;

namespace DialcronSB.Views;

public sealed class OpcaoSituacao
{
	public required string Codigo { get; init; }
	public required string Texto { get; init; }
}

public sealed class AtendimentoItem
{
	public required BelAtendimento Atendimento { get; init; }
	public required CadAtendimento Cabecalho { get; init; }
	public required string Horario { get; init; }
	public required string NomeCliente { get; init; }
	public required string NomeProfissional { get; init; }
	public required string SituacaoTexto { get; init; }
}

public partial class AgendaPage : ContentPage
{
	private static readonly OpcaoSituacao[] _situacoes =
	{
		new() { Codigo = "a", Texto = "Agendado" },
		new() { Codigo = "v", Texto = "Confirmado" },
		new() { Codigo = "f", Texto = "Concluído" },
		new() { Codigo = "c", Texto = "Cancelado" },
		new() { Codigo = "n", Texto = "Não compareceu" }
	};

	private readonly ObservableCollection<AtendimentoItem> _atendimentos = new();
	private readonly ObservableCollection<Servico> _servicosDisponiveis = new();
	private readonly ObservableCollection<Cliente> _clientesDisponiveis = new();
	private List<Cliente> _todosClientes = new();
	private AtendimentoItem? _selecionado;
	private Cliente? _clienteSelecionado;
	private bool _carregando;

	public AgendaPage()
	{
		InitializeComponent();

		AtendimentosCollection.ItemsSource = _atendimentos;
		ServicosCollection.ItemsSource = _servicosDisponiveis;
		ClientesCollection.ItemsSource = _clientesDisponiveis;
		SituacaoPicker.ItemsSource = _situacoes;
		DiaPicker.Date = DateTime.Today;
		DataPicker.Date = DateTime.Today;
		HoraPicker.Time = TimeSpan.FromHours(DateTime.Now.Hour);
		SituacaoPicker.SelectedIndex = 0;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await CarregarCadastrosAsync();
		await CarregarDiaAsync();
	}

	private async Task CarregarCadastrosAsync()
	{
		try
		{
			var connection = await Database.Instance.InitializeAsync();
			var unidades = await new UnidadeDAO(connection).GetAllAsync();
			var clientes = await new ClienteDAO(connection).GetAllAsync();
			var profissionais = await new ProfissionalDAO(connection).GetAllAsync();
			var servicos = await new ServicoDAO(connection).GetAllAsync();

			var unidadeAnterior = UnidadePicker.SelectedItem as Unidade;
			var clienteAnterior = _clienteSelecionado;
			var profissionalAnterior = ProfissionalPicker.SelectedItem as Profissional;

			UnidadePicker.ItemsSource = unidades;
			_todosClientes = clientes;
			ProfissionalPicker.ItemsSource = profissionais.Where(p => p.EstaAtivo).ToList();

			if (unidadeAnterior is not null)
				UnidadePicker.SelectedItem = unidades.FirstOrDefault(u => u.IdUnidade == unidadeAnterior.IdUnidade);
			else if (unidades.Count > 0)
				UnidadePicker.SelectedIndex = 0;

			_clienteSelecionado = clienteAnterior is not null
				? clientes.FirstOrDefault(c => c.IdCliente == clienteAnterior.IdCliente)
				: null;
			AplicarFiltroClientes();
			AtualizarClienteSelecionadoLabel();

			if (profissionalAnterior is not null)
				ProfissionalPicker.SelectedItem = profissionais.FirstOrDefault(p => p.Id == profissionalAnterior.Id);

			_servicosDisponiveis.Clear();
			foreach (var servico in servicos.Where(s => s.EstaAtivo))
				_servicosDisponiveis.Add(servico);
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Agenda", $"Não foi possível carregar os cadastros: {ex.Message}", "OK");
		}
	}

	private void AplicarFiltroClientes()
	{
		var termo = ClienteBuscaBar.Text?.Trim() ?? string.Empty;
		var termoDigitos = Formatador.SomenteDigitos(termo);
		var buscarPorDigitos = termoDigitos.Length > 0;

		_clientesDisponiveis.Clear();

		foreach (var cliente in _todosClientes.Where(c =>
			string.IsNullOrEmpty(termo)
			|| c.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase)
			|| (buscarPorDigitos && (
				Formatador.SomenteDigitos(c.CpfCnpj).Contains(termoDigitos, StringComparison.Ordinal)
				|| Formatador.SomenteDigitos($"{c.DddTelefone}{c.Telefone}").Contains(termoDigitos, StringComparison.Ordinal)
				|| Formatador.SomenteDigitos($"{c.DddCelular}{c.Celular}").Contains(termoDigitos, StringComparison.Ordinal)))))
		{
			_clientesDisponiveis.Add(cliente);
		}

		ClientesVaziosLabel.IsVisible = _clientesDisponiveis.Count == 0;
	}

	private void OnClienteBuscaTextChanged(object? sender, TextChangedEventArgs e)
	{
		AplicarFiltroClientes();

		if (_clienteSelecionado is not null
			&& !_clientesDisponiveis.Any(c => c.IdCliente == _clienteSelecionado.IdCliente))
		{
			_clienteSelecionado = null;
			ClientesCollection.SelectedItem = null;
			AtualizarClienteSelecionadoLabel();
		}
	}

	private void OnClienteSelecionado(object? sender, SelectionChangedEventArgs e)
	{
		_clienteSelecionado = e.CurrentSelection.FirstOrDefault() as Cliente;
		AtualizarClienteSelecionadoLabel();
	}

	private void AtualizarClienteSelecionadoLabel()
	{
		ClienteSelecionadoLabel.Text = _clienteSelecionado is null
			? string.Empty
			: $"Selecionado: {_clienteSelecionado.Nome}";
		ClienteSelecionadoLabel.IsVisible = _clienteSelecionado is not null;
	}

	private async Task CarregarDiaAsync()
	{
		try
		{
			var connection = await Database.Instance.InitializeAsync();
			var dao = new AtendimentoDAO(connection);
			var atendimentos = await dao.GetByDiaAsync(DiaPicker.Date ?? DateTime.Today);
			var cabecalhos = (await dao.GetCabecalhosAsync()).ToDictionary(c => c.Id);
			var clientes = (await new ClienteDAO(connection).GetAllAsync()).ToDictionary(c => c.IdCliente);
			var profissionais = (await new ProfissionalDAO(connection).GetAllAsync()).ToDictionary(p => p.Id);

			_atendimentos.Clear();

			foreach (var atendimento in atendimentos)
			{
				var cabecalho = cabecalhos.GetValueOrDefault(atendimento.Atendimento);

				if (cabecalho is null)
					continue;

				_atendimentos.Add(new AtendimentoItem
				{
					Atendimento = atendimento,
					Cabecalho = cabecalho,
					Horario = DateTime.Parse(atendimento.DtaAtendimento, System.Globalization.CultureInfo.InvariantCulture).ToString("HH:mm"),
					NomeCliente = cabecalho.Cliente is int idCliente && clientes.TryGetValue(idCliente, out var cliente) ? cliente.Nome : "-",
					NomeProfissional = profissionais.TryGetValue(atendimento.Profissional, out var profissional) ? profissional.Nome : "-",
					SituacaoTexto = _situacoes.FirstOrDefault(s => s.Codigo == atendimento.Situacao)?.Texto ?? atendimento.Situacao
				});
			}
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Agenda", $"Não foi possível carregar: {ex.Message}", "OK");
		}
	}

	private async void OnDiaSelecionado(object? sender, DateChangedEventArgs e)
	{
		await CarregarDiaAsync();
	}

	private async void OnAtendimentoSelecionado(object? sender, SelectionChangedEventArgs e)
	{
		_selecionado = e.CurrentSelection.FirstOrDefault() as AtendimentoItem;

		if (_selecionado is null)
		{
			LimparFormulario();
			return;
		}

		await PreencherFormularioAsync(_selecionado);
	}

	private void OnServicosSelecionados(object? sender, SelectionChangedEventArgs e)
	{
		if (_carregando)
			return;

		var total = e.CurrentSelection.OfType<Servico>().Sum(s => s.Preco);
		ValorEntry.Text = total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
	}

	private void OnIncluirClicked(object? sender, EventArgs e)
	{
		AtendimentosCollection.SelectedItem = null;
		_selecionado = null;
		LimparFormulario();
	}

	private async void OnSalvarClicked(object? sender, EventArgs e)
	{
		try
		{
			if (UnidadePicker.SelectedItem is not Unidade unidade)
			{
				await DisplayAlertAsync("Agenda", "Selecione a unidade. Se não houver nenhuma, cadastre em Unidades.", "OK");
				return;
			}

			if (_clienteSelecionado is null)
			{
				await DisplayAlertAsync("Agenda", "Selecione o cliente.", "OK");
				return;
			}

			var cliente = _clienteSelecionado;

			if (ProfissionalPicker.SelectedItem is not Profissional profissional)
			{
				await DisplayAlertAsync("Agenda", "Selecione o profissional.", "OK");
				return;
			}

			var servicos = ServicosCollection.SelectedItems.OfType<Servico>().Select(s => s.Id).ToList();

			if (servicos.Count == 0)
			{
				await DisplayAlertAsync("Agenda", "Selecione ao menos um serviço.", "OK");
				return;
			}

			var situacao = (SituacaoPicker.SelectedItem as OpcaoSituacao)?.Codigo ?? "a";
			var dataHora = (DataPicker.Date ?? DateTime.Today).Date + (HoraPicker.Time ?? TimeSpan.Zero);
			var agora = AtendimentoDAO.FormatarDataHora(DateTime.Now);

			var dao = new AtendimentoDAO(await Database.Instance.InitializeAsync());

			if (_selecionado is null)
			{
				var cabecalho = new CadAtendimento
				{
					Unidade = unidade.IdUnidade,
					Cliente = cliente.IdCliente,
					Descricao = Conversor.ParaTextoOuNulo(ObservacaoEntry.Text),
					Situacao = situacao,
					DtaCriacao = agora
				};

				var atendimento = new BelAtendimento
				{
					Profissional = profissional.Id,
					DtaAtendimento = AtendimentoDAO.FormatarDataHora(dataHora),
					DtaCriacao = agora,
					Situacao = situacao,
					Valor = Conversor.ParaDouble(ValorEntry.Text),
					Pago = PagoSwitch.IsToggled ? 1 : 0,
					Observacao = Conversor.ParaTextoOuNulo(ObservacaoEntry.Text)
				};

				await dao.InsertAsync(cabecalho, atendimento, servicos);
			}
			else
			{
				var cabecalho = _selecionado.Cabecalho;
				cabecalho.Unidade = unidade.IdUnidade;
				cabecalho.Cliente = cliente.IdCliente;
				cabecalho.Descricao = Conversor.ParaTextoOuNulo(ObservacaoEntry.Text);
				cabecalho.Situacao = situacao;

				var atendimento = _selecionado.Atendimento;
				atendimento.Profissional = profissional.Id;
				atendimento.DtaAtendimento = AtendimentoDAO.FormatarDataHora(dataHora);
				atendimento.Situacao = situacao;
				atendimento.Valor = Conversor.ParaDouble(ValorEntry.Text);
				atendimento.Pago = PagoSwitch.IsToggled ? 1 : 0;
				atendimento.Observacao = Conversor.ParaTextoOuNulo(ObservacaoEntry.Text);

				await dao.UpdateAsync(cabecalho, atendimento, servicos);
			}

			DiaPicker.Date = dataHora.Date;
			_selecionado = null;
			await CarregarDiaAsync();
			LimparFormulario();
			await DisplayAlertAsync("Agenda", "Atendimento salvo com sucesso.", "OK");
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Agenda", $"Não foi possível salvar: {ex.Message}", "OK");
		}
	}

	private async void OnExcluirClicked(object? sender, EventArgs e)
	{
		if (_selecionado is null)
			return;

		var confirmar = await DisplayAlertAsync("Excluir", "Excluir este atendimento?", "Sim", "Não");

		if (!confirmar)
			return;

		try
		{
			var dao = new AtendimentoDAO(await Database.Instance.InitializeAsync());
			await dao.DeleteAsync(_selecionado.Cabecalho, _selecionado.Atendimento);

			_selecionado = null;
			LimparFormulario();
			await CarregarDiaAsync();
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Agenda", $"Não foi possível excluir: {ex.Message}", "OK");
		}
	}

	private async Task PreencherFormularioAsync(AtendimentoItem item)
	{
		_carregando = true;

		try
		{
			var connection = await Database.Instance.InitializeAsync();
			var servicosDoAtendimento = await new AtendimentoDAO(connection).GetServicosAsync(item.Atendimento.Id);
			var idsServicos = servicosDoAtendimento.Select(s => s.Servico).ToHashSet();

			TituloFormulario.Text = "Editar atendimento";
			UnidadePicker.SelectedItem = (UnidadePicker.ItemsSource as IEnumerable<Unidade>)?
				.FirstOrDefault(u => u.IdUnidade == item.Cabecalho.Unidade);
			_clienteSelecionado = _todosClientes.FirstOrDefault(c => c.IdCliente == item.Cabecalho.Cliente);
			ClienteBuscaBar.Text = _clienteSelecionado?.Nome ?? string.Empty;
			AplicarFiltroClientes();
			ClientesCollection.SelectedItem = _clienteSelecionado;
			AtualizarClienteSelecionadoLabel();
			ProfissionalPicker.SelectedItem = (ProfissionalPicker.ItemsSource as IEnumerable<Profissional>)?
				.FirstOrDefault(p => p.Id == item.Atendimento.Profissional);
			SituacaoPicker.SelectedItem = _situacoes.FirstOrDefault(s => s.Codigo == item.Atendimento.Situacao);

			var dataHora = DateTime.Parse(item.Atendimento.DtaAtendimento, System.Globalization.CultureInfo.InvariantCulture);
			DataPicker.Date = dataHora.Date;
			HoraPicker.Time = dataHora.TimeOfDay;
			ValorEntry.Text = item.Atendimento.Valor.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
			PagoSwitch.IsToggled = item.Atendimento.Pago == 1;
			ObservacaoEntry.Text = item.Atendimento.Observacao;

			ServicosCollection.SelectedItems.Clear();
			foreach (var servico in _servicosDisponiveis.Where(s => idsServicos.Contains(s.Id)))
				ServicosCollection.SelectedItems.Add(servico);

			ExcluirBtn.IsEnabled = true;
		}
		finally
		{
			_carregando = false;
		}
	}

	private void LimparFormulario()
	{
		_carregando = true;

		TituloFormulario.Text = "Novo atendimento";
		_clienteSelecionado = null;
		ClienteBuscaBar.Text = string.Empty;
		AplicarFiltroClientes();
		ClientesCollection.SelectedItem = null;
		AtualizarClienteSelecionadoLabel();
		ProfissionalPicker.SelectedItem = null;
		SituacaoPicker.SelectedIndex = 0;
		DataPicker.Date = DiaPicker.Date;
		HoraPicker.Time = TimeSpan.FromHours(DateTime.Now.Hour);
		ValorEntry.Text = string.Empty;
		PagoSwitch.IsToggled = false;
		ObservacaoEntry.Text = string.Empty;
		ServicosCollection.SelectedItems.Clear();
		ExcluirBtn.IsEnabled = false;

		_carregando = false;
	}
}
