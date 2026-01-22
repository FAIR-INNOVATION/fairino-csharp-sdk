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
            try
            {
                // 确保日志目录存在
                if (!Directory.Exists(filePath))
                {
                    // Console.WriteLine($"Log directory does not exist, creating: {filePath}");
                    Directory.CreateDirectory(filePath);

                    // 检查是否创建成功
                    if (Directory.Exists(filePath))
                    {
                        Console.WriteLine($"Log directory created successfully: {filePath}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create log directory: {filePath}");
                    }
                }
                else
                {
                    Console.WriteLine($"Log directory already exists: {filePath}");
                }

                logFilePath = filePath;
                curLogType = logType;
                logSaveDays = saveFileDays;
                writeLogAsyncFlag = true;
                curLogLevel = logLevel;
                logSaveFileNum = saveFileNum;
                logFileName = GetNewLogFileName();
                Thread t = new Thread(WriteLogAsyncThread);
                t.IsBackground = true; // 设置为后台线程
                t.Start();
            }
            catch (Exception ex)
            {
                // 如果初始化失败，回退到当前目录
                Console.WriteLine($"Log initialization failed: {ex.Message}, using current directory.");
                logFilePath = Environment.CurrentDirectory;
                logFileName = Path.Combine(logFilePath, $"FrLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");
            }
        }

        public int SetLogLevel(FrLogLevel level)
        {
            curLogLevel = level;
            return 0;
        }

        public int LogInfo(string logStr, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (curLogLevel > FrLogLevel.INFO)
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
            if (checkLogFileSizeFlag > CHECKFILESIZE) // 1000行日志检测一次大小
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(logFileName);
                    long logFileSize = fileInfo.Length;
                    if (logFileSize > MAXFILESIZE)
                    {
                        logFileName = GetNewLogFileName();
                        checkLogFileSizeFlag = 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Check log file size failed: {ex.Message}");
                }
            }

            bool lockToken = false;
            slockAsyncLog.Enter(ref lockToken);
            try
            {
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
            }
            finally
            {
                if (lockToken)
                {
                    slockAsyncLog.Exit(false);
                }
            }
            return 0;
        }

        private int WriteLogDirect(string logStr)
        {
            try
            {
                // 确保目录存在
                EnsureLogDirectoryExists();

                using (StreamWriter sw = new StreamWriter(logFileName, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine(logStr);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WriteLogDirect failed: {ex.Message}");
                return -1;
            }
        }

        private int WriteLogBuffer(string logStr)
        {
            logBuf.Add(logStr);
            int curLogCount = logBuf.Count;

            // 如果是第一条日志或者缓冲区满了，立即写入
            if (curLogCount == 1 || curLogCount > maxLogBufCount)
            {
                try
                {
                    // 确保目录存在
                    EnsureLogDirectoryExists();

                    using (StreamWriter sw = new StreamWriter(logFileName, true, System.Text.Encoding.Default))
                    {
                        for (int i = 0; i < curLogCount; i++)
                        {
                            sw.WriteLine(logBuf[i]);
                        }
                    }
                    logBuf.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WriteLogBuffer failed: {ex.Message}");
                }
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
            try
            {
                // Console.WriteLine($"GetNewLogFileName: Checking directory {logFilePath}");

                // 确保日志目录存在
                if (!Directory.Exists(logFilePath))
                {
                    Console.WriteLine($"Directory {logFilePath} does not exist, creating...");
                    Directory.CreateDirectory(logFilePath);
                }

                string pattern = "FrLog_*.log";
                string[] allLogFiles = Directory.GetFiles(logFilePath, pattern);
                // Console.WriteLine($"Found {allLogFiles.Length} existing log files");

                // 按创建时间排序(最新的在前面)
                var sortedFiles = allLogFiles
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .ToList();

                // 删除超过天数的旧文件
                DateTime cutoffDate = DateTime.Now.AddDays(-logSaveDays);
                var filesToDeleteByDate = sortedFiles.Where(f => f.CreationTime < cutoffDate).ToList();
                //Console.WriteLine($"Found {filesToDeleteByDate.Count} files older than {logSaveDays} days to delete");

                foreach (var file in filesToDeleteByDate)
                {
                    try
                    {
                      //  Console.WriteLine($"Deleting old log file by date: {file.Name}, created: {file.CreationTime}");
                        file.Delete();
                        sortedFiles.Remove(file);
                      //  Console.WriteLine($"Successfully deleted: {file.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete old log file {file.Name}: {ex.Message}");
                    }
                }

                // 如果文件数量仍然超过限制，删除最旧的文件
                // Console.WriteLine($"Current file count: {sortedFiles.Count}, max allowed: {logSaveFileNum}");
                while (sortedFiles.Count >= logSaveFileNum)
                {
                    var oldestFile = sortedFiles.Last();
                    try
                    {
                        Console.WriteLine($"Deleting excess log file: {oldestFile.Name}, created: {oldestFile.CreationTime}");
                        oldestFile.Delete();
                        sortedFiles.Remove(oldestFile);
                        Console.WriteLine($"Successfully deleted excess file: {oldestFile.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete excess log file {oldestFile.Name}: {ex.Message}");
                    }
                }

                // 使用 Path.Combine 替代字符串拼接
                string newFileName = Path.Combine(logFilePath, "FrLog_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log");
                //Console.WriteLine($"New log file name: {newFileName}");

                // 检查是否要删除当前正在使用的日志文件
                if (!string.IsNullOrEmpty(logFileName) && File.Exists(logFileName))
                {
                    Console.WriteLine($"Current log file exists: {logFileName}");
                    try
                    {
                        var currentFileInfo = new FileInfo(logFileName);
                        if (currentFileInfo.CreationTime < cutoffDate)
                        {
                            Console.WriteLine($"WARNING: Current log file would be deleted due to age: {logFileName}");
                        }
                        if (sortedFiles.Count >= logSaveFileNum)
                        {
                            Console.WriteLine($"WARNING: Current log file might be deleted due to file count limit");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error checking current log file: {ex.Message}");
                    }
                }

                return newFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetNewLogFileName failed: {ex.Message}");
                // 回退方案
                string fallbackFile = Path.Combine(Environment.CurrentDirectory, $"FrLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");
                Console.WriteLine($"Using fallback file: {fallbackFile}");
                return fallbackFile;
            }
        }

        private void WriteLogAsyncThread()
        {
            while (writeLogAsyncFlag)
            {
                try
                {
                    if (curLogType == FrLogType.ASYNC && logBuf.Count > 0)  //只要有就写
                    {
                        bool lockToken = false;
                        slockAsyncLog.Enter(ref lockToken);
                        asyncWriteBuf.AddRange(logBuf);
                        logBuf.Clear();
                        if (lockToken)
                        {
                            slockAsyncLog.Exit(false);
                        }

                        // 确保目录存在
                        EnsureLogDirectoryExists();

                        using (StreamWriter sw = new StreamWriter(logFileName, true, System.Text.Encoding.Default))
                        {
                            for (int i = 0; i < asyncWriteBuf.Count; i++)
                            {
                                sw.WriteLine(asyncWriteBuf[i]);
                            }
                        }
                        asyncWriteBuf.Clear();
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WriteLogAsyncThread error: {ex.Message}");
                    Thread.Sleep(100); // 出错时稍作等待
                }
            }
            Console.WriteLine("log close thread");
        }

        public int LogClose()
        {
            bool lockToken = false;
            slockAsyncLog.Enter(ref lockToken);
            try
            {
                int curLogCount = logBuf.Count;
                if (curLogCount > 0)
                {
                    try
                    {
                        // 确保目录存在
                        EnsureLogDirectoryExists();

                        using (StreamWriter sw = new StreamWriter(logFileName, true, System.Text.Encoding.Default))
                        {
                            for (int i = 0; i < curLogCount; i++)
                            {
                                sw.WriteLine(logBuf[i]);
                            }
                        }
                        logBuf.Clear();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"LogClose write failed: {ex.Message}");
                    }
                }
            }
            finally
            {
                if (lockToken)
                {
                    slockAsyncLog.Exit(false);
                }
            }
            writeLogAsyncFlag = false;
            return 0;
        }

        // 辅助方法：确保日志目录存在
        private void EnsureLogDirectoryExists()
        {
            try
            {
                var directory = Path.GetDirectoryName(logFileName);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EnsureLogDirectoryExists failed: {ex.Message}");
            }
        }
    }
}