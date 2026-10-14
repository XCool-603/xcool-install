#Requires -Version 5.1
<#
.SYNOPSIS
    生成多尺寸应用图标（.ico）。

.DESCRIPTION
    项目原有的 app_install.ico 只有 64x64 一帧。Windows 在「大图标 / 超大图标」
    视图下找不到 256x256 帧，会退回显示通用图标 —— 看起来就像「图标没生效」。

    本脚本按品牌色（#1677FF）与主界面同一个盾牌图形，逐尺寸**矢量重绘**
    （而不是把小图放大），输出 16/24/32/48/64/128/256 七帧：
      - 128 及以下用 BMP(DIB) 帧，兼容性最好
      - 256 用 PNG 帧（BMP 要 262KB，PNG 只有几 KB）
    PeIconPatcher 两种编码都支持，见其类注释。

.PARAMETER Output
    输出 .ico 路径。默认 <仓库根>\src\Installer.Runtime\app.ico

.PARAMETER AlsoBuilder
    同时写一份到 src\Installer.Builder\app.ico

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\make-icon.ps1
#>
[CmdletBinding()]
param(
    [string]$Output,
    [switch]$AlsoBuilder
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$repo = Split-Path -Parent $PSScriptRoot
if (-not $Output) { $Output = Join-Path $repo 'src\Installer.Runtime\app.ico' }

$Brand = [System.Drawing.Color]::FromArgb(255, 22, 119, 255)   # #1677FF
$White = [System.Drawing.Color]::White

function New-IconBitmap([int]$size) {
    $bmp = New-Object System.Drawing.Bitmap($size, $size,
        [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)

    # ── 圆角方块（品牌底）──
    $rad = [single]($size * 0.22)
    $d = [single]($rad * 2)
    $rp = New-Object System.Drawing.Drawing2D.GraphicsPath
    $rp.AddArc(0, 0, $d, $d, 180, 90)
    $rp.AddArc($size - $d, 0, $d, $d, 270, 90)
    $rp.AddArc($size - $d, $size - $d, $d, $d, 0, 90)
    $rp.AddArc(0, $size - $d, $d, $d, 90, 90)
    $rp.CloseFigure()
    $brandBrush = New-Object System.Drawing.SolidBrush($Brand)
    $g.FillPath($brandBrush, $rp)

    # ── 白色盾牌（与主界面 pageHeader 的 IconSvg 同形）──
    # viewBox 28x28：M14 6.5 l-6.5 3.2 v4.6 c0 4 2.8 7.8 6.5 8.7
    #                c3.7-.9 6.5-4.7 6.5-8.7 V9.7 L14 6.5 z
    $s = [single]($size / 28.0)
    $sp = New-Object System.Drawing.Drawing2D.GraphicsPath
    $sp.AddLine(14 * $s, 6.5 * $s, 7.5 * $s, 9.7 * $s)
    $sp.AddLine(7.5 * $s, 9.7 * $s, 7.5 * $s, 14.3 * $s)
    $sp.AddBezier(7.5 * $s, 14.3 * $s, 7.5 * $s, 18.3 * $s, 10.3 * $s, 22.1 * $s, 14 * $s, 23 * $s)
    $sp.AddBezier(14 * $s, 23 * $s, 17.7 * $s, 22.1 * $s, 20.5 * $s, 18.3 * $s, 20.5 * $s, 14.3 * $s)
    $sp.AddLine(20.5 * $s, 14.3 * $s, 20.5 * $s, 9.7 * $s)
    $sp.CloseFigure()
    $whiteBrush = New-Object System.Drawing.SolidBrush($White)
    $g.FillPath($whiteBrush, $sp)

    $whiteBrush.Dispose(); $brandBrush.Dispose()
    $sp.Dispose(); $rp.Dispose(); $g.Dispose()
    return $bmp
}

function ConvertTo-BmpFrame([System.Drawing.Bitmap]$bmp) {
    $w = $bmp.Width; $h = $bmp.Height
    $ms = New-Object System.IO.MemoryStream
    $bw = New-Object System.IO.BinaryWriter($ms)
    # BITMAPINFOHEADER
    $bw.Write([int]40)             # biSize
    $bw.Write([int]$w)             # biWidth
    $bw.Write([int]($h * 2))       # biHeight = XOR + AND
    $bw.Write([int16]1)            # biPlanes
    $bw.Write([int16]32)           # biBitCount
    $bw.Write([int]0)              # biCompression = BI_RGB
    $bw.Write([int]($w * $h * 4))  # biSizeImage
    $bw.Write([int]0); $bw.Write([int]0)
    $bw.Write([int]0); $bw.Write([int]0)
    # XOR 位图：自下而上 BGRA
    for ($y = $h - 1; $y -ge 0; $y--) {
        for ($x = 0; $x -lt $w; $x++) {
            $c = $bmp.GetPixel($x, $y)
            $bw.Write([byte]$c.B); $bw.Write([byte]$c.G)
            $bw.Write([byte]$c.R); $bw.Write([byte]$c.A)
        }
    }
    # AND 掩码：1bpp，每行补齐到 4 字节。32bpp 下靠 alpha，这里全 0。
    $maskRow = [int]([math]::Floor(($w + 31) / 32) * 4)
    $zero = New-Object byte[] $maskRow
    for ($y = 0; $y -lt $h; $y++) { $bw.Write($zero, 0, $maskRow) }
    $bw.Flush()
    $bytes = $ms.ToArray()
    $bw.Dispose(); $ms.Dispose()
    return $bytes
}

function ConvertTo-PngFrame([System.Drawing.Bitmap]$bmp) {
    $ms = New-Object System.IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    $bytes = $ms.ToArray()
    $ms.Dispose()
    return $bytes
}

$sizes = @(16, 24, 32, 48, 64, 128, 256)
$frames = @()
foreach ($sz in $sizes) {
    $bmp = New-IconBitmap $sz
    $data = if ($sz -ge 256) { ConvertTo-PngFrame $bmp } else { ConvertTo-BmpFrame $bmp }
    $frames += [pscustomobject]@{ Size = $sz; Data = $data }
    $bmp.Dispose()
}

# ── 组装 ICONDIR + ICONDIRENTRY + 图像数据 ──
$ms = New-Object System.IO.MemoryStream
$bw = New-Object System.IO.BinaryWriter($ms)
$bw.Write([int16]0)                # reserved
$bw.Write([int16]1)                # type = icon
$bw.Write([int16]$frames.Count)
$offset = 6 + 16 * $frames.Count
foreach ($f in $frames) {
    $dim = if ($f.Size -ge 256) { 0 } else { $f.Size }   # 256 记作 0
    $bw.Write([byte]$dim)
    $bw.Write([byte]$dim)
    $bw.Write([byte]0)             # 调色板数
    $bw.Write([byte]0)             # reserved
    $bw.Write([int16]1)            # planes
    $bw.Write([int16]32)           # bitCount
    $bw.Write([int]$f.Data.Length)
    $bw.Write([int]$offset)
    $offset += $f.Data.Length
}
foreach ($f in $frames) { $bw.Write($f.Data, 0, $f.Data.Length) }
$bw.Flush()
$all = $ms.ToArray()
$bw.Dispose(); $ms.Dispose()

$dir = Split-Path -Parent $Output
if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
[System.IO.File]::WriteAllBytes($Output, $all)

Write-Host ("已生成 {0}" -f $Output)
Write-Host ("  大小 {0:N0} 字节，{1} 帧" -f $all.Length, $frames.Count)
foreach ($f in $frames) {
    Write-Host ("    {0,3}x{0,-3}  {1,7:N0} B  {2}" -f $f.Size, $f.Data.Length,
        $(if ($f.Size -ge 256) { 'PNG' } else { 'BMP' }))
}

if ($AlsoBuilder) {
    $b = Join-Path $repo 'src\Installer.Builder\app.ico'
    [System.IO.File]::WriteAllBytes($b, $all)
    Write-Host ("已同步到 {0}" -f $b)
}