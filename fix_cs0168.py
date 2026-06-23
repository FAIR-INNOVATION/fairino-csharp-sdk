"""Fix CS0168: replace catch (Exception ex) with catch when ex is unused."""
import re, os

# Read the build log and get the exact files with CS0168 warnings
with open(r'd:\C#SDK\各版本sdk\fairino-csharp_-sdk-3.9.7\build_warn.txt', 'rb') as f:
    raw = f.read()
text = raw.decode('utf-8', errors='replace')

# Collect unique files with CS0168
cs0168_files = set()
for line in text.split('\n'):
    if 'CS0168' in line:
        pm = re.match(r'\s*(.+?)\(\d+,\d+\)', line)
        if pm:
            cs0168_files.add(pm.group(1).strip())

print(f'Files with CS0168: {len(cs0168_files)}')
for f in sorted(cs0168_files):
    print(f'  {f}')

total_fixed = 0

for filepath in sorted(cs0168_files):
    if not os.path.exists(filepath):
        print(f'  SKIP (not found): {filepath}')
        continue

    with open(filepath, 'rb') as f:
        content = f.read()

    # Work with bytes to avoid encoding issues
    # Find all 'catch (Exception XXX)' patterns
    # Strategy: for each match, check if XXX is used in the catch block

    text_content = content.decode('utf-8', errors='replace')

    # Find all matches of catch (Type var)
    pattern = re.compile(r'\bcatch\s*\(\s*(\w+(?:\.\w+)?)\s+(\w+)\s*\)')

    replacements = []
    for m in pattern.finditer(text_content):
        ex_type = m.group(1)
        ex_var = m.group(2)
        catch_start = m.end()

        # Find the matching } for this catch block
        # Count braces from the opening { after 'catch (...)'
        brace_pos = text_content.find('{', catch_start)
        if brace_pos == -1:
            continue

        depth = 1
        pos = brace_pos + 1
        while pos < len(text_content) and depth > 0:
            if text_content[pos] == '{':
                depth += 1
            elif text_content[pos] == '}':
                depth -= 1
            pos += 1

        catch_body_end = pos  # position after the matching }
        catch_body = text_content[brace_pos + 1:catch_body_end - 1]

        # Check if ex_var is used as a whole word in the catch body
        if not re.search(r'\b' + re.escape(ex_var) + r'\b', catch_body):
            # Replace 'catch (XXX ex_var)' with 'catch'
            new_catch = 'catch'
            replacements.append((m.start(), m.end(), new_catch))

    # Apply replacements from end to start
    if replacements:
        result = list(text_content)
        for start, end, new_text in sorted(replacements, reverse=True):
            result[start:end] = new_text
        new_content = ''.join(result)

        # Preserve CRLF
        new_content = new_content.replace('\r\n', '\n').replace('\n', '\r\n')
        with open(filepath, 'wb') as f:
            f.write(new_content.encode('utf-8'))

        fixed = len(replacements)
        print(f'  {os.path.basename(filepath)}: {fixed} fixes')
        total_fixed += fixed

print(f'\nTotal CS0168 fixes: {total_fixed}')
