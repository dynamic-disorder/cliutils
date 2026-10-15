using System.Globalization;

namespace CliUtils;

/// <summary>
/// Provides helper methods for colored console output.
/// </summary>
public static class ConsoleColorHelper
{
    /// <summary>
    /// Resets the console foreground color to default (Gray).
    /// </summary>
    public static void ResetColor()
    {
        Console.ResetColor();
    }

    /// <summary>
    /// Writes a line with specified foreground color.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="color">The foreground color to use.</param>
    public static void WriteLine(string message, ConsoleColor color)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    /// <summary>
    /// Writes a line with specified foreground color, using string interpolation.
    /// </summary>
    /// <param name="color">The foreground color to use.</param>
    /// <param name="message">The message to write.</param>
    public static void WriteLine(ConsoleColor color, string message)
    {
        WriteLine(message, color);
    }

    /// <summary>
    /// Writes a formatted line with specified foreground color.
    /// </summary>
    /// <param name="color">The foreground color to use.</param>
    /// <param name="format">The format string.</param>
    /// <param name="args">The arguments to format.</param>
    public static void WriteLine(ConsoleColor color, string format, params object[] args)
    {
        WriteLine(string.Format(CultureInfo.InvariantCulture, format, args), color);
    }

    /// <summary>
    /// Writes a success message in green.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteSuccess(string message)
    {
        WriteLine(message, ConsoleColor.Green);
    }

    /// <summary>
    /// Writes an error message in red.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteError(string message)
    {
        WriteLine(message, ConsoleColor.Red);
    }

    /// <summary>
    /// Writes a warning message in yellow.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteWarning(string message)
    {
        WriteLine(message, ConsoleColor.Yellow);
    }

    /// <summary>
    /// Writes an informational message in cyan.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteInfo(string message)
    {
        WriteLine(message, ConsoleColor.Cyan);
    }

    /// <summary>
    /// Writes a debug message in gray.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteDebug(string message)
    {
        WriteLine(message, ConsoleColor.Gray);
    }

    /// <summary>
    /// Writes a translation pair with source in one color and target in another.
    /// </summary>
    /// <param name="counter">Current translation count.</param>
    /// <param name="total">Total entries.</param>
    /// <param name="sourceText">Source text.</param>
    /// <param name="targetText">Translated text.</param>
    public static void WriteTranslation(int counter, int total, string sourceText, string targetText)
    {
        var originalColor = Console.ForegroundColor;
        Console.Write(DateTime.Now.ToLongTimeString() + $"-Translated {counter}/{total}: ");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(sourceText);
        
        Console.ForegroundColor = originalColor;
        Console.Write(" -> ");
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(targetText);
        
        Console.ForegroundColor = originalColor;
        Console.WriteLine();
    }

    /// <summary>
    /// Writes a file operation result with appropriate coloring based on content.
    /// Success messages (starting with "Done:") are shown in green, errors (starting with "Error:") in red.
    /// </summary>
    /// <param name="result">The result message from file operations.</param>
    public static void WriteFileResult(string result)
    {
        if (result.StartsWith("Done:", StringComparison.OrdinalIgnoreCase))
        {
            WriteSuccess(result);
        }
        else if (result.StartsWith("Error:", StringComparison.OrdinalIgnoreCase))
        {
            WriteError(result);
        }
        else
        {
            Console.WriteLine(result);
        }
    }
}