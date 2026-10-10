import json, collections
c = collections.Counter(); fam = collections.Counter(); sizes = collections.Counter()
for k in ['p1','p2','p3']:
    d = json.loads(open(f'scratch/out/{k}.json', encoding='utf-8').read())
    def walk(n):
        if 'fontSize' in n:
            c[n.get('lineHeight')] += 1
            fam[n.get('fontFamily')] += 1
            sizes[n.get('fontSize')] += 1
        for ch in n.get('children', []) or []:
            walk(ch)
    walk(d[0]['layer_data'])
print('lineHeight:', dict(c))
print('fontFamily:', dict(fam))
print('fontSize:', dict(sorted(sizes.items())))
