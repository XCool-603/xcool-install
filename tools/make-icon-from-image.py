#!/usr/bin/env python3
"""
从一张图片生成多尺寸应用图标（.ico）。

做四件事：
  1. 去背景 —— 从四角洪水填充（带容差），把连通的浅色背景变透明。
     用洪水填充而不是「全图白色→透明」，是为了不把图标内部的白色
     （比如 SETUP 字样、银色条）也挖空。
  2. 裁边 —— 按 **alpha 通道**算包围盒（不能直接用 getbbox：它对 RGBA
     是看所有通道非零，透明像素的 RGB 还在，会把整张图都算进去），
     再补成正方形并留边距，让图标在每个尺寸下都居中、大小一致。
  3. 预乘缩放 —— 直接 resize RGBA 会把透明像素的 RGB 混进边缘（白边）。
     先按 alpha 预乘，缩放后再还原，边缘才干净。
  4. 出 7 帧 —— 16/24/32/48/64/128/256，逐尺寸重采样（不是把小图放大）。
     128 及以下写 BMP(DIB) 帧，256 写 PNG 帧。PeIconPatcher 两种都支持。

用法：
    python make-icon-from-image.py --src setup.jpg --out app.ico --threshold 60
"""
import argparse
import io
import struct
import sys

from PIL import Image, ImageDraw

try:
    import numpy as np
except ImportError:
    print("需要 numpy", file=sys.stderr)
    sys.exit(2)

SIZES = [16, 24, 32, 48, 64, 128, 256]
SENTINEL = (255, 0, 255)          # 洪水填充的哨兵色，之后按它挖 alpha


def remove_background(im, threshold):
    """从四角洪水填充，返回 (RGBA 图, 背景像素数)。透明处 RGB 一并置 0。"""
    rgb = im.convert("RGB")
    w, h = rgb.size
    for seed in [(0, 0), (w - 1, 0), (0, h - 1), (w - 1, h - 1)]:
        if rgb.getpixel(seed) == SENTINEL:
            continue
        ImageDraw.floodfill(rgb, seed, SENTINEL, thresh=threshold)

    arr = np.array(rgb)
    is_bg = ((arr[:, :, 0] == SENTINEL[0]) &
             (arr[:, :, 1] == SENTINEL[1]) &
             (arr[:, :, 2] == SENTINEL[2]))

    out = np.zeros((h, w, 4), dtype=np.uint8)
    out[:, :, :3] = arr
    out[:, :, 3] = np.where(is_bg, 0, 255)
    out[is_bg, 0:3] = 0                     # 透明处 RGB 归零，避免缩放时串色
    return Image.fromarray(out, "RGBA"), int(is_bg.sum())


def alpha_bbox(im):
    """按 alpha 通道算不透明区域的包围盒，返回 (l, t, r, b)；全透明返回 None。"""
    a = np.array(im)[:, :, 3]
    ys, xs = np.nonzero(a)
    if xs.size == 0:
        return None
    return int(xs.min()), int(ys.min()), int(xs.max()) + 1, int(ys.max()) + 1


def to_square(im, margin_ratio):
    """按不透明区域裁边，补成正方形并留边距。"""
    box = alpha_bbox(im)
    if box is None:
        return im, (0, 0, im.size[0], im.size[1])
    left, top, right, bottom = box
    side = max(right - left, bottom - top)
    pad = int(round(side * margin_ratio))
    side += 2 * pad
    cx, cy = (left + right) // 2, (top + bottom) // 2
    canvas = Image.new("RGBA", (side, side), (0, 0, 0, 0))
    canvas.paste(im, (side // 2 - cx, side // 2 - cy), im)
    return canvas, box


def resize_premultiplied(im, size):
    """预乘 alpha 后再缩放，避免透明像素的 RGB 混进边缘形成白边。"""
    arr = np.asarray(im).astype(np.float32)
    a = arr[:, :, 3:4] / 255.0
    pre = np.concatenate([arr[:, :, :3] * a, arr[:, :, 3:4]], axis=2)
    small = Image.fromarray(np.clip(pre, 0, 255).astype(np.uint8), "RGBA")
    small = small.resize((size, size), Image.LANCZOS)

    s = np.asarray(small).astype(np.float32)
    sa = s[:, :, 3:4] / 255.0
    with np.errstate(divide="ignore", invalid="ignore"):
        rgb = np.where(sa > 0, s[:, :, :3] / np.maximum(sa, 1e-6), 0.0)
    return Image.fromarray(
        np.concatenate([np.clip(rgb, 0, 255), s[:, :, 3:4]], axis=2).astype(np.uint8), "RGBA")


def bmp_frame(im):
    """编码成 .ico 里的 BMP(DIB) 帧（32bpp BGRA + AND 掩码）。"""
    w, h = im.size
    px = im.load()
    rows = []
    for y in range(h - 1, -1, -1):          # BMP 自下而上
        row = bytearray()
        for x in range(w):
            r, g, b, a = px[x, y]
            row += bytes((b, g, r, a))
        rows.append(bytes(row))
    xor = b"".join(rows)
    mask_row = ((w + 31) // 32) * 4
    and_mask = b"\x00" * (mask_row * h)      # 32bpp 靠 alpha，掩码全 0
    header = struct.pack("<IiiHHIIiiII", 40, w, h * 2, 1, 32, 0, len(xor), 0, 0, 0, 0)
    return header + xor + and_mask


def png_frame(im):
    buf = io.BytesIO()
    im.save(buf, format="PNG", optimize=True)
    return buf.getvalue()


def write_ico(frames, path):
    n = len(frames)
    offset = 6 + 16 * n
    entries = bytearray()
    blobs = bytearray()
    for size, blob in frames:
        dim = 0 if size >= 256 else size     # 256 在目录项里记作 0
        entries += struct.pack("<BBBBHHII", dim, dim, 0, 0, 1, 32, len(blob), offset)
        offset += len(blob)
        blobs += blob
    with open(path, "wb") as f:
        f.write(struct.pack("<HHH", 0, 1, n))
        f.write(entries)
        f.write(blobs)


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--src", required=True)
    ap.add_argument("--out", required=True)
    ap.add_argument("--threshold", type=int, default=60,
                    help="洪水填充容差（0-255）。背景阴影越重，需要越大。")
    ap.add_argument("--margin", type=float, default=0.04,
                    help="正方形画布四周留白比例，默认 0.04")
    ap.add_argument("--report", action="store_true",
                    help="只打印统计，不写 .ico（用于调容差）")
    args = ap.parse_args()

    src = Image.open(args.src)
    print(f"源图      : {src.size[0]}x{src.size[1]} {src.mode}")

    rgba, bg = remove_background(src, args.threshold)
    total = rgba.size[0] * rgba.size[1]
    print(f"背景占比  : {bg * 100.0 / total:.1f}%  ({bg:,} / {total:,})")

    square, box = to_square(rgba, args.margin)
    bw, bh = box[2] - box[0], box[3] - box[1]
    print(f"物体包围盒: {bw}x{bh}  (占原图 {bw * 100.0 / src.size[0]:.1f}% 宽)")
    print(f"裁边补方  : {square.size[0]}x{square.size[1]}")

    if bg == 0:
        print("警告: 一个背景像素都没去掉，容差太小", file=sys.stderr)
    elif bg > total * 0.95:
        print("警告: 去掉了 95% 以上，容差太大，图形可能被吃掉", file=sys.stderr)

    if args.report:
        return

    frames = []
    for size in SIZES:
        small = resize_premultiplied(square, size)
        frames.append((size, png_frame(small) if size >= 256 else bmp_frame(small)))

    write_ico(frames, args.out)
    print(f"已写出    : {args.out}")
    for size, blob in frames:
        print(f"    {size:>3}x{size:<3}  {len(blob):>7,} B  {'PNG' if size >= 256 else 'BMP'}")


if __name__ == "__main__":
    main()
