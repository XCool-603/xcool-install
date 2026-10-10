import json, re

for k in ['p1','p2','p3']:
    raw = open(f'scratch/{k}.meta.json', encoding='utf-8-sig').read()
    outer = json.loads(raw)
    inner = json.loads(outer['content'][0]['text'])
    xml = inner['result']
    m = re.search(r'<frame id="([0-9a-f\-]+)" name="([^"]*)" x="([^"]*)" y="([^"]*)" w="([^"]*)" h="([^"]*)"', xml)
    print(k, 'root:', m.groups() if m else None)
    print('   node count in meta:', xml.count('<frame') + xml.count('<text-rect') + xml.count('<rectangle') + xml.count('<paragraph') + xml.count('<svg'))

    d = json.loads(open(f'scratch/out/{k}.json', encoding='utf-8').read())
    root = d[0]['layer_data']
    n = 0
    radii = set(); fills = set(); strokes = set()
    def walk(node):
        global n
        n += 1
        if 'cornerRadius' in node: radii.add(node['cornerRadius'])
        if 'fills' in node: fills.add(json.dumps(node['fills'], ensure_ascii=False))
        if 'stroke' in node: strokes.add(json.dumps(node['stroke'].get('fills'), ensure_ascii=False) + f" w={node['stroke'].get('thickness')}")
        for c in node.get('children', []) or []:
            walk(c)
    walk(root)
    print('   design node count:', n, 'root w/h:', root.get('width'), root.get('height'))
    print('   cornerRadius values:', sorted(radii, key=lambda v: (isinstance(v,str), v)))
    print('   fills:', sorted(fills))
    print('   strokes:', sorted(strokes))
