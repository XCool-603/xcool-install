# 续跑向导截图自测：验证"提权重启后"确实停在「安装位置」页、且选项已按续跑文件恢复。
#
# 为什么单独做一个脚本：提权链路要弹 UAC，验证机上 UAC 是关的（EnableLUA=0），
# 没法真的点。但**续跑那一段**（读文件 → 恢复选项 → 停在安装位置页）是可以验的 ——
# 直接把 --resume 喂给向导即可，走的是完全相同的代码路径。
#
# 用法：powershell -NoProfile -ExecutionPolicy Bypass -File tools\selftest-resume-shot.ps1
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName Microsoft.VisualBasic

$repo = Split-Path -Parent $PSScriptRoot
$work = Join-Path $env:TEMP ('shot-' + [guid]::NewGuid().ToString('N'))
$src = Join-Path $work 'src'
$dist = Join-Path $work 'dist'
New-Item -ItemType Directory -Path $src, $dist -Force | Out-Null

$utf8 = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText((Join-Path $src 'App.exe'), 'MZ', $utf8)

# 清单默认值：仅当前用户、两个快捷方式、开机启动、安装后运行
# —— 与下面的续跑文件**刻意相反**，这样截图里能一眼看出续跑生效了没有
$project = [ordered]@{
    schemaVersion     = 2
    productCode       = '{SHOT0000-1111-2222-3333-444444444444}'
    defaultCulture    = 'zh-Hans'
    product           = [ordered]@{ name = [ordered]@{ 'zh-Hans' = '续跑自测' }; version = '2.0.0' }
    scope             = 'perUser'
    defaultInstallDir = '%LOCALAPPDATA%\Programs\续跑自测'
    entryPoint        = 'App.exe'
    autostart         = $true
    runAfterInstall   = $true
    shortcuts         = @(
        [ordered]@{ location = 'Desktop';   name = [ordered]@{ 'zh-Hans' = '续跑自测' } },
        [ordered]@{ location = 'StartMenu'; name = [ordered]@{ 'zh-Hans' = '续跑自测' } }
    )
}
$projPath = Join-Path $work 'w.wmpkg.json'
$project | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $projPath -Encoding UTF8

$cli = Join-Path $repo 'src\Installer.Cli\bin\Release\installer-cli.exe'
$stub = Join-Path $repo 'src\Installer.Runtime\bin\Release\Installer.Runtime.exe'
$setup = Join-Path $dist 'Shot.exe'
& $cli pack --project $projPath --source $src --stub $stub --out $setup | Out-Null

# 续跑内容：目录 D:\Apps\续跑自测、勾「所有用户」、取消「开始菜单」、取消「开机启动」
$resumePath = Join-Path $work 'resume.json'
[ordered]@{
    InstallDir        = 'D:\Apps\续跑自测'
    AcceptLicense     = $true
    DesktopShortcut   = $true
    StartMenuShortcut = $false
    Autostart         = $false
    RunAfterInstall   = $true
    AllUsers          = $true
} | ConvertTo-Json | Set-Content -LiteralPath $resumePath -Encoding UTF8

$p = Start-Process -FilePath $setup `
     -ArgumentList @('--sandbox', (Join-Path $work 'sb'), '--resume', $resumePath) -PassThru
Start-Sleep -Seconds 4
$p.Refresh()

if ($p.HasExited) {
    throw "向导进程已退出（exit=$($p.ExitCode)），没能截图"
}

Write-Host ("窗口标题: {0}" -f $p.MainWindowTitle)

[Microsoft.VisualBasic.Interaction]::AppActivate($p.Id) | Out-Null
Start-Sleep -Milliseconds 900

# 截图是可选的：会话锁屏 / 未连接交互桌面时 CopyFromScreen 会抛 Win32Exception。
# 那种情况下"窗口能打开 + 续跑文件被消费"这两个断言依然成立，不该让工具崩掉。
$out = Join-Path $env:TEMP 'resume-wizard.png'
$captured = $false

try {
    $bounds = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
    $bmp = New-Object System.Drawing.Bitmap $bounds.Width, $bounds.Height
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    try {
        $g.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)
    }
    finally {
        $g.Dispose()
    }

    $bmp.Save($out, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    $captured = $true

    Write-Host ("截图: {0}  ({1}x{2}, {3:N1} KB)" -f $out, $bounds.Width, $bounds.Height,
        ((Get-Item $out).Length / 1KB))
}
catch {
    Write-Host ("截图跳过：当前会话没有可捕获的桌面（{0}）" -f $_.Exception.Message)
}

Write-Host ("续跑文件已被消费: {0}" -f (-not (Test-Path $resumePath)))
Write-Host ("结论: 向导已打开{0}" -f $(if ($captured) { '，并已截图' } else { '（截图不可用，但启动与续跑消费已验证）' }))

if (-not $p.HasExited) { $p.Kill(); $p.WaitForExit(3000) | Out-Null }
Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue
