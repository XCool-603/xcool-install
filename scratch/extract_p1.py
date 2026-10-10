import json, io

d = json.load(open('scratch/out/p1.json', encoding='utf-8'))
root = d[0]['layer_data']

out = io.StringIO()

def walk(n, depth=0, ax=0.0, ay=0.0):
    if not isinstance(n, dict):
        return
    x = n.get('x')
    y = n.get('y')
    # absolute origin = parent's absolute origin + own x/y (x/y are relative to parent)
    absx = ax + (x if isinstance(x, (int, float)) else 0)
    absy = ay + (y if isinstance(y, (int, float)) else 0)
    ind = '  ' * depth
    out.write(f"{ind}{n.get('type')} | {n.get('name')!r} | id={n.get('id')}\n")
    out.write(f"{ind}   rel=({x},{y}) abs=({absx},{absy}) size=({n.get('width')},{n.get('height')})\n")
    skip = {'type', 'name', 'id', 'x', 'y', 'width', 'height', 'children'}
    for k, v in n.items():
        if k in skip:
            continue
        out.write(f"{ind}   .{k} = {json.dumps(v, ensure_ascii=False)}\n")
    for c in n.get('children', []) or []:
        walk(c, depth + 1, absx, absy)

walk(root)
open('scratch/out/p1_full.txt', 'w', encoding='utf-8').write(out.getvalue())
print('bytes', len(out.getvalue()))
