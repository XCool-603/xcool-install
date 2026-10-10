import json, sys, os

os.makedirs('scratch/out', exist_ok=True)

for k in ['p1', 'p2', 'p3']:
    raw = open(f'scratch/{k}.design.json', 'r', encoding='utf-8-sig').read()
    outer = json.loads(raw)
    inner = outer['content'][0]['text']
    obj = json.loads(inner)
    # obj may be {status, result}
    if isinstance(obj, dict) and 'result' in obj:
        res = obj['result']
        if isinstance(res, str):
            res = json.loads(res)
    else:
        res = obj
    with open(f'scratch/out/{k}.json', 'w', encoding='utf-8') as f:
        json.dump(res, f, ensure_ascii=False, indent=1)
    print(k, type(res).__name__, (list(res.keys())[:20] if isinstance(res, dict) else len(res)))

# also meta
for k in ['p1', 'p2', 'p3']:
    raw = open(f'scratch/{k}.meta.json', 'r', encoding='utf-8-sig').read()
    outer = json.loads(raw)
    inner = outer['content'][0]['text']
    obj = json.loads(inner)
    print(k, 'meta keys:', list(obj.keys())[:10])
