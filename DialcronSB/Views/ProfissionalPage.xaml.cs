using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DialcronSB.Views;

public partial class ProfissionalPage : ContentPage
{
	private readonly ObservableCollection<ProfissionalItem> _profissionais = new();
	private ProfissionalItem? _selecionado;
	private int _proximoId = 1;

	public ProfissionalPage()
	{
		InitializeComponent();

		ProfissionaisCollection.ItemsSource = _profissionais;
	}

	private void OnAtivoToggled(object? sender, ToggledEventArgs e)
	{
		AtivoStatusLabel.Text = e.Value ? "Ativo" : "Inativo";
	}

	private void OnSelecaoChanged(object? sender, SelectionChangedEventArgs e)
	{
		_selecionado = e.CurrentSelection.FirstOrDefault() as ProfissionalItem;

		if (_selecionado is null)
			return;

		NomeEntry.Text = _selecionado.Nome;
		AtivoSwitch.IsToggled = _selecionado.Ativo;
	}

	private void OnIncluirClicked(object? sender, EventArgs e)
	{
		_selecionado = null;
		ProfissionaisCollection.SelectedItem = null;
		NomeEntry.Text = string.Empty;
		AtivoSwitch.IsToggled = true;
		NomeEntry.Focus();
	}

	private async void OnSalvarClicked(object? sender, EventArgs e)
	{
		var nome = NomeEntry.Text?.Trim() ?? string.Empty;

		if (string.IsNullOrWhiteSpace(nome))
		{
			await DisplayAlertAsync("Profissionais", "Informe o nome do profissional.", "OK");
			return;
		}

		if (_selecionado is null)
		{
			_selecionado = new ProfissionalItem
			{
				Id = _proximoId++,
				Nome = nome,
				Ativo = AtivoSwitch.IsToggled
			};

			_profissionais.Add(_selecionado);
			ProfissionaisCollection.SelectedItem = _selecionado;
		}
		else
		{
			_selecionado.Nome = nome;
			_selecionado.Ativo = AtivoSwitch.IsToggled;
		}
	}

	private void OnCancelarClicked(object? sender, EventArgs e)
	{
		_selecionado = null;
		ProfissionaisCollection.SelectedItem = null;
		NomeEntry.Text = string.Empty;
		AtivoSwitch.IsToggled = true;
	}
}

public sealed class ProfissionalItem : INotifyPropertyChanged
{
	private string _nome = string.Empty;

	public int Id { get; set; }

	public string Nome
	{
		get => _nome;
		set
		{
			if (_nome == value)
				return;

			_nome = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nome)));
		}
	}

	public bool Ativo { get; set; }

	public event PropertyChangedEventHandler? PropertyChanged;
}
