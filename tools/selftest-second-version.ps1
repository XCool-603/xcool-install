# 复现「装第二版时按不上去」：静默装 v1 → 用 v2 的向导逐页走，每页截图。
#
# 为什么要按坐标点击：UIA 看不到 AntdUI 按钮（不是标准 Button 控件类型），
# 只能按屏幕坐标点。坐标取自 1920x1080、窗口居中时的实际渲染结果。
#
# 用法：powershell -NoProfile -ExecutionPolicy Bypass -File tools\selftest-second-version.ps1
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName Microsoft.VisualBasic

$repo = Split-Path -Parent $PSScriptRoot

# 点击工具：仓库里带了源码（tools\uiauto），首次运行时编译一次。
# 之所以要独立编译，是因为本机 Add-Type 和 csc.exe 都用不了（见 tools\uiauto\click.csproj 的注释）。
$clickProj = Join-Path $PSScriptRoot 'uiauto\click.csproj'
$clickOut = Join-Path $env:TEMP 'uiauto-click'
$clickExe = Join-Path $clickOut 'click.exe'

if (-not (Test-Path $clickExe)) {
    Write-Host '首次运行，正在编译点击工具…'
    & dotnet build $clickProj -c Release -o $clickOut -v:q --nologo | Out-Null
    if (-not (Test-Path $clickExe)) { throw "点击工具编译失败：$clickProj" }
}

$work = Join-Path $env:TEMP ('v2test-' + [guid]::NewGuid().ToString('N'))
$src = Join-Path $work 'src'
$dist = Join-Path $work 'dist'
$shots = Join-Path $work 'shots'
New-Item -ItemType Directory -Path $src, $dist, $shots -Force | Out-Null

$utf8 = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText((Join-Path $src 'App.exe'), 'MZ-v1', $utf8)
[System.IO.File]::WriteAllText((Join-Path $src 'App.dll'), 'dll-v1', $utf8)

$cli = Join-Path $repo 'src\Installer.Cli\bin\Release\installer-cli.exe'
$stub = Join-Path $repo 'src\Installer.Runtime\bin\Release\Installer.Runtime.exe'

function New-Package([string]$version, [string]$outName) {
    $project = [ordered]@{
        schemaVersion     = 2
        productCode       = '{V2TEST00-1111-2222-3333-444444444444}'
        defaultCulture    = 'zh-Hans'
        product           = [ordered]@{ name = [ordered]@{ 'zh-Hans' = '二版测试' }; version = $version }
        scope             = 'perUser'
        defaultInstallDir = '%LOCALAPPDATA%\Programs\二版测试'
        entryPoint        = 'App.exe'
        shortcuts         = @([ordered]@{ location = 'Desktop'; name = [ordered]@{ 'zh-Hans' = '二版测试' } })
    }
    $projPath = Join-Path $work ('v' + $version + '.wmpkg.json')
    $project | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $projPath -Encoding UTF8
    $out = Join-Path $dist $outName
    & $cli pack --project $projPath --source $src --stub $stub --out $out | Out-Null
    return $out
}

$v1 = New-Package '1.0.0' 'V1.exe'
$v2 = New-Package '2.0.0' 'V2.exe'

$target = Join-Path $work 'installed'
$res = Join-Path $work 'v1.json'
$p = Start-Process -FilePath $v1 -ArgumentList @('--silent', '--target', $target, '--result', $res) -Wait -PassThru -NoNewWindow
$v1Ok = (Get-Content $res -Raw -Encoding UTF8 | ConvertFrom-Json).ok
Write-Host ("v1 安装 exit={0} ok={1}" -f $p.ExitCode, $v1Ok)

$proc = Start-Process -FilePath $v2 -ArgumentList @('--target', $target) -PassThru
Start-Sleep -Seconds 4
$proc.Refresh()
if ($proc.HasExited) { throw "v2 向导已退出 exit=$($proc.ExitCode)" }
Write-Host ("v2 向导: {0}" -f $proc.MainWindowTitle)

function Save-Shot([string]$name) {
    try {
        $bounds = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
        $bmp = New-Object System.Drawing.Bitmap $bounds.Width, $bounds.Height
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.CopyFromScreen($bounds.Location, [System.Drawing.Point]::Empty, $bounds.Size)
        $g.Dispose()
        $path = Join-Path $shots ($name + '.png')
        $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
        Write-Host ("  截图 -> {0}" -f $path)
        return $path
    }
    catch {
        Write-Host ("  截图失败：{0}" -f $_.Exception.Message)
        return $null
    }
}

# 主按钮位置**随页面高度变化**（向导每页 ApplyPageSize 会改窗口大小）：
#   欢迎页   → (1210, 833)
#   安装设置 → (1210, 990)   ← 卡片更高，底栏整体下移
$primaryByStep = @{ 1 = @(1210, 833); 2 = @(1210, 990); 3 = @(1210, 990) }

# 第三方工具（本机是 LdTerm 的「系统提示」）会弹一个居中的提示框，和安装器无关，
# 但会让自动化点击落空 —— 先点它的「关闭(C)」。
$promptClose = @(1155, 580)

[Microsoft.VisualBasic.Interaction]::AppActivate($proc.Id) | Out-Null
Start-Sleep -Milliseconds 600

for ($step = 1; $step -le 3; $step++) {
    Save-Shot ("第{0}步-点击前" -f $step) | Out-Null

    & $clickExe $promptClose[0] $promptClose[1] | Out-Null
    Start-Sleep -Milliseconds 400

    [Microsoft.VisualBasic.Interaction]::AppActivate($proc.Id) | Out-Null
    Start-Sleep -Milliseconds 300

    $pt = $primaryByStep[$step]
    & $clickExe $pt[0] $pt[1] | Out-Null

    # 第 2 次点击会真正开始安装，多等一会儿
    Start-Sleep -Milliseconds $(if ($step -ge 2) { 6000 } else { 1500 })

    $proc.Refresh()
    if ($proc.HasExited) {
        Write-Host ("  第 {0} 次点击后向导已退出 exit={1}" -f $step, $proc.ExitCode)
        break
    }

    Save-Shot ("第{0}步-点击后" -f $step) | Out-Null
}

if (-not $proc.HasExited) { $proc.Kill(); $proc.WaitForExit(3000) | Out-Null }

Write-Host ("截图目录：{0}" -f $shots)

# 清理安装
$u = Join-Path $target 'Uninstall.exe'
if (Test-Path $u) {
    Start-Process -FilePath $u -ArgumentList @('--uninstall', '--silent') -Wait -NoNewWindow | Out-Null
}

Write-Host '（保留截图目录，便于查看）'
