#Requires -Version 5.1
<#
.SYNOPSIS
    检查一个安装包里固化的安装逻辑是否包含指定的修复。

.DESCRIPTION
    安装包 = [stub 区][payload][manifest][footer 128B]。安装逻辑全在 stub 区里
    （Installer.Core / Installer.UI 等程序集被嵌进 stub 的资源），
    所以直接在包里搜关键标识符，就能判断这个包是不是「修复前打的」。

    为什么不比 SHA-256：stub 里嵌的是 @(ReferencePath) 全部引用程序集，
    引用集合会随构建环境变化，重建一次哈希就变了 —— 用它判断会误报。
    直接搜代码里的标识符才准。

.PARAMETER Package
    要检查的安装包（Setup.exe）。

.PARAMETER Marker
    关键标识符，默认 SuppressAutoLaunch（「没点完成就自动运行」那次修复引入的）。

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\check-package.ps1 -Package D:\out\Setup.exe
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Package,
    [string]$Marker = 'SuppressAutoLaunch'
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $Package)) { throw "找不到安装包：$Package" }

$bytes = [System.IO.File]::ReadAllBytes($Package)

if ($bytes.Length -lt 256) { throw '文件太小，不是本系统的安装包。' }
$footerAt = $bytes.Length - 128
$magic = [System.Text.Encoding]::ASCII.GetString($bytes, $footerAt, 6)
if ($magic -ne 'WMINS2') { throw "不是本系统的安装包（footer 魔数 = '$magic'，应为 WMINS2）。" }

$stubLen = [System.BitConverter]::ToInt64($bytes, $footerAt + 16)
$payloadLen = [System.BitConverter]::ToInt64($bytes, $footerAt + 24)

# 字节级搜 ASCII 标识符（不用字符串转换，避免编码/对齐问题）
function Test-Ascii([byte[]]$hay, [int]$limit, [string]$needle) {
    $n = [System.Text.Encoding]::ASCII.GetBytes($needle)
    $end = [Math]::Min($limit, $hay.Length) - $n.Length
    for ($i = 0; $i -le $end; $i++) {
        $ok = $true
        for ($j = 0; $j -lt $n.Length; $j++) {
            if ($hay[$i + $j] -ne $n[$j]) { $ok = $false; break }
        }
        if ($ok) { return $true }
    }
    return $false
}

Write-Host ''
Write-Host '安装包：' -NoNewline; Write-Host $Package
Write-Host ('  大小         : {0:N0} 字节' -f $bytes.Length)
Write-Host ('  stub 区      : {0:N0} 字节' -f $stubLen)
Write-Host ('  payload      : {0:N0} 字节' -f $payloadLen)
Write-Host ('  打包时间     : {0}' -f (Get-Item $Package).LastWriteTime)
Write-Host ''

$hit = Test-Ascii $bytes ([int]$stubLen) $Marker
Write-Host ('  关键标识符   : {0}' -f $Marker)
Write-Host ''

if ($hit) {
    Write-Host '结论：包含该修复 —— 安装逻辑是最新的。' -ForegroundColor Green
    exit 0
}

Write-Host '结论：不包含该修复 —— 这是【修复前】打的包。' -ForegroundColor Yellow
Write-Host '      安装包在打包那一刻就固化了，之后改的源码不会进到已生成的包里。' -ForegroundColor Yellow
Write-Host '      请用「制作工具\安装包制作助手.exe」重新打包后再测。' -ForegroundColor Yellow
exit 1