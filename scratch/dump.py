import json, sys, io

ORDER = ['type','name','id','width','height','x','y','layout','padding','gap','alignItems','justifyContent',
         'cornerRadius','fills','stroke','fontFamily','fontSize','lineHeight','fontFill','textAlign',
         'textAlignVertical','opacity','effects','content']

def fmt(v):
    if isinstance(v, str) and len(v) > 60:
        return json.dumps(v, ensure_ascii=False)
    return json.dumps(v, ensure_ascii=False)

out = io.StringIO()

def walk(n, depth=0):
    ind = '  ' * depth
    if not isinstance(n, dict):
        return
    head = f"{ind}[{n.get('type')}] {n.get('name')}"
    out.write(head + '\n')
    parts = []
    for k in ORDER:
        if k in ('type','name'):
            continue
        if k in n and n[k] not in (None, '', [], {}):
            parts.append(f"{k}={fmt(n[k])}")
    if parts:
        out.write(ind + '    ' + ' | '.join(parts) + '\n')
    for c in n.get('children', []) or []:
        walk(c, depth+1)

for k in ['p1','p2','p3']:
    d = json.load(open(f'scratch/out/{k}.json', encoding='utf-8'))
    root = d[0]['layer_data']
    out.write('='*100 + '\n' + f'### {k}  root={root.get("name")} {root.get("width")}x{root.get("height")}\n' + '='*100 + '\n')
    walk(root)

open('scratch/out/tree.txt','w',encoding='utf-8').write(out.getvalue())
print('ok', len(out.getvalue()))
