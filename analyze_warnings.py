import re

with open(r'd:\C#SDK\各版本sdk\fairino-csharp_-sdk-3.9.7\build_warn.txt', 'rb') as f:
    raw = f.read()
text = raw.decode('utf-8', errors='replace')
lines = text.split('\n')

by_type = {}
for line in lines:
    m = re.search(r'(CS\d{4})', line)
    if m:
        code = m.group(1)
        if code not in by_type:
            by_type[code] = []
        by_type[code].append(line.strip()[:250])

for code in ['CS0219', 'CS0168', 'CS0169', 'CS0162', 'CS0414', 'CS0114']:
    items = by_type.get(code, [])
    print(f'\n=== {code} ({len(items)} occurrences) ===')
    for item in items[:3]:
        print(f'  {item}')
    # File distribution
    files = {}
    for item in items:
        # Extract filename from path
        parts = item.split('\\')
        fname = parts[-1].split('(')[0].strip() if parts else '?'
        files[fname] = files.get(fname, 0) + 1
    print(f'  --- Files ---')
    for f, c in sorted(files.items(), key=lambda x: -x[1])[:10]:
        print(f'    {f}: {c}')
