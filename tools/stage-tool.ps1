#Requires -Version 5.1
<#
.SYNOPSIS
    构建并把制作端组装成一个可以直接双击运行的文件夹。

.DESCRIPTION
    产物是仓库根目录下的「制作工具\」，内容：

        安装包制作助手.exe          制作端本体
        Installer.Runtime.exe       模板 stub（制作端会自动在**同目录**找它，用户不需要选）
        AntdUI.dll / Installer.*.dll / Newtonsoft.Json.dll
        安装包制作助手.exe.config

    「制作工具\」在 .gitignore 里 —— 它是构建产物，不进仓库。

.PARAMETER Output
    输出目录。默认 <仓库根>\制作工具

.PARAMETER Configuration
    构建配置，默认 Release。

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\stage-tool.ps1
#>
[CmdletBinding()]
param(
    [string]$Output,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$repo = Split-Path -Parent $PSScriptRoot
if (-not $Output) { $Output = Join-Path $repo '制作工具' }

function Write-Step($text) { Write-Host "==> $text" -ForegroundColor Cyan }

Write-Step "构建解决方案（$Configuration）"
& dotnet build (Join-Path $repo 'Installer.sln') -c $Configuration -v q --nologo
if ($LASTEXITCODE -ne 0) { throw "构建失败（exit $LASTEXITCODE）" }

$builderOut = Join-Path $repo "src\Installer.Builder\bin\$Configuration"
$runtimeExe = Join-Path $repo "src\Installer.Runtime\bin\$Configuration\Installer.Runtime.exe"

if (-not (Test-Path (Join-Path $builderOut '安装包制作助手.exe'))) {
    throw "找不到制作端产物：$builderOut\安装包制作助手.exe"
}
if (-not (Test-Path $runtimeExe)) {
    throw "找不到 stub：$runtimeExe"
}

Write-Step "组装到 $Output"
if (Test-Path $Output) { Remove-Item -Recurse -Force $Output }
New-Item -ItemType Directory -Force -Path $Output | Out-Null

# 制作端本体 + 它直接依赖的几个程序集（其余依赖已嵌进 stub 里）
$files = @(
    '安装包制作助手.exe',
    '安装包制作助手.exe.config',
    'AntdUI.dll',
    'Installer.UI.dll',
    'Installer.Core.dll',
    'Installer.Abstractions.dll',
    'Newtonsoft.Json.dll'
)

foreach ($name in $files) {
    $src = Join-Path $builderOut $name
    if (Test-Path $src) {
        Copy-Item -LiteralPath $src -Destination $Output -Force
    } else {
        Write-Warning "缺少 $name（制作端可能起不来）"
    }
}

# 模板 stub：制作端的 StubLocator 会在自己所在目录里找它
Copy-Item -LiteralPath $runtimeExe -Destination $Output -Force

Write-Step '完成'
Get-ChildItem -File -LiteralPath $Output |
    Sort-Object Name |
    ForEach-Object { '  {0,12:N0}  {1}' -f $_.Length, $_.Name }

Write-Host ''
Write-Host "双击运行：$(Join-Path $Output '安装包制作助手.exe')" -ForegroundColor Green
