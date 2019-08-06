using UnityEngine;
using System;
using System.IO;

public class GameLogger
{
    //Variables
    private static StreamWriter gameLogger;
    private static int standardLogLevel;
    private static int errorLogLevel;

    //Strings
    public static readonly string FILE_PATH = "/Path";
    public static readonly string ENTERING_MESSAGE = "Entering : ";
    public static readonly string EXCEPTION_MESSAGE = "The following error has occured : ";
    public static readonly string EXITING_MESSAGE = "Exiting : ";


    /// <summary>
    /// Creates an instance of a logger based on the file path inside the class.
    /// </summary>
    public GameLogger()
    {
        gameLogger = new StreamWriter(FILE_PATH);
    }

    /// <summary>
    /// Functionality used when entering a method. Logs "Entering : METHOD_NAME".
    /// </summary>
    public static void entering(String methodName)
    {
        gameLogger.WriteLine(ENTERING_MESSAGE + methodName);
    }

    /// <summary>
    /// Functionality used when exiting a method. Logs "Exiting : METHOD_NAME".
    /// </summary>
    public static void exiting(String methodName)
    {
        gameLogger.WriteLine(EXITING_MESSAGE + methodName);
    }

    /// <summary>
    /// Functionality used when logging an error. Logs "The following error has occured : EXCEPTION_NAME" followed by exception details.
    /// </summary>
    public static void logException(Exception exception)
    {
        gameLogger.WriteLine(EXITING_MESSAGE + exception.ToString() + "\n" + exception);
    }

    /// <summary>
    /// Functionality used when logging an error and you have a custom message to display. Logs "The following error has occured : EXCEPTION_NAME" followed by exception details, then the
    /// custom message.
    /// </summary>
    public static void logException(Exception exception, String customExceptionMessage)
    {
        gameLogger.WriteLine(EXITING_MESSAGE + exception.ToString() + "\n" + exception + "\n" + customExceptionMessage);
    }
}

