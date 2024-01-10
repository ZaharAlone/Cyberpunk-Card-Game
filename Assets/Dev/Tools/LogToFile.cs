using System.IO;
using UnityEngine;

public class LogToFile : MonoBehaviour
{
    string logFilePath = "log.txt";

    void Start()
    {
        Application.logMessageReceived += LogToFileHandler;
    }

    void LogToFileHandler(string logString, string stackTrace, LogType type)
    {
        string log = $"[{type}] {logString}\n{stackTrace}\n";
        File.AppendAllText(logFilePath, log);

        // Вывести логи в консоль
        Debug.Log(log);
    }
}