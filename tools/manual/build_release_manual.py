# -*- coding: utf-8 -*-
"""生成《RD36-协作机器人C#SDK用户手册版本更新》系列文档。

用法：
    python tools/manual/build_release_manual.py tools/manual/manual_4.0.2.json

脚本以“上一版本手册 docx”为模板，保留封面、目录域、页眉页脚、标题样式，
清空正文后按配置重写：版本变更说明 -> 参数更改接口 -> 新增接口 -> 代码示例。
代码段格式（四边框 + 每块从 1 开始的行号 + Times New Roman 10.5pt）与 4.0.1 一致。
"""
import json
import os
import re
import shutil
import sys
from copy import deepcopy

from docx import Document
from docx.shared import Pt
from docx.oxml import OxmlElement
from docx.oxml.ns import qn

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
REPO_ROOT = os.path.dirname(os.path.dirname(SCRIPT_DIR))


def read_lines(path):
    with open(path, encoding="utf-8-sig") as f:
        return f.read().splitlines()


def norm_api_line(line):
    s = line.strip()
    if s.startswith("/**"):
        return "/**"
    if s.startswith("*/"):
        return "*/"
    if s.startswith("*"):
        return " " + s
    return s


def extract_api(fr_lines, name):
    start = None
    for i, ln in enumerate(fr_lines):
        if re.match(r"\s*public int %s\(" % re.escape(name), ln):
            start = i
            break
    if start is None:
        raise SystemExit("接口未在 FRRobot.cs 中找到: " + name)
    c_start = start - 1
    while c_start >= 0 and fr_lines[c_start].strip() == "":
        c_start -= 1
    if c_start < 0 or fr_lines[c_start].strip() != "*/":
        raise SystemExit("接口注释未找到: " + name)
    j = c_start
    while j >= 0 and not fr_lines[j].strip().startswith("/**"):
        j -= 1
    return [norm_api_line(ln) for ln in fr_lines[j:start + 1]]


def extract_test(test_lines, name):
    start = None
    for i, ln in enumerate(test_lines):
        if re.match(r"\s*public (void|int) %s\(" % re.escape(name), ln):
            start = i
            break
    if start is None:
        raise SystemExit("测试函数未在 Test.cs 中找到: " + name)
    out = [test_lines[start][8:] if test_lines[start].startswith("        ") else test_lines[start]]
    depth = test_lines[start].count("{") - test_lines[start].count("}")
    for ln in test_lines[start + 1:]:
        out.append(ln[8:] if ln.startswith("        ") else ln)
        depth += ln.count("{") - ln.count("}")
        if depth == 0:
            break
    return out


def sdk_section_gen(start):
    major, minor = start.split(".")
    n = int(minor)
    while True:
        yield "%s.%d" % (major, n)
        n += 1


def scale_path(p):
    if os.path.isabs(p):
        return p
    return os.path.join(REPO_ROOT, p)


def build(cfg_path):
    with open(cfg_path, encoding="utf-8") as f:
        cfg = json.load(f)

    version = cfg["version"]
    date = cfg["date"]
    template = scale_path(cfg["template"])
    out_dir = scale_path(cfg["output_dir"])
    out_name = "RD36-协作机器人C#SDK用户手册版本更新(%s)-V1.0-%s.docx" % (version, date)
    out_ws = os.path.join(REPO_ROOT, out_name)
    fr_path = scale_path(cfg.get("frrobot_cs", "src/FRRobot/FRRobot.cs"))
    test_path = scale_path(cfg.get("test_cs", "examples/testFrRobot/Test.cs"))

    if not os.path.exists(template):
        raise SystemExit("模板不存在: " + template)

    doc = Document(template)
    body = doc.element.body

    # ---- 代码段模板：边框 + 行号 + 字体 ----
    tpl_p = None
    for p in doc.paragraphs:
        if p.style.name == "HTML Preformatted" and p.text.strip():
            tpl_p = p
            break
    if tpl_p is None:
        raise SystemExit("模板中未找到代码段样式段落")
    tpl_ppr = deepcopy(tpl_p._p.pPr)
    tpl_rpr = deepcopy(tpl_p.runs[0]._r.rPr)

    # ---- 行号编号：每块独立 numId + startOverride=1 ----
    numbering = doc.part.numbering_part.element
    nums = numbering.findall(qn('w:num'))
    max_num_id = max([int(n.get(qn('w:numId'))) for n in nums] or [0])
    base_num = None
    for n in nums:
        if n.get(qn('w:numId')) == "2":
            base_num = n
            break
    if base_num is None:
        raise SystemExit("模板编号 numId=2 不存在")
    abs_id = base_num.find(qn('w:abstractNumId')).get(qn('w:val'))

    def new_code_num_id():
        nonlocal max_num_id
        max_num_id += 1
        num = OxmlElement('w:num')
        num.set(qn('w:numId'), str(max_num_id))
        an = OxmlElement('w:abstractNumId')
        an.set(qn('w:val'), abs_id)
        num.append(an)
        lvl_override = OxmlElement('w:lvlOverride')
        lvl_override.set(qn('w:ilvl'), '0')
        start_override = OxmlElement('w:startOverride')
        start_override.set(qn('w:val'), '1')
        lvl_override.append(start_override)
        num.append(lvl_override)
        numbering.append(num)
        return str(max_num_id)

    # ---- 保留封面/TOC，清空正文 ----
    for child in list(body.iterchildren())[3:]:
        if child.tag == qn('w:sectPr'):
            continue
        body.remove(child)

    # ---- 封面版本号 ----
    new_minor = version.split(".")[-1]
    for p in body[0].iter(qn('w:p')):
        texts = [t for t in p.iter(qn('w:t'))]
        vals = [t.text for t in texts]
        if "4.0." in vals:
            idx = vals.index("4.0.")
            if idx + 1 < len(texts):
                texts[idx + 1].text = new_minor

    # ---- 文档属性 ----
    cp = doc.core_properties
    cp.title = "RD36-协作机器人C#SDK用户手册版本更新(%s)" % version
    cp.subject = "C#SDK 版本更新说明"
    modified = date
    if len(modified) == 8:
        cp.comments = "V1.0-%s" % modified

    # ---- 排版辅助 ----
    def h1(text):
        return doc.add_paragraph(text, style="Heading 1")

    def h2(text):
        return doc.add_paragraph(text, style="Heading 2")

    def h3(text):
        return doc.add_paragraph(text, style="Heading 3")

    def body_item(text):
        p = doc.add_paragraph(style="Normal")
        r = p.add_run(text)
        r.font.size = Pt(10.5)
        p.paragraph_format.first_line_indent = Pt(0)
        ind = p._p.pPr.find(qn('w:ind'))
        if ind is not None:
            ind.set(qn('w:firstLineChars'), '0')
        return p

    def sdk_section(text):
        p = doc.add_paragraph(style="Normal")
        r = p.add_run(text)
        r.font.size = Pt(10.5)
        return p

    def pre(lines):
        if not lines:
            return
        num_id = new_code_num_id()
        for line in lines:
            p = doc.add_paragraph()
            el = p._p
            if el.pPr is not None:
                el.remove(el.pPr)
            ppr = deepcopy(tpl_ppr)
            numpr = ppr.find(qn('w:numPr'))
            if numpr is not None:
                numpr.find(qn('w:numId')).set(qn('w:val'), num_id)
            el.insert(0, ppr)
            run = p.add_run(line)
            r = run._r
            if r.rPr is not None:
                r.remove(r.rPr)
            r.insert(0, deepcopy(tpl_rpr))

    fr_lines = read_lines(fr_path)
    test_lines = read_lines(test_path)

    # ---- 正文 ----
    h1("版本变更说明")
    for c in cfg["changes"]:
        body_item(c)

    number = sdk_section_gen(cfg.get("sdk_section_start", "16.1"))

    def render_example(ex):
        h2(ex["title"])
        sdk_section("SDK章节：" + ex.get("sdk_section", next(number)))
        pre(extract_test(test_lines, ex["function"]))

    changed = cfg.get("changed_apis", [])
    if changed:
        h1("参数更改接口")
        for api in changed:
            h2(api["title"])
            h3("接口描述")
            sdk_section("SDK章节：" + api.get("sdk_section", "—"))
            pre(extract_api(fr_lines, api["name"]))
            if api.get("example"):
                render_example(api["example"])

    new_apis = cfg.get("new_apis", [])
    if new_apis:
        h1("新增接口")
        for api in new_apis:
            h2(api["title"])
            h3("接口描述")
            sdk_section("SDK章节：" + api.get("sdk_section", next(number)))
            pre(extract_api(fr_lines, api["name"]))
            if api.get("example"):
                render_example(api["example"])

    for ex in cfg.get("examples", []):
        render_example(ex)

    # ---- 目录域自动更新 ----
    settings = doc.settings.element
    if settings.find(qn('w:updateFields')) is None:
        uf = OxmlElement('w:updateFields')
        uf.set(qn('w:val'), 'true')
        settings.append(uf)

    # 目标文件可能正被 Word 占用，占用时另存为 -new.docx
    saved_ws = out_ws
    try:
        doc.save(out_ws)
    except PermissionError:
        saved_ws = out_ws[:-5] + "-new.docx"
        doc.save(saved_ws)
        print("WARN: 原文件被占用，已另存为", saved_ws)

    os.makedirs(out_dir, exist_ok=True)
    dest = os.path.join(out_dir, out_name)
    copied = True
    try:
        shutil.copyfile(saved_ws, dest)
    except PermissionError:
        copied = False
        dest_alt = dest[:-5] + "-new.docx"
        shutil.copyfile(saved_ws, dest_alt)
        print("WARN: 手册目录目标文件被占用，已另存为:", dest_alt)

    # ---- 自检 ----
    chk = Document(saved_ws)
    headings = [p.text for p in chk.paragraphs if p.style.name.startswith("Heading")]
    code_ps = [p for p in chk.paragraphs if p._p.pPr is not None and p._p.pPr.find(qn('w:pBdr')) is not None]
    if not code_ps:
        raise SystemExit("自检失败：代码段没有边框")
    print("version   :", version)
    print("saved     :", saved_ws)
    print("copied    :", dest if copied else "(skipped, file locked)")
    print("headings  :", len(headings))
    print("code paras:", len(code_ps))
    print("size      :", os.path.getsize(saved_ws))


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("usage: python tools/manual/build_release_manual.py <config.json>")
        raise SystemExit(2)
    build(sys.argv[1])
