制作端界面 —— 与 Calicat 设计稿的自动对比
#
# 用法：powershell -ExecutionPolicy Bypass -File tools\verify-ui.ps1
#
# 做两件事：
#   ① 逐点比对渲染图与设计稿的颜色（设计稿存在 docs\界面截图\制作端\设计稿-对照.png）
#   ② 检查 bounds.txt 里的结构不变量（复选框对齐、控件不被父容器裁切）
#
# 退出码 0 = 全部通过。

param(
    [string]$Design = "",
    [switch]$KeepWorkDir
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$root = Split-Path -Parent $PSScriptRoot
if (-not $Design) {
    $Design = Join-Path $root 'docs\界面截图\制作端\设计稿-对照.png'
}

$work = Join-Path $env:TEMP 'installer-ui-verify'
if (Test-Path $work) { Remove-Item $work -Recurse -Force }
New-Item -ItemType Directory -Path $work -Force | Out-Null

$pass = 0
$fail = 0

function Check($ok, $name, $detail) {
    if ($ok) {
        $script:pass++
        Write-Host ("  [OK]   " + $name) -ForegroundColor Green
    } else {
        $script:fail++
        Write-Host ("  [FAIL] " + $name + "  " + $detail) -ForegroundColor Red
    }
}

function Hex($c) { '#{0:X2}{1:X2}{2:X2}' -f $c.R, $c.G, $c.B }

function SameColor($bmp, $ref, $x, $y) {
    return (Hex $bmp.GetPixel($x, $y)) -eq (Hex $ref.GetPixel($x, $y))
}

# ─────────────────────────────────────────────────────────────
Write-Host "== 0. 渲染两个状态" -ForegroundColor Cyan

$exe = Join-Path $root 'src\Installer.Builder\bin\Release\安装包制作助手.exe'
if (-not (Test-Path $exe)) { throw "找不到 $exe，请先构建 Release" }

$demo = Join-Path $env:TEMP 'builder-demo\空中小火车'

$emptyDir = Join-Path $work 'empty'
$selDir = Join-Path $work 'selected'
New-Item -ItemType Directory -Path $emptyDir, $selDir -Force | Out-Null

Start-Process -FilePath $exe -ArgumentList @('--render-ui', $emptyDir) -Wait -NoNewWindow
Start-Process -FilePath $exe -ArgumentList @('--render-ui', $selDir, '--with-folder', $demo) -Wait -NoNewWindow

$emptyPng = Join-Path $emptyDir '主界面.png'
$selPng = Join-Path $selDir '主界面.png'
Check (Test-Path $emptyPng) '未选状态渲染成功' $emptyPng
Check (Test-Path $selPng) '已选状态渲染成功' $selPng

$empty = New-Object System.Drawing.Bitmap($emptyPng)
$sel = New-Object System.Drawing.Bitmap($selPng)

# ─────────────────────────────────────────────────────────────
Write-Host "== 1. 窗口尺寸与设计稿一致" -ForegroundColor Cyan

$ref = $null
if (Test-Path $Design) {
    $ref = New-Object System.Drawing.Bitmap($Design)
    Check ($empty.Width -eq $ref.Width -and $empty.Height -eq $ref.Height) `
        '窗口尺寸一致' "我的 $($empty.Width)x$($empty.Height) / 设计稿 $($ref.Width)x$($ref.Height)"
} else {
    Write-Host "  [跳过] 没有设计稿：$Design" -ForegroundColor Yellow
}

# ─────────────────────────────────────────────────────────────
Write-Host "== 2. 结构不变量（来自 bounds.txt）" -ForegroundColor Cyan

function Read-Bounds($dir) {
    $p = Join-Path $dir 'bounds.txt'
    if (-not (Test-Path $p)) { throw "缺少 bounds.txt：$p" }
    return Get-Content -LiteralPath $p -Encoding UTF8
}

function Get-Bound($lines, $name) {
    foreach ($l in $lines) {
        if ($l -match ("\[" + [regex]::Escape($name) + "\] Bounds=\{X=(-?\d+),Y=(-?\d+),Width=(\d+),Height=(\d+)\}")) {
            return @{
                X = [int]$Matches[1]; Y = [int]$Matches[2]
                W = [int]$Matches[3]; H = [int]$Matches[4]
            }
        }
    }
    return $null
}

$selLines = Read-Bounds $selDir
$emptyLines = Read-Bounds $emptyDir

# 2.1 四个快捷方式复选框必须同一条水平线
$names = @('chkDesktop', 'chkStartMenu', 'chkAutostart', 'chkRunAfter')
$ys = @()
foreach ($n in $names) {
    $b = Get-Bound $selLines $n
    if ($b) { $ys += $b.Y }
}
if ($ys.Count -eq 4) {
    $minY = ($ys | Measure-Object -Minimum).Minimum
    $maxY = ($ys | Measure-Object -Maximum).Maximum
    Check ($minY -eq $maxY) '四个快捷方式复选框在同一水平线' "Y = $($ys -join ', ')"
} else {
    Check $false '四个快捷方式复选框在同一水平线' "只找到 $($ys.Count) 个"
}

# 2.2 控件不能被父容器裁切（控件底边 ≤ 父容器高）
$parents = @{
    'folderStep'   = @('dropPanel', 'entryLayout', 'lblStep')
    'infoStep'     = @('txtNameZh', 'txtVersion', 'txtPublisher')
    'shortcutStep' = @('flow')
}
foreach ($parent in $parents.Keys) {
    $pb = Get-Bound $selLines $parent
    if (-not $pb) { continue }
    foreach ($child in $parents[$parent]) {
        $cb = Get-Bound $selLines $child
        if (-not $cb) { continue }
        # 子控件的 Y 是相对父容器的
        $bottom = $cb.Y + $cb.H
        Check ($bottom -le $pb.H) `
            "$child 未被 $parent 裁切" "底边 $bottom > 父高 $($pb.H)"
    }
}

# 2.3 关键控件必须有合理高度（防止被压扁）
$minHeights = @{ 'cboEntry' = 28; 'txtVersion' = 36; 'txtPublisher' = 36; 'txtNameZh' = 36; 'txtNameEn' = 36 }
foreach ($n in $minHeights.Keys) {
    $b = Get-Bound $selLines $n
    if ($b) {
        Check ($b.H -ge $minHeights[$n]) "$n 高度正常" "$($b.H) < $($minHeights[$n])"
    }
}

# ─────────────────────────────────────────────────────────────
Write-Host "== 3. 与设计稿逐点比对颜色" -ForegroundColor Cyan

if ($ref) {
    $points = @(
        @{ n = '顶部栏底色';        x = 460; y = 30 },
        @{ n = '页面背景';          x = 460; y = 78 },
        @{ n = '卡片1 底色';        x = 460; y = 110 },
        @{ n = '拖拽区底色';        x = 200; y = 250 },
        @{ n = '底部栏底色';        x = 460; y = 820 },
        @{ n = '状态栏底色';        x = 460; y = 905 }
    )
    foreach ($p in $points) {
        $mineHex = Hex $empty.GetPixel($p.x, $p.y)
        $refHex = Hex $ref.GetPixel($p.x, $p.y)
        Check ($mineHex -eq $refHex) $p.n "我的 $mineHex / 设计稿 $refHex"
    }

    # 淡化卡片：未选状态必须是 #F7F8FA；已选状态必须回到白色。
    # 注意：卡片2 的 Y 位置随卡片1 收缩而变，采样点必须按 bounds 算，不能写死。
    function CardProbe($lines, $bmp) {
        $b = Get-Bound $lines 'card2'
        if (-not $b) { return $null }
        # 卡片在 contentLayout 里，contentLayout 从顶部栏(64)下方开始且上内边距 20
        $y = 64 + 20 + $b.Y + [int]($b.H / 2)
        return Hex $bmp.GetPixel(460, $y)
    }

    $card2empty = CardProbe $emptyLines $empty
    Check ($card2empty -eq '#F7F8FA') '未选时卡片2 淡化底色' "实际 $card2empty"

    $card2sel = CardProbe $selLines $sel
    Check ($card2sel -eq '#FFFFFF') '已选时卡片2 恢复白色' "实际 $card2sel"
}

# ─────────────────────────────────────────────────────────────
Write-Host "== 4. 高级设置对话框" -ForegroundColor Cyan

$advLicense = Join-Path $emptyDir '高级选项-许可协议.png'
$advInstall = Join-Path $emptyDir '高级选项-安装细节.png'
Check (Test-Path $advLicense) '高级设置（许可协议）渲染成功' $advLicense
Check (Test-Path $advInstall) '高级设置（安装细节）渲染成功' $advInstall

if ((Test-Path $advLicense) -and (Test-Path $advInstall)) {
    $al = New-Object System.Drawing.Bitmap($advLicense)
    $ai = New-Object System.Drawing.Bitmap($advInstall)

    # 标题栏必须是白底（和主界面一致，不能是系统原生灰）
    Check ((Hex $ai.GetPixel(350, 25)) -eq '#FFFFFF') '高级设置标题栏白底' "实际 $(Hex $ai.GetPixel(350, 25))"

    # 内容区必须是白卡片（不能是整片页面灰）
    Check ((Hex $ai.GetPixel(350, 200)) -eq '#FFFFFF') '高级设置内容区白卡片' "实际 $(Hex $ai.GetPixel(350, 200))"

    # 页脚必须白底
    Check ((Hex $ai.GetPixel(350, 540)) -eq '#FFFFFF') '高级设置页脚白底' "实际 $(Hex $ai.GetPixel(350, 540))"

    # 页面背景（卡片外）应是 #F5F7FA
    Check ((Hex $ai.GetPixel(8, 200)) -eq '#F5F7FA') '高级设置页面背景' "实际 $(Hex $ai.GetPixel(8, 200))"

    # 不能出现系统原生标题栏（经典标题栏是浅蓝渐变 #A0C8F0 一类）
    $top = Hex $ai.GetPixel(350, 8)
    Check ($top -eq '#FFFFFF') '高级设置没有系统原生标题栏' "顶部像素 $top"

    $al.Dispose()
    $ai.Dispose()
}

# ─────────────────────────────────────────────────────────────
Write-Host "== 5. 事件处理必须有订阅" -ForegroundColor Cyan
# 重写 Designer 时很容易漏掉 "this.btn.Click += ..."，
# 编译不报错、按钮却点了没反应。这里静态扫一遍。
$proj = @((Join-Path $root 'src\Installer.Builder'), (Join-Path $root 'src\Installer.UI'))
$srcFiles = @()
foreach ($projItem in $proj) {
    $srcFiles += Get-ChildItem -Recurse -File -Filter '*.cs' -LiteralPath $projItem |
        Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' -and $_.Name -notlike '*.Designer.cs' }
}

$unwired = @()
foreach ($sf in $srcFiles) {
    $code = [System.IO.File]::ReadAllText($sf.FullName, [System.Text.UTF8Encoding]::new($false))
    $handlers = [regex]::Matches($code, '(?:private|protected|public)\s+void\s+(\w+)\s*\(\s*object\s+sender') |
        ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
    if (-not $handlers) { continue }

    $dpath = $sf.FullName -replace '\.cs$', '.Designer.cs'
    $dtxt = if (Test-Path $dpath) { [System.IO.File]::ReadAllText($dpath, [System.Text.UTF8Encoding]::new($false)) } else { '' }

    foreach ($h in $handlers) {
        $wired = ($dtxt -match [regex]::Escape($h)) -or ($code -match ('\+=\s*' + [regex]::Escape($h)))
        if (-not $wired) { $unwired += "$($sf.Name):$h" }
    }
}
Check ($unwired.Count -eq 0) '所有事件处理方法都已订阅' ($unwired -join ', ')

# ─────────────────────────────────────────────────────────────
Write-Host "== 6. 向导每页与设计稿逐点比对" -ForegroundColor Cyan
# 比对区域取 y ∈ [0, 500]：覆盖品牌头(0~124)、步骤条(124~164)、
# 页面标题与第一块内容。设计稿每页高度不同，取公共区段才可比。
$designDir = Join-Path $root 'docs\界面截图\安装向导-设计稿'
$implDir = Join-Path $root 'docs\界面截图\安装向导-当前实现'

$wizardMap = @(
    @{ design = '1-欢迎3.png';        impl = 'Welcome.png';          name = '欢迎' },
    @{ design = '2-许可协议.png';      impl = 'License.png';          name = '许可协议' },
    @{ design = '3-安装设置2.png';     impl = 'Path.png';             name = '安装设置' },
    @{ design = '4-正在安装3.png';     impl = 'Installing.png';       name = '正在安装' },
    @{ design = '5-安装完成3.png';     impl = 'Finish.png';           name = '安装完成' },
    @{ design = '6-确认卸载.png';      impl = 'UninstallConfirm.png'; name = '确认卸载' },
    @{ design = '7-正在卸载.png';      impl = 'Uninstalling.png';     name = '正在卸载' },
    @{ design = '8-卸载完成.png';      impl = 'Uninstalled.png';      name = '卸载完成' }
)

$probeBottom = 500
$step = 4
$tol = 12

foreach ($m in $wizardMap) {
    $dp = Join-Path $designDir $m.design
    $ip = Join-Path $implDir $m.impl

    if (-not (Test-Path $dp) -or -not (Test-Path $ip)) {
        Write-Host ("  [跳过] {0}（缺文件）" -f $m.name) -ForegroundColor Yellow
        continue
    }

    $d = New-Object System.Drawing.Bitmap($dp)
    $i = New-Object System.Drawing.Bitmap($ip)

    $total = 0
    $hit = 0
    for ($y = 0; $y -lt $probeBottom; $y += $step) {
        if ($y -ge $d.Height -or $y -ge $i.Height) { break }
        for ($x = 0; $x -lt 688; $x += $step) {
            if ($x -ge $d.Width -or $x -ge $i.Width) { break }
            $cd = $d.GetPixel($x, $y)
            $ci = $i.GetPixel($x, $y)
            $total++
            if ([math]::Abs($cd.R - $ci.R) -le $tol -and
                [math]::Abs($cd.G - $ci.G) -le $tol -and
                [math]::Abs($cd.B - $ci.B) -le $tol) { $hit++ }
        }
    }

    $pct = if ($total -gt 0) { [math]::Round(100.0 * $hit / $total, 1) } else { 0 }
    Check ($pct -ge 90) ("{0} 与设计稿一致度 ≥90%" -f $m.name) ("实际 {0}%" -f $pct)

    $d.Dispose()
    $i.Dispose()
}

# ─────────────────────────────────────────────────────────────
Write-Host ""
if ($fail -eq 0) {
    Write-Host "全部 $pass 项通过。" -ForegroundColor Green
} else {
    Write-Host "通过 $pass 项，失败 $fail 项。" -ForegroundColor Red
}

$empty.Dispose()
$sel.Dispose()
if ($ref) { $ref.Dispose() }

if (-not $KeepWorkDir) { Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue }

exit $(if ($fail -eq 0) { 0 } else { 1 })
