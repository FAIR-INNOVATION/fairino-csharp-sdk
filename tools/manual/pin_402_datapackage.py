# -*- coding: utf-8 -*-
"""把中文手册现有 SDK 章节编号固定进中英文配置，并追加 DataPackageUpgrade 参数更改条目。"""
import json
import os
import re
import sys

from docx import Document

CJK = re.compile(r"[\u4e00-\u9fff]")
MANUAL = r"D:\C#SDK\手册\4.0.2\RD36-协作机器人C#SDK用户手册版本更新(4.0.2)-V1.0-20261123.docx"
ZH_CFG = "tools/manual/config_4.0.2.json"
EN_CFG = "tools/manual/config_4.0.2_en.json"

ZH_CHANGE = ("修改DataPackageUpgrade()数据备份包上传升级接口，新增isSafetyParamUpgrade是否升级安全配置参数、"
             "pwd安全密码参数；默认false/null时仍按原无参数接口下发，兼容旧版控制器；")
EN_CHANGE = ("Modified the DataPackageUpgrade() data backup package upload upgrade interface to add "
             "isSafetyParamUpgrade (whether to upgrade safety configuration parameters) and pwd (safety password); "
             "when they are false/null, the original no-parameter interface is still used for compatibility with older controllers;")

ZH_API = {
    "name": "DataPackageUpgrade",
    "title": "数据备份包上传升级",
    "sdk_section": "16.38（参数更新）",
    "example": {
        "function": "TestDataPackageUpgrade",
        "title": "数据备份包上传升级代码示例",
        "sdk_section": "16.39（参数更新）",
    },
}
EN_API = {
    "name": "DataPackageUpgrade",
    "title": "Data backup package upload upgrade",
    "sdk_section": "16.38 (parameter update)",
    "example": {
        "function": "TestDataPackageUpgrade",
        "title": "Sample code for data backup package upload upgrade",
        "sdk_section": "16.39 (parameter update)",
    },
}


def extract_sections(path):
    doc = Document(path)
    out = []
    for p in doc.paragraphs:
        t = p.text.strip()
        if t.startswith("SDK章节："):
            out.append(t.replace("SDK章节：", "", 1).strip())
    return out


def flatten(cfg):
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
    return seq


def add_change(cfg, text):
    changes = cfg.get("changes", [])
    idx = next((i for i, c in enumerate(changes) if "SDK_VERSION" in c), len(changes))
    changes.insert(idx, text)
    cfg["changes"] = changes


def main():
    sections = extract_sections(MANUAL)
    print("zh manual sdk lines:", len(sections))

    with open(ZH_CFG, encoding="utf-8") as f:
        zh = json.load(f)
    with open(EN_CFG, encoding="utf-8") as f:
        en = json.load(f)

    zseq = flatten(zh)
    eseq = flatten(en)
    if len(zseq) != len(sections) or len(eseq) != len(sections):
        raise SystemExit("数量不一致: zh cfg %d, en cfg %d, docx %d" % (len(zseq), len(eseq), len(sections)))

    for item, val in zip(zseq, sections):
        item["sdk_section"] = val
    for item, val in zip(eseq, sections):
        if not CJK.search(val):
            item["sdk_section"] = val

    zh.setdefault("changed_apis", []).append(ZH_API)
    en.setdefault("changed_apis", []).append(EN_API)
    add_change(zh, ZH_CHANGE)
    add_change(en, EN_CHANGE)

    for path, cfg in ((ZH_CFG, zh), (EN_CFG, en)):
        with open(path, "w", encoding="utf-8") as f:
            json.dump(cfg, f, ensure_ascii=False, indent=2)
            f.write("\n")
        print("updated:", path)


if __name__ == "__main__":
    main()
