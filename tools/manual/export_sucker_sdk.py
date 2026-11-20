# -*- coding: utf-8 -*-
"""导出末端吸盘适配 SDK 接口与用例到 末端吸盘适配SDK.txt（与源码同步）。"""
import os
import re
import shutil
import sys

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
REPO_ROOT = os.path.dirname(os.path.dirname(SCRIPT_DIR))

FR = os.path.join(REPO_ROOT, "src/FRRobot/FRRobot.cs")
TEST = os.path.join(REPO_ROOT, "examples/testFrRobot/Test.cs")
OUT = os.path.join(REPO_ROOT, "末端吸盘适配SDK.txt")
DEST = r"D:\C#SDK\手册\4.0.2\末端吸盘适配SDK.txt"

HEADER = """// 4.0.2 末端吸盘适配 SDK 接口与用例（自动生成，与源码同步）
// 1. SetAxleSuckerFunc / GetAxleSuckerFunc 为 16 项功能码数组形式，func[0]~func[15] 对应 Bit0~Bit15；
//    新增 Bit11-通道一控制使能、Bit12-通道二控制使能。
// 2. SetAxleLuaEnableDeviceType 第 5 参 suckerEnable 默认 -1：不传时仍按原四参数下发，兼容旧固件。
// 3. GetAxleLuaEnableDevice 返回 48 项（0-7 力传感器、8-15 夹爪、16-23 IO 预留、24-39 灵巧手、40-47 末端吸盘）；
//    四数组重载兼容 >=40 项的旧返回，吸盘使能由五数组重载读取第 40~47 项。
// 4. WaitAxleSuckerState 参数顺序为 (channel, timeout, state, strategy)。
"""


def read_lines(path):
    with open(path, encoding="utf-8-sig") as f:
        return f.read().splitlines()


def norm(line):
    s = line.strip()
    if s.startswith("/**"):
        return "/**"
    if s.startswith("*/"):
        return "*/"
    if s.startswith("*"):
        return " " + s
    return s


def extract_api(lines, name, match=None):
    cands = [i for i, ln in enumerate(lines) if re.match(r"\s*public int %s\(" % re.escape(name), ln)]
    if not cands:
        raise SystemExit("api not found: " + name)
    start = cands[0]
    if match:
        for i in cands:
            if match in lines[i]:
                start = i
                break
    c = start - 1
    while c >= 0 and lines[c].strip() == "":
        c -= 1
    if lines[c].strip() != "*/":
        raise SystemExit("comment not found: " + name)
    j = c
    while not lines[j].strip().startswith("/**"):
        j -= 1
    # 函数体：从签名的开括号到配对闭括号
    k = start
    while k < len(lines) and "{" not in lines[k]:
        k += 1
    depth = 0
    end = k
    for idx in range(k, len(lines)):
        depth += lines[idx].count("{") - lines[idx].count("}")
        if depth == 0:
            end = idx
            break
    out = ["        " + norm(ln) for ln in lines[j:start]]
    out.extend(lines[start:end + 1])
    return out


def extract_test(lines, name):
    start = None
    for i, ln in enumerate(lines):
        if re.match(r"\s*public void %s\(" % re.escape(name), ln):
            start = i
            break
    if start is None:
        raise SystemExit("test not found: " + name)
    out = []
    j = start - 1
    while j >= 0 and (lines[j].strip().startswith("///") or lines[j].strip() == ""):
        j -= 1
    out.extend(lines[j + 1:start + 1])
    depth = lines[start].count("{") - lines[start].count("}")
    for ln in lines[start + 1:]:
        out.append(ln)
        depth += ln.count("{") - ln.count("}")
        if depth == 0:
            break
    return out


def main():
    fr = read_lines(FR)
    test = read_lines(TEST)
    parts = [HEADER, "", "// ==================== 4.0.2 末端吸盘接口 ====================", ""]
    apis = [
        ("SetAxleSuckerFunc", None),
        ("GetAxleSuckerFunc", None),
        ("SetAxleSuckerCtrl", None),
        ("SetAxleSuckerAct", None),
        ("GetAxleSuckerState", None),
        ("WaitAxleSuckerState", None),
    ]
    for name, match in apis:
        parts.extend(extract_api(fr, name, match))
        parts.append("")
    parts.append("// ==================== 末端设备使能（含吸盘） ====================")
    parts.append("")
    for name, match in [
        ("SetAxleLuaEnableDeviceType", None),
        ("GetAxleLuaEnableDeviceType", "suckerEnable"),
        ("GetAxleLuaEnableDevice", "suckerEnable"),
    ]:
        parts.extend(extract_api(fr, name, match))
        parts.append("")
    parts.append("// ==================== 末端吸盘接口测试用例 ====================")
    parts.append("")
    parts.extend(extract_test(test, "TestAxleSucker"))
    parts.append("")

    text = "\n".join(parts)
    with open(OUT, "w", encoding="utf-8-sig", newline="\r\n") as f:
        f.write(text)
    os.makedirs(os.path.dirname(DEST), exist_ok=True)
    shutil.copyfile(OUT, DEST)
    print("saved:", OUT)
    print("copied:", DEST)
    print("lines:", len(parts))


if __name__ == "__main__":
    main()
