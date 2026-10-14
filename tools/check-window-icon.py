import ctypes, ctypes.wintypes as wt, subprocess, sys, time, hashlib
import numpy as np

u = ctypes.windll.user32; g = ctypes.windll.gdi32; sh = ctypes.windll.shell32
WM_GETICON = 0x7F
ICON_SMALL, ICON_BIG, ICON_SMALL2 = 0, 1, 2
GCLP_HICON, GCLP_HICONSM = -14, -34

u.SendMessageW.argtypes = [wt.HWND, wt.UINT, wt.WPARAM, wt.LPARAM]; u.SendMessageW.restype = ctypes.c_void_p
u.GetClassLongPtrW.argtypes = [wt.HWND, ctypes.c_int]; u.GetClassLongPtrW.restype = ctypes.c_void_p
u.GetIconInfo.argtypes = [ctypes.c_void_p, ctypes.c_void_p]; u.GetIconInfo.restype = wt.BOOL
g.GetObjectW.argtypes = [ctypes.c_void_p, ctypes.c_int, ctypes.c_void_p]; g.GetObjectW.restype = ctypes.c_int
g.DeleteObject.argtypes = [ctypes.c_void_p]; g.DeleteObject.restype = wt.BOOL
g.GetDIBits.argtypes = [ctypes.c_void_p, ctypes.c_void_p, wt.UINT, wt.UINT, ctypes.c_void_p, ctypes.c_void_p, wt.UINT]
g.GetDIBits.restype = ctypes.c_int
sh.ExtractIconExW.argtypes = [wt.LPCWSTR, ctypes.c_int, ctypes.c_void_p, ctypes.c_void_p, wt.UINT]
sh.ExtractIconExW.restype = wt.UINT

class ICONINFO(ctypes.Structure):
    _fields_ = [("fIcon", wt.BOOL), ("xHotspot", wt.DWORD), ("yHotspot", wt.DWORD),
                ("hbmMask", ctypes.c_void_p), ("hbmColor", ctypes.c_void_p)]
class BITMAP(ctypes.Structure):
    _fields_ = [("bmType", wt.LONG), ("bmWidth", wt.LONG), ("bmHeight", wt.LONG),
                ("bmWidthBytes", wt.LONG), ("bmPlanes", wt.WORD), ("bmBitsPixel", wt.WORD),
                ("bmBits", ctypes.c_void_p)]
class BMIH(ctypes.Structure):
    _fields_ = [("biSize", wt.DWORD), ("biWidth", wt.LONG), ("biHeight", wt.LONG),
                ("biPlanes", wt.WORD), ("biBitCount", wt.WORD), ("biCompression", wt.DWORD),
                ("biSizeImage", wt.DWORD), ("biXPelsPerMeter", wt.LONG), ("biYPelsPerMeter", wt.LONG),
                ("biClrUsed", wt.DWORD), ("biClrImportant", wt.DWORD)]

def hicon_info(hicon):
    if not hicon: return "NULL(无)"
    ii = ICONINFO()
    if not u.GetIconInfo(hicon, ctypes.byref(ii)): return "GetIconInfo 失败"
    try:
        bm = BITMAP()
        if not g.GetObjectW(ii.hbmColor, ctypes.sizeof(bm), ctypes.byref(bm)): return "GetObject 失败"
        w_, h_ = bm.bmWidth, bm.bmHeight
        hdr = BMIH(); hdr.biSize = ctypes.sizeof(BMIH)
        hdr.biWidth = w_; hdr.biHeight = -h_; hdr.biPlanes = 1; hdr.biBitCount = 32
        buf = ctypes.create_string_buffer(w_*h_*4)
        hdc = u.GetDC(None); g.GetDIBits(hdc, ii.hbmColor, 0, h_, buf, ctypes.byref(hdr), 0); u.ReleaseDC(None, hdc)
        arr = np.frombuffer(buf, dtype=np.uint8).reshape(h_, w_, 4)
        rgba = np.ascontiguousarray(arr[:, :, [2,1,0,3]])
        return f"{w_}x{h_}  sha={hashlib.sha256(rgba.tobytes()).hexdigest()[:16]}  不透明={100.0*(rgba[:,:,3]>0).mean():.1f}%"
    finally:
        if ii.hbmColor: g.DeleteObject(ii.hbmColor)
        if ii.hbmMask: g.DeleteObject(ii.hbmMask)

exe = sys.argv[1]
p = subprocess.Popen([exe])
hwnds = []
CB = ctypes.WINFUNCTYPE(ctypes.c_bool, wt.HWND, wt.LPARAM)
def cb(hwnd, lp):
    pid = wt.DWORD(); u.GetWindowThreadProcessId(hwnd, ctypes.byref(pid))
    if pid.value == p.pid: hwnds.append(hwnd)
    return True

for _ in range(30):
    hwnds.clear()
    u.EnumWindows(CB(cb), 0)
    if hwnds: break
    time.sleep(1)

print(f"进程 {p.pid}  窗口 {len(hwnds)} 个（含不可见）")
for hwnd in hwnds:
    n = u.GetWindowTextLengthW(hwnd)
    buf = ctypes.create_unicode_buffer(n+1); u.GetWindowTextW(hwnd, buf, n+1)
    vis = bool(u.IsWindowVisible(hwnd))
    print(f"\n窗口 hwnd={hwnd}  可见={vis}  标题='{buf.value}'")
    for label, which in (("ICON_BIG", ICON_BIG), ("ICON_SMALL", ICON_SMALL), ("ICON_SMALL2", ICON_SMALL2)):
        print(f"  WM_GETICON {label:11}: {hicon_info(u.SendMessageW(hwnd, WM_GETICON, which, 0))}")
    for label, idx in (("GCLP_HICON", GCLP_HICON), ("GCLP_HICONSM", GCLP_HICONSM)):
        print(f"  {label:19}: {hicon_info(u.GetClassLongPtrW(hwnd, idx))}")

large = (ctypes.c_void_p*1)(); small = (ctypes.c_void_p*1)()
sh.ExtractIconExW(exe, 0, large, small, 1)
print(f"\nexe 资源 32x32 : {hicon_info(large[0])}")
print(f"exe 资源 16x16 : {hicon_info(small[0])}")
p.kill()