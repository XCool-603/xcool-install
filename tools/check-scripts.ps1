#Requires -Version 5.1
<#
.SYNOPSIS
    校验所有 PowerShell 脚本的文件头完整性。

.DESCRIPTION
    本机只有 Windows PowerShell 5.1，它会把无 BOM 的 UTF-8 脚本按 ANSI 读，
    中文注释直接变乱码；而某些编辑器在写回文件时会吃掉首行的井号。

    这两件事已经造成过真实事故：

      - tools/e2e.ps1 的 Requires 指令与块注释起始符两行被吞，
        导致注释块未闭合、脚本解析失败、根本没执行；
        但外层只看到「没有 FAIL」而误判通过。
      - tools/verify-ui.ps1 首行变成裸命令，param 被当成命令调用，
        参数全部失效，9 项设计稿颜色比对被静默跳过，
        却照样报「全部通过」。

    所以这个检查必须进 CI：脚本自身的健康不能靠人肉发现。

    检查五项：
      1. 文件以 UTF-8 BOM 开头
      2. 首行是注释（井号开头）或空行
      3. 块注释起止符数量配对
      4. 能被 PowerShell 解析器无错解析
      5. param 之前没有语句（CmdletBinding 属性除外）

.PARAMETER Fix
    自动修复能修的：补 BOM、补首行丢失的井号。
    不能自动修的（注释块不配对、语法错误）只报告。

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\check-scripts.ps1
    powershell -ExecutionPolicy Bypass -File tools\check-scripts.ps1 -Fix
#>
[CmdletBinding()]
param(
    [switch]$Fix
)

$ErrorActionPreference = 'Stop'

$repo = Split-Path -Parent $PSScriptRoot
$bomBytes = [byte[]](0xEF, 0xBB, 0xBF)

# 块注释的起止符，用变量拼出来，避免本文件自己的注释被误判
$blockOpen = '<' + '#'
$blockClose = '#' + '>'

function Get-ScriptFiles {
    Get-ChildItem -Recurse -File -Filter '*.ps1' -LiteralPath $repo -ErrorAction SilentlyContinue |
        Where-Object {
            $rel = $_.FullName.Substring($repo.Length + 1)
            $rel -notmatch '^(bin|obj|\.git)\\' -and
            $rel -notmatch '\\(bin|obj)\\' -and
            $rel -notmatch '^安装包制作和卸载\\'
        }
}

$problems = 0
$fixedCount = 0
$files = @(Get-ScriptFiles)
Write-Host ("检查 {0} 个脚本…" -f $files.Count)
Write-Host ''

foreach ($file in $files) {
    $rel = $file.FullName.Substring($repo.Length + 1)
    $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
    $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    $issues = @()
    $changed = $false

    if (-not $hasBom) {
        if ($Fix) {
            $out = New-Object byte[] ($bytes.Length + 3)
            [Array]::Copy($bomBytes, 0, $out, 0, 3)
            [Array]::Copy($bytes, 0, $out, 3, $bytes.Length)
            [System.IO.File]::WriteAllBytes($file.FullName, $out)
            $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
            $hasBom = $true
            $changed = $true
            Write-Host ("  [已修] {0}  补 UTF-8 BOM" -f $rel) -ForegroundColor Cyan
        } else {
            $issues += '缺少 UTF-8 BOM（中文会变乱码）'
        }
    }

    $text = [System.Text.Encoding]::UTF8.GetString($bytes)
    if ($text.StartsWith([char]0xFEFF)) { $text = $text.Substring(1) }
    $lines = New-Object 'System.Collections.Generic.List[string]'
    foreach ($x in ($text -split "`r`n")) { $lines.Add($x) }
    if ($lines.Count -lt 2) {
        $lines.Clear()
        foreach ($x in ($text -split "`n")) { $lines.Add($x) }
    }

    $first = $lines[0].TrimEnd()
    if ($first -ne '' -and -not $first.StartsWith('#')) {
        if ($Fix -and $first -match '^(Requires\s|requires\s)') {
            $lines[0] = '#' + $lines[0]
            [System.IO.File]::WriteAllLines($file.FullName, $lines, (New-Object System.Text.UTF8Encoding($true)))
            $changed = $true
            Write-Host ("  [已修] {0}  首行补回井号" -f $rel) -ForegroundColor Cyan
        } else {
            $issues += ("首行不是注释：{0}" -f $first)
        }
    }

    # 按**状态机**数起止符：已经处于块注释里时，注释文本中写的起止符字面量不算数。
    # 这一点很要紧 —— 说明文字里写一句「把起始符补回去」就会让朴素计数误报
    # （ci.ps1 就被这样误报过一次）。
    # 真正会出事的是注释里出现结束符：PowerShell 的块注释**不嵌套**，
    # 第一个结束符就把注释合上了，后面的内容会被当成代码解析（verify-ui.ps1 中过这招）。
    $openCount = 0
    $closeCount = 0
    $depth = 0
    foreach ($raw in $lines) {
        if ($depth -eq 0) {
            if ($raw.Contains($blockOpen)) {
                $openCount++
                $depth = 1
                if ($raw.Contains($blockClose)) { $depth = 0 }
            } elseif ($raw.Contains($blockClose)) {
                $closeCount++
            }
        } elseif ($raw.Contains($blockClose)) {
            $closeCount++
            $depth = 0
        }
    }

    if ($depth -ne 0) {
        $issues += '块注释没有闭合（起始符之后找不到结束符）'
    } elseif ($openCount -ne $closeCount) {
        $issues += ("块注释起止符不配对（起 {0} / 止 {1}）" -f $openCount, $closeCount)
    }

    $parseErrors = $null
    [void][System.Management.Automation.Language.Parser]::ParseFile($file.FullName, [ref]$null, [ref]$parseErrors)
    if ($parseErrors -and $parseErrors.Count -gt 0) {
        $issues += ("语法错误 {0} 处，首个：{1}" -f $parseErrors.Count, $parseErrors[0].Message)
    }

    $inBlock = $false
    $stmtsBeforeParam = 0
    $sawParam = $false
    foreach ($raw in $lines) {
        $s = $raw.Trim()
        if ($s -eq '') { continue }
        if ($inBlock) {
            if ($s.Contains($blockClose)) { $inBlock = $false }
            continue
        }
        if ($s.StartsWith($blockOpen)) {
            if (-not $s.Contains($blockClose)) { $inBlock = $true }
            continue
        }
        if ($s.StartsWith('#')) { continue }
        if ($s -match '^\[CmdletBinding') { continue }
        if ($s -match '^param\s*\(') { $sawParam = $true; break }
        $stmtsBeforeParam++
    }
    if ($sawParam -and $stmtsBeforeParam -gt 0) {
        $issues += ("param 之前有 {0} 条语句（参数会失效）" -f $stmtsBeforeParam)
    }

    if ($issues.Count -gt 0) {
        $problems++
        Write-Host ("  [问题] {0}" -f $rel) -ForegroundColor Red
        foreach ($issue in $issues) { Write-Host ("         - {0}" -f $issue) -ForegroundColor Red }
    } elseif ($changed) {
        $fixedCount++
    } else {
        Write-Host ("  [OK]   {0}" -f $rel) -ForegroundColor DarkGray
    }
}

Write-Host ''
if ($problems -eq 0) {
    Write-Host ("全部 {0} 个脚本检查通过。" -f $files.Count) -ForegroundColor Green
    if ($fixedCount -gt 0) { Write-Host ("（自动修复了 {0} 个）" -f $fixedCount) -ForegroundColor Cyan }
    exit 0
}

Write-Host ("{0} 个脚本有问题。" -f $problems) -ForegroundColor Red
if (-not $Fix) { Write-Host '加 -Fix 可以自动修复能修的项。' -ForegroundColor Yellow }
exit 1