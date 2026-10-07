# Como Criar uma Tela

Guia passo a passo para criar telas (Pages) no DialcronSB, seguindo o padrao do projeto: **XAML + code-behind, sem ViewModel**, com o code-behind chamando os DAOs direto.

---

## 1. Onde ficam as telas

Todas as telas ficam em `DialcronSB/Views/`:

```
DialcronSB/Views/
├── Navegacao.cs                # Helper de navegacao (Navegacao.IrPara)
├── AgendaPage.xaml              # Tela + code-behind (.xaml.cs)
├── CadastrosPage.xaml           # Menu de cadastros
├── ClientePage.xaml             # Cadastro de clientes
├── ServicoPage.xaml             # Cadastro de servicos
└── ProfissionalPage.xaml        # Cadastro de profissionais
```

Padroes de nomenclatura:

| Item | Padrao | Exemplo |
|------|--------|---------|
| Arquivo XAML | `{Feature}Page.xaml` | `ClientePage.xaml` |
| Code-behind | `{Feature}Page.xaml.cs` | `ClientePage.xaml.cs` |
| Classe | `partial class {Feature}Page : ContentPage` | `ClientePage` |
| Namespace | `DialcronSB.Views` | `namespace DialcronSB.Views;` |
| Rota | minuscula, sem acento | `cliente`, `cadastros`, `agenda` |

> A `MainPage` fica na raiz do projeto (`DialcronSB/MainPage.xaml`), e nao em `Views/`.

---

## 2. Passo a passo

### Passo 1 - Criar os dois arquivos

Em `DialcronSB/Views/`, crie o par de arquivos da nova tela:

- `MinhaTelaPage.xaml`
- `MinhaTelaPage.xaml.cs`

### Passo 2 - Registrar a rota em `AppShell.xaml.cs`

Toda tela precisa de rota, senao a navegacao falha. Em `DialcronSB/AppShell.xaml.cs`, dentro do construtor, ao lado das outras:

```csharp
Routing.RegisterRoute("minha-tela", typeof(MinhaTelaPage));
```

O nome da rota e em minusculas e sem acento; o `typeof` aponta para a classe em `DialcronSB.Views` (o `using DialcronSB.Views;` ja existe no arquivo).

### Passo 3 - Adicionar o botao que abre a tela

No XAML da tela de origem, use `x:Name`, `Clicked` e `SemanticProperties.Hint`:

```xml
<Button
    x:Name="MinhaTelaBtn"
    Text="Minha tela"
    Clicked="OnMinhaTelaClicked"
    SemanticProperties.Hint="Abre a minha tela"
    HorizontalOptions="Fill" />
```

E no code-behind da tela de origem, o handler chama o helper `Navegacao`:

```csharp
private async void OnMinhaTelaClicked(object? sender, EventArgs e)
{
	await Navegacao.IrPara("minha-tela");
}
```

> Use sempre `Navegacao.IrPara(...)`, nunca `Navigation.PushAsync(...)`: o helper (`Views/Navegacao.cs`) ja trata tela do Shell nulo e erro de navegacao, mostrando alerta ao usuario.

### Passo 4 - Se a tela precisa do banco

O singleton `Database` entrega a `SQLiteAsyncConnection`; cada DAO e criado com ela no construtor:

```csharp
using DialcronSB.Data;

var dao = new MeuDAO(await Database.Instance.InitializeAsync());
var registros = await dao.GetAllAsync();
```

Como `Database.InitializeAsync()` e idempotente, pode chamar em qualquer tela. Exemplo real: `Data/DAO/BackupDAO.cs`.

### Passo 5 - Compilar

```bash
cd C:\Git\DialcronSB\DialcronSB
dotnet build -f net10.0-windows10.0.19041.0
```

Para conferir no Android tambem:

```bash
dotnet build -f net10.0-android
```

---

## 3. Exemplo 1 - Tela simples (menu de botoes)

### `Views/MinhaTelaPage.xaml`

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="DialcronSB.Views.MinhaTelaPage"
             Title="Minha tela">

    <VerticalStackLayout
        Padding="30,0"
        Spacing="15"
        VerticalOptions="Center">

        <Label
            Text="Minha tela"
            Style="{StaticResource Headline}"
            SemanticProperties.HeadingLevel="Level1" />

        <Label
            Text="Descricao da tela"
            Style="{StaticResource SubHeadline}"
            SemanticProperties.HeadingLevel="Level2" />

        <Button
            x:Name="OpcaoUmBtn"
            Text="Opcao 1"
            Clicked="OnOpcaoUmClicked"
            SemanticProperties.Hint="Abre a opcao 1"
            HorizontalOptions="Fill" />

        <Button
            x:Name="OpcaoDoisBtn"
            Text="Opcao 2"
            Clicked="OnOpcaoDoisClicked"
            SemanticProperties.Hint="Abre a opcao 2"
            HorizontalOptions="Fill" />

    </VerticalStackLayout>

</ContentPage>
```

### `Views/MinhaTelaPage.xaml.cs`

```csharp
namespace DialcronSB.Views;

public partial class MinhaTelaPage : ContentPage
{
	public MinhaTelaPage()
	{
		InitializeComponent();
	}

	private async void OnOpcaoUmClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("minha-tela");
	}

	private async void OnOpcaoDoisClicked(object? sender, EventArgs e)
	{
		await Navegacao.IrPara("minha-tela");
	}
}
```

---

## 4. Exemplo 2 - Tela de listagem (CollectionView)

Lista carregada do banco (ou de uma lista local), com estado vazio e indicador de carregamento.

### `Views/ListaPage.xaml`

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="DialcronSB.Views.ListaPage"
             Title="Lista">

    <Grid Padding="30,0" RowSpacing="15">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <VerticalStackLayout Grid.Row="0" Spacing="15">
            <Label
                Text="Lista"
                Style="{StaticResource Headline}"
                SemanticProperties.HeadingLevel="Level1" />

            <SearchBar
                x:Name="BuscaBar"
                Placeholder="Buscar"
                SearchCommand="{Binding BuscarCommand}" />
        </VerticalStackLayout>

        <ActivityIndicator
            x:Name="CarregandoIndicator"
            Grid.Row="1"
            IsRunning="False"
            IsVisible="False" />

        <CollectionView
            x:Name="ItensCollection"
            Grid.Row="1"
            SelectionMode="None"
            EmptyView="Nenhum registro encontrado">
            <CollectionView.ItemTemplate>
                <DataTemplate>
                    <VerticalStackLayout Padding="0,10" Spacing="4">
                        <Label
                            Text="{Binding Nome}"
                            FontSize="16" />
                        <Label
                            Text="{Binding Detalhe}"
                            FontSize="13" />
                    </VerticalStackLayout>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
    </Grid>

</ContentPage>
```

### `Views/ListaPage.xaml.cs`

```csharp
using DialcronSB.Data;

namespace DialcronSB.Views;

public partial class ListaPage : ContentPage
{
	private readonly ObservableCollection<ItemLista> _itens = new();

	public ListaPage()
	{
		InitializeComponent();

		ItensCollection.ItemsSource = _itens;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		try
		{
			CarregandoIndicator.IsVisible = true;
			CarregandoIndicator.IsRunning = true;

			var dao = new MeuDAO(await Database.Instance.InitializeAsync());

			_itens.Clear();

			foreach (var item in await dao.GetAllAsync())
				_itens.Add(item);
		}
		catch (Exception ex)
		{
			await DisplayAlertAsync("Lista", $"Não foi possível carregar: {ex.Message}", "OK");
		}
		finally
		{
			CarregandoIndicator.IsRunning = false;
			CarregandoIndicator.IsVisible = false;
		}
	}
}
```

> `OnAppearing` e `async void` porque o override nao permite `async Task`. Carregue dados sempre nele (e nao no construtor), para a lista recarregar toda vez que a tela aparece.

> No Exemplo 2, `ItemLista` representa a classe de exibicao da linha. **Atencao**: `Models/` ainda esta vazio; quando o model existir, use `Text="{Binding Nome}"` direto no model e apague o model intermediario.

---

## 5. Exemplo 3 - Tela com acesso ao banco

### `Views/ListaPage.xaml.cs` (trecho do DAO)

```csharp
using DialcronSB.Data;

protected override async void OnAppearing()
{
	base.OnAppearing();

	var dao = new MeuDAO(await Database.Instance.InitializeAsync());

	_itens.Clear();

	foreach (var item in await dao.GetAllAsync())
		_itens.Add(item);
}
```

O DAO segue o padrao do projeto: recebe a `SQLiteAsyncConnection` no construtor e expoe metodos `async`. Referencia real: `Data/DAO/BackupDAO.cs`.

```csharp
using System.Globalization;
using SQLite;

namespace DialcronSB.Data.DAO;

public class MeuDAO
{
	private readonly SQLiteAsyncConnection _connection;

	public MeuDAO(SQLiteAsyncConnection connection)
	{
		_connection = connection;
	}

	public Task<List<MeuModel>> GetAllAsync()
	{
		return _connection.Table<MeuModel>().ToListAsync();
	}
}
```

> **Estado atual**: os models (`Models/`) estao vazios e o unico DAO existente e o `BackupDAO`. Este exemplo e o modelo a seguir quando voce criar o model e o DAO do dominio.

---

## 6. Convencoes do projeto

### Codigo C# (code-behind)

- Indentacao com **tabs** (arquivos `.cs`)
- Namespace unico no formato `namespace DialcronSB.Views;`
- Handlers de evento: `private async void OnXClicked(object? sender, EventArgs e)`
- Metodos e variaveis em **camelCase**;私有/privados com prefixo `_` (`_itens`)
- `async/await` em qualquer operacao com banco
- Sem ViewModel: o code-behind chama o DAO direto
- **Sem comentarios** no codigo e **sem emojis**

### XAML

- Indentacao com **4 espaços** (arquivos `.xaml`)
- `x:Class` tem que bater com o namespace + nome da classe (`DialcronSB.Views.MinhaTelaPage`)
- `x:Name` + `Clicked` ligando o controle ao handler do code-behind
- Estilos prontos: `Style="{StaticResource Headline}"`, `Style="{StaticResource SubHeadline}"`
- Cores sempre por `StaticResource` (ver `Resources/Styles/Colors.xaml`); nunca valor fixo no XAML
- `SemanticProperties.Hint` em botoes e `SemanticProperties.HeadingLevel` em titulos
- Layout externo com `Padding="30,0"`, `Spacing="15"` e `VerticalOptions="Center"`
- `HorizontalOptions="Fill"` em botoes full-width
- Rotulos com acentuacao correta em textos visiveis ("Descricao", "Opcao" sem acento so em **identificadores**)

### Banco

- Todo acesso passa por um DAO em `Data/DAO/` (nunca SQL na Page)
- Rotas sempre registradas no `AppShell.xaml.cs`
- Navegar sempre com `Navegacao.IrPara(rota)`

---

## 7. Checklist antes de commitar

```
[ ] Arquivo em DialcronSB/Views/{Feature}Page.xaml criado
[ ] Code-behind {Feature}Page.xaml.cs criado, herdando ContentPage
[ ] x:Class = DialcronSB.Views.{Feature}Page
[ ] Rota registrada no AppShell.xaml.cs
[ ] Botao com x:Name + Clicked + SemanticProperties.Hint
[ ] Handler usando Navegacao.IrPara (nada de PushAsync)
[ ] Acesso a dados via DAO (nenhum SQL na Page)
[ ] Cores via StaticResource (nenhum valor fixo)
[ ] dotnet build -f net10.0-windows10.0.19041.0 sem erro
```

---

## 8. Erros comuns

| Erro | Causa | Solucao |
|------|-------|---------|
| `No route found for minha-tela` | Rota nao registrada | `Routing.RegisterRoute("minha-tela", typeof(MinhaTelaPage));` no `AppShell.xaml.cs` |
| `XamlParseException` / tela em branco | `x:Class` diferente do namespace/classe do arquivo | `x:Class="DialcronSB.Views.MinhaTelaPage"` e `partial class MinhaTelaPage` no namespace certo |
| Botao nao faz nada | Handler com nome diferente do `Clicked` | `Clicked="OnXClicked"` tem que bater com `OnXClicked(...)` no code-behind |
| Tela abre vazia ao voltar | Dados carregados no construtor | Carregue em `OnAppearing`, nao no construtor |
| `SQLiteConnection` nulo | DAO criado sem `InitializeAsync()` | `new MeuDAO(await Database.Instance.InitializeAsync())` |
| Estilo/estetica quebrada | Valor de cor fixo no XAML | Use `StaticResource` do `Colors.xaml` |
| Delay na navegacao | Faltou `await` no handler | Handlers que aguardam precisam ser `async` + `await` |