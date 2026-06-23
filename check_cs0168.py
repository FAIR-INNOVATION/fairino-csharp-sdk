import re
from collections import defaultdict

with open(r'd:\C#SDK\各版本sdk\fairino-csharp_-sdk-3.9.7\build_warn.txt', 'rb') as f:
    raw = f.read()
text = raw.decode('utf-8', errors='replace')

cs0168 = []
for line in text.split('\n'):
    if 'CS0168' in line:
        m = re.match(r'\s*(.+?)\((\d+),\d+\):\s*warning CS0168:\s*(.+?)\[', line)
        if m:
            fpath = m.group(1).strip()
            line_no = int(m.group(2))
            msg = m.group(3).strip()
            cs0168.append((fpath, line_no, msg))

print(f'Total CS0168 warnings: {len(cs0168)}')

by_file = defaultdict(list)
for fpath, line_no, msg in cs0168:
    parts = fpath.replace('\\', '/').split('/')
    fname = parts[-1]
    by_file[fname].append((line_no, msg))

for fname in sorted(by_file.keys()):
    entries = by_file[fname]
    print(f'\n{fname}: {len(entries)} occurrences')
    unique_lines = sorted(set(e[0] for e in entries))
    print(f'  Unique lines: {unique_lines[:20]}')
    for ln, msg in entries[:3]:
        print(f'  L{ln}: {msg}')
