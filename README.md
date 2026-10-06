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
| `Documentacao\Diagrama\SalaoDiagrama.drawio` | Diagrama de classes e EDR (editavel em draw.io) com as entidades do dominio, atributos, metodos e cardinalidades dos relacionamentos (ex.: 1:0..N). Serve de base para a secao 3.4 do documento Word. |
| `Documentacao\Dialcron30SL.pdf` | Entregável documentação. |
| `Documentacao\Dialcron30SLApresentacao.pdf` | Entregável apresentação. |
| `Documentacao\CriacaoDeTelas.md` | Guia de desenvolvimento: passo a passo para criar telas (XAML + code-behind, rotas, navegacao, acesso ao banco via DAO) e convencoes de codigo. |


## Funcionalidades

- Cadastro de clientes (CRUD + busca)
- Cadastro de servicos (nome, preco, duracao)
- Cadastro de profissionais
- Atendimento com calendario visual
- Backup e restore do banco de dados
- Publicacao standalone (sem necessidade de .NET instalado)

> Escopo atual: agendamento e atribuicao de servicos. Relatorios de faturamento/financeiro ficam para um segundo momento.

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
├── Diagrama/
│   ├── SalaoDiagrama.drawio      # Diagrama de classes (editavel)
│   └── SalaoDiagrama.drawio.png  # Diagrama de classes (exportado)
├── Dialcron30SL.docx             # Documento de Especificacao de Software
├── Dialcron30SL.pptx             # Apresentacao (pitch)
├── DialcronSB.sql            # Script de criacao do banco (SQLite)
├── DialcronMySQL.sql             # Banco original Dialcron3 (MySQL)
└── TemplateUNA.pptx              # Template da apresentacao
publish/                          # Publicacao standalone (Windows)
└── DialcronSBDB.db           # Banco "fonte" versionado (sincronizado entre PCs)
DialcronSB/
├── publish.ps1                   # Script de publicacao standalone
├── Models/
│   ├── Matriz.cs                 # Modelo de matriz
│   ├── Unidade.cs                # Modelo de unidade
│   ├── Cliente.cs                # Modelo de cliente
│   ├── Usuario.cs                # Modelo de usuario
│   ├── Servico.cs                # Modelo de servico
│   ├── Profissional.cs           # Modelo de profissional
│   ├── Atendimento.cs            # Modelo de atendimento (global)
│   ├── BelAtendimento.cs         # Modelo de atendimento (salao)
│   ├── AtendimentoServico.cs     # Modelo pivoi N:N
│   └── TipoAtendimento.cs        # Tipo de atendimento (salao)
├── Data/
│   ├── Database.cs               # Singleton + caminhos platform-specific
│   └── DAO/
│       ├── MatrizDAO.cs          # CRUD de matrizes
│       ├── UnidadeDAO.cs         # CRUD de unidades
│       ├── ClienteDAO.cs         # CRUD de clientes
│       ├── UsuarioDAO.cs         # CRUD de usuarios
│       ├── ServicoDAO.cs         # CRUD de servicos
│       ├── ProfissionalDAO.cs    # CRUD de profissionais
│       ├── AtendimentoDAO.cs     # CRUD de atendimentos (global)
│       ├── BelAtendimentoDAO.cs  # CRUD de atendimentos (salao)
│       ├── AtendimentoServicoDAO.cs # CRUD pivoi N:N
│       ├── TipoAtendimentoDAO.cs # CRUD tipos de atendimento (salao)
│       └── RelatorioDAO.cs      # Queries (previsto para 2a fase: relatorios financeiros)
├── Views/                        # Pages XAML + code-behind (inclui MainPage)
├── MainPage.xaml                 # Tela de Backup/Restore
├── App.xaml.cs                   # Validacao de banco na inicializacao
├── AppShell.xaml                 # Shell de navegacao
└── MauiProgram.cs               # Configuracao DI + startup
```

## Inicializacao do Banco de Dados

O banco `publish/DialcronSBDB.db` e versionado no git, permitindo sincronizar os dados entre varios PCs (basta clonar/atualizar o repo para ter o banco mais recente ao lado do executavel).

Ao abrir o app, o banco e validado automaticamente:

- **Windows**: Verifica se `DialcronSBDB.db` existe ao lado do `.exe` (na pasta `publish/`)
- **Android**: Verifica se o banco existe em `FilesDir/DialcronAgendamentos/DialcronSBDB.db`

Se o banco nao existir, ele e criado automaticamente com todas as 12 tabelas. A tela de Backup/Restore exibe o caminho e tamanho do banco.

### Sincronizacao entre PCs

1. Rode o app num PC (o banco e gerado/atualizado em `publish/DialcronSBDB.db`)
2. Commite o `publish/DialcronSBDB.db` atualizado (nao ha mais banco na raiz)
3. Nos demais PCs, atualize o repo e rode o app - o banco mais recente ja estara la

> Cuidado: por ser um arquivo binario, editar o banco em varios PCs ao mesmo tempo gera conflito de merge no git. Sincronize um PC por vez.

## Como Compilar e Rodar

### Windows

```bash
# Compilar para Windows (Debug)
dotnet build -f net10.0-windows10.0.19041.0

# Rodar no Windows
dotnet run -f net10.0-windows10.0.19041.0
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

Gera um unico executavel com o runtime .NET embutido (sem precisar instalar o .NET na maquina). A saida e a pasta `publish/` (na raiz do repo), onde tambem reside o banco de dados:

- `DialcronSB.exe` (~250 MB) - app + runtime embutidos num unico exe (single-file)
- `DialcronSBDB.db` - banco de dados (versionado, já residente em `publish/`)

```bash
# Usar o script
.\publish.ps1

# Ou manualmente
dotnet publish -f net10.0-windows10.0.19041.0 -c Release -o publish
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
| cad_matriz | cad_matriz | Empresa dona (segmento BEL) |
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

### cad_matriz (Global)

| Campo        | Tipo   | Descricao                    |
|--------------|--------|------------------------------|
| id_matriz    | int    | PK, auto-increment           |
| nome         | text   | Nome da matriz/empresa       |
| segmento     | text   | Segmento do negocio (BEL = salao) |
| ativo        | text   | S = ativo, N = inativo       |
| dta_criacao  | text   | Data de criacao              |

### cad_unidade (Global)

| Campo         | Tipo   | Descricao                    |
|---------------|--------|------------------------------|
| id_unidade    | int    | PK, auto-increment           |
| matriz        | int    | FK -> cad_matriz             |
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

- O banco SQLite e criado automaticamente ao abrir o app (com confirmacao do usuario)
- O banco e versionado no git e sincronizado manualmente entre PCs via push/pull (nao e sincronizacao em tempo real)
- Para multi-usuario/servidor, seria necessario adicionar uma API REST futuramente
- A tabela `cad_matriz` registra o segmento do negocio (BEL = salao de beleza)
- A tabela `bel_atendimento_servico` (pivoi) permite varios servicos por atendimento
- Hierarquia: `cad_atendimento` (global, registro do agendamento) -> `bel_atendimento` (segmento, dados do atendimento no salao)
- Para walk-in (sem agendamento), o sistema cria automaticamente uma cad_atendimento com dta_criacao = dta_atendimento
