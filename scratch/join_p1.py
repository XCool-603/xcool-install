import json, io
import xml.etree.ElementTree as ET

# ================= meta geometry =================
raw = open('scratch/p1.meta.json', encoding='utf-8-sig').read()
outer = json.loads(raw)
obj = json.loads(outer['content'][0]['text'])
root_xml = ET.fromstring(obj['result'])

def num(v):
    try:
        return float(v)
    except (TypeError, ValueError):
        return None

meta = {}
order = []

def walk_meta(el, ptl, depth=0):
    for ch in el:
        cid, w, h = ch.get('id'), num(ch.get('w')), num(ch.get('h'))
        cx, cy = num(ch.get('x')), num(ch.get('y'))
        if cid and None not in (w, h, cx, cy):
            tl = (cx - w / 2.0, cy - h / 2.0) if ptl is None else \
                 (ptl[0] + cx - w / 2.0, ptl[1] + cy - h / 2.0)
            meta[cid] = dict(tag=ch.tag, name=ch.get('name'), x=tl[0], y=tl[1],
                             w=w, h=h, depth=depth)
            order.append(cid)
            walk_meta(ch, tl, depth + 1)
        else:
            walk_meta(ch, ptl, depth)

walk_meta(root_xml, None)

# page origin = tl of the page frame (id c4e35ccf...)
PAGE_ID = 'c4e35ccf-10d2-4dd8-bdde-e05bdf830f31'
ox, oy = meta[PAGE_ID]['x'], meta[PAGE_ID]['y']
print(f'page origin (canvas abs) = ({ox}, {oy})  size=({meta[PAGE_ID]["w"]},{meta[PAGE_ID]["h"]})')

# ================= design tree =================
d = json.load(open('scratch/out/p1.json', encoding='utf-8'))
design = d[0]['layer_data']

out = io.StringIO()
missing = []

def fmt_rgba(s):
    return s

def walk(n, depth=0):
    cid = n.get('id')
    m = meta.get(cid)
    ind = '  ' * depth
    if m:
        px, py = m['x'] - ox, m['y'] - oy
        geo = f"page=({px:.1f},{py:.1f}) size=({m['w']:g},{m['h']:g})"
    else:
        missing.append((n.get('name'), n.get('type'), cid))
        geo = 'page=(NOT_IN_META)'
    out.write(f"{ind}<{n.get('type')}> {n.get('name')!r}  {geo}\n")
    for k, v in n.items():
        if k in ('type', 'name', 'id', 'children', 'x', 'y'):
            continue
        out.write(f"{ind}    .{k} = {json.dumps(v, ensure_ascii=False)}\n")
    for c in n.get('children', []) or []:
        walk(c, depth + 1)

walk(design)
out.write('\n=== design nodes missing from meta ===\n')
for nm, ty, cid in missing:
    out.write(f'  {ty} | {nm!r} | {cid}\n')
out.write(f'\nmeta nodes: {len(meta)}  design nodes missing: {len(missing)}\n')

open('scratch/out/p1_join.txt', 'w', encoding='utf-8').write(out.getvalue())
print('written', len(out.getvalue()))
