using System;
using System.Collections.Generic;
using System.Text;

namespace fairino
{
    /// <summary>
    /// 通信帧结构
    /// </summary>
    public struct FRAME
    {
        public string head;        // 帧头 "/f/b"
        public string tail;        // 帧尾 "/b/f"
        public int count;          // 帧计数
        public int cmdID;          // 命令ID
        public int contentLen;     // 内容长度
        public string content;     // 内容
    }

    /// <summary>
    /// 帧处理辅助类
    /// </summary>
    public static class FrameHandle
    {
        /// <summary>
        /// 从数据流中分割出完整帧
        /// </summary>
        public static List<string> SplitFrame(string data)
        {
            var result = new List<string>();
            int pos = 0;
            while (pos < data.Length)
            {
                int start = data.IndexOf("/f/b", pos);
                if (start == -1) break;

                int end = data.IndexOf("/b/f", start);
                if (end == -1) break;

                // 提取完整帧
                result.Add(data.Substring(start, end + 4 - start));
                pos = end + 4;
            }
            return result;
        }

        /// <summary>
        /// 解包帧字符串为FRAME对象（参考C++实现，手动查找分隔符）
        /// </summary>
        public static FRAME UnpacketFrame(string frameStr)
        {
            FRAME frame = new FRAME();

            // 1. 基本长度检查
            if (frameStr.Length < 27)
                return frame;

            // 2. 验证帧头帧尾
            if (!frameStr.StartsWith("/f/b"))
                return frame;
            if (!frameStr.EndsWith("/b/f"))
                return frame;

            // 3. 提取中间数据（去掉头尾）
            string data = frameStr.Substring(4, frameStr.Length - 8);

            // 4. 手动分割字段，查找前5个"III"分隔符
            List<string> parts = new List<string>();
            int start = 0;
            for (int i = 0; i < 5; i++) // 共5个分隔符
            {
                int pos = data.IndexOf("III", start);
                if (pos == -1)
                    return frame; // 格式错误

                string field = data.Substring(start, pos - start);
                parts.Add(field);
                start = pos + 3;
            }

            // 剩余部分（理论上应为空，因为后面是帧尾，但安全起见保留）
            if (start < data.Length)
                parts.Add(data.Substring(start));
            else
                parts.Add("");

            // 现在parts至少应有5个元素（实际6个，索引0和5为空）
            if (parts.Count < 5)
                return frame;

            // 填充frame（索引1=count，索引2=cmdID，索引3=contentLen，索引4=content）
            frame.head = "/f/b";
            frame.tail = "/b/f";

            if (!int.TryParse(parts[1], out frame.count))
                return frame;
            if (!int.TryParse(parts[2], out frame.cmdID))
                return frame;
            if (!int.TryParse(parts[3], out frame.contentLen))
                return frame;

            frame.content = parts[4];

            // 验证内容长度
            if (frame.contentLen > 0 && frame.content.Length != frame.contentLen)
            {
                frame.content = "";
                frame.contentLen = 0;
            }

            return frame;
        }
        /// <summary>
        /// 打包帧为字符串
        /// </summary>
        public static string PackFrame(FRAME frame)
        {
            return $"/f/bIII{frame.count}III{frame.cmdID}III{frame.content.Length}III{frame.content}III/b/f";
        }

        /// <summary>
        /// 从lua错误内容中解析行号和错误码（针对500错误）
        /// </summary>
        public static void GetRobotLUAProgram500ErrCode(string content, out int errLineNum, out int luaErrCode)
        {
            errLineNum = 0;
            luaErrCode = 0;

            // 查找 ".lua" 位置
            int luaPos = content.IndexOf(".lua");
            if (luaPos == -1) return;

            // 第一个冒号（文件名后的冒号）
            int colon1 = content.IndexOf(':', luaPos);
            if (colon1 == -1) return;

            // 第二个冒号（行号后的冒号）
            int colon2 = content.IndexOf(':', colon1 + 1);
            if (colon2 == -1) return;

            // 提取行号
            string lineStr = content.Substring(colon1 + 1, colon2 - colon1 - 1);
            if (!int.TryParse(lineStr, out errLineNum))
                return;

            // 查找 "errcode"
            int errcodePos = content.IndexOf("errcode", colon2);
            if (errcodePos == -1) return;

            // 提取错误码数字
            int codeStart = errcodePos + 7; // "errcode"长度7
            while (codeStart < content.Length && !char.IsDigit(content[codeStart]))
                codeStart++;

            int codeEnd = codeStart;
            while (codeEnd < content.Length && char.IsDigit(content[codeEnd]))
                codeEnd++;

            if (codeEnd > codeStart)
            {
                int.TryParse(content.Substring(codeStart, codeEnd - codeStart), out luaErrCode);
            }
        }
    }
}