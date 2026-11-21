<#
.SYNOPSIS
  Compila/publica o DialcronSB (MAUI) para Android, Windows ou ambos.

.DESCRIPTION
  Pergunta qual alvo deseja compilar quando -Target nao e informado:
    1) Android
    2) Windows
    3) Android + Windows

  Windows: executa dotnet publish para net10.0-windows10.0.19041.0.
           A saida standalone fica em .\release (conforme Directory.Build.props).
  Android: executa dotnet publish para net10.0-android e copia o APK gerado para .\release.

.PARAMETER Target
  Alvo de publicacao: android, windows ou ambos. Se omitido, sera solicitado interativamente.

.PARAMETER Configuration
  Configuracao de build. Padrao: Release.

.EXAMPLE
  .\publish.ps1
  .\publish.ps1 -Target windows
  .\publish.ps1 -Target ambos -Configuration Release
#>
param(
	[ValidateSet('', 'android', 'windows', 'ambos')]
	[string]$Target = '',

	[ValidateSet('Debug', 'Release')]
	[string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$projectFile = Join-Path $PSScriptRoot 'DialcronSB.csproj'

if (-not (Test-Path $projectFile)) {
	throw "Projeto nao encontrado: $projectFile"
}

function Resolve-Target {
	param([string]$Value)

	if (-not [string]::IsNullOrWhiteSpace($Value)) {
		return $Value.ToLowerInvariant()
	}

	Write-Host "Selecione o alvo para compilar:" -ForegroundColor Cyan
	Write-Host "  1) Android"
	Write-Host "  2) Windows"
	Write-Host "  3) Android + Windows"
	$choice = Read-Host "Opcao"

	switch ($choice.Trim()) {
		'1' { return 'android' }
		'2' { return 'windows' }
		'3' { return 'ambos' }
		'android' { return 'android' }
		'windows' { return 'windows' }
		'ambos' { return 'ambos' }
		default { throw "Opcao invalida: $choice. Use 1, 2, 3, android, windows ou ambos." }
	}
}

function Publish-WindowsTarget {
	param([string]$Config)

	Write-Host ""
	Write-Host "Publicando Windows ($Config)..." -ForegroundColor Cyan
	dotnet publish $projectFile -c $Config -f net10.0-windows10.0.19041.0
	if ($LASTEXITCODE -ne 0) {
		throw "Falha no dotnet publish para Windows (codigo $LASTEXITCODE)."
	}

	$releaseDir = Join-Path $PSScriptRoot 'release'
	$exe = Get-ChildItem -Path $releaseDir -Filter "DialcronSB-Win-$Config*.exe" -ErrorAction SilentlyContinue |
		Sort-Object LastWriteTime -Descending | Select-Object -First 1

	Write-Host "Windows concluido." -ForegroundColor Green
	if ($exe) {
		Write-Host "Executavel: $($exe.FullName)"
	} else {
		Write-Host "Saida: $releaseDir"
	}
}

function Publish-AndroidTarget {
	param([string]$Config)

	Write-Host ""
	Write-Host "Publicando Android ($Config)..." -ForegroundColor Cyan
	dotnet publish $projectFile -c $Config -f net10.0-android
	if ($LASTEXITCODE -ne 0) {
		throw "Falha no dotnet publish para Android (codigo $LASTEXITCODE)."
	}

	$searchPaths = @(
		Join-Path $PSScriptRoot 'bin'
		Join-Path $PSScriptRoot 'build'
	) | Where-Object { Test-Path $_ }

	$apk = $null
	if ($searchPaths.Count -gt 0) {
		$apk = Get-ChildItem -Path $searchPaths -Recurse -Filter '*.apk' -ErrorAction SilentlyContinue |
			Sort-Object LastWriteTime -Descending | Select-Object -First 1
	}

	Write-Host "Android concluido." -ForegroundColor Green
	if ($apk) {
		Write-Host "APK (origem): $($apk.FullName)"

		$releaseDir = Join-Path $PSScriptRoot 'release'
		if (-not (Test-Path $releaseDir)) {
			New-Item -ItemType Directory -Path $releaseDir | Out-Null
		}

		$releaseApk = Join-Path $releaseDir $apk.Name
		Copy-Item -Path $apk.FullName -Destination $releaseApk -Force
		Write-Host "APK (release): $releaseApk"
	} else {
		Write-Host "APK nao encontrado automaticamente em .\bin ou .\build; verifique a saida do publish." -ForegroundColor Yellow
	}
}

$resolvedTarget = Resolve-Target -Value $Target

switch ($resolvedTarget) {
	'windows' { Publish-WindowsTarget -Config $Configuration }
	'android' { Publish-AndroidTarget -Config $Configuration }
	'ambos' {
		Publish-WindowsTarget -Config $Configuration
		Publish-AndroidTarget -Config $Configuration
	}
}

Write-Host ""
Write-Host "Concluido com sucesso!" -ForegroundColor Green
