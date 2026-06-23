"""Fix CS0168 and CS0219 warnings: remove unused variable declarations and assignments."""
import re, os
from collections import defaultdict

# Read warning log
with open(r'd:\C#SDK\各版本sdk\fairino-csharp_-sdk-3.9.7\build_warn.txt', 'rb') as f:
    raw = f.read()
text = raw.decode('utf-8', errors='replace')

# Parse warnings
WARNING_DIR = r'd:\C#SDK\各版本sdk\fairino-csharp_-sdk-3.9.7'

warnings_by_file = defaultdict(list)
for line in text.split('\n'):
    m = re.match(r'\s*(.+?)\((\d+),\d+\):\s*warning\s+(CS\d{4}):\s*(.+?)\s*\[', line)
    if m:
        fpath = m.group(1).strip()
        line_no = int(m.group(2))
        code = m.group(3)
        msg = m.group(4)
        if code in ('CS0168', 'CS0219'):
            # Extract variable name from the message
            var_match = re.search(r"variable '(\w+)'", msg)
            var_name = var_match.group(1) if var_match else None
            warnings_by_file[fpath].append({
                'line': line_no,
                'code': code,
                'var': var_name,
                'msg': msg
            })

print(f'Files to fix: {len(warnings_by_file)}')

# For each file, sort warnings by line descending so we can delete lines safely
for fpath, warns in sorted(warnings_by_file.items()):
    real_path = fpath
    if not os.path.exists(real_path):
        # Try to find the file using a path component search
        base = os.path.basename(fpath)
        found = False
        for root, dirs, files in os.walk(WARNING_DIR):
            if base in files:
                candidate = os.path.join(root, base)
                if os.path.exists(candidate):
                    real_path = candidate
                    found = True
                    break
        if not found:
            print(f'  SKIP: {base} (not found)')
            continue

    with open(real_path, 'rb') as f:
        content = f.read()
    source_lines = content.decode('utf-8', errors='replace').split('\n')

    # Group warnings by line, sort descending
    line_warns = defaultdict(list)
    for w in warns:
        line_warns[w['line']].append(w)

    fixed_lines = set()
    modifications = []

    for line_no in sorted(line_warns.keys(), reverse=True):
        if line_no < 1 or line_no > len(source_lines):
            continue
        idx = line_no - 1
        src_line = source_lines[idx]
        stripped = src_line.strip()
        ws = src_line[:len(src_line) - len(src_line.lstrip())] if src_line else ''

        ws_for_warns = line_warns[line_no]

        # Strategy depends on the type of line
        # Case 1: Simple declaration without initializer: "int rtn;" / "byte state;"
        simple_decl = re.match(r'^(\w+(?:\[\])?(?:\s*<\w+>)?)\s+(\w+)\s*;?\s*(?://.*)?$', stripped)
        if simple_decl:
            var_type = simple_decl.group(1)
            var_name = simple_decl.group(2)
            # Check if this matches one of our warnings
            for w in ws_for_warns:
                if w['var'] == var_name:
                    # Safe to remove the entire line
                    source_lines[idx] = None  # mark for removal
                    fixed_lines.add(line_no)
                    break

        # Case 2: Assignment without usage: "int rtn = ...;" or "float x = 100.0f;"
        assign_decl = re.match(r'^(\w+(?:\[\])?(?:\s*<\w+>)?)\s+(\w+)\s*=\s*(.+?);?\s*(?://.*)?$', stripped)
        if assign_decl:
            var_type = assign_decl.group(1)
            var_name = assign_decl.group(2)
            rhs = assign_decl.group(3)
            for w in ws_for_warns:
                if w['var'] == var_name and w['code'] == 'CS0219':
                    # Check if RHS has side effects (method/property/constructor calls)
                    side_effects = bool(re.search(r'\w+\.\w+\s*\(|new\s+\w+\s*\(', rhs))
                    if side_effects:
                        # Keep the call, discard the assignment
                        call_match = re.match(r'^\s*(\w+(?:\[\])?(?:\s*<\w+>)?)\s+\w+\s*=\s*(.+?);?\s*$', src_line)
                        if call_match:
                            rhs_only = call_match.group(2).rstrip(';').strip()
                            source_lines[idx] = ws + rhs_only + ';'
                            fixed_lines.add(line_no)
                    else:
                        # Pure value assignment, safe to remove
                        source_lines[idx] = None
                        fixed_lines.add(line_no)
                    break

        # Case 3: catch (Exception ex) where ex unused → change to catch
        catch_match = re.match(r'^(\s*)catch\s*\(\s*(\w+)\s+(\w+)\s*\)\s*$', src_line)
        if catch_match:
            ex_var = catch_match.group(3)
            for w in ws_for_warns:
                if w['var'] == ex_var:
                    source_lines[idx] = catch_match.group(1) + 'catch'
                    fixed_lines.add(line_no)
                    break

    # Remove None lines and write back
    new_lines = [l for l in source_lines if l is not None]
    if fixed_lines:
        new_text = '\r\n'.join(new_lines)
        with open(real_path, 'wb') as f:
            f.write(new_text.encode('utf-8'))
        print(f'  {os.path.basename(real_path)}: fixed {len(fixed_lines)} lines')
    else:
        print(f'  {os.path.basename(real_path)}: no changes')

print('\nDone.')
