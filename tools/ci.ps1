#Requires -Version 5.1
<#
.SYNOPSIS
    xcool-install 的统一 CI 入口：构建（Debug + Release）→ 单测 → e2e → verify-ui。

.DESCRIPTION
    本地和 CI 跑的是同一份脚本，避免"CI 上绿、本地红"这类漂移。

    ⚠ 为什么不能只看退出码
    ────────────────────────────────────────────────────────────────
    tools\e2e.ps1 出过一次真实的假绿：文件头的 #Requires 和 <# 两行被编辑器吞掉，
    脚本解析失败、根本没执行；而外层只检查输出里有没有 "[FAIL]"，
    没看到 FAIL 就判通过 —— 一次"看起来全绿"的假通过。

    所以本脚本每一步都在退出码之外，额外断言**成功摘要行**：

        构建         "已成功生成" + "0 个错误"
        单元测试     "已通过!" + "失败: 0" + 总计 ≥ 186
        e2e          "全部 169 项断言通过。"（≥ 169 项）
        verify-ui    "全部 40 项通过。"（≥ 40 项）

    摘要行抓不到，即使退出码是 0 也判失败。断言数用**下限**而不是精确值：
    断言只会增加，卡死精确值会让"多加了一条断言"变成假红；
    真正要防的是"摘要行整个不见了"和"项数悄悄变少"（后者正是文件头被吞掉时的症状）。

.PARAMETER Only
    只跑指定环节，便于本地排查。可多选：build / test / e2e / ui。

.PARAMETER LogDir
    每一步的完整输出落到这个目录（默认 artifacts\ci-logs\），
    CI 失败时用 upload-artifact 上传，本地排查也方便。

.PARAMETER E2eMinChecks
    e2e 摘要行里断言数的最小值。

.PARAMETER UiMinChecks
    verify-ui 摘要行里通过项数的最小值。

.PARAMETER TestMinCount
    单测摘要里"总计"的最小值。

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\ci.ps1

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\ci.ps1 -Only e2e

.NOTES
    本文件必须保存为 **UTF-8 with BOM**。
    Windows PowerShell 5.1 会把无 BOM 的 .ps1 按 ANSI(GBK) 读取，中文全乱码，
    下面这些摘要行断言就永远匹配不上 —— 而那种失败极难看出来。
#>
[CmdletBinding()]
param(
    # ⚠️ 这里必须列全所有步骤的 Key，否则 -Only 传不进去。
    # （历史 bug：只列了 build/test/e2e/ui，但实际步骤有 6 个，
    #   于是 -Only clean 和 -Only vs2017 都会直接报 ValidateSet 错误。）
    [ValidateSet('clean', 'build', 'test', 'e2e', 'ui', 'vs2017')]
    [string[]]$Only = @('clean', 'build', 'test', 'e2e', 'ui', 'vs2017'),

    [string]$LogDir = '',
    # 190 = 本机实测总数（含 12.5 外部 DLL 段 7 项、老约定兼容键 8 项）。
    # 12.5 需要 NuGet 缓存里的 Newtonsoft.Json 13.0.4；取不到会少 7 项 → 183。
    [int]$E2eMinChecks = 183,
    # 62 = 本机实测总数（含第 8 节"产品名跟随"7 项 + 第 9 节"两端用词一致"5 项）
    [int]$UiMinChecks = 62,
    [int]$TestMinCount = 198
)

$ErrorActionPreference = 'Stop'

$Repo = Split-Path -Parent $PSScriptRoot
if (-not $LogDir) { $LogDir = Join-Path $Repo 'artifacts\ci-logs' }

$Solution = Join-Path $Repo 'Installer.sln'
$TestProj = Join-Path $Repo 'tests\Installer.Core.Tests\Installer.Core.Tests.csproj'
$E2eScript = Join-Path $PSScriptRoot 'e2e.ps1'
$UiScript = Join-Path $PSScriptRoot 'verify-ui.ps1'

# ─────────────────────────────────────────────────────────────
# 编码：dotnet 按控制台代码页决定输出编码，PowerShell 5.1 也按它解码。
# 不钉成 UTF-8，中文摘要行会变乱码 —— 断言永远匹配不上，却看不出原因。
# ─────────────────────────────────────────────────────────────
try {
    [Console]::OutputEncoding = New-Object System.Text.UTF8Encoding($false)
    $OutputEncoding = New-Object System.Text.UTF8Encoding($false)
}
catch {
    Write-Host "  （提示）控制台输出编码切 UTF-8 失败：$($_.Exception.Message)" -ForegroundColor Yellow
}

# CI runner（GitHub Actions / Gitee Go）默认 en-US，dotnet 的摘要会变成英文。
# 这里尽量钉成中文；下面每个断言同时认中文和英文，语言包缺失时也不会误判。
$env:DOTNET_CLI_UI_LANGUAGE = 'zh-CN'
$env:VSLANG = '2052'
$env:DOTNET_NOLOGO = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'

if (-not (Test-Path $LogDir)) { New-Item -ItemType Directory -Path $LogDir -Force | Out-Null }

# ─────────────────────────────────────────────────────────────
# 工具函数
# ─────────────────────────────────────────────────────────────

function Convert-BytesSmart {
    <#
        把一段字节按「UTF-8 还是 ANSI(GBK)」里更干净的那个解码。

        判据：解出来的 U+FFFD（替换字符）更少的那份更可信。
        UTF-8 字节被当成 GBK 解、或反过来，都会产生一堆 U+FFFD。
    #>
    param([byte[]]$Bytes, [int]$Offset, [int]$Count)

    if ($Count -le 0) { return '' }

    $repl = [string][char]0xFFFD
    $utf8 = [System.Text.Encoding]::UTF8.GetString($Bytes, $Offset, $Count)
    $ansi = [System.Text.Encoding]::Default.GetString($Bytes, $Offset, $Count)

    $badUtf8 = $utf8.Length - $utf8.Replace($repl, '').Length
    $badAnsi = $ansi.Length - $ansi.Replace($repl, '').Length

    if ($badUtf8 -le $badAnsi) { return $utf8 }
    return $ansi
}

function Read-TextSmart {
    <#
        读取一个日志文件，**逐行**判断编码。

        为什么不能整个文件只判一次：
          PowerShell 5.1 在 stdout 被重定向到文件时，写出去的编码会在中途变。
          实测 tools\e2e.ps1 的日志：前半段是 GBK、后半段是 UTF-8（子进程调用
          SetConsoleOutputCP 之类会改掉共享控制台的代码页）。
          整文件挑一种解码，必然有一半是乱码 —— 摘要行正好落在哪一半就成了运气。
          按行判断时每行只有一种编码，两边都能正确还原。
    #>
    param([string]$Path)

    if (-not (Test-Path $Path)) { return '' }
    $bytes = [System.IO.File]::ReadAllBytes($Path)
    if ($bytes.Length -eq 0) { return '' }

    $sb = New-Object System.Text.StringBuilder
    $start = 0
    for ($i = 0; $i -le $bytes.Length; $i++) {
        if ($i -eq $bytes.Length -or $bytes[$i] -eq 0x0A) {
            [void]$sb.Append((Convert-BytesSmart $bytes $start ($i - $start)))
            if ($i -lt $bytes.Length) { [void]$sb.Append("`n") }
            $start = $i + 1
        }
    }
    return $sb.ToString()
}

function Invoke-Native {
    <#
        跑一个外部程序并拿回「退出码 + stdout + stderr」。

        为什么不用 & / 2>&1：
          · PowerShell 5.1 会把原生命令写到 stderr 的每一行当成"终止性错误"，
            $ErrorActionPreference='Stop' 时直接中断脚本 —— dotnet 和 powershell
            都会往 stderr 写正常信息。
          · Start-Process 的 -RedirectStandardOutput/-RedirectStandardError 是句柄级
            重定向，不经过 PS 的管道格式化，中文摘要行原样落盘，再按上面的
            Read-TextSmart 读回来即可。
    #>
    param(
        [string]$File,
        [string[]]$Arguments,
        [string]$Tag
    )

    $outFile = Join-Path $LogDir "$Tag.out.txt"
    $errFile = Join-Path $LogDir "$Tag.err.txt"

    $argList = @($Arguments | ForEach-Object { if ($_ -match '\s') { '"' + $_ + '"' } else { $_ } })

    $p = Start-Process -FilePath $File -ArgumentList $argList `
        -NoNewWindow -Wait -PassThru `
        -RedirectStandardOutput $outFile -RedirectStandardError $errFile

    $stdout = Read-TextSmart $outFile
    $stderr = Read-TextSmart $errFile

    # 逐行解码已经能处理混合编码；这里再额外保留「整文件按 UTF-8 / 按 ANSI」两种读法，
    # 断言时三种都搜一遍 —— 万一某行跨了编码边界，也不会因此误判成"没有摘要行"。
    $variants = @(
        ($stdout + "`n" + $stderr),
        (([System.Text.Encoding]::UTF8.GetString([System.IO.File]::ReadAllBytes($outFile))) + "`n" +
         ([System.Text.Encoding]::UTF8.GetString([System.IO.File]::ReadAllBytes($errFile)))),
        (([System.Text.Encoding]::Default.GetString([System.IO.File]::ReadAllBytes($outFile))) + "`n" +
         ([System.Text.Encoding]::Default.GetString([System.IO.File]::ReadAllBytes($errFile))))
    )

    return [pscustomobject]@{
        ExitCode = $p.ExitCode
        StdOut   = $stdout
        StdErr   = $stderr
        All      = ($stdout + "`n" + $stderr)
        Variants = $variants
        LogFile  = $outFile
    }
}

function Test-AnyMatch {
    param([string[]]$Texts, [string]$Pattern)
    foreach ($t in $Texts) { if ($t -match $Pattern) { return $true } }
    return $false
}

function Get-FirstNumber {
    # 取第一个参与匹配的捕获组（正则里用 | 分支时，只有一个组会 Success）。
    # Texts 按可信度排序，逐个尝试，第一个匹配上的就用它。
    param([string[]]$Texts, [string]$Pattern)

    foreach ($Text in $Texts) {
        $m = [regex]::Match($Text, $Pattern)
        if (-not $m.Success) { continue }
        for ($i = 1; $i -lt $m.Groups.Count; $i++) {
            if ($m.Groups[$i].Success) { return [int]$m.Groups[$i].Value }
        }
    }
    return -1
}

function Get-Digest {
    param([string[]]$Texts, [string]$Pattern)

    foreach ($Text in $Texts) {
        $m = [regex]::Match($Text, $Pattern)
        if ($m.Success) { return $m.Value.Trim() }
    }
    return ''
}

function Format-Cell {
    # 中文按 2 字符宽计算，否则汇总表在控制台里会歪成一团
    param([string]$Text, [int]$Width)

    $w = 0
    foreach ($ch in $Text.ToCharArray()) {
        $w += $(if ([int]$ch -gt 0x2E80) { 2 } else { 1 })
    }
    return $Text + (' ' * [Math]::Max(0, $Width - $w))
}

function Show-Tail {
    param([string]$Text, [int]$Lines = 40)

    $all = @($Text -split "`r?`n" | Where-Object { $_ -ne '' })
    if ($all.Count -eq 0) { return }
    $from = [Math]::Max(0, $all.Count - $Lines)
    foreach ($l in $all[$from..($all.Count - 1)]) {
        Write-Host "        $l" -ForegroundColor DarkGray
    }
}

# ─────────────────────────────────────────────────────────────
# 环节定义
#
# Expect / Digest / CountRegex 里的正则都写成「中文|英文」两套：
# CI runner 是 en-US，即使钉了 DOTNET_CLI_UI_LANGUAGE，语言包缺失时仍会回落英文。
# ─────────────────────────────────────────────────────────────
$steps = @(
    @{
        Key        = 'clean'
        Name       = '干净还原'
        Tag        = '00-clean-restore'
        Expect     = @('已还原|Restored|Determining projects|正在确定要还原')
        Digest     = '已还原|Restored|已成功|Determining projects'
        CountRegex = ''
        MinCount   = 0
        Run        = {
            # 清 bin/obj 再 restore —— 这一步是**必须的**，不是洁癖。
            #
            # 如果只在热 obj 上构建，「资产文件缺少 RID 目标」（NETSDK1047）这类
            # 配置错误会被旧 assets 掩盖：本地一直绿、干净克隆直接挂。
            # 本项目真踩过一次 —— 把 RuntimeIdentifier 移除后 CI 全绿，
            # 但清掉 obj 后整个解决方案构建不了（net461 的 Exe 只要
            # PlatformTarget=x86，SDK 就会推 RuntimeIdentifier=win-x86，
            # 而 restore 只为**显式声明**的 RID 生成资产目标）。
            # 注意过滤条件必须用**相对路径**判断。
            # 仓库根目录本身就叫「安装包制作和卸载」，所以用绝对路径
            # -notmatch '\\安装包制作和卸载\\' 去排除嵌套副本时，
            # 会把所有路径都排除掉 —— 这一步就成了空转（真踩过，2 秒就"通过"了）。
            $repoRoot = Split-Path -Parent $PSScriptRoot
            $removed = 0
            Get-ChildItem -Recurse -Directory -LiteralPath $repoRoot -ErrorAction SilentlyContinue |
                Where-Object {
                    if ($_.Name -ne 'bin' -and $_.Name -ne 'obj') { return $false }
                    $rel = $_.FullName.Substring($repoRoot.Length + 1)
                    return ($rel -notlike '制作工具\*') -and ($rel -notlike '安装包制作和卸载\*')
                } |
                ForEach-Object {
                    Remove-Item -Recurse -Force $_.FullName -ErrorAction SilentlyContinue
                    $removed++
                }

            Write-Host ("    已清理 {0} 个 bin/obj 目录" -f $removed)
            Invoke-Native 'dotnet' @('restore', $Solution, '--nologo') '00-clean-restore'
        }
    },
    @{
        Key        = 'build'
        Name       = '构建 Debug'
        Tag        = '01-build-debug'
        Expect     = @('已成功生成|Build succeeded', '0 个错误|0 Error\(s\)')
        Digest     = '已成功生成.*|Build succeeded.*'
        CountRegex = ''
        MinCount   = 0
        Run        = {
            Invoke-Native 'dotnet' @('build', $Solution, '-c', 'Debug', '--nologo', '-v', 'm') '01-build-debug'
        }
    },
    @{
        Key        = 'build'
        Name       = '构建 Release'
        Tag        = '02-build-release'
        Expect     = @('已成功生成|Build succeeded', '0 个错误|0 Error\(s\)')
        Digest     = '已成功生成.*|Build succeeded.*'
        CountRegex = ''
        MinCount   = 0
        Run        = {
            Invoke-Native 'dotnet' @('build', $Solution, '-c', 'Release', '--nologo', '-v', 'm') '02-build-release'
        }
    },
    @{
        Key        = 'test'
        Name       = '单元测试'
        Tag        = '03-unit-tests'
        Expect     = @('已通过!|Passed!', '失败:\s*0[，,]|Failed:\s*0[，,]')
        Digest     = '已通过!|Passed!'
        CountRegex = '总计:\s*(\d+)|Total:\s*(\d+)'
        MinCount   = $TestMinCount
        Run        = {
            Invoke-Native 'dotnet' @('test', $TestProj, '-c', 'Debug', '--nologo') '03-unit-tests'
        }
    },
    @{
        Key        = 'e2e'
        Name       = '端到端 e2e'
        Tag        = '04-e2e'
        Expect     = @('全部\s+\d+\s+项断言通过。')
        Digest     = '全部\s+\d+\s+项断言通过。'
        CountRegex = '全部\s+(\d+)\s+项断言通过。'
        MinCount   = $E2eMinChecks
        Run        = {
            Invoke-Native 'powershell' @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $E2eScript) '04-e2e'
        }
    },
    @{
        Key        = 'ui'
        Name       = '界面校验 verify-ui'
        Tag        = '05-verify-ui'
        Expect     = @('全部\s+\d+\s+项通过。')
        Digest     = '全部\s+\d+\s+项通过。'
        CountRegex = '全部\s+(\d+)\s+项通过。'
        MinCount   = $UiMinChecks
        Run        = {
            Invoke-Native 'powershell' @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $UiScript) '05-verify-ui'
        }
    },
    @{
        Key        = 'vs2017'
        Name       = 'VS2017 兼容性'
        Tag        = '06-vs2017'
        Expect     = @('VS2017-OK|VS2017-SKIP')
        Digest     = 'VS2017-OK|VS2017-SKIP'
        CountRegex = ''
        MinCount   = 0
        Run        = {
            # 用 **VS2017 自带的 MSBuild**（不是 dotnet）构建测试项目。
            #
            # 为什么必须有这一步：VS2017 自带 .NET Core SDK 1.x，而 dotnet 用的是 10.x。
            # 两者对 SDK 风格 csproj 的支持范围差很多，只在 dotnet 下验证会漏掉
            # 真实用户（在 VS2017 里打开解决方案）遇到的错误。这个坑已经踩过两次：
            #
            #   1) NETSDK1050 —— net462 项目引用 netstandard2.0 包（xunit 2.9.3）时，
            #      VS2017 的 Microsoft.NET.Build.Extensions.NETFramework.targets(67,5)
            #      误判「SDK 版本过低」。
            #      修法：加 <UsingMicrosoftNETSdk>true</UsingMicrosoftNETSdk>。
            #   2) CS0122 —— <InternalsVisibleTo> 是 SDK 2.0+ 才支持的 MSBuild 项，
            #      VS2017 不认它，测试拿不到 internal 的 WizardHostImpl。
            #      修法：改成 Properties\AssemblyInfo.cs 里手写程序集特性。
            #
            # 只构建测试项目即可当金丝雀：它既引用 netstandard2.0 包、
            # 又依赖 InternalsVisibleTo，上面两个问题都会在这里暴露。
            $msbuild = $null
            $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
            if (Test-Path $vswhere) {
                $found = & $vswhere -version '[15.0,16.0)' -requires Microsoft.Component.MSBuild `
                                    -find 'MSBuild\**\Bin\MSBuild.exe' 2>$null | Select-Object -First 1
                if ($found -and (Test-Path $found)) { $msbuild = $found }
            }
            if (-not $msbuild) {
                foreach ($c in @(
                    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe",
                    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe",
                    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
                )) { if (Test-Path $c) { $msbuild = $c; break } }
            }

            if (-not $msbuild) {
                # CI runner 上没装 VS2017 是正常的 —— 跳过，不算失败。
                Write-Host '    VS2017-SKIP  本机没装 VS2017，跳过（本地开发机请务必跑到）'
                return [pscustomobject]@{
                    ExitCode = 0
                    StdOut   = 'VS2017-SKIP'
                    StdErr   = ''
                    All      = 'VS2017-SKIP'
                    Variants = @('VS2017-SKIP')
                    LogFile  = ''
                }
            }

            Write-Host ("    用 " + $msbuild)
            $repoRoot = Split-Path -Parent $PSScriptRoot
            $testProj = Join-Path $repoRoot 'tests\Installer.Core.Tests\Installer.Core.Tests.csproj'

            $r = Invoke-Native $msbuild @($testProj, '/t:Build', '/p:Configuration=Debug', '/v:q', '/nologo') '06-vs2017'

            if ($r.ExitCode -ne 0) {
                Write-Host '    VS2017 构建失败：' -ForegroundColor Red
                @($r.All -split "`r?`n" | Where-Object { $_ -match 'error ' }) |
                    Select-Object -First 6 | ForEach-Object { Write-Host ('      ' + $_.Trim()) }
                return $r
            }

            Write-Host '    VS2017-OK  测试项目用 VS2017 MSBuild 构建通过（0 错误）'

            # 成功摘要行要能被 harness 的 Expect 断言搜到 —— 所以并进 Variants。
            # （MSBuild 自己不会打印 "VS2017-OK"，得我们补上。）
            return [pscustomobject]@{
                ExitCode = 0
                StdOut   = ($r.StdOut + "`nVS2017-OK")
                StdErr   = $r.StdErr
                All      = ($r.All + "`nVS2017-OK")
                Variants = @(
                    ($r.Variants[0] + "`nVS2017-OK"),
                    ($r.Variants[1] + "`nVS2017-OK"),
                    ($r.Variants[2] + "`nVS2017-OK")
                )
                LogFile  = $r.LogFile
            }
        }
    }
)

# ─────────────────────────────────────────────────────────────
# 环境信息（CI 排查时最想知道的东西）
# ─────────────────────────────────────────────────────────────
Write-Host ''
Write-Host '════════════════════════════════════════════════════════════' -ForegroundColor Cyan
Write-Host ' xcool-install CI' -ForegroundColor Cyan
Write-Host '════════════════════════════════════════════════════════════' -ForegroundColor Cyan
Write-Host ("  仓库      : {0}" -f $Repo)
Write-Host ("  日志目录  : {0}" -f $LogDir)

$prevEap = $ErrorActionPreference
$ErrorActionPreference = 'Continue'
try {
    $dotnetVer = (& dotnet --version 2>&1 | Select-Object -First 1)
    if (-not $dotnetVer) { $dotnetVer = '（未安装？）' }
}
catch {
    $dotnetVer = "（找不到 dotnet：$($_.Exception.Message)）"
}
finally {
    $ErrorActionPreference = $prevEap
}

$stepNames = (($steps | Where-Object { $Only -contains $_.Key } | ForEach-Object { $_.Name }) -join ' → ')

Write-Host ("  dotnet    : {0}" -f $dotnetVer)
Write-Host ("  PowerShell: {0}" -f $PSVersionTable.PSVersion)
Write-Host ("  环节      : {0}" -f $stepNames)

# ─────────────────────────────────────────────────────────────
# 依次执行
# ─────────────────────────────────────────────────────────────
$results = @()
$failure = $null
$swTotal = [System.Diagnostics.Stopwatch]::StartNew()

foreach ($step in $steps) {
    if ($Only -notcontains $step.Key) { continue }

    Write-Host ''
    Write-Host ("══ " + $step.Name) -ForegroundColor Cyan
    Write-Host ('─' * 60) -ForegroundColor DarkCyan

    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $r = & $step.Run
    $sw.Stop()

    $secs = [Math]::Round($sw.Elapsed.TotalSeconds, 1)
    $note = ''

    # ① 退出码
    if ($r.ExitCode -ne 0) {
        $failure = "步骤「$($step.Name)」退出码 = $($r.ExitCode)（期望 0）。"
        $results += [pscustomobject]@{ Name = $step.Name; Ok = $false; Seconds = $secs; Digest = "退出码 $($r.ExitCode)" }
        Write-Host ("  [FAIL] 退出码 = {0}（期望 0）" -f $r.ExitCode) -ForegroundColor Red
        Write-Host '  输出尾部：' -ForegroundColor Yellow
        Show-Tail $r.All 40
        break
    }

    # ② 成功摘要行 —— 这一步才是"防假绿"的关键
    $missing = @($step.Expect | Where-Object { -not (Test-AnyMatch $r.Variants $_) })
    if ($missing.Count -gt 0) {
        $failure = "步骤「$($step.Name)」输出里找不到成功摘要行：$($missing -join '、')。`n" +
                   "        这说明脚本可能压根没跑完 —— 历史上 tools\e2e.ps1 就出过文件头被`n" +
                   "        编辑器吞掉、解析失败却看起来通过的事。退出码只说明进程没崩，`n" +
                   "        不足以下结论，所以摘要行必须单独断言。"
        $results += [pscustomobject]@{ Name = $step.Name; Ok = $false; Seconds = $secs; Digest = '缺少成功摘要行' }
        Write-Host ("  [FAIL] 缺少成功摘要行：{0}" -f ($missing -join '、')) -ForegroundColor Red
        Write-Host '  实际输出里出现的「全部/已通过/已成功」相关行：' -ForegroundColor Yellow
        $hits = @($r.Variants[0] -split "`r?`n" | Where-Object { $_ -match '全部|已通过|已成功|Passed|succeeded' })
        if ($hits.Count -eq 0) { Write-Host '        （一行都没有）' -ForegroundColor DarkGray }
        else { foreach ($h in $hits) { Write-Host "        $h" -ForegroundColor DarkGray } }
        Write-Host '  输出尾部：' -ForegroundColor Yellow
        Show-Tail $r.All 40
        break
    }

    # ③ 项数下限 —— 防"某项被静默跳过"（文件头被吞掉时项数会悄悄变少）
    if ($step.MinCount -gt 0 -and $step.CountRegex) {
        $n = Get-FirstNumber $r.Variants $step.CountRegex
        if ($n -lt $step.MinCount) {
            $failure = "步骤「$($step.Name)」摘要里的项数 = $n，少于下限 $($step.MinCount)。`n" +
                       "        摘要行格式对得上但数量变少，通常意味着有断言被静默跳过。"
            $results += [pscustomobject]@{ Name = $step.Name; Ok = $false; Seconds = $secs; Digest = "项数 $n < $($step.MinCount)" }
            Write-Host ("  [FAIL] 项数 {0} < 下限 {1}（有断言被静默跳过？）" -f $n, $step.MinCount) -ForegroundColor Red
            Show-Tail $r.All 40
            break
        }
        $note = "（{0} 项）" -f $n
    }

    $digest = Get-Digest $r.Variants $step.Digest
    [System.IO.File]::WriteAllText(
        (Join-Path $LogDir "$($step.Tag).log"), $r.All, (New-Object System.Text.UTF8Encoding($false)))

    $results += [pscustomobject]@{ Name = $step.Name; Ok = $true; Seconds = $secs; Digest = $digest }
    Write-Host ("  [OK]   {0}  ({1}s)  {2}" -f $digest, $secs, $note) -ForegroundColor Green
}

$swTotal.Stop()

# ─────────────────────────────────────────────────────────────
# 汇总表
# ─────────────────────────────────────────────────────────────
Write-Host ''
Write-Host '════════════════════════════════════════════════════════════' -ForegroundColor Cyan
Write-Host ' 汇总' -ForegroundColor Cyan
Write-Host '════════════════════════════════════════════════════════════' -ForegroundColor Cyan
Write-Host ('  ' + (Format-Cell '环节' 24) + (Format-Cell '结果' 8) + (Format-Cell '耗时' 10) + '成功摘要') -ForegroundColor White
Write-Host ('  ' + ('─' * 72)) -ForegroundColor DarkCyan

foreach ($x in $results) {
    $mark = $(if ($x.Ok) { 'OK' } else { 'FAIL' })
    $color = $(if ($x.Ok) { 'Green' } else { 'Red' })
    Write-Host ('  ' + (Format-Cell $x.Name 24) + (Format-Cell $mark 8) +
        (Format-Cell ("{0}s" -f $x.Seconds) 10) + $x.Digest) -ForegroundColor $color
}

Write-Host ('  ' + ('─' * 72)) -ForegroundColor DarkCyan
Write-Host ('  ' + (Format-Cell '合计' 24) + (Format-Cell '' 8) + (Format-Cell ("{0}s" -f [Math]::Round($swTotal.Elapsed.TotalSeconds, 1)) 10)) -ForegroundColor White

# ─────────────────────────────────────────────────────────────
if ($failure) {
    Write-Host ''
    Write-Host '════════════════════════════════════════════════════════════' -ForegroundColor Red
    Write-Host ' CI 失败' -ForegroundColor Red
    Write-Host '════════════════════════════════════════════════════════════' -ForegroundColor Red
    Write-Host "  $failure" -ForegroundColor Red
    Write-Host ''
    Write-Host ("  完整日志：{0}" -f $LogDir) -ForegroundColor Yellow
    exit 1
}

Write-Host ''
Write-Host ("CI 全部通过：{0} 个环节。" -f $results.Count) -ForegroundColor Green
Write-Host ''
exit 0
