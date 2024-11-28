using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fairino
{       
    public enum FrLogLevel /* 日志等级 */
    {
        DEBUG = 0,
        INFO = 1,
        WARN = 2,
        ERROR = 3
    }

    public enum FrLogType  /* 日志输出方式 */
    {
        DIRECT = 0,  //直接输出
        BUFFER = 1,  //缓冲输出
        ASYNC = 2    //异步输出
    }
    internal class Log
    {
        private FrLogType curLogType = FrLogType.DIRECT;
        private FrLogLevel curLogLevel = FrLogLevel.INFO;
        private string logFileName = "";
        private string logFilePath = "";
        private List<string> logBuf = new List<string>();
        private List<string> asyncWriteBuf = new List<string>();
        private int maxLogBufCount = 100;    //缓冲输出队列长度
        private int checkLogFileSizeFlag = 0;
        private const int CHECKFILESIZE = 1000;
        private bool writeLogAsyncFlag = true;
        private static SpinLock slockAsyncLog;
        private const long MAXFILESIZE = 10000000;
        private int logSaveDays = 10;
        private int logSaveFileNum = 10;
        public Log() 
        {
            logFilePath = System.Environment.CurrentDirectory;
            logFileName = GetNewLogFileName();
            writeLogAsyncFlag = true;
            Thread t = new Thread(WriteLogAsyncThread);
            t.Start();
        }

        public Log(FrLogType logType, FrLogLevel logLevel, string filePath, int saveFileNum, int saveFileDays) 
        {
            logFilePath = filePath;
            curLogType = logType;
            logSaveDays = saveFileDays;
            writeLogAsyncFlag = true;
            curLogLevel = logLevel;
            logSaveFileNum = saveFileNum;
            logFileName = GetNewLogFileName();
            Thread t = new Thread(WriteLogAsyncThread);
            t.Start();

        }

        public int SetLogLevel(FrLogLevel level)
        {
            curLogLevel = level;
            return 0;
        }

        public int LogInfo(string logStr, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if(curLogLevel > FrLogLevel.INFO)
            {
                return 0;
            }
            string fullLogStr = "[INFO] [" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + "(" + lineNumber + " " + memberName + ") " + logStr;
            LogWrite(fullLogStr);
            return 0;
        }

        public int LogWarn(string logStr, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0) 
        {
            if (curLogLevel > FrLogLevel.WARN)
            {
                return 0;
            }
            string fullLogStr = "[WARN] [" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "] " + "(" + lineNumber + " " + memberName + ") " + logStr;
            LogWrite(fullLogStr);
            return 0;
        }

        public int LogError(string logStr, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0) 
        {
            if (curLogLevel > FrLogLevel.ERROR)
            {
                return 0;
            }
            string fullLogStr = "[ERROR] [" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "] " + "(" + lineNumber + " " + memberName + ") " + logStr;
            LogWrite(fullLogStr);
            return 0;
        }

        public int LogDebug(string logStr, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0) 
        {
            if (curLogLevel > FrLogLevel.DEBUG)
            {
                return 0;
            }
            string fullLogStr = "[DEBUG] [" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "] " + "(" + lineNumber + " " + memberName + ") " + logStr;
            
           
            LogWrite(fullLogStr);
            return 0;
        }

        private int LogWrite(string logStr)
        {
            Console.WriteLine(logStr);

            checkLogFileSizeFlag++;
            if(checkLogFileSizeFlag > CHECKFILESIZE) // 1000行日志检测一次大小
            {
                FileInfo fileInfo = new FileInfo(logFileName);
                long logFileSize = fileInfo.Length;
                if(logFileSize > MAXFILESIZE)
                {
                    logFileName = GetNewLogFileName();
                    checkLogFileSizeFlag = 0;
                }
            }

            bool lockToken = false;
            slockAsyncLog.Enter(ref lockToken);
            if (curLogType == FrLogType.DIRECT)
            {
                WriteLogDirect(logStr);
            }
            else if (curLogType == FrLogType.BUFFER)
            {
                WriteLogBuffer(logStr);
            }
            else if (curLogType == FrLogType.ASYNC)
            {
                WriteLogAsync(logStr);
            }
            if (lockToken)
            {
                slockAsyncLog.Exit(false);
            }
            return 0;
        }

        private int WriteLogDirect(string logStr)
        {
            StreamWriter sw = new StreamWriter(logFileName, true, System.Text.Encoding.Default);
            sw.WriteLine(logStr);
            sw.Close();
            return 0;
        }

        private int WriteLogBuffer(string logStr)
        {
            logBuf.Add(logStr);
            int curLogCount = logBuf.Count;
            if(curLogCount > maxLogBufCount)
            {
                StreamWriter sw = new StreamWriter(logFileName, true, System.Text.Encoding.Default);
                for(int i = 0; i < curLogCount; i++)
                {
                    sw.WriteLine(logBuf[i]);
                }
                sw.Close();
                logBuf.Clear();
            }
            return 0;
        }

        private int WriteLogAsync(string logStr) 
        {
            logBuf.Add(logStr);   
            return 0;
        }

        private string GetNewLogFileName()
        {
            int logFileCount = 0;
            string[] fileNames = Directory.GetFiles(logFilePath, "*.log", SearchOption.AllDirectories);
            List<string> files = fileNames.OrderBy(ss => new FileInfo(ss).CreationTime).ToList();  //按照创建日期排序
            foreach (string fileName in files)
            {   
                if (fileName.Contains("FrLog"))
                {
                    logFileCount++;
                }
            }
            if(logFileCount > logSaveFileNum)
            {
                DateTime curTime = DateTime.Now.AddDays((-1) * logSaveDays);  
                foreach (string fileName in files)
                {
                    string[] sArray = fileName.Split(new string[] { "FrLog_", ".log" }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime dt = DateTime.ParseExact(sArray[1], "yyyy-MM-dd_HH-mm-ss", System.Globalization.CultureInfo.CurrentCulture);
                    if (dt < curTime && logFileCount > logSaveFileNum)
                    {
                        File.Delete(fileName);
                        logFileCount--;
                    }
                }
            }
            

            return logFilePath + "\\FrLog_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
        }

        private void WriteLogAsyncThread()
        {
            while(writeLogAsyncFlag)
            {
                if(curLogType == FrLogType.ASYNC && logBuf.Count > 0)  //只要有就写
                {
                    bool lockToken = false;
                    slockAsyncLog.Enter(ref lockToken);
                    asyncWriteBuf.AddRange(logBuf);
                    logBuf.Clear();
                    if(lockToken)
                    {
                        slockAsyncLog.Exit(false);
                    }

                    StreamWriter sw = new StreamWriter(logFileName, true, System.Text.Encoding.Default);
                    for (int i = 0; i < asyncWriteBuf.Count; i++)
                    {
                        sw.WriteLine(asyncWriteBuf[i]);
                    }
                    sw.Close();
                    asyncWriteBuf.Clear();
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
            Console.WriteLine("log close tread");
        }

        public int LogClose()
        {
            bool lockToken = false;
            slockAsyncLog.Enter(ref lockToken);
            int curLogCount = logBuf.Count;
            if(curLogCount > 0) 
            {
                StreamWriter sw = new StreamWriter(logFileName, true, System.Text.Encoding.Default);
                for (int i = 0; i < curLogCount; i++)
                {
                    sw.WriteLine(logBuf[i]);
                }
                sw.Close();
                logBuf.Clear();
            }
            slockAsyncLog.Exit(false);
            writeLogAsyncFlag = false;
            return 0;
        }

    }
}
