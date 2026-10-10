import json, io, re
import xml.etree.ElementTree as ET

# ---------- meta geometry ----------
raw = open('scratch/p1.meta.json', encoding='utf-8-sig').read()
obj = json.loads(json.loads(raw)['content'][0]['text'])
root_xml = ET.fromstring(obj['result'])

def num(v):
    try:
        return float(v)
    except (TypeError, ValueError):
        return None

meta = {}
def walk_meta(el, ptl, depth=0):
    for ch in el:
        cid, w, h = ch.get('id'), num(ch.get('w')), num(ch.get('h'))
        cx, cy = num(ch.get('x')), num(ch.get('y'))
        if cid and None not in (w, h, cx, cy):
            tl = (cx - w / 2.0, cy - h / 2.0) if ptl is None else \
                 (ptl[0] + cx - w / 2.0, ptl[1] + cy - h / 2.0)
            meta[cid] = dict(x=tl[0], y=tl[1], w=w, h=h, depth=depth)
            walk_meta(ch, tl, depth + 1)
        else:
            walk_meta(ch, ptl, depth)
walk_meta(root_xml, None)

PAGE = 'c4e35ccf-10d2-4dd8-bdde-e05bdf830f31'
OX, OY = meta[PAGE]['x'], meta[PAGE]['y']

# ---------- design tree ----------
design = json.load(open('scratch/out/p1.json', encoding='utf-8'))[0]['layer_data']
by_id, parent_of = {}, {}
def walk_d(n, p=None):
    by_id[n['id']] = n
    parent_of[n['id']] = p
    for c in n.get('children', []) or []:
        walk_d(c, n['id'])
walk_d(design)

def hexof(s):
    if not isinstance(s, str):
        return s
    s = s.strip()
    if s.startswith('#'):
        return s.upper()
    if s.startswith('rgba'):
        p = s[s.index('(') + 1:s.index(')')].split(',')
        r, g, b = (int(float(x)) for x in p[:3])
        a = float(p[3]) if len(p) > 3 else 1.0
        hx = '#%02X%02X%02X' % (r, g, b)
        return hx if a >= 1 else f'{hx} {int(round(a*100))}%'
    return s

WEIGHT = {'SourceHanSans-Regular': 'Regular', 'SourceHanSans-SemiBold': 'SemiBold',
          'SourceHanSans-Bold': 'Bold', 'remixicon': '图标字体'}

def pad(p):
    if p is None:
        return '0'
    if isinstance(p, (int, float)):
        return f'{p:g}'
    return ' '.join(f'{v:g}' for v in p)

def absbox(lid):
    n = by_id[lid]
    if lid in meta:
        return (meta[lid]['x'] - OX, meta[lid]['y'] - OY, meta[lid]['w'], meta[lid]['h'])
    pid = parent_of[lid]
    px, py, _, _ = absbox(pid)
    return (px + n['x'], py + n['y'], n['width'], n['height'])

def fm(v):
    return '未找到' if v is None else (f'{v:g}' if isinstance(v, float) else str(v))

ROWS = [
    ('页面根 空中小火车-安装向导-欢迎 3', PAGE, '页面背景；clip=true；纵向 auto-layout，align=start'),
    ('外层卡片 安装向导窗口', '58df16f7-c9c1-4b11-ba72-0451d36f51ab', '纵向 auto-layout；clip=true'),
    ('品牌横幅 顶部横幅', 'a83b11b6-8fca-4b11-a307-9c658f8e2404', '横向 auto-layout，align=center；自身圆角 0，上两角视觉圆角由父卡片 clip 产生'),
    ('应用图标容器', '98a01da5-16b7-4fde-ad80-07f5bc6b3086', '居中容器'),
    ('应用图标（等距立方体）', 'f6cd6a12-ef39-4bd8-976f-b6957a0071f0', 'clip=true；由 3 个 SVG path 组成'),
    ('  矢量-顶面', 'c02f3d46-4512-4fc6-bce1-55df06cfd9a2', 'fill #FFFFFF，opacity 0.95'),
    ('  矢量-左面', '92214b6f-1a1a-4cb1-b18a-e5fcb4a492d5', 'fill #FFFFFF，opacity 0.72'),
    ('  矢量-右面', '0755eef4-224c-47da-b8f1-3672e6654028', 'fill #FFFFFF，opacity 0.52'),
    ('标题区', '830c0140-5a32-4bba-a93e-79624be33103', '纵向 auto-layout'),
    ('产品名', '7950444c-6498-4937-b2f3-1bab35739229', 'textAlign=left，垂直居中'),
    ('副标题容器', 'bcb2d191-55d3-4949-989e-c2eca41ce4c0', '纵向 auto-layout'),
    ('副标题', '78bfdbaa-4baf-435e-816f-42f239784ef4', 'textAlign=left，垂直居中'),
    ('步骤条', 'b9518d9f-9033-4950-aae6-27cae9dd3796', '横向 auto-layout，align=center；无 gap，靠 padding-left 12 的容器拉开间距'),
    ('步骤1 圆点 步骤标记', '71ce200b-055f-4280-9746-d55133d27915', '实心蓝底，无描边'),
    ('步骤1 数字', '31cfef7f-d608-44fa-ad79-38077b77c59e', '居中'),
    ('步骤1 文字 欢迎', '0b0bf707-c413-486e-8d40-1115cdc892e1', '当前步骤高亮'),
    ('步骤2 圆点 步骤标记', 'e5fc4b45-b33b-499c-a20f-e0d323dda5ce', '白底'),
    ('步骤2 数字', 'dde5c531-2849-4737-a944-049432b02f87', '居中'),
    ('步骤2 文字 许可协议', '70308204-f587-4d68-809c-c95206f23998', '未激活'),
    ('步骤3 圆点 步骤标记', 'f9dea2ed-efba-48f5-a510-31c07b072638', '白底'),
    ('步骤3 数字', 'f0627b52-332d-449c-9fe0-0edc327db832', '居中'),
    ('步骤3 文字 安装设置', '280879c5-aa74-4cf5-9db5-4cb4b73e6eaa', '未激活'),
    ('步骤4 圆点 步骤标记', 'e08a9d2c-a397-4ca0-836d-d368fc7403eb', '白底'),
    ('步骤4 数字', '23331bba-3fec-44f3-a559-761dd75f646f', '居中'),
    ('步骤4 文字 正在安装', 'eb26f9b7-d710-4a94-8900-0cc3b0dc1c08', '未激活'),
    ('步骤5 圆点 步骤标记', '49284dc9-1463-4eb7-aeff-c9425d7f2ec3', '白底'),
    ('步骤5 数字', '094c801f-7749-4578-99b5-596d234f081e', '居中'),
    ('步骤5 文字 完成', 'c949fb87-d867-48ba-a304-290cbbae4e01', '未激活'),
    ('连接线1', '34c7ea69-0af3-4d67-b5fb-ca7f8dcd67e1', '父容器 padding-left=12；渲染值比布局推导值右移 0.625px（推导 x=127）'),
    ('连接线2', 'b5c6eb8a-c199-49b2-a0d4-683dbd91a370', '父容器 padding-left=12；渲染值比布局推导值右移 0.625px（推导 x=263）'),
    ('连接线3', 'f53bc5ba-e300-4e2d-80dc-09da88d6457d', '父容器 padding-left=12；渲染值比布局推导值右移 0.625px（推导 x=398.5）'),
    ('连接线4', 'c9fab7c8-dd62-4140-a724-55f67c8b4f3e', '父容器 padding-left=12；渲染值比布局推导值右移 0.625px（推导 x=534.25）'),
    ('内容区', 'f9b69d3a-b718-41f1-9510-c44f77a053be', '纵向 auto-layout，padding=32'),
    ('页面标题', '5a3942bd-e577-4b54-bf20-440f23635e92', 'textAlign=left，垂直居中'),
    ('正文段1 容器', 'ecd0db67-21e6-46d8-a201-87c32521cd26', '纵向 auto-layout'),
    ('正文段1', '3d8f1890-7f5b-461c-8313-2d8e151118c7', '单行；lineHeight 1.2 → 15.6px'),
    ('正文段2 容器', 'c1a5ea11-6e24-4eec-9760-52aa8d7eaabc', '纵向 auto-layout'),
    ('正文段2', '8c5257ba-436a-4078-a5db-63e1f9dbea2f', '单行；lineHeight 1.2 → 15.6px'),
    ('信息卡外层容器', 'cdf3e99a-736a-41bb-9b83-360d0d16f2a5', '纵向 auto-layout'),
    ('信息卡', 'f614b0b3-c650-40c9-ba4e-7593337195f5', '纵向 auto-layout，padding=20；无描边'),
    ('信息行1 信息行-安装位置', '0f4ebed4-40e2-4b8c-9f9f-cd04cbf27bd0', '横向 auto-layout，align=center'),
    ('信息行1 图标块', 'dcd8f2ff-2be7-4ca9-bee5-ea286a7a8382', '居中容器'),
    ('信息行1 图标', '229c85b4-ef6f-44bb-97a1-c0f33437d780', 'Remixicon ri-folder-line'),
    ('信息行1 文本列', 'f45ecfc0-6e3f-42ed-8668-d1ccdfacfe9f', '纵向 auto-layout'),
    ('信息行1 标签 默认安装位置', 'b574f727-bfd2-45e8-aa6c-885fb13470dc', ''),
    ('信息行1 值 C:\\Program Files\\空中小火车', 'be4e7a98-1db1-465c-98a4-ffa596ff59b9', ''),
    ('信息行2 外层容器', '51b685a2-ed42-4e9b-a264-f17f67257e0a', '纵向 auto-layout，padding-top 16 → 行间距 16'),
    ('信息行2 信息行-空间', 'b4773e51-921a-4ae5-949d-1dacd4133367', '横向 auto-layout，align=center'),
    ('信息行2 图标块', '9eb7548f-bc79-4f5a-91d9-f3ec25eaa9f3', '居中容器'),
    ('信息行2 图标', '1c71b251-f4eb-43a9-9b0f-180f6824902d', 'Remixicon ri-hard-drive-2-line'),
    ('信息行2 文本列', 'a53f8b35-9fc4-46e9-a6a2-3d1796275f6f', '纵向 auto-layout'),
    ('信息行2 标签 需要磁盘空间', '52ba9d62-d67b-4870-90bb-d8b862504cca', ''),
    ('信息行2 值 90.6 MB（本机可用 128.4 GB）', 'a1633806-e97d-4325-a21f-663e49e63747', ''),
    ('信息行3 外层容器', 'cfd2c3ca-1bfd-4d3d-93f8-3ff97750d00a', '纵向 auto-layout，padding-top 16 → 行间距 16'),
    ('信息行3 信息行-版本', 'a06b2b89-0b22-4cca-92c2-97fc9098bf3e', '横向 auto-layout，align=center'),
    ('信息行3 图标块', 'd027b230-74a2-4ecc-afce-daacea7f73cd', '居中容器'),
    ('信息行3 图标', '40ccb830-116f-4a76-b682-76db85f295bd', 'Remixicon ri-price-tag-3-line'),
    ('信息行3 文本列', '1abab5e2-a31d-4741-8824-62d69f0c9cc2', '纵向 auto-layout'),
    ('信息行3 标签 版本与发布者', 'fb05c3dc-0a7e-4320-a448-01528f529e1f', ''),
    ('信息行3 值 1.0.0 · 空中小火车团队', 'a34db3a6-fe62-44dd-9b9e-2e7541e37222', ''),
    ('提示条外层容器', '36a38944-0091-4a18-b53a-3aa6d2cc4e88', '纵向 auto-layout'),
    ('提示条 下一步提示条', '0ceeb3d6-6e50-4e8a-bc83-7923f932b0c0', '横向 auto-layout，align=center；无描边'),
    ('提示条 ⓘ 图标', '76e4f16b-cbf2-4c8d-98aa-b641385fa56a', 'Remixicon ri-information-line'),
    ('提示条 文字', '3bdf86ad-64f8-4736-8b25-218c1296ce27', ''),
    ('底部按钮栏', '7db07b7b-82d6-4e14-802f-191cf352d0cf', '横向 auto-layout，justify=space-between，align=center'),
    ('底栏左侧步骤文字', 'd5b0c1a8-25b2-4862-af82-f973680c238c', ''),
    ('按钮组', '2fe9a540-62a3-46fd-9f8d-9f2ab853dd22', '横向 auto-layout'),
    ('取消按钮', '48c10a51-7b87-4a60-a135-746e0ad60087', '白底 + 1px 灰边'),
    ('取消按钮文字', '5cf93dbf-2e19-41c6-92de-18c0df5bc191', '居中'),
    ('下一步按钮', '6867cf05-4658-49a8-8924-9b8e9325b9d7', '蓝色主按钮'),
    ('下一步按钮文字', '3c205cac-b9d0-4728-b287-ec559bc029b5', '居中'),
    ('下一步箭头图标', 'e0504505-bfab-40a3-9dc0-8a0d38bb42b3', 'Remixicon ri-arrow-right-line'),
]

out = io.StringIO()
out.write('| 元素名 | 绝对x | 绝对y | w | h | 文案 | 字号 | 字重 | 文字色 | 底色 | 圆角 | 内边距 | 备注 |\n')
out.write('|---|---|---|---|---|---|---|---|---|---|---|---|---|\n')

for label, lid, note in ROWS:
    n = by_id[lid]
    x, y, w, h = absbox(lid)
    content = n.get('content', '')
    fs = n.get('fontSize')
    fam = n.get('fontFamily')
    weight = WEIGHT.get(fam, '—') if fam else '—'
    tcol = n.get('fontFill') or n.get('fills')
    if not isinstance(tcol, str):
        tcol = None
    bg = n.get('fills') if isinstance(n.get('fills'), str) else None
    if content:
        bg = None
    radius = n.get('cornerRadius')
    notes = [note]
    st = n.get('stroke')
    if isinstance(st, dict):
        notes.append(f"描边 {st.get('thickness'):g}px {hexof(st.get('fills'))}")
    for e in n.get('effects', []) or []:
        notes.append(f"阴影 x{e['offsetX']:g} y{e['offsetY']:g} blur{e['blur']:g} {hexof(e['color'])}")
    if n.get('gap') is not None and not any('gap' in s for s in notes):
        notes.append(f"gap {n['gap']:g}")
    seen_n, uniq = set(), []
    for s in notes:
        if s and s not in seen_n:
            seen_n.add(s)
            uniq.append(s)

    if n.get('type') == 'svg':
        d = re.search(r'd="([^"]+)"', content)
        disp = f"SVG path d={d.group(1)!r}" if d else '（SVG 路径）'
    elif fam == 'remixicon' and content:
        disp = f"U+{ord(content):04X}"
    else:
        disp = content if content else '—'
    if not content:
        tcol = None

    row = [label, f'{x:g}', f'{y:g}', fm(w), fm(h), disp,
           fm(fs) if fs else '—', weight,
           hexof(tcol) if tcol else '—',
           hexof(bg) if bg else 'transparent',
           fm(radius) if radius is not None else '0',
           pad(n.get('padding')), '；'.join(uniq)]
    out.write('| ' + ' | '.join(row) + ' |\n')

open('scratch/out/p1_table.md', 'w', encoding='utf-8').write(out.getvalue())
print('rows', len(ROWS))

# ---------- validation: child box inside parent box ----------
bad = 0
def check(n, path=''):
    global bad
    p = path + '/' + str(n.get('name'))
    if n['id'] in meta:
        px, py, pw, ph = absbox(n['id'])
        for c in n.get('children', []) or []:
            if c['id'] in meta:
                cx, cy, cw, ch = absbox(c['id'])
                if cx < px - 0.6 or cy < py - 0.6 or cx + cw > px + pw + 0.6 or cy + ch > py + ph + 0.6:
                    print(f'  OVERFLOW {p}/{c.get("name")} child=({cx:g},{cy:g},{cw:g},{ch:g}) parent=({px:g},{py:g},{pw:g},{ph:g})')
                    bad += 1
    for c in n.get('children', []) or []:
        check(c, p)
check(design)
print('overflows:', bad)
