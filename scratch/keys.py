import json, collections

d = json.load(open('scratch/out/p1.json', encoding='utf-8'))
root = d[0]['layer_data']

keys = collections.Counter()
def walk(n, depth=0):
    if not isinstance(n, dict):
        return
    for k in n.keys():
        keys[k] += 1
    for c in n.get('children', []) or []:
        walk(c, depth+1)

walk(root)
for k, v in sorted(keys.items()):
    print(f'{v:5d}  {k}')
