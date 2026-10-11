# Dialcron - App de Agendamento para Salao de Beleza

Aplicativo multiplataforma (Windows + Android) para gerenciamento de agendamentos de salao de beleza, desenvolvido com .NET MAUI. Mapeado a partir do banco Dialcron3 (MySQL).

## Arquivos de Requisitos

Este repositorio reune os artefatos de levantamento, analise e garantia de qualidade de requisitos do projeto, produzidos na disciplina de Gestao e Qualidade de Software. A finalidade de cada arquivo e:

| Arquivo | Finalidade |
|---------|------------|
| `Documentacao\Dialcron30SL.docx` | Documento de Especificacao de Software. Contem a visao geral do sistema, a matriz de requisitos (RF01-RF06 e RNF01-RNF06, com prioridade e criterio de aceite), a modelagem de dados e relacionamentos, o resumo do diagrama de classes, a estrategia de versionamento e o plano de QA (cenarios de teste CT01-CT04).|
| `Documentacao\Dialcron30SL.pptx` | Apresentacao executiva e tecnica (pitch para stakeholders). Resume o problema e escopo, requisitos, arquitetura, pipeline de desenvolvimento e resultados esperado do projeto. |
| `Documentacao\DialcronSB.sql` | Script de criacao do banco (SQLite) que materializa a modelagem de dados descrita no documento de especificacao. |
| `Documentacao\DialcronMySQL.sql` | Script original do banco Dialcron3 (MySQL), fonte do mapeamento para o SQLite. |
| `Documentacao\SalaoDiagrama.drawio` | Diagrama de classes e EDR (editavel em draw.io) com as entidades do dominio, atributos, metodos e cardinalidades dos relacionamentos (ex.: 1:0..N). Serve de base para a secao 3.4 do documento Word. |
| `Documentacao\Dialcron30SL.pdf` | Entregável documentação. |
| `Documentacao\Dialcron30SLApresentacao.pdf` | Entregável apresentação. |
| `Documentacao\CriacaoDeTelas.md` | Guia de desenvolvimento: passo a passo para criar telas (XAML + code-behind, rotas, navegacao, acesso ao banco via DAO) e convencoes de codigo. |
| `Documentacao\PR-TEMPLATE.txt` | Modelo padrao de descricao de pull request (Resumo, Alteracoes, Testes, Observacoes), usado ao abrir um PR no GitHub. |


## Funcionalidades

- Cadastro de clientes (CRUD + busca)
- Cadastro de servicos (nome, preco, duracao)
- Cadastro de profissionais
- Atendimento com calendario visual
- Backup e restore do banco de dados
- Publicacao standalone (sem necessidade de .NET instalado)

> Escopo atual: agendamento e atribuicao de servicos. Relatorios de faturamento/financeiro ficam para um segundo momento.

## Paleta de Cores

A identidade visual utiliza a paleta definida em [`DialcronSB/Resources/Styles/Colors.xaml`](DialcronSB/Resources/Styles/Colors.xaml):

| Chave | Cor (HEX) | Finalidade |
|-------|-----------|------------|
| `Primary` | `#8E3B52` | Cor principal |
| `PrimaryDark` | `#DFA6AF` | Primária (modo escuro) |
| `Secondary` | `#F5E8E5` | Cor secundária |
| `Tertiary` | `#C77C8A` | Destaques |
| `Creme` | `#FFF8F5` | Fundo claro |
| `White` | `#FFFFFF` | Branco |
| `Black` / `Gray900` | `#3B3032` | Texto principal |
| `Gray100` - `Gray600`, `Gray950` | Tons terrosos | Neutros |
| `DarkBackground` | `#2F1D23` | Fundo (modo escuro) |
| `DarkSurface` | `#3B262C` | Superfícies (modo escuro) |
| `Sucesso` | `#72A68A` | Estados positivos |
| `Atencao` | `#D6A84F` | Alertas |
| `Erro` | `#C95C62` | Erros |

## Guia de Criação de Telas

Consulte o [Guia de Criação de Telas](Documentacao/CriacaoDeTelas.md) para orientações sobre criação de páginas (XAML + code-behind), rotas, navegação e acesso a dados via DAO.

## Pre-requisitos

- [.NET 10+ SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/)
- Extensao **C# Dev Kit** no VS Code
- **Workload MAUI** instalado (ver comando abaixo)
- **Android SDK** (para compilar para Android - via Android Studio ou standalone)
- **JDK 21** (requisito do target `net10.0-android`, API 36)

## Instalacao

```bash
# 1. Instalar o workload do MAUI
dotnet workload install maui

# 2. Criar o projeto
dotnet new maui -n DialcronSB

# 3. Entrar na pasta do projeto
cd DialcronSB

# 4. Restaurar pacotes
dotnet restore
```

## Instalar NuGet Packages

```bash
# Dentro da pasta do projeto DialcronSB/
dotnet add package sqlite-net-pcl
dotnet add package Plugin.Maui.Calendar
```

## Abrir o Projeto no VS Code

```bash
# Pelo terminal
cd C:\Git\DialcronSB\DialcronSB
code .

# Ou abrir o VS Code e ir em File > Open Folder
# Selecionar a pasta C:\Git\DialcronSB\DialcronSB
```

## Estrutura do Projeto

```
Documentacao/                    # Artefatos de requisitos e documentacao
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
DocumentacaoPrivada/            # Arquivos locais (ignorados pelo git; nao vao para o repositorio)
DialcronSB/
├── Directory.Build.props         # Saida em build/, exe DialcronSB-Win-<Config> e publish standalone em release/
├── DialcronSB-Release.lnk        # Atalho para DialcronSB/build/DialcronSB-Win-Release.exe
├── build/                        # Saida de compilacao (gerada; nao versionada)
├── release/                      # Executavel standalone single-file (gerada; nao versionada)
├── Resources/Raw/
│   └── DialcronSB_PADRAO.db      # Banco-padrao (seed) embutido no app
├── Models/                       # Modelos do dominio (a implementar)
├── Data/
│   ├── Database.cs               # Singleton + caminhos platform-specific
│   └── DAO/
│       └── BackupDAO.cs          # Copia consistente do banco (VACUUM INTO)
├── Views/                        # Pages XAML + code-behind
├── MainPage.xaml                 # Tela de Backup/Restore
├── App.xaml.cs                   # Validacao de banco na inicializacao
├── AppShell.xaml                 # Shell de navegacao
└── MauiProgram.cs               # Configuracao DI + startup
```

## Inicializacao do Banco de Dados

O banco de trabalho `DialcronSBDB.db` e criado no primeiro uso do app, copiando o banco-padrao (seed) `Resources/Raw/DialcronSB_PADRAO.db`, que esta embutido no pacote e versionado no git.

Ao abrir o app, o banco e validado automaticamente:

- **Windows**: Verifica se `DialcronSBDB.db` existe ao lado do `.exe` (em `DialcronSB/build/`; ou `DialcronSB/release/` quando publicado)
- **Android**: Verifica se o banco existe em `FilesDir/DialcronAgendamentos/DialcronSBDB.db`

Se o banco nao existir, ele e criado automaticamente a partir do seed, com as **9 tabelas** do schema (`cad_*` e `bel_*`). A tela de Backup exibe o caminho e tamanho do banco.

### Sincronizacao entre PCs

O seed `Resources/Raw/DialcronSB_PADRAO.db` e versionado e serve de base para novos bancos. O banco de trabalho `DialcronSBDB.db` tambem pode ser versionado em `DialcronSB/build/` (o `.gitignore` o permite), mas a pasta `build/` e gerada sob demanda pela compilacao e normalmente nao contem o banco no repositorio.

> Cuidado: por ser um arquivo binario, editar o banco em varios PCs ao mesmo tempo gera conflito de merge no git. Sincronize um PC por vez.

## Como Compilar e Rodar

### Windows

A saida de build e consolidada na pasta `DialcronSB/build/` pelo `Directory.Build.props`, e o executavel recebe o sufixo de configuracao: `DialcronSB-Win-Debug.exe` ou `DialcronSB-Win-Release.exe`.

```bash
# Compilar para Windows (Debug) - gera DialcronSB/build/DialcronSB-Win-Debug.exe
dotnet build -f net10.0-windows10.0.19041.0

# Compilar para Windows (Release) - gera DialcronSB/build/DialcronSB-Win-Release.exe
dotnet build -c Release -f net10.0-windows10.0.19041.0

# Rodar no Windows
dotnet run -c Release -f net10.0-windows10.0.19041.0
```

### Android

```bash
# Compilar para Android (emulador ou dispositivo)
dotnet build -f net10.0-android

# Rodar em emulador/dispositivo Android
dotnet run -f net10.0-android

# Gerar APK Release (unsigned; assinado: ver secao CI/CD)
dotnet publish -f net10.0-android -c Release
```

### Publicacao Standalone (Windows)

Gera um unico executavel com o runtime .NET embutido (sem precisar instalar o .NET na maquina). A saida e a pasta `DialcronSB/release/`, onde o app cria/usa o banco de trabalho na primeira execucao:

- `DialcronSB-Win-Release.exe` (~250 MB) - app + runtime embutidos num unico exe (single-file)
- `DialcronSBDB.db` - banco de trabalho, criado automaticamente na primeira execucao (a partir do seed embutido)

As propriedades de publicacao standalone (`SelfContained`, `PublishSingleFile`, `PublishTrimmed=false` e a pasta `DialcronSB/release/`) ficam no `Directory.Build.props`.

```bash
# Publicar (gera DialcronSB/release/DialcronSB-Win-Release.exe)
dotnet publish -c Release -f net10.0-windows10.0.19041.0
```

> Obs: publique SEM `-r win-x64` explicito (evita erro NU1102 ao tentar baixar o runtime Mono).

## CI/CD com GitHub Actions

O repositorio usa `.github/workflows/build.yml` para compilar automaticamente Windows e Android a cada push na `main` (tambem em pull requests e via `workflow_dispatch`).

### Artifacts (aba Actions > Artifacts)

- **Dialcron-Windows.zip** - executavel Windows standalone single-file (exe + banco, com runtime embutido)
- **Dialcron-Android** - APK para instalar no celular

Sem a keystore configurada, o CI gera um **APK Debug** (assinado com a chave de debug, instalavel). Para gerar **APK Release assinado**, configure as secrets do repositorio (Settings > Secrets and variables > Actions):

| Secret                       | Descricao                              |
|------------------------------|----------------------------------------|
| `ANDROID_KEYSTORE_BASE64`    | Keystore `.jks` codificado em base64   |
| `ANDROID_KEYSTORE_PASSWORD`  | Senha do keystore (store pass)         |
| `ANDROID_KEY_ALIAS`          | Alias da chave de assinatura           |
| `ANDROID_KEY_PASSWORD`       | Senha da chave                         |

O numero de versao do APK e incrementado automaticamente a cada execucao do workflow.

## Mapeamento MySQL (Dialcron3) -> SQLite (Dialcron)

### Tabelas Globais (compartilhadas entre segmentos)

| Tabela MySQL | Tabela SQLite | Descricao |
|--------------|---------------|-----------|
| cad_unidade | cad_unidade | Unidades/filiais |
| cad_cliente | cad_cliente | Clientes |
| cad_usuario | cad_usuario | Usuarios do sistema |
| cad_atendimento | cad_atendimento | Atendimentos (base global) |

### Tabelas Segmento Salao (bel_*)

| Tabela MySQL | Tabela SQLite | Descricao |
|--------------|---------------|-----------|
| med_especialidade | bel_servico | Servicos do salao |
| med_medico | bel_profissional | Profissionais do salao |
| med_agenda | bel_atendimento | Atendimentos do salao (herda de cad_atendimento) |
| (nova) | bel_atendimento_servico | Tabela pivoi N:N |
| (nova) | bel_tipo_atendimento | Tipos de atendimento do salao |

### Mudancas em relacao ao MySQL

- **Composite PK** -> `INTEGER PRIMARY KEY AUTOINCREMENT` (SQLite nao usa multiplas PKs)
- **AUTO_INCREMENT** -> `AUTOINCREMENT`
- **varchar/int/datetime** -> `TEXT` (SQLite e flexivel com tipos)
- **decimal** -> `REAL`
- **char(1)** -> `TEXT` (mantido para compatibilidade)
- **Constraints FK** -> `PRAGMA foreign_keys = ON` + `FOREIGN KEY` no CREATE TABLE
- **Indices** -> Criados explicitamente com `CREATE INDEX`

## Modelos de Dados (SQLite)

### cad_unidade (Global)

| Campo         | Tipo   | Descricao                    |
|---------------|--------|------------------------------|
| id_unidade    | int    | PK, auto-increment           |
| nome_fantasia | text   | Nome fantasia                |
| razao_social  | text   | Razao social                 |
| cnpj          | text   | CNPJ                         |
| insc_estadual | text   | Inscricao estadual           |
| insc_municipal| text   | Inscricao municipal          |
| cep           | int    | CEP                          |
| logradouro    | text   | Rua/avenida                  |
| numero        | int    | Numero                       |
| bairro        | text   | Bairro                       |
| complemento   | text   | Complemento                  |
| municipio     | text   | Cidade                       |
| uf            | text   | Estado (UF)                  |
| pais          | text   | Pais                         |
| email         | text   | Email                        |
| ddd           | int    | DDD                          |
| telefone      | int    | Telefone                     |
| cliente       | int    | FK -> cad_cliente            |

### cad_cliente (Global)

| Campo         | Tipo   | Descricao                    |
|---------------|--------|------------------------------|
| cliente       | int    | PK, auto-increment           |
| nome          | text   | Nome completo                |
| logradouro    | text   | Endereco                     |
| numero        | int    | Numero                       |
| cpf_cnpj      | text   | CPF ou CNPJ                  |
| dta_nascimento| text   | Data de nascimento           |
| dta_criacao   | text   | Data de cadastro             |
| ddd_telefone  | int    | DDD telefone                 |
| telefone      | int    | Telefone                     |
| ddd_celular   | int    | DDD celular                  |
| celular       | int    | Celular                      |
| email         | text   | Email                        |
| sexo          | text   | M/F                          |

### cad_usuario (Global)

| Campo         | Tipo   | Descricao                    |
|---------------|--------|------------------------------|
| id            | int    | PK, auto-increment           |
| login         | text   | Login                        |
| senha         | text   | Senha                        |
| nome          | text   | Nome completo                |
| perfil        | text   | Perfil de acesso             |
| email         | text   | Email                        |
| grupo         | int    | Grupo de permissoes          |
| ativo         | text   | S = ativo, N = inativo       |
| dta_criacao   | text   | Data de criacao              |
| dta_inativacao| text   | Data de inativacao           |

### cad_atendimento (Global)

| Campo              | Tipo   | Descricao                              |
|--------------------|--------|----------------------------------------|
| id                 | int    | PK, auto-increment                     |
| unidade            | int    | FK -> cad_unidade                      |
| cliente            | int    | FK -> cad_cliente                      |
| descricao          | text   | Descricao do atendimento               |
| situacao           | text   | a=agendado, c=cancelado, v=confirmado, f=concluido, n=nao_compareceu |
| dta_criacao        | text   | Data/hora de criacao do agendamento    |
| dta_encerramento   | text   | Data/hora de encerramento              |
| usuario_criacao    | int    | FK -> cad_usuario                      |
| usuario_encerramento| int   | FK -> cad_usuario                      |

### bel_servico (Segmento Salao)

| Campo          | Tipo   | Descricao                    |
|----------------|--------|------------------------------|
| id             | int    | PK, auto-increment           |
| unidade        | int    | FK -> cad_unidade            |
| nome           | text   | Nome do servico              |
| preco          | real   | Preco em R$                  |
| duracao_minutos| int    | Duracao em minutos           |
| ativo          | text   | S = ativo, N = inativo       |

### bel_profissional (Segmento Salao)

| Campo         | Tipo   | Descricao                    |
|---------------|--------|------------------------------|
| id            | int    | PK, auto-increment           |
| unidade       | int    | FK -> cad_unidade            |
| nome          | text   | Nome completo                |
| especialidade | text   | Especialidade                |
| ddd_telefone  | int    | DDD telefone                 |
| telefone      | int    | Telefone                     |
| ddd_celular   | int    | DDD celular                  |
| celular       | int    | Celular                      |
| ddd_outros    | int    | DDD telefone outros          |
| telefone_outros| int   | Telefone outros              |
| ativo         | text   | S = ativo, N = inativo       |

### bel_atendimento (Segmento Salao)

| Campo           | Tipo   | Descricao                              |
|-----------------|--------|----------------------------------------|
| id              | int    | PK, auto-increment                     |
| atendimento     | int    | FK -> cad_atendimento                  |
| profissional    | int    | FK -> bel_profissional                 |
| dta_atendimento | text   | Data/hora do atendimento               |
| dta_criacao     | text   | Data/hora de criacao do registro       |
| dta_encerramento| text   | Data/hora de encerramento              |
| situacao        | text   | a=agendado, c=cancelado, v=confirmado, f=concluido, n=nao_compareceu |
| valor           | real   | Valor total do atendimento             |
| pago            | int    | 0=nao pago, 1=pago                    |
| observacao      | text   | Observacoes                            |

### bel_atendimento_servico (Pivoi N:N)

| Campo        | Tipo   | Descricao                    |
|--------------|--------|------------------------------|
| id           | int    | PK, auto-increment           |
| atendimento  | int    | FK -> bel_atendimento        |
| servico      | int    | FK -> bel_servico            |

### bel_tipo_atendimento (Segmento Salao)

| Campo                 | Tipo   | Descricao                    |
|-----------------------|--------|------------------------------|
| id                    | int    | PK, auto-increment           |
| unidade               | int    | FK -> cad_unidade            |
| des_tipo_atendimento  | text   | Descricao do tipo            |
| ativo                 | text   | S = ativo, N = inativo       |

## Notas

- O banco SQLite e criado automaticamente ao abrir o app, a partir do seed embutido (`DialcronSB_PADRAO.db`)
- O banco-padrao (seed) e versionado no git; o banco de trabalho pode ser sincronizado manualmente entre PCs via push/pull (nao e sincronizacao em tempo real)
- A pasta `DocumentacaoPrivada/` e local (ignorada pelo `.gitignore`): use-a para arquivos pessoais/notas que nao devem ir para o repositorio remoto
- Para multi-usuario/servidor, seria necessario adicionar uma API REST futuramente
- A tabela `bel_atendimento_servico` (pivoi) permite varios servicos por atendimento
- Hierarquia: `cad_atendimento` (global, registro do agendamento) -> `bel_atendimento` (segmento, dados do atendimento no salao)
- Para walk-in (sem agendamento), o sistema cria automaticamente uma cad_atendimento com dta_criacao = dta_atendimento
