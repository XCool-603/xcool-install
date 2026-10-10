import json, xml.etree.ElementTree as ET

WANT = {'路径输入行','路径输入框','浏览按钮','快捷目录列表','目录标签-Program Files','任务行1','任务-桌面快捷方式','汇总条','选项组','磁盘信息行','分隔线','区块-安装位置','内容区','同意条款行','协议正文框','勾选框'}
for k in ['p2','p3']:
    raw = open(f'scratch/{k}.meta.json', encoding='utf-8-sig').read()
    xml = json.loads(json.loads(raw)['content'][0]['text'])['result']
    root = ET.fromstring(xml)
    out = []
    def show(el, depth=0):
        if el.get('name') in WANT:
            out.append('  '*depth + f"<{el.tag}> name={el.get('name')!r} x={el.get('x')} y={el.get('y')} w={el.get('w')} h={el.get('h')}")
        for c in el:
            show(c, depth+1)
    show(root)
    open(f'scratch/out/meta_{k}.txt','w',encoding='utf-8').write('\n'.join(out))
    print(k, len(out))
