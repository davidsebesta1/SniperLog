using SniperLogNetworkLibrary.CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SniperLogServer.Logging;

/// <summary>
/// Logger for errors and debugging.
/// </summary>
public static class Logger
{
    private static string _filePath;
    private static object _lock = new object();

    private static string _workingDirPath;

    public static string WorkingDirectoryPath
    {
        get
        {
            if (string.IsNullOrEmpty(_workingDirPath))
            {
                string dllLocation = Extensions.GetExecutablePath();
                _workingDirPath = Path.GetDirectoryName(dllLocation);
            }
            return _workingDirPath;
        }
    }

    /// <summary>
    /// Gets the absolute file path to the log file for this session.
    /// </summary>
    public static string FilePath
    {
        get
        {
            if (!string.IsNullOrEmpty(_filePath))
                return _filePath;

            string filename = $"log-{DateTime.Now.ToString("dd-mm-yyyy_HH-mm")}.txt";
            if (!string.IsNullOrEmpty(WorkingDirectoryPath))
            {
                string res = Path.Combine(WorkingDirectoryPath, "logs", filename);
                Directory.CreateDirectory(Path.GetDirectoryName(res));
                return res;
            }

            string dllLocation = Extensions.GetExecutablePath();

            string res2 = Path.Combine(Path.GetDirectoryName(dllLocation), "logs", filename);
            Directory.CreateDirectory(Path.GetDirectoryName(res2));
            return res2;
        }
    }

    /// <summary>
    /// Logs the object string representatiom.
    /// </summary>
    /// <param name="message">Object to be logged.</param>
    public static async Task Log(object message)
    {
        await Log(message.ToString(), ConsoleColor.White);
    }

    /// <summary>
    /// Logs the string into output stream(s)-
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="color">Color of the text.</param>
    /// <param name="consoleWrite">Whether the output should be written to console aswell.</param>
    public static async Task Log(string message, ConsoleColor color = ConsoleColor.White, bool consoleWrite = true)
    {
        if (string.IsNullOrEmpty(message))
            return;

        StringBuilder sb = StringBuilderPool.Get();
        sb.Append("[LOG] [");
        sb.Append(DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
        sb.Append("] ");
        sb.Append(message);


        string messageLog = StringBuilderPool.ReturnToString(sb);

        lock (_lock)
        {
            using (StreamWriter sw = File.AppendText(FilePath))
            {
                Console.ForegroundColor = color;

                if (consoleWrite)
                    Console.WriteLine(message);
                sw.WriteLine(messageLog);
            }
        }
    }

    /// <summary>
    /// Logs the exception and a stack trace.
    /// </summary>
    /// <param name="exception">Exception to be logged.</param>
    public static async Task LogError(Exception exception)
    {
        if (exception == null)
            return;

        await Log(exception.Message, ConsoleColor.Red);
        await Log(exception.StackTrace ?? "No stack trace", ConsoleColor.Red, false);
    }
}
