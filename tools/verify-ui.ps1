#Requires -Version 5.1
<#
.SYNOPSIS
    制作端界面 —— 与 Calicat 设计稿的自动对比
#

# 用法：powershell -ExecutionPolicy Bypass -File tools\verify-ui.ps1
#
# 做两件事：
#   ① 逐点比对渲染图与设计稿的颜色（设计稿存在 docs\界面截图\制作端\设计稿-对照.png）
#   ② 检查 bounds.txt 里的结构不变量（复选框对齐、控件不被父容器裁切）
#
# 退出码 0 = 全部通过。
#>

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
$skip = 0

function Check($ok, $name, $detail) {
    if ($ok) {
        $script:pass++
        Write-Host ("  [OK]   " + $name) -ForegroundColor Green
    } else {
        $script:fail++
        Write-Host ("  [FAIL] " + $name + "  " + $detail) -ForegroundColor Red
    }
}

# 记录一次「跳过」。跳过不是通过 —— 最后会据此判失败，
# 否则「设计稿文件丢了」会伪装成「全部通过」。
function Skip($name, $reason) {
    $script:skip++
    Write-Host ("  [跳过] " + $name + "  " + $reason) -ForegroundColor Yellow
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
    Skip "窗口尺寸一致" "没有设计稿：$Design"
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
    @{ design = '6-确认卸载.png';      impl = 'uninstall\UninstallConfirm.png'; name = '确认卸载' },
    @{ design = '7-正在卸载.png';      impl = 'uninstall\Uninstalling.png';     name = '正在卸载' },
    @{ design = '8-卸载完成.png';      impl = 'uninstall\Uninstalled.png';      name = '卸载完成' }
)

$probeBottom = 500
$step = 4
$tol = 12

foreach ($m in $wizardMap) {
    $dp = Join-Path $designDir $m.design
    $ip = Join-Path $implDir $m.impl

    if (-not (Test-Path $dp) -or -not (Test-Path $ip)) {
        Skip $m.name "缺文件（设计稿或实现图）"
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
Write-Host "== 7. 向导文案与图标（颜色比对看不出这一类问题）" -ForegroundColor Cyan
# 为什么要有这一节：第 6 节只采样颜色。文案写错了、图标没画，
# 颜色比对完全没反应 —— 前几轮就是因此误判「几何对了就等于接近设计稿」。
$strFile = Join-Path $root 'src\Installer.UI\Common\WizardStrings.cs'
$strText = if (Test-Path $strFile) {
    [System.IO.File]::ReadAllText($strFile, [System.Text.UTF8Encoding]::new($false))
} else { '' }

# 设计稿第 3 版的原句，必须逐字出现在文案表里
$designCopy = @(
    @{ key = 'Welcome.Title';    frag = '欢迎使用「{0}」安装向导' },
    @{ key = 'Welcome.Body';     frag = '这个向导将引导你完成「{0} {1}」的安装，整个过程大约需要 1 分钟。' },
    @{ key = 'Welcome.Body2';    frag = '继续之前，建议先关闭其他正在运行的程序，避免安装过程中出现文件占用。' },
    @{ key = 'Welcome.Hint';     frag = '下一步需要先阅读并同意「许可协议」，点击「下一步」继续。' },
    @{ key = 'Welcome.CaptionDir';     frag = '默认安装位置' },
    @{ key = 'Welcome.CaptionSpace';   frag = '需要磁盘空间' },
    @{ key = 'Welcome.CaptionVersion'; frag = '版本与发布者' }
)
foreach ($c in $designCopy) {
    Check ($strText.Contains($c.frag)) ("文案「{0}」与设计稿一致" -f $c.key) $c.frag
}

# 图标：设计稿要求的三处必须有非空 SVG
$iconChecks = @(
    @{ file = 'Views\WelcomeView.Designer.cs';  what = '信息卡图标块'; frag = 'boxDir.Radius = 9' },
    @{ file = 'Views\WelcomeView.Designer.cs';  what = '提示条 ⓘ';     frag = 'lblHintIcon.PrefixSvg' },
    @{ file = 'Forms\WizardForm.Designer.cs';   what = '主按钮箭头';   frag = 'btnPrimary.IconSvg' }
)
foreach ($c in $iconChecks) {
    $p = Join-Path (Join-Path $root 'src\Installer.UI') $c.file
    $txt = if (Test-Path $p) { [System.IO.File]::ReadAllText($p, [System.Text.UTF8Encoding]::new($false)) } else { '' }
    Check ($txt.Contains($c.frag)) ("{0} 已定义" -f $c.what) $c.frag
}

# ─────────────────────────────────────────────────────────────
Write-Host "== 8. 产品名跟随（用户反馈：换了启动程序产品名根本不变）" -ForegroundColor Cyan

# 用户反馈：界面上"产品名跟随文件夹名"勾着，但换启动程序时产品名根本不变。
# 根因是自动命名被一段隐藏的"用户改过就不覆盖"启发式永久关掉了，
# 而那个复选框宿主从来没读过 —— 勾了、取消了都没有任何反应。
#
# 这条**交互**链路 --render-ui 看不见（它只能拍某个瞬间的画面），
# 所以制作端提供了 --selftest-autoname 无头入口，把三条保证固化成可断言输出。
$autoDir = Join-Path $work 'autoname'
New-Item -ItemType Directory -Path $autoDir -Force | Out-Null
$noBom = [System.Text.UTF8Encoding]::new($false)
[System.IO.File]::WriteAllText((Join-Path $autoDir 'PhotoStudio.exe'), 'fake', $noBom)
[System.IO.File]::WriteAllText((Join-Path $autoDir 'Helper.exe'), 'fake', $noBom)

$autoOut = Join-Path $work 'autoname.txt'
$autoErr = Join-Path $work 'autoname.err'
Start-Process -FilePath $exe -ArgumentList @('--selftest-autoname', $autoDir) -Wait -NoNewWindow `
    -RedirectStandardOutput $autoOut -RedirectStandardError $autoErr
$auto = @(Get-Content -LiteralPath $autoOut -Encoding UTF8)

function Get-Auto($lines, $key) {
    $m = $lines | Where-Object { $_ -like ($key + '=*') } | Select-Object -First 1
    if ($null -eq $m) { return '<缺失>' }
    return $m.Substring($key.Length + 1)
}

# 完成标记是裸行（没有 key=value），单独判断
Check ($auto -contains 'AUTONAME-DONE') `
      '产品名自检跑完（没有中途退出）' ($auto -join ' | ')
Check ((Get-Auto $auto 'autoNameOnLoad') -eq 'True') '新建工程默认打开「产品名跟随」'
Check ((Get-Auto $auto 'pickChanged') -eq 'True') `
      '换启动程序 → 产品名跟着变（这条就是用户报的 bug）' `
      ("pickLast=$(Get-Auto $auto 'pickLast')  pickFirst=$(Get-Auto $auto 'pickFirst')")
Check ((Get-Auto $auto 'pickFirst') -eq 'Helper') `
      '产品名 = 启动程序的 exe 名（去扩展名）' (Get-Auto $auto 'pickFirst')
Check ((Get-Auto $auto 'autoNameAfterTyping') -eq 'False') `
      '用户自己敲产品名后「跟随」自动关掉（输入不会被下一次选择覆盖）'
Check ((Get-Auto $auto 'nameAfterTyping') -eq 'MyOwnName') `
      '用户输入的产品名留住了' (Get-Auto $auto 'nameAfterTyping')
Check ((Get-Auto $auto 'nameAfterRecheck') -eq 'Helper') `
      '重新勾上「跟随」→ 立刻按当前启动程序重算' (Get-Auto $auto 'nameAfterRecheck')

# ─────────────────────────────────────────────────────────────
Write-Host "== 9. 制作端与安装端「附加任务」用词一致" -ForegroundColor Cyan

# 用户反馈：制作流程和安装过程里同一组选项（尤其"开机自启"）用词不一致 ——
# 制作端写「开机自动启动」，安装端写「安装后自动运行」，根本认不出是同一件事。
#
# 根因：同一组 key 被「安装设置」页和「完成」页复用，而设计稿两页的措辞本来就不同
# （安装设置页"开机时自动启动"，完成页"安装后自动运行"），于是必然有一边写错。
# 修法是两页各用一套 key，再断言两端的用词逐字相同。
$bldFile = Join-Path $root 'src\Installer.Builder\Common\BuilderStrings.cs'
$bldText = if (Test-Path $bldFile) {
    [System.IO.File]::ReadAllText($bldFile, [System.Text.UTF8Encoding]::new($false))
} else { '' }

$taskWording = @(
    @{ name = '桌面快捷方式';     frag = '创建桌面快捷方式' },
    @{ name = '开始菜单快捷方式'; frag = '创建开始菜单快捷方式' },
    @{ name = '开机自启';         frag = '开机时自动启动' },
    @{ name = '完成后运行';       frag = '安装完成后运行' }
)
foreach ($w in $taskWording) {
    Check ($strText.Contains($w.frag) -and $bldText.Contains($w.frag)) `
        ("制作端与安装端用词一致：{0}" -f $w.name) `
        ("安装端={0} 制作端={1}" -f $strText.Contains($w.frag), $bldText.Contains($w.frag))
}

# 完成页必须用自己那套 key —— 一旦有人图省事改回 Options.*，这条立刻失败
Check ($strText.Contains('Finish.ChipDesktop') -and $strText.Contains('Finish.ChipStartMenu') `
       -and $strText.Contains('Finish.ChipAutostart') -and $strText.Contains('Finish.RunNow')) `
      '完成页有独立的标签文案（不与安装设置页共用 key）'

# ─────────────────────────────────────────────────────────────
$empty.Dispose()
$sel.Dispose()
if ($ref) { $ref.Dispose() }

if (-not $KeepWorkDir) { Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue }

# ─────────────────────────────────────────────────────────────
# 汇总必须放在**所有**检查之后 —— 之前它印在第 7 节之前，
# 于是摘要里的数字漏掉了第 7 节的 10 条（报 40，实际 50）。
Write-Host ""
if ($fail -eq 0 -and $skip -eq 0) {
    Write-Host "全部 $pass 项通过。" -ForegroundColor Green
} elseif ($fail -eq 0) {
    Write-Host "通过 $pass 项，但有 $skip 项被跳过 —— 跳过不等于通过。" -ForegroundColor Red
} else {
    Write-Host "通过 $pass 项，失败 $fail 项，跳过 $skip 项。" -ForegroundColor Red
}

exit $(if ($fail -eq 0 -and $skip -eq 0) { 0 } else { 1 })





