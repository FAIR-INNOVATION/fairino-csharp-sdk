# -*- coding: utf-8 -*-
"""把中文手册里更新后的“SDK章节”编号同步到英文手册配置。"""
import json
import os
import re
import sys

from docx import Document

CJK = re.compile(r"[\u4e00-\u9fff]")
MANUAL_DIR = r"D:\C#SDK\手册\4.0.2"
ZH_NAME = "RD36-协作机器人C#SDK用户手册版本更新(4.0.2)-V1.0-20261123.docx"
EN_CFG = os.path.join("tools", "manual", "config_4.0.2_en.json")


def pick_zh_doc():
    candidates = [
        os.path.join(MANUAL_DIR, ZH_NAME),
        ZH_NAME,
    ]
    existing = [p for p in candidates if os.path.exists(p)]
    if not existing:
        raise SystemExit("找不到中文 4.0.2 手册 docx")
    return max(existing, key=os.path.getmtime)


def extract_sdk_sections(path):
    doc = Document(path)
    out = []
    for p in doc.paragraphs:
        t = p.text.strip()
        if t.startswith("SDK章节："):
            out.append(t.replace("SDK章节：", "", 1).strip())
        elif t.startswith("SDK Chapter:"):
            out.append(t.replace("SDK Chapter:", "", 1).strip())
    return out


def main():
    zh = pick_zh_doc()
    print("source:", zh)
    sections = extract_sdk_sections(zh)
    print("sdk lines in zh:", len(sections))

    with open(EN_CFG, encoding="utf-8") as f:
        cfg = json.load(f)

    seq = []
    for api in cfg.get("changed_apis", []):
        seq.append(api)
        if api.get("example"):
            seq.append(api["example"])
    for api in cfg.get("new_apis", []):
        seq.append(api)
        if api.get("example"):
            seq.append(api["example"])
    for ex in cfg.get("examples", []):
        seq.append(ex)

    if len(seq) != len(sections):
        raise SystemExit("数量不一致: config=%d, docx=%d" % (len(seq), len(sections)))

    skipped = 0
    for item, val in zip(seq, sections):
        if CJK.search(val):
            skipped += 1
            continue
        item["sdk_section"] = val

    with open(EN_CFG, "w", encoding="utf-8") as f:
        json.dump(cfg, f, ensure_ascii=False, indent=2)
        f.write("\n")
    print("updated:", EN_CFG, "skipped(non-numeric zh):", skipped)
    for item, val in list(zip(seq, sections))[:8]:
        print(" ", item.get("title") or item.get("function"), "->", item.get("sdk_section"))


if __name__ == "__main__":
    main()
