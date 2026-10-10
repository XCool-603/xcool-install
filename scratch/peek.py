import json

d = json.load(open('scratch/out/p1.json', encoding='utf-8'))
print('type', type(d), 'len', len(d))
root = d[0]
print('root type', type(root))
if isinstance(root, dict):
    for kk, vv in root.items():
        s = json.dumps(vv, ensure_ascii=False)
        print(f'  {kk}: {type(vv).__name__} {s[:200]}')
