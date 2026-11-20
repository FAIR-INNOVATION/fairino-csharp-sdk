# C#SDK 版本更新手册生成工具

用于在每个版本发布时生成《RD36-协作机器人C#SDK用户手册版本更新》。

## 已固化的格式

- 以**上一版本手册 docx 为模板**：封面底图、目录域、页眉页脚、标题样式（思源黑体 CN 18/14/13pt）、自动标题编号全部保留。
- 代码段与 4.0.1 完全一致：
  - 四边框（pBdr，单线 sz=4）
  - 左侧行号，**每个代码块从 1 重新计数**（numId + startOverride）
  - Times New Roman 10.5pt / cs Consolas
- 版本变更说明条目顶格（取消首行缩进）、10.5pt，与 4.0.1 一致。
- 标题自动编号（1 / 1.1 / 1.1.1），TOC 域设置 `updateFields`，Word 打开时提示更新目录。

## 发布时怎么用

1. 复制上一版配置：
   ```
   cp tools/manual/config_4.0.2.json tools/manual/config_4.0.3.json
   ```
2. 编辑新配置：
   - `version` / `date`：版本号与发布日期（date 为 `YYYYMMDD`）
   - `template`：上一版本手册路径（放 `D:/C#SDK/手册/<上一版本>/`）
   - `output_dir`：输出目录，如 `D:/C#SDK/手册/4.0.3`
   - `sdk_section_start`：新接口在完整手册中的起始章节号（如 `"16.43"`），脚本按顺序自动编号；个别接口可用 `sdk_section` 覆盖
   - `changes`：版本变更说明条目（每条一句，以“；/。”结尾）
   - `changed_apis`：参数有变化的接口（`name` + `title` + `sdk_section`）
   - `new_apis`：新增接口（`name` + `title`），注释和函数原型**自动从 `src/FRRobot/FRRobot.cs` 抓取**
   - `examples`：代码示例（`function` 为 `Test.cs` 中的函数名 + `title`），代码**自动从 `examples/testFrRobot/Test.cs` 抓取**
3. 运行：
   ```
   python tools/manual/build_release_manual.py tools/manual/config_4.0.3.json
   ```
   （使用工作区自带的 Python 运行时即可；依赖 python-docx）
4. 脚本会：
   - 在仓库根目录生成同名 docx
   - 复制到 `output_dir`
   - 自检代码段边框数量、标题数量，并打印路径与文件大小
5. 打开生成的 docx，确认目录（Word 提示更新域时选“是”），通读一遍接口注释与代码示例。

## 注意事项

- `template` 必须是带封面/目录的上一版本手册，否则样式和封面会缺失。
- 接口注释来自 `FRRobot.cs`，**注释改动即文档改动**；接口要在 `public int <name>(` 形式下才能被识别。
- 代码示例来自 `Test.cs` 的 `public void/int <function>()`，同一函数里的 `{ }` 需配对（字符串内的花括号也会参与计数，避免在示例里写不平衡的 `{`）。
- 目录是 Word 域，LibreOffice/WPS 预览可能不刷新；正式分发前在 Word 中更新一次域。
- 生成物是二进制 docx，是否入库按团队习惯决定（当前放在 `D:\C#SDK\手册\<版本>\`，仓库只保留本工具）。
