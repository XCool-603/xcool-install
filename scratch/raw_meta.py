import json, re
raw = open('scratch/p1.meta.json', encoding='utf-8-sig').read()
xml = json.loads(json.loads(raw)['content'][0]['text'])['result']
ids = [
    '1fdea983-daa1-4834-8d19-90071105dde6',
    '34c7ea69-0af3-4d67-b5fb-ca7f8dcd67e1',
    '774ea819-f24e-4066-b7e4-96d21eb42116',
    '5138918c-04fc-41ac-a640-b4f4684d48fc',
    '02f4c786-d509-4f6f-bfbc-4ac7f868ed2f',
]
for lid in ids:
    m = re.search(r'<[a-z\-]+ id="' + lid + r'"[^>]*>', xml)
    print(m.group(0) if m else lid + '  NOT FOUND')
