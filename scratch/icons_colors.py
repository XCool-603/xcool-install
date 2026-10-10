import json

d = json.load(open('scratch/out/p1.json', encoding='utf-8'))
root = d[0]['layer_data']

print('=== icon glyph contents (code points) ===')
def walk(n, path=''):
    p = path + '/' + str(n.get('name'))
    ff = n.get('fontFamily', '')
    c = n.get('content')
    if c is not None and ff == 'remixicon':
        print(f"{p!r:70} fontSize={n.get('fontSize')} color={n.get('fills') or n.get('fontFill')} "
              f"chars={[hex(ord(ch)) for ch in c]} len={len(c)}")
    for ch in n.get('children', []) or []:
        walk(ch, p)
walk(root)

print()
print('=== distinct colors -> hex ===')
def hexof(s):
    s = s.strip()
    if s.startswith('#'):
        return s.upper()
    if s.startswith('rgba'):
        parts = s[s.index('(')+1:s.index(')')].split(',')
        r, g, b = (int(float(x)) for x in parts[:3])
        a = float(parts[3]) if len(parts) > 3 else 1.0
        h = '#%02X%02X%02X' % (r, g, b)
        return h if a >= 1 else f'{h} @{int(round(a*100))}%'
    return s

seen = set()
def walk2(n):
    for k in ('fills', 'fontFill'):
        v = n.get(k)
        if isinstance(v, str):
            seen.add(v)
    st = n.get('stroke')
    if isinstance(st, dict) and isinstance(st.get('fills'), str):
        seen.add(st['fills'])
    for e in n.get('effects', []) or []:
        if isinstance(e, dict) and 'color' in e:
            seen.add(e['color'])
    for ch in n.get('children', []) or []:
        walk2(ch)
walk2(root)
for s in sorted(seen):
    print(f'  {s:32} -> {hexof(s)}')

print()
print('=== distinct fontFamily ===')
fams = set()
def walk3(n):
    if n.get('fontFamily'):
        fams.add(n['fontFamily'])
    for ch in n.get('children', []) or []:
        walk3(ch)
walk3(root)
print(' ', sorted(fams))
