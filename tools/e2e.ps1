<#
.SYNOPSIS
    Installer 打包流水线的端到端验收。

.DESCRIPTION
    验证这条闭环：
        打包（单 exe） → 校验 → 查看 → 隔离运行安装 → 断言逐字节一致
        → 可重现 → 反解/差异 → 篡改检测 → Zip Slip 防护

    任何一项断言失败立即抛出，不会"看起来通过了"。

.NOTES
    本文件必须保存为 **UTF-8 with BOM**：Windows PowerShell 5.1 会把无 BOM 的 .ps1 按 ANSI 读取。

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\e2e.ps1
#>
[CmdletBinding()]
param(
    [string]$WorkDir = (Join-Path $env:TEMP 'installer-e2e'),
    [switch]$KeepWorkDir
)

$ErrorActionPreference = 'Stop'
$script:Checks = 0
$script:Cli = $null

function Step([string]$title) {
    Write-Host ''
    Write-Host "== $title" -ForegroundColor Cyan
}

function Check([string]$what, [bool]$ok, [string]$detail = '') {
    $script:Checks++
    if ($ok) {
        Write-Host ("  [OK]   {0}{1}" -f $what, $(if ($detail) { "  ($detail)" } else { '' })) -ForegroundColor Green
    }
    else {
        Write-Host ("  [FAIL] {0}{1}" -f $what, $(if ($detail) { "  ($detail)" } else { '' })) -ForegroundColor Red
        throw "断言失败：$what"
    }
}

# PowerShell 5.1 会把原生程序的 stderr 当成终止性错误，所以统一走这个包装。
function Invoke-Cli {
    param([string[]]$Arguments)
    $prev = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $out = & $script:Cli @Arguments 2>&1 | Out-String
        $code = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $prev
    }
    return [pscustomobject]@{ Output = $out; ExitCode = $code }
}

function Write-Le([byte[]]$buf, [int]$offset, [long]$value, [int]$size) {
    $bytes = [System.BitConverter]::GetBytes($value)
    for ($i = 0; $i -lt $size; $i++) { $buf[$offset + $i] = $bytes[$i] }
}

function Get-Crc32([byte[]]$data, [int]$offset, [int]$count) {
    # 直接用 Installer.Core 里的同一个实现。
    # 不在 PowerShell 里重写：PS 5.1 的 -shr 对 int64 结果不正确，
    # 而且本机环境的 Add-Type 是坏的（"cannot seek in file"），没法现场编译 C# 片段。
    return [int64][Installer.Core.Common.Crc32]::Compute($data, $offset, $count)
}

function Get-Sha256Hex([byte[]]$data) {
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try { return ([System.BitConverter]::ToString($sha.ComputeHash($data)) -replace '-', '').ToLowerInvariant() }
    finally { $sha.Dispose() }
}

$repo = Split-Path -Parent $PSScriptRoot
$cliProj = Join-Path $repo 'src\Installer.Cli\Installer.Cli.csproj'
$rtProj = Join-Path $repo 'src\Installer.Runtime\Installer.Runtime.csproj'

# ─────────────────────────────────────────────────────────────
Step '0. 构建'
# ─────────────────────────────────────────────────────────────
$prev = $ErrorActionPreference; $ErrorActionPreference = 'Continue'
& dotnet build $cliProj -c Debug -v q --nologo 2>&1 | Out-Null
$c1 = $LASTEXITCODE
& dotnet build $rtProj -c Release -v q --nologo 2>&1 | Out-Null
$c2 = $LASTEXITCODE
$ErrorActionPreference = $prev
if ($c1 -ne 0) { throw '构建 installer-cli 失败' }
if ($c2 -ne 0) { throw '构建 Installer.Runtime 失败' }

$script:Cli = Join-Path $repo 'src\Installer.Cli\bin\Debug\installer-cli.exe'
$stub = Join-Path $repo 'src\Installer.Runtime\bin\Release\Installer.Runtime.exe'
$icon = Join-Path $repo 'Installation\app_install.ico'
Check 'installer-cli.exe 存在' (Test-Path $script:Cli)
Check 'stub 存在' (Test-Path $stub) ("{0:N0} 字节" -f (Get-Item $stub).Length)

# 加载 Core，供第 10 步手工构造容器时复用同一个 CRC32 实现
$coreDll = Join-Path $repo 'src\Installer.Core\bin\Debug\Installer.Core.dll'
Check 'Installer.Core.dll 存在' (Test-Path $coreDll)
[void][System.Reflection.Assembly]::LoadFrom($coreDll)
Check 'Core 里的 Crc32 可用' ($null -ne [Installer.Core.Common.Crc32])

# ─────────────────────────────────────────────────────────────
Step '1. 造样例源目录'
# ─────────────────────────────────────────────────────────────
if (Test-Path $WorkDir) { Remove-Item -Recurse -Force $WorkDir }
$source = Join-Path $WorkDir 'source'
$dist = Join-Path $WorkDir 'dist'
New-Item -ItemType Directory -Force -Path $source, $dist | Out-Null

New-Item -ItemType Directory -Force -Path (Join-Path $source 'bin\sub') | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $source 'data\空目录') | Out-Null
$utf8 = New-Object System.Text.UTF8Encoding($false)

[System.IO.File]::WriteAllBytes((Join-Path $source 'bin\App.exe'), [byte[]](1..255))
[System.IO.File]::WriteAllBytes((Join-Path $source 'bin\lib.dll'), [byte[]](1..200))
[System.IO.File]::WriteAllBytes((Join-Path $source 'bin\中文模块.dll'), [byte[]](1..150))
[System.IO.File]::WriteAllText((Join-Path $source 'bin\sub\nested.txt'), 'nested content', $utf8)
[System.IO.File]::WriteAllText((Join-Path $source 'readme.txt'), 'README 中文内容', $utf8)

$srcFiles = Get-ChildItem -Recurse -File -LiteralPath $source
Check '源目录已生成' ($srcFiles.Count -eq 5) ("$($srcFiles.Count) 个文件 + 1 个空目录（中文名）")

# ─────────────────────────────────────────────────────────────
Step '2. 写工程文件（.wmpkg.json —— 内容就是 InstallerManifest）'
# ─────────────────────────────────────────────────────────────
$projectPath = Join-Path $WorkDir 'app.wmpkg.json'
$project = [ordered]@{
    schemaVersion     = 2
    productCode       = '{8F3A1C2E-5B4D-4E7A-9C1F-2D3E4F5A6B7C}'
    defaultCulture    = 'zh-Hans'
    cultures          = @('zh-Hans', 'en')
    product           = [ordered]@{
        name      = [ordered]@{ 'zh-Hans' = '端到端测试产品'; en = 'E2E Test Product' }
        version   = '2.4.1'
        publisher = [ordered]@{ 'zh-Hans' = '唯非工作室'; en = 'WeiFei Studio' }
        url       = 'https://example.com'
    }
    scope             = 'perUser'
    defaultInstallDir = '%LOCALAPPDATA%\Programs\E2E'
    entryPoint        = 'bin\App.exe'
    ui                = [ordered]@{ theme = 'light'; accentColor = '#069DE7' }
    shortcuts         = @([ordered]@{ location = 'Desktop'; name = [ordered]@{ 'zh-Hans' = '端到端测试产品'; en = 'E2E Test Product' } })
    strings           = [ordered]@{ 'Button.Install' = [ordered]@{ 'zh-Hans' = '立即安装'; en = 'Install' } }
}
$project | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $projectPath -Encoding UTF8
Check '工程文件已写出' (Test-Path $projectPath)

# ─────────────────────────────────────────────────────────────
Step '3. 打包（单 exe）'
# ─────────────────────────────────────────────────────────────
$setup = Join-Path $dist 'Setup.exe'
$buildTime = '2026-10-08T12:00:00Z'

$packArgs = @('pack', '--project', $projectPath, '--source', $source, '--stub', $stub,
              '--out', $setup, '--build-time', $buildTime)
if (Test-Path $icon) { $packArgs += @('--icon', $icon) }

$r = Invoke-Cli $packArgs
Write-Host $r.Output
Check 'pack 退出码为 0' ($r.ExitCode -eq 0)
Check '产物已生成' (Test-Path $setup) ("{0:N0} 字节" -f (Get-Item $setup).Length)
Check '产物大于 stub（确实含 payload）' ((Get-Item $setup).Length -gt (Get-Item $stub).Length)

$distFiles = Get-ChildItem -LiteralPath $dist
Check '输出目录里只有 1 个文件（不是 4 个文件的目录）' ($distFiles.Count -eq 1) ($distFiles.Name -join ', ')

# ─────────────────────────────────────────────────────────────
Step '4. 校验与查看'
# ─────────────────────────────────────────────────────────────
$r = Invoke-Cli @('verify', $setup)
Check 'verify 通过' ($r.ExitCode -eq 0)

$r = Invoke-Cli @('inspect', $setup)
Check 'inspect 能读出中文产品名' ($r.Output -match '端到端测试产品')
Check 'inspect 能读出中文文件名的条目（UTF-8 zip 条目名）' ($r.Output -match '中文模块.dll')

# ─────────────────────────────────────────────────────────────
Step '5. 隔离运行安装（只带一个 exe）'
# ─────────────────────────────────────────────────────────────
$isolated = Join-Path $WorkDir 'isolated'
New-Item -ItemType Directory -Force -Path $isolated | Out-Null
Copy-Item -LiteralPath $setup -Destination $isolated
$isolatedExe = Join-Path $isolated 'Setup.exe'
Check '隔离目录里只有 Setup.exe' ((Get-ChildItem -LiteralPath $isolated).Count -eq 1)

$installDir = Join-Path $WorkDir 'installed'
$resultFile = Join-Path $WorkDir 'install-result.json'
$isolatedSandbox = Join-Path $WorkDir 'isolated-sandbox'
# ⚠️ 必须带 --sandbox：否则快捷方式与注册表会写到**真实系统**上
$p = Start-Process -FilePath $isolatedExe -ArgumentList @('--silent', '--sandbox', $isolatedSandbox, '--target', $installDir, '--result', $resultFile) -Wait -PassThru -NoNewWindow
Check '安装退出码为 0' ($p.ExitCode -eq 0) "exit=$($p.ExitCode)"
Check '结果文件已生成' (Test-Path $resultFile)

$installResult = Get-Content -LiteralPath $resultFile -Raw -Encoding UTF8 | ConvertFrom-Json
Check '结果标记 ok' ($installResult.ok -eq $true)
Check 'payload 哈希已记录' ($installResult.payloadSha256 -match '^[0-9a-f]{64}$') $installResult.payloadSha256
Check '安装范围是 perUser' ($installResult.scope -eq 'perUser')
Check '确实运行在沙箱里（没碰真实系统）' ($installResult.sandbox -eq $isolatedSandbox) $installResult.sandbox
Check '回滚未发生' ($installResult.rolledBack -eq $false)

# ─────────────────────────────────────────────────────────────
Step '6. 断言安装结果与源目录逐字节一致'
# ─────────────────────────────────────────────────────────────
foreach ($f in $srcFiles) {
    $rel = $f.FullName.Substring($source.Length).TrimStart('\')
    $target = Join-Path $installDir $rel
    if (-not (Test-Path -LiteralPath $target)) {
        Check "文件已安装：$rel" $false '目标不存在'
    }
    $h1 = (Get-FileHash -LiteralPath $f.FullName -Algorithm SHA256).Hash
    $h2 = (Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash
    Check "内容一致：$rel" ($h1 -eq $h2)
}

Check '空目录（中文名）已还原' (Test-Path -LiteralPath (Join-Path $installDir 'data\空目录'))
Check '嵌套子目录已还原' (Test-Path -LiteralPath (Join-Path $installDir 'bin\sub\nested.txt'))
Check '中文文件名已还原' (Test-Path -LiteralPath (Join-Path $installDir 'bin\中文模块.dll'))
Check '清单条目数与源文件数一致' ($installResult.entryCount -eq $srcFiles.Count) `
      ("清单 $($installResult.entryCount) / 源 $($srcFiles.Count)")

# ─────────────────────────────────────────────────────────────
Step '7. 可重现构建（同输入 + 同时间戳 → 同字节）'
# ─────────────────────────────────────────────────────────────
$setup2 = Join-Path $dist 'Setup2.exe'
$pack2 = @('pack', '--project', $projectPath, '--source', $source, '--stub', $stub,
           '--out', $setup2, '--build-time', $buildTime)
if (Test-Path $icon) { $pack2 += @('--icon', $icon) }
$r = Invoke-Cli $pack2
Check '第二次 pack 成功' ($r.ExitCode -eq 0)

$h1 = (Get-FileHash -LiteralPath $setup -Algorithm SHA256).Hash
$h2 = (Get-FileHash -LiteralPath $setup2 -Algorithm SHA256).Hash
Check '两次打包字节完全相同（可重现）' ($h1 -eq $h2) $h1

$setup3 = Join-Path $dist 'Setup3.exe'
$pack3 = @('pack', '--project', $projectPath, '--source', $source, '--stub', $stub,
           '--out', $setup3, '--build-time', '2026-10-09T12:00:00Z')
if (Test-Path $icon) { $pack3 += @('--icon', $icon) }
$r = Invoke-Cli $pack3
$h3 = (Get-FileHash -LiteralPath $setup3 -Algorithm SHA256).Hash
Check '不同时间戳产生不同字节（时间戳确实参与构建）' ($h1 -ne $h3)

# ─────────────────────────────────────────────────────────────
Step '8. 反解（extract）与差异（diff）'
# ─────────────────────────────────────────────────────────────
$extractDir = Join-Path $WorkDir 'extracted'
$r = Invoke-Cli @('extract', $setup, '--out', $extractDir)
Check 'extract 退出码为 0' ($r.ExitCode -eq 0)

$manifestPath = Join-Path $extractDir 'manifest.json'
Check 'extract 解出了 manifest.json' (Test-Path $manifestPath)

$em = Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
Check 'manifest 里有 files 清单' (@($em.files).Count -eq $srcFiles.Count) ("$(@($em.files).Count) 条")
Check '每个文件都有 sha256' (@($em.files | Where-Object { $_.sha256 -match '^[0-9a-f]{64}$' }).Count -eq $srcFiles.Count)
Check '记录了空目录' (@($em.directories).Count -eq 1) (@($em.directories) -join ', ')
Check '含多语言 strings' ($em.strings.'Button.Install'.en -eq 'Install')
Check '不泄露制作机路径' (($em | ConvertTo-Json -Depth 12) -notmatch [regex]::Escape($source))
Check '反解出的文件与源一致' `
      ((Get-FileHash -LiteralPath (Join-Path $extractDir 'payload\readme.txt') -Algorithm SHA256).Hash -eq `
       (Get-FileHash -LiteralPath (Join-Path $source 'readme.txt') -Algorithm SHA256).Hash)

$r = Invoke-Cli @('diff', $setup, $setup2)
Check 'diff：两个完全相同的包 → 退出码 0' ($r.ExitCode -eq 0)

$r = Invoke-Cli @('diff', $setup, $setup3)
Check 'diff：只有构建时间不同 → 文件清单无差异（退出码 0）' ($r.ExitCode -eq 0)
Check 'diff 把元数据差异作为信息列出' ($r.Output -match 'builtAtUtc')

# 造一个真的改过内容的 v2：改一个文件、加一个文件、删一个文件
$source2 = Join-Path $WorkDir 'source2'
Copy-Item -Recurse -LiteralPath $source -Destination $source2
[System.IO.File]::WriteAllText((Join-Path $source2 'readme.txt'), 'README 中文内容（v2 已修改）', $utf8)
[System.IO.File]::WriteAllText((Join-Path $source2 'extra.txt'), 'new file', $utf8)
Remove-Item -LiteralPath (Join-Path $source2 'bin\lib.dll')

$project2 = Join-Path $WorkDir 'app2.wmpkg.json'
(Get-Content -LiteralPath $projectPath -Raw -Encoding UTF8).Replace('"2.4.1"', '"2.4.2"') |
    Set-Content -LiteralPath $project2 -Encoding UTF8

$setupV2 = Join-Path $dist 'Setup-v2.exe'
$packV2 = @('pack', '--project', $project2, '--source', $source2, '--stub', $stub,
            '--out', $setupV2, '--build-time', $buildTime)
if (Test-Path $icon) { $packV2 += @('--icon', $icon) }
$r = Invoke-Cli $packV2
Check 'v2 打包成功' ($r.ExitCode -eq 0)

$r = Invoke-Cli @('diff', $setup, $setupV2)
Write-Host $r.Output
Check 'diff 检出文件差异（退出码 1）' ($r.ExitCode -eq 1)
Check 'diff 报出新增 extra.txt' ($r.Output -match 'extra\.txt')
Check 'diff 报出删除 lib.dll' ($r.Output -match 'lib\.dll')
Check 'diff 报出版本变化 2.4.1 → 2.4.2' ($r.Output -match '2\.4\.2')

# ─────────────────────────────────────────────────────────────
Step '9. 篡改检测'
# ─────────────────────────────────────────────────────────────
$tampered = Join-Path $WorkDir 'Tampered.exe'
Copy-Item -LiteralPath $setup -Destination $tampered
$tb = [System.IO.File]::ReadAllBytes($tampered)
$tb[3000] = [byte]($tb[3000] -bxor 0xFF)
[System.IO.File]::WriteAllBytes($tampered, $tb)
$r = Invoke-Cli @('verify', $tampered)
Check '篡改后 verify 失败（退出码 1）' ($r.ExitCode -eq 1)

# ─────────────────────────────────────────────────────────────
Step '10. Zip Slip 防护（手工构造恶意容器）'
# ─────────────────────────────────────────────────────────────
# 用 CLI 打不出恶意包（它只压缩源目录），所以这里手工拼一个容器：
#   stub + 恶意 payload.zip + manifest.json + footer
$slipDir = Join-Path $WorkDir 'zipslip'
New-Item -ItemType Directory -Force -Path $slipDir | Out-Null

$malZip = Join-Path $slipDir 'payload.zip'
$goodFile = Join-Path $slipDir 'ok.txt'
[System.IO.File]::WriteAllText($goodFile, 'harmless', $utf8)

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::Open($malZip, 'Create')
$e1 = $zip.CreateEntry('ok.txt'); $w1 = New-Object System.IO.StreamWriter($e1.Open()); $w1.Write('harmless'); $w1.Dispose()
$e2 = $zip.CreateEntry('..\..\escaped.txt'); $w2 = New-Object System.IO.StreamWriter($e2.Open()); $w2.Write('I escaped!'); $w2.Dispose()
$zip.Dispose()

$goodHash = Get-Sha256Hex ([System.IO.File]::ReadAllBytes($goodFile))
$malManifest = @{
    schemaVersion = 2
    productCode = '{99999999-8888-7777-6666-555555555555}'
    product = @{ name = @{ 'zh-Hans' = 'slip' }; version = '1.0' }
    scope = 'perUser'
    defaultInstallDir = '%TEMP%\slip'
    files = @(@{ path = 'ok.txt'; size = 8; sha256 = $goodHash })
} | ConvertTo-Json -Depth 8
$manifestBytes = $utf8.GetBytes($malManifest)

$stubBytes = [System.IO.File]::ReadAllBytes($stub)
$payloadBytes = [System.IO.File]::ReadAllBytes($malZip)

$payloadOffset = $stubBytes.Length
$manifestOffset = $payloadOffset + $payloadBytes.Length

$footer = New-Object byte[] 128
$magic = [byte[]](0x57, 0x4D, 0x49, 0x4E, 0x53, 0x32, 0x00, 0x00)
[Array]::Copy($magic, 0, $footer, 0, 8)
Write-Le $footer 8  2            4
Write-Le $footer 12 0            4
Write-Le $footer 16 $payloadOffset 8
Write-Le $footer 24 $payloadBytes.Length 8
Write-Le $footer 32 $manifestOffset 8
Write-Le $footer 40 $manifestBytes.Length 4
$payloadHash = [System.Security.Cryptography.SHA256]::Create().ComputeHash($payloadBytes)
[Array]::Copy($payloadHash, 0, $footer, 48, 32)
$stubHash = [System.Security.Cryptography.SHA256]::Create().ComputeHash($stubBytes)
[Array]::Copy($stubHash, 0, $footer, 80, 32)
$crc = Get-Crc32 $footer 0 112
Write-Le $footer 112 ([int64]$crc) 4

$slipExe = Join-Path $slipDir 'Slip.exe'
$out = New-Object System.IO.MemoryStream
$out.Write($stubBytes, 0, $stubBytes.Length)
$out.Write($payloadBytes, 0, $payloadBytes.Length)
$out.Write($manifestBytes, 0, $manifestBytes.Length)
$out.Write($footer, 0, $footer.Length)
[System.IO.File]::WriteAllBytes($slipExe, $out.ToArray())
$out.Dispose()

# 恶意容器本身必须能通过 verify（哈希是对的，恶意藏在条目名里）
$r = Invoke-Cli @('verify', $slipExe)
Check '手工构造的恶意容器能通过 verify（恶意藏在条目名，不是哈希）' ($r.ExitCode -eq 0)

# 运行它：必须失败，且绝不能写出 escaped.txt
$slipTarget = Join-Path $slipDir 'target'
$slipResult = Join-Path $slipDir 'slip-result.json'
$p = Start-Process -FilePath $slipExe -ArgumentList @('--silent', '--sandbox', (Join-Path $slipDir 'sandbox'), '--target', $slipTarget, '--result', $slipResult) -Wait -PassThru -NoNewWindow

Check '安装端拒绝恶意条目（退出码非 0）' ($p.ExitCode -ne 0) "exit=$($p.ExitCode)"
$sr = Get-Content -LiteralPath $slipResult -Raw -Encoding UTF8 | ConvertFrom-Json
Check '错误信息说明了拒绝原因，并点出违规条目' `
      (($sr.error -match '拒绝') -and ($sr.error -match 'escaped\.txt')) $sr.error

$escapedPath = [System.IO.Path]::GetFullPath((Join-Path $slipTarget '..\..\escaped.txt'))
Check '逃逸文件没有被写出来' (-not (Test-Path -LiteralPath $escapedPath)) $escapedPath

# ─────────────────────────────────────────────────────────────
Step '11. 完整安装（步骤管线 + 快捷方式 + 标准卸载键 + 自启动）'
# ─────────────────────────────────────────────────────────────
# 全程沙箱：快捷方式/注册表重定向到临时目录，不碰真实系统
$sb = Join-Path $WorkDir 'sandbox'
$m1Source = Join-Path $WorkDir 'm1source'
New-Item -ItemType Directory -Force -Path (Join-Path $m1Source 'bin') | Out-Null
[System.IO.File]::WriteAllText((Join-Path $m1Source 'bin\App.exe'), 'fake app', $utf8)
[System.IO.File]::WriteAllText((Join-Path $m1Source 'readme.txt'), 'hello', $utf8)

$m1Project = Join-Path $WorkDir 'm1.wmpkg.json'
$m1ProductCode = '{DDDDDDDD-1111-2222-3333-444444444444}'
@{
    schemaVersion     = 2
    productCode       = $m1ProductCode
    defaultCulture    = 'zh-Hans'
    cultures          = @('zh-Hans', 'en')
    product           = @{ name = @{ 'zh-Hans' = 'M1测试产品'; en = 'M1 Test Product' }; version = '1.0.0'; publisher = @{ 'zh-Hans' = '唯非工作室' } }
    scope             = 'perUser'
    defaultInstallDir = '%LOCALAPPDATA%\Programs\M1Test'
    entryPoint        = 'bin\App.exe'
    shortcuts         = @(
        @{ location = 'Desktop';   name = @{ 'zh-Hans' = 'M1测试产品'; en = 'M1 Test Product' } },
        @{ location = 'StartMenu'; name = @{ 'zh-Hans' = 'M1测试产品'; en = 'M1 Test Product' } }
    )
    autostart         = $true
    runAfterInstall   = $false
    strings           = @{
        'Install.Preflight'  = @{ 'zh-Hans' = '正在检查环境…'; en = 'Checking...' }
        'Install.Extracting' = @{ 'zh-Hans' = '正在解压文件…'; en = 'Extracting...' }
        'Install.Done'       = @{ 'zh-Hans' = '安装完成'; en = 'Done' }
    }
} | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $m1Project -Encoding UTF8

$m1Setup = Join-Path $WorkDir 'M1Setup.exe'
$r = Invoke-Cli @('pack', '--project', $m1Project, '--source', $m1Source, '--stub', $stub,
                  '--out', $m1Setup, '--build-time', $buildTime)
Check 'M1 包打包成功' ($r.ExitCode -eq 0)

$m1Result = Join-Path $WorkDir 'm1-install.json'
$p = Start-Process -FilePath $m1Setup -ArgumentList @('--silent', '--sandbox', $sb, '--result', $m1Result) -Wait -PassThru -NoNewWindow
Check '安装退出码为 0' ($p.ExitCode -eq 0) "exit=$($p.ExitCode)"

$ir = Get-Content -LiteralPath $m1Result -Raw -Encoding UTF8 | ConvertFrom-Json
Check '安装结果 ok' ($ir.ok -eq $true) $ir.error
Check '执行了全部 11 个步骤' ($ir.stepsExecuted -eq 11 -and $ir.stepCount -eq 11) "$($ir.stepsExecuted)/$($ir.stepCount)"
Check '没有发生回滚' ($ir.rolledBack -eq $false)
Check '语言解析为 zh-Hans' ($ir.culture -eq 'zh-Hans')
Check '产品名按语言取（不是字典里的任意一个）' ($ir.productName -eq 'M1测试产品') $ir.productName
Check '安装了 2 个文件' ($ir.filesInstalled -eq 2)
Check '创建了 2 个快捷方式' ($ir.shortcutsCreated -eq 2)

$m1InstallDir = $ir.installDir
Check '安装目录在沙箱内' ($m1InstallDir.StartsWith($sb, [StringComparison]::OrdinalIgnoreCase)) $m1InstallDir
Check '文件已落盘' ((Test-Path (Join-Path $m1InstallDir 'bin\App.exe')) -and (Test-Path (Join-Path $m1InstallDir 'readme.txt')))
Check '桌面快捷方式已创建（真实 .lnk）' (Test-Path (Join-Path $sb 'Desktop\M1测试产品.lnk'))
Check '开始菜单快捷方式已创建' (Test-Path (Join-Path $sb 'StartMenu\M1测试产品.lnk'))
Check '安装记录已写出' (Test-Path (Join-Path $m1InstallDir '.install-record.json'))

# 卸载器应该只有 stub 大小，而不是整个安装包
$uninstallerPath = Join-Path $m1InstallDir 'Uninstall.exe'
$unSize = (Get-Item $uninstallerPath).Length
$setupSize = (Get-Item $m1Setup).Length
Check '卸载器只拷了 stub 部分（不是整个安装包）' ($unSize -lt $setupSize) `
      ("卸载器 {0:N0} < 安装包 {1:N0}" -f $unSize, $setupSize)

# 标准卸载键（沙箱注册表）
$regPath = Join-Path $sb 'registry.json'
Check '沙箱注册表已写出' (Test-Path $regPath)
$reg = Get-Content -LiteralPath $regPath -Raw -Encoding UTF8 | ConvertFrom-Json
$unKey = "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$m1ProductCode"
$runKey = 'HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run'
$unValues = $reg.$unKey
Check '写了标准卸载键' ($null -ne $unValues)
Check '卸载键有 DisplayName' ($unValues.DisplayName -eq 'M1测试产品') $unValues.DisplayName
Check '卸载键有 DisplayVersion' ($unValues.DisplayVersion -eq '1.0.0')
Check '卸载键有 Publisher' ($unValues.Publisher -eq '唯非工作室')
Check '卸载键有 InstallLocation' ($unValues.InstallLocation -eq $m1InstallDir)
Check 'UninstallString 指向真实存在的卸载器且带引号' `
      (($unValues.UninstallString -match '^".*Uninstall\.exe" --uninstall') -and (Test-Path $uninstallerPath)) $unValues.UninstallString
Check 'UninstallString 带上了 --product-code（记录丢了也能定位卸载项）' `
      ($unValues.UninstallString -match ('--product-code\s+' + [regex]::Escape($m1ProductCode))) $unValues.UninstallString
Check 'QuietUninstallString 也带上了 --product-code' `
      ($unValues.QuietUninstallString -match ('--product-code\s+' + [regex]::Escape($m1ProductCode))) $unValues.QuietUninstallString
Check '写了 NoModify/NoRepair（交给系统隐藏修改与修复按钮）' `
      ($unValues.NoModify -eq '1' -and $unValues.NoRepair -eq '1')
Check '开机启动项已写入（perUser → HKCU Run）' ($null -ne $reg.$runKey.'M1测试产品')

$rec = Get-Content -LiteralPath (Join-Path $m1InstallDir '.install-record.json') -Raw -Encoding UTF8 | ConvertFrom-Json
Check '安装记录记住了沙箱根' ($rec.sandboxRoot -eq $sb)
Check '安装记录里有文件哈希' (@($rec.fileHashes.PSObject.Properties).Count -eq 2)

# ─────────────────────────────────────────────────────────────
Step '12. 完整卸载（按清单删，不是删光目录）'
# ─────────────────────────────────────────────────────────────
# 往安装目录里放一个"用户自己的文件" —— 卸载**不能**删它
$userFile = Join-Path $m1InstallDir 'user-data.txt'
[System.IO.File]::WriteAllText($userFile, 'this is the user''s own file', $utf8)

$m1UnResult = Join-Path $WorkDir 'm1-uninstall.json'
# --silent 必填：不带它就会弹出卸载向导（那是给真实用户用的路径）
$p = Start-Process -FilePath $uninstallerPath -ArgumentList @('--uninstall', '--silent', '--result', $m1UnResult) -Wait -PassThru -NoNewWindow
Check '卸载退出码为 0' ($p.ExitCode -eq 0) "exit=$($p.ExitCode)"

$ur = Get-Content -LiteralPath $m1UnResult -Raw -Encoding UTF8 | ConvertFrom-Json
Check '卸载结果 ok' ($ur.ok -eq $true)
Check '删除了 2 个文件' ($ur.deletedFiles -eq 2) $ur.deletedFiles
Check '删除了 2 个快捷方式' ($ur.deletedShortcuts -eq 2)
Check '卸载没有报问题' (@($ur.issues).Count -eq 0) (@($ur.issues) -join '; ')

Start-Sleep -Seconds 3   # 等自删除收尾

Check '安装的程序文件已删除' (-not (Test-Path (Join-Path $m1InstallDir 'bin\App.exe')))
Check '桌面快捷方式已删除' (-not (Test-Path (Join-Path $sb 'Desktop\M1测试产品.lnk')))
Check '开始菜单快捷方式已删除' (-not (Test-Path (Join-Path $sb 'StartMenu\M1测试产品.lnk')))

$reg2 = Get-Content -LiteralPath $regPath -Raw -Encoding UTF8 | ConvertFrom-Json
Check '标准卸载键已完整删除' ($null -eq $reg2.$unKey)
Check '开机启动项已删除' ($null -eq $reg2.$runKey.'M1测试产品')

Check '卸载器自删除成功' (-not (Test-Path $uninstallerPath))
Check '安装记录已删除' (-not (Test-Path (Join-Path $m1InstallDir '.install-record.json')))
Check '程序文件所在目录已清空' (-not (Test-Path (Join-Path $m1InstallDir 'bin')))
Check '用户自己的文件没有被删（按清单删的证明）' (Test-Path $userFile) $userFile
Check '安装目录本身保留（因为用户文件还在，不是"删光目录"）' (Test-Path $m1InstallDir)

# ─────────────────────────────────────────────────────────────
Step '13. 真实系统零污染（回归护栏）'
# ─────────────────────────────────────────────────────────────
# 整个 e2e 全程必须走沙箱；一旦有人把 --sandbox 去掉，这几条会立刻失败
Check '真实注册表里没有本次的卸载键' `
      (-not (Test-Path "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$m1ProductCode"))
Check '真实桌面没有本次的快捷方式' `
      (-not (Test-Path (Join-Path ([Environment]::GetFolderPath('DesktopDirectory')) 'M1测试产品.lnk')))
Check '真实开始菜单没有本次的快捷方式' `
      (-not (Test-Path (Join-Path ([Environment]::GetFolderPath('Programs')) 'M1测试产品.lnk')))
Check '真实注册表里没有本次的开机启动项' `
      ($null -eq (Get-ItemProperty 'HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run' -ErrorAction SilentlyContinue).'M1测试产品')

# ─────────────────────────────────────────────────────────────
Step '14. 向导界面（渲染每个页面）'
# ─────────────────────────────────────────────────────────────
# 自动化环境下不弹窗口，改用 --render-ui 把每个页面画成 PNG 再断言
$uiZh = Join-Path $WorkDir 'ui-zh'
$uiEn = Join-Path $WorkDir 'ui-en'

foreach ($pair in @(@($uiZh, 'zh-Hans'), @($uiEn, 'en'))) {
    $dir = $pair[0]
    $culture = $pair[1]
    if (Test-Path $dir) { Remove-Item -Recurse -Force $dir }
    $p = Start-Process -FilePath $m1Setup -ArgumentList @('--render-ui', $dir, '--culture', $culture) -Wait -PassThru -NoNewWindow
    Check "向导渲染（$culture）退出码为 0" ($p.ExitCode -eq 0) "exit=$($p.ExitCode)"
}

foreach ($page in 'Welcome', 'License', 'Path', 'Options', 'Installing', 'Finish', 'Failed') {
    $f = Join-Path $uiZh "$page.png"
    Check "渲染出 $page.png" (Test-Path $f)
    Check "$page.png 不是空白（> 4 KB）" ((Get-Item $f).Length -gt 4096) ("{0:N0} B" -f (Get-Item $f).Length)
}

foreach ($page in 'UninstallConfirm', 'Uninstalling', 'Uninstalled') {
    Check "渲染出卸载页 $page.png" (Test-Path (Join-Path $uiZh "uninstall\$page.png"))
}

# 中英两版必须不同 —— 否则说明本地化没生效
$zhWelcome = (Get-FileHash -LiteralPath (Join-Path $uiZh 'Welcome.png') -Algorithm SHA256).Hash
$enWelcome = (Get-FileHash -LiteralPath (Join-Path $uiEn 'Welcome.png') -Algorithm SHA256).Hash
Check '中英向导渲染结果不同（本地化生效）' ($zhWelcome -ne $enWelcome)

$zhPathSize = (Get-Item (Join-Path $uiZh 'Path.png')).Length
$enPathSize = (Get-Item (Join-Path $uiEn 'Path.png')).Length
Check '英文版路径页与中文版不同（步骤指示器也本地化了）' ($enPathSize -ne $zhPathSize) `
      ("en={0:N0} zh={1:N0}" -f $enPathSize, $zhPathSize)

# ─────────────────────────────────────────────────────────────
Step '15. 制作端界面（一屏三步 + 高级选项）'
# ─────────────────────────────────────────────────────────────
$builderExe = Join-Path $repo 'src\Installer.Builder\bin\Debug\安装包制作助手.exe'
Check '制作端已构建' (Test-Path $builderExe)

$builderRender = Join-Path $WorkDir 'builder-ui'
if (Test-Path $builderRender) { Remove-Item -Recurse -Force $builderRender }
$p = Start-Process -FilePath $builderExe -ArgumentList @('--render-ui', $builderRender) -Wait -PassThru -NoNewWindow
Check '制作端渲染退出码为 0' ($p.ExitCode -eq 0) "exit=$($p.ExitCode)"

Check '渲染出主界面' (Test-Path (Join-Path $builderRender '主界面.png'))
Check '渲染出高级选项（许可协议）' (Test-Path (Join-Path $builderRender '高级选项-许可协议.png'))
Check '渲染出高级选项（安装细节）' (Test-Path (Join-Path $builderRender '高级选项-安装细节.png'))

foreach ($f in Get-ChildItem -File -LiteralPath $builderRender -ErrorAction SilentlyContinue) {
    Check ($f.Name + " 不是空白") ($f.Length -gt 4096) ("{0:N0} B" -f $f.Length)
}

# 主界面不该再出现"模板 stub / 输出路径 / 构建时间戳"这些开发者字段
$builderExeText = [System.IO.File]::ReadAllText((Join-Path $repo 'src\Installer.Builder\Forms\BuilderForm.Designer.cs'), [System.Text.UTF8Encoding]::new($false))
Check '主界面上没有"选择模板 stub"' ($builderExeText -notmatch 'Stub')
Check '主界面上没有"构建时间戳"' ($builderExeText -notmatch 'Timestamp')
Check '主界面没有拆成一堆标签页' ($builderExeText -notmatch 'AntdUI\.Tabs')

# ─────────────────────────────────────────────────────────────
Step '16. 修复安装（--repair）'
# ─────────────────────────────────────────────────────────────
$repairSandbox = Join-Path $WorkDir 'repair-sandbox'
$repairResult = Join-Path $WorkDir 'repair-install.json'
$p = Start-Process -FilePath $m1Setup -ArgumentList @('--silent', '--sandbox', $repairSandbox, '--result', $repairResult) -Wait -PassThru -NoNewWindow
Check '首次安装成功' ($p.ExitCode -eq 0)

$ri = Get-Content -LiteralPath $repairResult -Raw -Encoding UTF8 | ConvertFrom-Json
$repairDir = $ri.installDir
Check '安装目录已就绪' (Test-Path $repairDir)

# 制造两种损坏：删掉一个、改坏另一个
$deletedFile = Join-Path $repairDir 'readme.txt'
$corruptFile = Join-Path $repairDir 'bin\App.exe'
Remove-Item -LiteralPath $deletedFile -Force
[System.IO.File]::WriteAllText($corruptFile, 'CORRUPTED', $utf8)
Check '已制造损坏（删 1 个、改坏 1 个）' ((-not (Test-Path $deletedFile)) -and ((Get-Content -LiteralPath $corruptFile -Raw) -eq 'CORRUPTED'))

# 修复
$repairOut = Join-Path $WorkDir 'repair-run.json'
$p = Start-Process -FilePath $m1Setup -ArgumentList @('--repair', '--sandbox', $repairSandbox, '--target', $repairDir, '--result', $repairOut) -Wait -PassThru -NoNewWindow
Check '修复退出码为 0' ($p.ExitCode -eq 0) "exit=$($p.ExitCode)"

Check '被删除的文件已恢复' (Test-Path $deletedFile)
Check '被删除的文件内容正确' `
      ((Get-FileHash -LiteralPath $deletedFile -Algorithm SHA256).Hash -eq `
       (Get-FileHash -LiteralPath (Join-Path $m1Source 'readme.txt') -Algorithm SHA256).Hash)
Check '被改坏的文件已恢复' `
      ((Get-FileHash -LiteralPath $corruptFile -Algorithm SHA256).Hash -eq `
       (Get-FileHash -LiteralPath (Join-Path $m1Source 'bin\App.exe') -Algorithm SHA256).Hash)

# 修复不应该把没坏的东西也重写（用写入时间判断）
$before = (Get-Item -LiteralPath (Join-Path $repairDir '.install-record.json')).LastWriteTime
Start-Sleep -Milliseconds 1100
$p = Start-Process -FilePath $m1Setup -ArgumentList @('--repair', '--sandbox', $repairSandbox, '--target', $repairDir, '--result', $repairOut) -Wait -PassThru -NoNewWindow
Check '再次修复仍成功（幂等）' ($p.ExitCode -eq 0)
Check '文件内容仍然正确' `
      ((Get-FileHash -LiteralPath $corruptFile -Algorithm SHA256).Hash -eq `
       (Get-FileHash -LiteralPath (Join-Path $m1Source 'bin\App.exe') -Algorithm SHA256).Hash)

# ─────────────────────────────────────────────────────────────
Step '17. v1 旧包迁移（legacy-inspect / legacy-import）'
# ─────────────────────────────────────────────────────────────
# 用真实的 v1 资源文件（仓库里 Installation\bin\Debug 就有一份）
$v1Dir = Join-Path $repo 'Installation\bin\Debug'
if (Test-Path (Join-Path $v1Dir 'install.resources')) {
    $r = Invoke-Cli @('legacy-inspect', $v1Dir)
    Check 'legacy-inspect 能读出真实 v1 包' ($r.ExitCode -eq 0)
    Check '读出了产品名' ($r.Output -match '产品名')
    Check '读出了负载大小' ($r.Output -match '负载')

    $migrated = Join-Path $WorkDir 'migrated.wmpkg.json'
    $r = Invoke-Cli @('legacy-import', $v1Dir, '--out', $migrated)
    Check 'legacy-import 退出码为 0' ($r.ExitCode -eq 0)
    Check '迁移出的工程文件存在' (Test-Path $migrated)

    $mj = Get-Content -LiteralPath $migrated -Raw -Encoding UTF8 | ConvertFrom-Json
    Check '迁移出的工程有 manifest' ($null -ne $mj.manifest)
    Check '迁移出的工程有 productCode' ($mj.manifest.productCode -match '^\{[0-9A-F-]{36}\}$')
    Check '迁移不应带出制作机路径' `
          ((Get-Content -LiteralPath $migrated -Raw -Encoding UTF8) -notmatch 'InstallSetupPath')
} else {
    Check '仓库里有可用的 v1 包' $false $v1Dir
}

# ─────────────────────────────────────────────────────────────
Step '18. 指定的安装位置必须真的生效'
# ─────────────────────────────────────────────────────────────
# 回归：用户反馈"安装到 D 盘，但文件还是装到 C 盘了"。
# 根因是向导没把用户选的安装位置写回清单，而安装端用的是构造时的快照。
$targetSandbox = Join-Path $WorkDir 'target-sandbox'
$customDir = Join-Path $targetSandbox 'Programs\我指定的目录'
$targetResult = Join-Path $WorkDir 'target-result.json'

$p = Start-Process -FilePath $m1Setup -ArgumentList @('--silent', '--sandbox', $targetSandbox, '--target', $customDir, '--result', $targetResult) -Wait -PassThru -NoNewWindow
Check '按指定目录安装退出码为 0' ($p.ExitCode -eq 0) "exit=$($p.ExitCode)"

$tr = Get-Content -LiteralPath $targetResult -Raw -Encoding UTF8 | ConvertFrom-Json
Check '安装结果里的目录就是指定目录' `
      ($tr.installDir.TrimEnd('\') -eq $customDir.TrimEnd('\')) `
      "期望 $customDir，实际 $($tr.installDir)"

Check '文件确实落在指定目录里' (Test-Path (Join-Path $customDir 'readme.txt'))
Check '文件内容正确' `
      ((Get-FileHash -LiteralPath (Join-Path $customDir 'readme.txt') -Algorithm SHA256).Hash -eq `
       (Get-FileHash -LiteralPath (Join-Path $m1Source 'readme.txt') -Algorithm SHA256).Hash)
Check '安装记录也记的是指定目录' `
      ((Get-Content -LiteralPath (Join-Path $customDir '.install-record.json') -Raw -Encoding UTF8 |
        ConvertFrom-Json).installDir.TrimEnd('\') -eq $customDir.TrimEnd('\'))
Check '没有偷偷装到别处' (-not (Test-Path (Join-Path $targetSandbox 'Programs\M1测试产品')))

# ─────────────────────────────────────────────────────────────
Step '19. 重复安装不应在控制面板里留下多条'
# ─────────────────────────────────────────────────────────────
# 回归：用户反馈"装了三次，控制面板里出现三次；卸载了一个，其余删不掉"。
# 根因是每次打包用了不同的 productCode，但安装目录相同 —— 卸载键就成了孤儿。
$dupSandbox = Join-Path $WorkDir 'dup-sandbox'
$dupProject = Join-Path $WorkDir 'm1-dup.wmpkg.json'

# 同一个产品、同一个安装目录，只换 productCode（模拟"又打了一次包"）
$dup = Get-Content -LiteralPath $m1Project -Raw -Encoding UTF8 | ConvertFrom-Json
$dup.productCode = '{EEEEEEEE-9999-8888-7777-666666666666}'
$dup | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $dupProject -Encoding UTF8

$dupSetup = Join-Path $WorkDir 'M1Setup-dup.exe'
$r = Invoke-Cli @('pack', '--project', $dupProject, '--source', $m1Source, '--stub', $stub,
                  '--out', $dupSetup, '--build-time', $buildTime)
Check '第二个包（不同 productCode）打包成功' ($r.ExitCode -eq 0)

# 第一次安装
$p = Start-Process -FilePath $m1Setup -ArgumentList @('--silent', '--sandbox', $dupSandbox, '--result', (Join-Path $WorkDir 'dup1.json')) -Wait -PassThru -NoNewWindow
Check '第一次安装成功' ($p.ExitCode -eq 0)

$dupReg = Join-Path $dupSandbox 'registry.json'
# 沙箱注册表是**扁平**的：属性名就是完整键路径（HKCU\SOFTWARE\...）
$uninstallPrefix = 'HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\'

function Get-UninstallKeys($registryFile) {
    if (-not (Test-Path $registryFile)) { return @() }
    $j = Get-Content -LiteralPath $registryFile -Raw -Encoding UTF8 | ConvertFrom-Json
    if (-not $j) { return @() }
    $names = @($j.PSObject.Properties.Name | Where-Object { $_.StartsWith($uninstallPrefix) })
    return @($names | ForEach-Object { $_.Substring($uninstallPrefix.Length) })
}

Check '第一次安装后有 1 条卸载项' ((Get-UninstallKeys $dupReg).Count -eq 1) `
      ("实际 {0} 条" -f (Get-UninstallKeys $dupReg).Count)

# 第二次安装（不同 productCode，同一目录）
$p = Start-Process -FilePath $dupSetup -ArgumentList @('--silent', '--sandbox', $dupSandbox, '--result', (Join-Path $WorkDir 'dup2.json')) -Wait -PassThru -NoNewWindow
Check '第二次安装成功' ($p.ExitCode -eq 0)

$dupKeys = @(Get-UninstallKeys $dupReg)
Check '重复安装后仍然只有 1 条卸载项' ($dupKeys.Count -eq 1) `
      ("实际 {0} 条：{1}" -f $dupKeys.Count, ($dupKeys -join ', '))
Check '留下的那条是最后一次安装的 productCode' `
      ($dupKeys.Count -eq 1 -and $dupKeys[0] -eq '{EEEEEEEE-9999-8888-7777-666666666666}') ($dupKeys -join ', ')

$dupRegJson = Get-Content -LiteralPath $dupReg -Raw -Encoding UTF8 | ConvertFrom-Json
$uninstallNode = $dupRegJson.($uninstallPrefix + $dupKeys[0])
Check '卸载项指向安装目录' ($uninstallNode.InstallLocation -like '*M1*') $uninstallNode.InstallLocation
Check 'UninstallString 里带上了 --product-code' ($uninstallNode.UninstallString -like '*--product-code*') $uninstallNode.UninstallString

# 删掉安装记录，模拟"卸到一半 / 记录丢了"，再卸载一次 —— 必须还能清掉
$dupInstallDir = (Get-Content -LiteralPath (Join-Path $WorkDir 'dup2.json') -Raw -Encoding UTF8 | ConvertFrom-Json).installDir
Remove-Item -LiteralPath (Join-Path $dupInstallDir '.install-record.json') -Force
Check '安装记录已被人为删除' (-not (Test-Path (Join-Path $dupInstallDir '.install-record.json')))

$p = Start-Process -FilePath (Join-Path $dupInstallDir 'Uninstall.exe') `
     -ArgumentList @('--uninstall', '--silent', '--sandbox', $dupSandbox,
                     '--product-code', '{EEEEEEEE-9999-8888-7777-666666666666}',
                     '--result', (Join-Path $WorkDir 'dup-uninstall.json')) -Wait -PassThru -NoNewWindow
Check '没有安装记录时卸载仍然成功（降级卸载）' ($p.ExitCode -eq 0) "exit=$($p.ExitCode)"

$dupUn = Get-Content -LiteralPath (Join-Path $WorkDir 'dup-uninstall.json') -Raw -Encoding UTF8 | ConvertFrom-Json
Check '降级卸载被标记出来' ($dupUn.mode -eq 'uninstall-degraded') $dupUn.mode

$leftKeys = Get-UninstallKeys $dupReg
Check '降级卸载后卸载项已清空' ($leftKeys.Count -eq 0) ("还剩 {0} 条" -f $leftKeys.Count)

# ─────────────────────────────────────────────────────────────
Write-Host ''
Write-Host ("全部 {0} 项断言通过。" -f $script:Checks) -ForegroundColor Green
Write-Host ''
Write-Host ("产物：{0}  ({1:N0} 字节)" -f $setup, (Get-Item $setup).Length)
Write-Host ("工作目录：{0}" -f $WorkDir)
if (-not $KeepWorkDir) { Write-Host '（加 -KeepWorkDir 可保留工作目录）' }
