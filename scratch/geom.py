import json, re, xml.etree.ElementTree as ET

for k in ['p1']:
    raw = open(f'scratch/{k}.meta.json', encoding='utf-8-sig').read()
    outer = json.loads(raw)
    xml = json.loads(outer['content'][0]['text'])['result']
    root = ET.fromstring(xml)

    def show(el, depth=0):
        name = el.get('name')
        if name in ('步骤条','步骤-欢迎','步骤标记','连接线','container','步骤-许可协议','底部按钮栏','按钮组','取消按钮','下一步按钮','信息卡','内容区','安装向导窗口','顶部横幅','应用图标容器','标题区'):
            print('  '*depth + f"<{el.tag}> {name} x={el.get('x')} y={el.get('y')} w={el.get('w')} h={el.get('h')}")
        for c in el:
            show(c, depth+1)
    show(root)
