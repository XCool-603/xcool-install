import json, re
raw = open('scratch/p1.meta.json', encoding='utf-8-sig').read()
xml = json.loads(json.loads(raw)['content'][0]['text'])['result']
ids = {
    'page frame':        'c4e35ccf-10d2-4dd8-bdde-e05bdf830f31',
    'window':            '58df16f7-c9c1-4b11-ba72-0451d36f51ab',
    'banner':            'a83b11b6-8fca-4b11-a307-9c658f8e2404',
    'icon container':    '98a01da5-16b7-4fde-ad80-07f5bc6b3086',
    'app icon':          'f6cd6a12-ef39-4bd8-976f-b6957a0071f0',
    'title area':        '830c0140-5a32-4bba-a93e-79624be33103',
    'product name':      '7950444c-6498-4937-b2f3-1bab35739229',
    'subtitle':          '78bfdbaa-4baf-435e-816f-42f239784ef4',
    'stepbar':           'b9518d9f-9033-4950-aae6-27cae9dd3796',
    'step1 group':       '02f4c786-d509-4f6f-bfbc-4ac7f868ed2f',
    'step1 marker':      '71ce200b-055f-4280-9746-d55133d27915',
    'step1 digit':       '31cfef7f-d608-44fa-ad79-38077b77c59e',
    'step1 label':       '0b0bf707-c413-486e-8d40-1115cdc892e1',
    'line1 container':   '1fdea983-daa1-4834-8d19-90071105dde6',
    'line1':             '34c7ea69-0af3-4d67-b5fb-ca7f8dcd67e1',
    'line2':             'b5c6eb8a-c199-49b2-a0d4-683dbd91a370',
    'content':           'f9b69d3a-b718-41f1-9510-c44f77a053be',
    'pagetitle':         '5a3942bd-e577-4b54-bf20-440f23635e92',
    'infocard':          'f614b0b3-c650-40c9-ba4e-7593337195f5',
    'row1':              '0f4ebed4-40e2-4b8c-9f9f-cd04cbf27bd0',
    'row1 iconbox':      'dcd8f2ff-2be7-4ca9-bee5-ea286a7a8382',
    'row1 icon':         '229c85b4-ef6f-44bb-97a1-c0f33437d780',
    'row1 label':        'b574f727-bfd2-45e8-aa6c-885fb13470dc',
    'row1 value':        'be4e7a98-1db1-465c-98a4-ffa596ff59b9',
    'tipbar':            '0ceeb3d6-6e50-4e8a-bc83-7923f932b0c0',
    'tip icon':          '76e4f16b-cbf2-4c8d-98aa-b641385fa56a',
    'tip text':          '3bdf86ad-64f8-4736-8b25-218c1296ce27',
    'footer':            '7db07b7b-82d6-4e14-802f-191cf352d0cf',
    'cancel btn':        '48c10a51-7b87-4a60-a135-746e0ad60087',
    'cancel text':       '5cf93dbf-2e19-41c6-92de-18c0df5bc191',
    'next btn':          '6867cf05-4658-49a8-8924-9b8e9325b9d7',
    'next text':         '3c205cac-b9d0-4728-b287-ec559bc029b5',
    'next arrow':        'e0504505-bfab-40a3-9dc0-8a0d38bb42b3',
}
print(f"{'node':18} {'tag':10} {'x':>9} {'y':>9} {'w':>8} {'h':>7}")
for name, lid in ids.items():
    m = re.search(r'<([a-z\-]+) id="' + lid + r'"[^>]*?/>', xml) or \
        re.search(r'<([a-z\-]+) id="' + lid + r'"[^>]*?>', xml)
    if not m:
        print(f'{name:18} NOT FOUND')
        continue
    tag = m.group(1)
    g = lambda a: (re.search(a + r'="([^"]*)"', m.group(0)) or [None, '-'])[1]
    print(f"{name:18} {tag:10} {g('x'):>9} {g('y'):>9} {g('w'):>8} {g('h'):>7}")
