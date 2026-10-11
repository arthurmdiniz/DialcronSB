# AGENTS-HOTRELOAD.md - Hot Reload no VS Code (DialcronSB)

Plano para rodar o DialcronSB no VS Code com C# Hot Reload + XAML Hot Reload.
IMPORTANTE: abrir a pasta RAIZ `C:\Git\DialcronSB` (nao a subpasta DialcronSB),
pois o `.vscode/` fica na raiz.

## Pre-requisitos (ja instalados)

| Item | Status |
|------|--------|
| Extensao .NET MAUI (`ms-dotnettools.dotnet-maui`) | OK |
| C# Dev Kit (`ms-dotnettools.csdevkit`) | OK |
| C# (`ms-dotnettools.csharp`) | OK |
| .NET SDK 10.0.401 + workload maui | OK |

## Por que debugger (e nao `dotnet watch`)

Em .NET 9+ o C# Hot Reload foi REMOVIDO do `dotnet watch`; so funciona pela
sessao de debug do C# Dev Kit. XAML Hot Reload e aplicado automaticamente pela
extensao .NET MAUI durante o debug.

## Arquivo 1 - `.vscode/tasks.json`

```json
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build",
            "command": "dotnet",
            "type": "process",
            "args": [
                "build",
                "${workspaceFolder}/DialcronSB/DialcronSB.csproj",
                "-f",
                "net10.0-windows10.0.19041.0",
                "/property:GenerateFullPaths=true",
                "/consoleloggerparameters:NoSummary"
            ],
            "problemMatcher": "$msCompile"
        }
    ]
}
```

## Arquivo 2 - `.vscode/launch.json`

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "DialcronSB (Windows - Hot Reload)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/DialcronSB/bin/Debug/net10.0-windows10.0.19041.0/win-x64/DialcronSB.exe",
            "args": [],
            "cwd": "${workspaceFolder}/DialcronSB/bin/Debug/net10.0-windows10.0.19041.0/win-x64",
            "console": "internalConsole",
            "stopAtEntry": false
        }
    ]
}
```

## Arquivo 3 - `.vscode/settings.json` (ja existe, manter)

```json
{
  "csharp.experimental.debug.hotReload": true,
  "csharp.debug.hotReloadOnSave": true,
  "csharp.debug.hotReloadVerbosity": "detailed"
}
```

## Como usar

1. `code C:\Git\DialcronSB`
2. F5 -> selecionar **DialcronSB (Windows - Hot Reload)**
3. XAML: editar + salvar -> aplica na hora
4. C#: salvar (hotReloadOnSave) ou clicar no icone 🔥 no toolbar de debug

## Notas / caveats

- O caminho usa o RID `win-x64` (confirmado em `bin\Debug\...\win-x64\DialcronSB.exe`).
- `.csproj` tem `WindowsPackageType=None` (roda como .exe solto) e
  `MauiXamlInflator=Runtime` - requisito para XAML Hot Reload.
- Se abrir a subpasta `DialcronSB` diretamente, o `.vscode` da raiz NAO e
  carregado e o hot reload nao habilita. Abrir sempre a raiz ou usar um
  `DialcronSB.code-workspace` (multi-root) - opcional.

## Opcional - `DialcronSB.code-workspace` (raiz)

```json
{
  "folders": [{ "path": "." }, { "path": "DialcronSB" }],
  "settings": {
    "csharp.experimental.debug.hotReload": true,
    "csharp.debug.hotReloadOnSave": true,
    "csharp.debug.hotReloadVerbosity": "detailed"
  }
}
```