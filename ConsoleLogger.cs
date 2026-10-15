namespace CliUtils;

/// <summary>
/// A simple logger class that captures console log entries in a list. This class provides a static method to write log messages, which are stored as instances of the ConsoleLogEntry class in a static
/// </summary>
public class ConsoleLogger
{
    /// <summary>
    /// A static list that holds all console log entries. Each entry is an instance of the ConsoleLogEntry class, which contains the log text. This list allows for easy retrieval and management of log messages generated during the application's execution, enabling features such as log analysis, filtering, or exporting logs for debugging and monitoring purposes.
    /// </summary>
    public static List<ConsoleLogEntry> Logs { get; } = [];

    /// <summary>
    /// Writes a log message to the console and stores it in the Logs list. This method creates a new ConsoleLogEntry instance with the provided log message and adds it to the Logs list, allowing for both real-time logging to the console and persistent storage of log messages for later reference or analysis. The log message is expected to be a string, and it can be formatted as needed before being passed to this method for logging.
    /// </summary>
    /// <param name="log">The log message to be written and stored.</param>
    public static void WriteLine(string log)
    {
        var logEntry = new ConsoleLogEntry(log);
        Logs.Add(logEntry);
    }
}

/// <summary>
/// Represents a single console log entry, containing the log text. This class serves as a data model for individual log messages captured by the ConsoleLogger, allowing for structured storage and easy access to the log content. Each instance of ConsoleLogEntry holds a single log message, which can be retrieved or processed as needed for debugging, monitoring, or analysis purposes within the application.
/// </summary>
/// <param name="log">The log message to be stored in this entry.</param>
public class ConsoleLogEntry(string log)
{
    /// <summary>
    /// Gets the log text for this console log entry. This property holds the actual log message that was captured, allowing for easy access and retrieval of the log content when needed. The log text is initialized through the constructor and is expected to be a string, which can contain any relevant information about the application's execution or state at the time the log was generated.
    /// </summary>
    public string LogText { get; } = log;
}
