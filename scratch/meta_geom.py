import json, io, re
import xml.etree.ElementTree as ET

# ---- load meta XML ----
raw = open('scratch/p1.meta.json', encoding='utf-8-sig').read()
outer = json.loads(raw)
obj = json.loads(outer['content'][0]['text'])
xml = obj['result']
root = ET.fromstring(xml)

meta = {}   # id -> dict
order = []

def num(v):
    if v is None:
        return None
    try:
        return float(v)
    except ValueError:
        return None

def walk_meta(el, parent_id, ptl, depth=0):
    for ch in el:
        cid = ch.get('id')
        w = num(ch.get('w'))
        h = num(ch.get('h'))
        cx = num(ch.get('x'))
        cy = num(ch.get('y'))
        if cid is None:
            continue
        if w is None or h is None or cx is None or cy is None:
            # container without geometry; recurse with same parent
            walk_meta(ch, parent_id, ptl, depth)
            continue
        # meta x/y == element CENTER relative to parent top-left
        if ptl is None:
            tl = (cx - w / 2.0, cy - h / 2.0)
        else:
            tl = (ptl[0] + cx - w / 2.0, ptl[1] + cy - h / 2.0)
        meta[cid] = dict(tag=ch.tag, name=ch.get('name'), x=tl[0], y=tl[1],
                         w=w, h=h, depth=depth, parent=parent_id)
        order.append(cid)
        walk_meta(ch, cid, tl, depth + 1)

walk_meta(root, None, None)

# root of meta
r = root[0] if root.tag != 'frame' else root
print('meta root tag', root.tag, root.attrib)
