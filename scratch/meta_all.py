import json, xml.etree.ElementTree as ET

raw = open('scratch/p1.meta.json', encoding='utf-8-sig').read()
xml = json.loads(json.loads(raw)['content'][0]['text'])['result']
root = ET.fromstring(xml)

lines = []
def show(el, depth=0):
    lines.append('  '*depth + f"<{el.tag}> name={el.get('name')!r} x={el.get('x')} y={el.get('y')} w={el.get('w')} h={el.get('h')}")
    for c in el:
        show(c, depth+1)
show(root)
open('scratch/out/meta_p1.txt','w',encoding='utf-8').write('\n'.join(lines))
print(len(lines))
