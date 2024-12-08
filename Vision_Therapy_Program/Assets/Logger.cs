using System.IO;
using UnityEngine;

public static class Logger
{
    private static string logFilePath = Path.Combine(Application.persistentDataPath, "Visual_Therapy_Log.txt");

    public static void Log(string message)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string logMessage = $"[{timestamp}] {message}";

        // Write to log file
        File.AppendAllText(logFilePath, logMessage + "\n");
    }

    public static void ClearLog()
    {
        File.WriteAllText(logFilePath, string.Empty); // Clear the log file
    }

    public static string GetLogFilePath()
    {
        return logFilePath;
    }
}
