using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace fairino
{
    /// <summary>CNDE数据包结构</summary>
    public class CNDE_PKG
    {
        public ushort Head { get; set; } = 0x5A5A;
        public byte Count { get; set; }
        public byte Type { get; set; }
        public ushort Len { get; set; }
        public List<byte> Data { get; set; } = new List<byte>();
        public ushort End { get; set; } = 0xA5A5;

        public void Clear()
        {
            Count = 0;
            Type = 0;
            Len = 0;
            Data.Clear();
        }
    }

    /// <summary>CNDE帧类型常量</summary>
    public static class CNDEFrameType
    {
        public const byte START = 2;
        public const byte STOP = 3;
        public const byte OUTPUT_STATE = 4;
        public const byte MESSAGE = 6;
        public const byte OUTPUT_CONFIG = 1;      // 新增：配置帧类型
    }

    /// <summary>CNDE帧处理工具类</summary>
    public static class CNDEFrameHandle
    {
        /// <summary>将CNDE_PKG打包为字节数组（小端）</summary>
        public static byte[] CNDEPkgToFrame(CNDE_PKG pkg)
        {
            var frame = new List<byte> { 0x5A, 0x5A, pkg.Count, pkg.Type };
            frame.AddRange(BitConverter.GetBytes(pkg.Len)); // 小端
            frame.AddRange(pkg.Data);
            frame.Add(0xA5); frame.Add(0xA5);
            return frame.ToArray();
        }

        /// <summary>从字节数组解析CNDE_PKG</summary>
        /// <returns>0成功，负数错误码</returns>
        //public static int FrameToCNDEPkg(byte[] frame, out CNDE_PKG pkg)
        //{
        //    pkg = new CNDE_PKG();
        //    if (frame.Length < 8) return -1;
        //    pkg.Head = (ushort)(frame[0] | (frame[1] << 8));
        //    if (pkg.Head != 0x5A5A) return -3;
        //    pkg.Count = frame[2];
        //    pkg.Type = frame[3];
        //    pkg.Len = (ushort)(frame[4] | (frame[5] << 8));
        //    if (pkg.Len != frame.Length - 8) return -2;
        //    pkg.Data.AddRange(frame.Skip(6).Take(pkg.Len));
        //    ushort tail = (ushort)(frame[frame.Length - 2] | (frame[frame.Length - 1] << 8));
        //    if (tail != 0xA5A5) return -4;
        //    pkg.End = tail;
        //    return 0;
        //}
        public static int FrameToCNDEPkg(byte[] frame, out CNDE_PKG pkg)
        {
            pkg = new CNDE_PKG();
            if (frame.Length < 8) return -1;

            // 解析头部
            ushort head = (ushort)(frame[0] | (frame[1] << 8));
            if (head != 0x5A5A) return -3;

            pkg.Count = frame[2];
            pkg.Type = frame[3];
            pkg.Len = (ushort)(frame[4] | (frame[5] << 8));

            // 计算完整帧的总长度（8字节固定头 + 数据长度）
            int totalLen = 8 + pkg.Len;
            if (frame.Length < totalLen) return -2; // 数据不完整

            // 检查帧尾（位置在 totalLen-2 和 totalLen-1）
            if (frame[totalLen - 2] != 0xA5 || frame[totalLen - 1] != 0xA5) return -4;

            // 提取数据
            pkg.Data.AddRange(frame.Skip(6).Take(pkg.Len));
            pkg.End = (ushort)(frame[totalLen - 2] | (frame[totalLen - 1] << 8));
            pkg.Head = head;  // 已确认正确

            return 0;
        }
    }
}