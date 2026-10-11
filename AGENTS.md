# AGENTS.md - Guia para Assistentes de Codigo

## Projeto

**Dialcron** - App de agendamento para salao de beleza, multiplataforma (Windows + Android). Mapeado a partir do banco Dialcron3 (MySQL).

## Stack Tecnologica

| Componente       | Tecnologia                        |
|------------------|-----------------------------------|
| Framework        | .NET MAUI                         |
| Arquitetura      | Code-behind + DAO (2 camadas)     |
| ORM              | sqlite-net-pcl                    |
| Calendario       | Plugin.Maui.Calendar              |
| Linguagem        | C#                                |

## Padroes e Convencoes

### Arquitetura

- **2 camadas**: Models + DAO (dados) e Views/Pages (interface)
- Pages XAML chamam os DAOs diretamente no code-behind (sem ViewModel)
- **DAO Pattern** para acesso a banco de dados (cada entidade tem seu DAO)
- **Injecao de dependencia** via `MauiProgram.cs`

### Nomenclatura

- Models: substantivos no singular (`Cliente`, `Servico`, `Profissional`, `Atendimento`)
- DAOs: `{NomeModel}DAO` (`ClienteDAO`, `ServicoDAO`, etc.)
- Views: `{NomeFeature}Page.xaml` (XAML + code-behind `{NomeFeature}Page.xaml.cs`)
- Metodos CRUD: `GetAllAsync`, `GetByIdAsync`, `InsertAsync`, `UpdateAsync`, `DeleteAsync`
- Propriedades: PascalCase
- Variaveis locais: camelCase
- Privados: `_camelCase` com underscore

### Nomenclatura de Tabelas (SQLite)

- **`cad_*`** = Tabelas GLOBAIS (clientes, unidades, usuarios, atendimentos, etc.)
- **`bel_*`** = Tabelas do segmento SALAO DE BELEZA
- Colunas: manter padrao MySQL (`dta_agendamento`, `dta_criacao`, `ativo`, etc.)

### Codigo

- Usar `async/await` para todas as operacoes com banco de dados
- Chamar os DAOs direto no code-behind das Pages (sem ViewModel)
- Manter logica de negocios simples, concentrada nos DAOs
- Comentarios APENAS quando solicitado pelo usuario
- Nao adicionar emojis nos arquivos

## Estrutura de Arquivos

```
.github/
└── workflows/
    └── build.yml                 # CI/CD: compila Windows + Android
.gitignore                        # Ignora build outputs; permite versionar DialcronSB/build/DialcronSBDB.db e o seed
Documentacao/                     # Artefatos de requisitos e documentacao
├── SalaoDiagrama.drawio          # Diagrama de classes (editavel)
├── SalaoDiagrama-Classe.drawio.png # Diagrama de classes (exportado)
├── SalaoDiagrama-DER.drawio.png  # Diagrama DER (exportado)
├── Dialcron30SL.docx             # Documento de Especificacao de Software
├── Dialcron30SL.pptx             # Apresentacao (pitch)
├── DialcronSB.sql                # Script de criacao do banco (SQLite)
├── DialcronMySQL.sql             # Banco original Dialcron3 (MySQL)
├── CriacaoDeTelas.md             # Guia de criacao de telas
├── Paleta.txt                    # Paleta de cores
├── PR-TEMPLATE.txt               # Modelo padrao de descricao de pull request
└── TemplateUNA.pptx              # Template da apresentacao
DocumentacaoPrivada/             # Arquivos locais (ignorados pelo git; nao vao para o repositorio remoto)
DialcronSB/
├── Directory.Build.props         # Saida em build/, exe DialcronSB-Win-<Config> e publish standalone em release/
├── DialcronSB-Release.lnk        # Atalho para DialcronSB/build/DialcronSB-Win-Release.exe
├── build/                        # Saida de compilacao (gerada; nao versionada)
├── release/                      # Executavel standalone single-file (gerada; nao versionada)
├── Resources/Raw/
│   └── DialcronSB_PADRAO.db      # Banco-padrao (seed), versionado e embutido no app
├── Models/                       # Modelos do dominio (a implementar)
├── Data/
│   ├── Database.cs               # Singleton + caminhos platform-specific
│   └── DAO/
│       └── BackupDAO.cs          # Copia consistente do banco (VACUUM INTO)
├── Views/                        # Pages XAML + code-behind
├── MainPage.xaml                 # Tela inicial + Backup (Salvar no Windows / Compartilhar no Android)
├── App.xaml.cs                   # Validacao de banco na inicializacao
├── AppShell.xaml                 # Shell de navegacao
└── MauiProgram.cs               # Configuracao DI + startup
```

## Inicializacao do Banco de Dados

### Caminhos por Plataforma

Definidos em `Database.BuildDbPath()`:

- **Windows**: `AppDomain.CurrentDomain.BaseDirectory` (ao lado do .exe, sem subpasta; com a saida consolidada, fica em `DialcronSB/build/` ou `DialcronSB/release/`)
- **Android**: `FileSystem.AppDataDirectory` + `DialcronAgendamentos` (= `FilesDir/DialcronAgendamentos/DialcronSBDB.db`)
- O `ApplicationId` e `br.com.dialcron.salao`; trocar o pacote exige desinstalar o app anterior

### Banco-Padrao (seed embutido)

- O arquivo `DialcronSB/Resources/Raw/DialcronSB_PADRAO.db` e o banco vazio com as 9 tabelas (schema identico ao `Documentacao/DialcronSB.sql`)
- Por estar em `Resources/Raw/**`, ele e embutido no pacote automaticamente pelo `MauiAsset` do `.csproj`
- No primeiro uso, `CreateDatabaseAsync()` copia o seed do pacote (`FileSystem.OpenAppPackageFileAsync`) para `DbPath`
- No Android o `.db` fica na pasta privada do app: sem permissao, invisivel no gerenciador de arquivos e apagado ao desinstalar. Inspecao via `adb shell run-as br.com.dialcron.salao ls files/DialcronAgendamentos/`

### Fluxo de Inicializacao (MainPage.OnAppearing)

```
MainPage aparece
  -> Database.Instance.InitializeAsync()
    -> ja inicializado? SIM: retorna a mesma conexao
    -> DatabaseExists()? SIM: OpenConnectionAsync()
                     NAO: CreateDatabaseAsync() (copia o seed)
    -> PRAGMA foreign_keys = ON
```

### Metodos Database.cs

- `Instance` (singleton estatico) - acessado direto pelas Pages (`AppShell`/`App` sao instanciados com `new`, sem DI)
- `DbPath` (propriedade estatica) - caminho do banco por plataforma
- `DatabaseExists()` - verifica se o arquivo existe
- `CreateDatabaseAsync()` - cria diretorio + copia o seed do pacote
- `OpenConnectionAsync()` - abre a conexao e ativa `PRAGMA foreign_keys = ON`
- `InitializeAsync()` - idempotente: cria o banco se faltar e devolve a `SQLiteAsyncConnection`

### Banco Versionado (sincronizacao entre PCs)

- O seed `DialcronSB/Resources/Raw/DialcronSB_PADRAO.db` e versionado no git e serve de base para novos bancos
- O banco de trabalho `DialcronSB/build/DialcronSBDB.db` e criado no primeiro uso (ao lado do `.exe`); o `.gitignore` permite versiona-lo, mas a pasta `build/` e gerada sob demanda e normalmente nao contem o banco no repositorio
- O target `CopySeedDatabase` (pos-publish) apenas remove o `.pdb` de debug; nao copia banco
- O `.gitignore` permite versionar `DialcronSB/build/DialcronSBDB.db` e `DialcronSB/Resources/Raw/*.db` (seed), ignorando os demais `*.db`
- A sincronizacao vale **apenas entre PCs**; no Android o banco vive no aparelho (use Backup/Restore para levar dados entre dispositivos)
- Cuidado: por ser binario, editar o banco em varios PCs ao mesmo tempo gera conflito de merge (sincronizar um PC por vez)

## Backup do Banco

- O botao "Backup" da `MainPage` gera o arquivo e o entrega conforme a plataforma
- Nome padrao: `DialcronSB_Backup_yyyyMMddHHmm.db` (`BackupDAO.BuildBackupFileName()`, com `InvariantCulture`)
- A copia usa `VACUUM INTO` (snapshot consistente em arquivo unico); o destino e apagado antes, pois o comando falha se o arquivo ja existir
- **Windows**: gera o arquivo em `FileSystem.CacheDirectory` e abre o dialogo nativo "Salvar como" via `FileSaver.Default.SaveAsync` (CommunityToolkit.Maui); o temporario e removido ao concluir
- **Android**: gera o arquivo em `FileSystem.CacheDirectory/sharing-root` (pasta compartilhada pelo FileProvider do MAUI) e abre o seletor de apps via `Share.Default.RequestAsync`
- O `AndroidManifest.xml` nao precisa de mudanca: o FileProvider e registrado pelo proprio Essentials

## Publicacao Standalone

### Configuracao (Directory.Build.props)

- `SelfContained=true` (runtime embutido)
- `PublishTrimmed=false` (sem trimming para MAUI)
- `WindowsPackageType=None` no `.csproj` (app desempacotado)
- `PublishSingleFile=true` (Windows) - empacota runtime + app em um unico `.exe`
- `IncludeNativeLibrariesForSelfExtract=true` + `IncludeAllContentForSelfExtract=true` (extraem dlls nativas/UWP em runtime)
- `OutputPath` = `DialcronSB/build/` (saida de build consolidada, sem subpastas no Windows) e `AssemblyName` = `DialcronSB-Win-$(Configuration)`
- `PublishDir` = `DialcronSB/release/` - Resultado: o `release/` fica apenas com `DialcronSB-Win-Release.exe` (~250 MB); o `DialcronSBDB.db` e criado ao lado dele no primeiro uso

### Alvo MSBuild `CopySeedDatabase` (apos Publish)

- O target nao copia banco: o `DialcronSBDB.db` e criado no primeiro uso do app, nao na publicacao
- Remove o `DialcronSB.pdb` (symbols de debug) da saida de publicacao
- So roda no Windows (nao interfere no build Android/CI)

### Comandos

```bash
# Publicar (gera DialcronSB/release/DialcronSB-Win-Release.exe; banco criado no 1o uso)
dotnet publish -c Release -f net10.0-windows10.0.19041.0
```

> Nota: publique SEM `-r win-x64` explicito. O MAUI Windows usa o runtime CoreCLR por padrao; forcar `-r win-x64` tenta baixar o runtime Mono e falha (NU1102).

### Limpeza de Idiomas

O target MSBuild `RemoveUnwantedLocales` roda apos Build e Publish, removendo pastas de idioma exceto:
- `pt-BR` (idioma principal)
- `en-us` (idioma fallback)
- `Platforms`, `Microsoft.UI.Xaml`, `NpuDetect` (pastas de sistema)

## CI/CD - GitHub Actions

Workflow: `.github/workflows/build.yml` - build automatico em push para `main`, pull request e `workflow_dispatch`.

### Ambiente

- Runner: `windows-latest` (compila Windows e Android no mesmo job)
- .NET SDK `10.0.x` via `actions/setup-dotnet`
- JDK 21 via `actions/setup-java` (distribuicao Microsoft) - requisito do target Android no .NET 10 (API 36)
- `dotnet workload install maui --ignore-failed-sources`
- Android SDK via `android-actions/setup-android@v3` + `sdkmanager` (platforms/build-tools 35 e 36)

### Artifacts

- **Windows**: `Dialcron-Windows.zip` - pasta de publicacao standalone (`SelfContained`, sem precisa de .NET instalado)
- **Android**: APK em `Dialcron-Android`
  - Com secrets configuradas: `Release` assinado (`AndroidKeyStore=true`, `AndroidSigningKeyStore`, etc., `AndroidPackageFormat=apk`)
  - Sem secrets: `Debug` APK (assinado com o debug keystore, instalavel)

### Secrets para APK Release assinado

| Secret                       | Descricao                              |
|------------------------------|----------------------------------------|
| `ANDROID_KEYSTORE_BASE64`    | Keystore `.jks` codificado em base64   |
| `ANDROID_KEYSTORE_PASSWORD`  | Senha do keystore (store pass)         |
| `ANDROID_KEY_ALIAS`          | Alias da chave de assinatura           |
| `ANDROID_KEY_PASSWORD`       | Senha da chave                         |

O `ApplicationVersion` e auto-incrementado com `github.run_number`.

### Publicar APK Release assinado localmente

```bash
dotnet publish -f net10.0-android -c Release \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=dialcron.keystore \
  -p:AndroidSigningKeyAlias=alias \
  -p:AndroidSigningKeyPass=senha \
  -p:AndroidSigningStorePass=senha \
  -p:AndroidPackageFormat=apk
```

## Dependencias NuGet

```bash
dotnet add package sqlite-net-pcl
dotnet add package Plugin.Maui.Calendar
dotnet add package CommunityToolkit.Maui
```

> `CommunityToolkit.Maui` esta fixado na **14.1.1**: a 15.x exige `Microsoft.Maui.Controls >= 10.0.90`, incompativel com o `MauiVersion 10.0.71` do projeto. O `UseMauiCommunityToolkit()` precisa estar no `MauiProgram.cs`.

## Abrir no VS Code (com F5 + Hot Reload)

As configuracoes de debug/hot reload ficam em `.vscode/` na **raiz do repositorio** (`.vscode/launch.json`, `tasks.json` e `settings.json`). Por isso, abra o VS Code na raiz:

```bash
cd C:\Git\DialcronSB
code .
```

- **F5** compila (`preLaunchTask`) e roda `DialcronSB/build/DialcronSB-Win-Debug.exe` (Debug consolidado)
- **Hot Reload (C#)**: settings habilitam `csharp.experimental.debug.hotReload` + `hotReloadOnSave` — edicoes no code-behind aplicam ao salvar durante o debug
- Se abrir a subpasta `DialcronSB` como workspace, o F5 nao enxerga `.vscode` e o hot reload nao funciona
- XAML "ao vivo" (editar e ver na hora) e mais limitado no VS Code; o suporte pleno e no Visual Studio ou via `dotnet watch`

## Comandos Uteis

```bash
# Restaurar pacotes
dotnet restore

# Compilar (Debug) - gera DialcronSB/build/DialcronSB-Win-Debug.exe
dotnet build -f net10.0-windows10.0.19041.0

# Rodar no Windows (Debug)
dotnet run -f net10.0-windows10.0.19041.0

# Publicar standalone (Release) - gera DialcronSB/release/DialcronSB-Win-Release.exe; banco criado no 1o uso
dotnet publish -c Release -f net10.0-windows10.0.19041.0

# Rodar no Android
dotnet run -f net10.0-android

# Gerar APK (unsigned ou Release assinado: ver secao CI/CD)
dotnet publish -f net10.0-android -c Release

# Limpar build
dotnet clean
```

## Mapeamento MySQL (Dialcron3) -> SQLite (Dialcron)

### Tabelas Globais

| MySQL            | SQLite          | Descricao                      |
|------------------|-----------------|--------------------------------|
| cad_unidade      | cad_unidade     | Unidades/filiais               |
| cad_cliente      | cad_cliente     | Clientes                       |
| cad_usuario      | cad_usuario     | Usuarios do sistema            |
| cad_atendimento  | cad_atendimento | Atendimentos (base global)     |

### Tabelas Segmento Salao

| MySQL            | SQLite              | Descricao                   |
|------------------|---------------------|-----------------------------|
| med_especialidade| bel_servico         | Servicos do salao           |
| med_medico       | bel_profissional    | Profissionais do salao      |
| med_agenda       | bel_atendimento     | Atendimentos do salao (herda de cad_atendimento) |
| (nova)           | bel_atendimento_servico | Tabela pivoi N:N        |
| (nova)           | bel_tipo_atendimento | Tipos de atendimento do salao |

### Adaptacoes MySQL -> SQLite

- Composite PK -> `INTEGER PRIMARY KEY AUTOINCREMENT`
- `AUTO_INCREMENT` -> `AUTOINCREMENT`
- varchar/int/datetime -> `TEXT`
- decimal -> `REAL`
- char(1) -> `TEXT` (mantido)
- Constraints FK -> `PRAGMA foreign_keys = ON`
- Indices -> `CREATE INDEX` explicito

## Fluxo de Implementacao

Ao implementar este projeto, seguir esta ordem:

1. Criar projeto MAUI + instalar NuGets
2. Criar Models (com atributos sqlite-net-pcl) - A IMPLEMENTAR
3. Criar Database.cs (singleton + caminhos platform-specific) - FEITO
4. Criar DAOs (CRUD por entidade) - A IMPLEMENTAR (apenas BackupDAO existe)
5. Configurar MauiProgram.cs (DI) - FEITO
6. Configurar validacao de banco na inicializacao - FEITO
7. Configurar publicacao standalone - FEITO
8. Criar Views/Pages (XAML + code-behind chamando os DAOs) - A IMPLEMENTAR
9. Testar compilacao Windows + Android

## Restricoes

- NAO usar Xamarin.Forms (usar apenas .NET MAUI)
- NAO criar arquivos duplicados
- NAO alterar a estrutura de pastas sem necessidade
- NAO versar chaves, senhas ou dados sensiveis
- Arquivos locais/pessoais (notas, textos de PR, rascunhos) vao em `DocumentacaoPrivada/`, que e ignorada pelo git e nao vai para o repositorio remoto
- NAO usar `Navigation.PushAsync` sem tratar erro de navegacao
- NAO adicionar dependencias extras sem necessidade
- Manter consistencia com o padrao DAO existente
- Cada DAO deve receber `SQLiteAsyncConnection` via construtor
- Manter padrao de nomenclatura `cad_*` (global) e `bel_*` (salao)

## Referencia de Modelos

### cad_unidade (Global)
- IdUnidade (int, PK), NomeFantasia (string), RazaoSocial (string), Cnpj (string), InscEstadual (string), InscMunicipal (string), Cep (int?), Logradouro (string), Numero (int?), Bairro (string), Complemento (string), Municipio (string), Uf (string), Pais (string), Email (string), Ddd (int?), Telefone (int?), Cliente (int?, FK)

### cad_cliente (Global)
- IdCliente (int, PK), Nome (string), Logradouro (string), Numero (int?), CpfCnpj (string), DtaNascimento (string), DtaCriacao (string), DddTelefone (int?), Telefone (int?), DddCelular (int?), Celular (int?), Email (string), Sexo (string)

### cad_usuario (Global)
- Id (int, PK), Login (string), Senha (string), Nome (string), Perfil (string), Email (string), Grupo (int?), Ativo (string), DtaCriacao (string), DtaInativacao (string)

### cad_atendimento (Global)
- Id (int, PK), Unidade (int, FK), Cliente (int?, FK), Descricao (string), Situacao (string), DtaCriacao (string), DtaEncerramento (string), UsuarioCriacao (int?, FK), UsuarioEncerramento (int?, FK)

### bel_servico (Segmento Salao)
- Id (int, PK), Unidade (int, FK), Nome (string), Preco (double), DuracaoMinutos (int), Ativo (string)

### bel_profissional (Segmento Salao)
- Id (int, PK), Unidade (int, FK), Nome (string), Especialidade (string), DddTelefone (int?), Telefone (int?), DddCelular (int?), Celular (int?), DddOutros (int?), TelefoneOutros (int?), Ativo (string)

### bel_atendimento (Segmento Salao)
- Id (int, PK), Atendimento (int, FK), Profissional (int, FK), DtaAtendimento (string), DtaCriacao (string), DtaEncerramento (string), Situacao (string), Valor (double), Pago (int), Observacao (string)

### bel_atendimento_servico (Pivoi N:N)
- Id (int, PK), Atendimento (int, FK), Servico (int, FK)

### bel_tipo_atendimento (Segmento Salao)
- Id (int, PK), Unidade (int, FK), DesTipoAtendimento (string), Ativo (string)
