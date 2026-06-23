import re, os

with open(r'd:\C#SDK\各版本sdk\fairino-csharp_-sdk-3.9.7\build_warn.txt', 'rb') as f:
    raw = f.read()
text = raw.decode('utf-8', errors='replace')

# Parse warning lines
warn_lines = []
for line in text.split('\n'):
    m = re.search(r'warning (CS\d{4})', line)
    if m:
        # Extract file path and line number
        pm = re.match(r'(.+?)\((\d+),(\d+)\)', line)
        if pm:
            fpath = pm.group(1).strip()
            line_no = int(pm.group(2))
            col = int(pm.group(3))
            code = m.group(1)
            warn_lines.append({
                'file': fpath,
                'line': line_no,
                'col': col,
                'code': code,
                'text': line.strip()
            })

# Filter CS0168
cs0168 = [w for w in warn_lines if w['code'] == 'CS0168']
cs0219 = [w for w in warn_lines if w['code'] == 'CS0219']

print(f'CS0168: {len(cs0168)} warnings')
print(f'CS0219: {len(cs0219)} warnings')

# Group CS0168 by file and line (there may be duplicates at same line)
cs0168_by_file = {}
for w in cs0168:
    fname = os.path.basename(w['file'])
    if fname not in cs0168_by_file:
        cs0168_by_file[fname] = []
    cs0168_by_file[fname].append(w['line'])

print('\nCS0168 by file (sorted by line):')
for fname in sorted(cs0168_by_file.keys()):
    lines = sorted(set(cs0168_by_file[fname]))
    print(f'  {fname}: lines {lines[:20]}... ({len(lines)} unique)')

# Group CS0219 by file
cs0219_by_file = {}
for w in cs0219:
    fname = os.path.basename(w['file'])
    if fname not in cs0219_by_file:
        cs0219_by_file[fname] = []
    cs0219_by_file[fname].append(w)

print('\nCS0219 by file:')
for fname in sorted(cs0219_by_file.keys()):
    lines = sorted(set(w['line'] for w in cs0219_by_file[fname]))
    print(f'  {fname}: {len(lines)} unique lines, {len(cs0219_by_file[fname])} total')
    for ln in lines[:10]:
        # Show first few chars of the warning text
        for w in cs0219_by_file[fname]:
            if w['line'] == ln:
                print(f'    L{ln}: {w["text"][:120]}')
                break
