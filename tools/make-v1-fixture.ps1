# 生成 v1 旧包的测试夹具（install.resources）。
#
# 为什么要脚本：v1 的 install.resources 原本是旧版制作端 GUI 运行时写出来的
# （见 Installation\FrmInstallation.cs），既不在 git 里、也无法用命令行重建。
# 一旦 bin 被清理，e2e 第 17 节（legacy-inspect / legacy-import）就没夹具可用了。
# 这个脚本按 LegacyV1Reader 认得的键，用 .NET 的 ResourceWriter 生成一份等价的包。
#
# 用法：powershell -ExecutionPolicy Bypass -File tools\make-v1-fixture.ps1

param(
    [string]$OutDir = ""
)

$ErrorActionPreference = 'Stop'

$repo = Split-Path -Parent $PSScriptRoot
if (-not $OutDir) {
    $OutDir = Join-Path $repo 'Installation\bin\Debug'
}

New-Item -ItemType Directory -Path $OutDir -Force | Out-Null
$target = Join-Path $OutDir 'install.resources'

# 负载：v1 的 bin 键放的是一个 zip 的字节。
# LegacyV1Reader 只从里面还原元数据（v1 的 zip 条目名是 ANSI 编码，跨语言会乱码），
# 所以这里放一个最小的合法 zip 就够。
$stage = Join-Path $env:TEMP ('v1-fixture-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $stage -Force | Out-Null
try {
    Set-Content -LiteralPath (Join-Path $stage 'App.exe') -Value 'v1 payload placeholder' -Encoding ASCII
    $zip = Join-Path $env:TEMP ('v1-fixture-' + [guid]::NewGuid().ToString('N') + '.zip')
    Compress-Archive -Path (Join-Path $stage '*') -DestinationPath $zip -Force
    $payload = [System.IO.File]::ReadAllBytes($zip)
}
finally {
    Remove-Item -Recurse -Force $stage -ErrorAction SilentlyContinue
    Remove-Item -Force $zip -ErrorAction SilentlyContinue
}

# 按 LegacyV1Reader 的 switch 分支写键
$writer = New-Object System.Resources.ResourceWriter($target)
try {
    $writer.AddResource('InstallProductName', 'V1 旧包测试产品')
    $writer.AddResource('InstallCompanyName', 'V1 测试厂商')
    $writer.AddResource('InstallProductExeName', 'App.exe')
    $writer.AddResource('InstallProductVersion', '1.0.0')
    $writer.AddResource('InstallCompanyUrl', 'https://example.invalid/')
    $writer.AddResource('InstallationPath', 'V1TestProduct')
    $writer.AddResource('LicenseAgreement', '这是 v1 旧包的许可协议占位文本。')
    $writer.AddResource('RegditFiles', 'a.dll;b.dll')
    $writer.AddResource('bin', $payload)
}
finally {
    $writer.Close()
}

Write-Host ("已生成 v1 夹具：" + $target + "（" + (Get-Item $target).Length + " 字节，负载 " + $payload.Length + " 字节）")
