using System.Globalization;

using Spectre.Console;

namespace CliUtils;

/// <summary>
/// Provides backward-compatible ConsoleColorHelper methods using Spectre.Console internally
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// Resets the console foreground color to default.
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
        AnsiConsole.Foreground = ConvertToSpectreColor(color);
        AnsiConsole.WriteLine(message);
        AnsiConsole.ResetColors();
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
        AnsiConsole.MarkupLine($"[green]{EscapeMarkup(message)}[/]");
    }

    /// <summary>
    /// Writes an error message in red.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteError(string message)
    {
        AnsiConsole.MarkupLine($"[red]{EscapeMarkup(message)}[/]");
    }

    /// <summary>
    /// Writes a warning message in yellow.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteWarning(string message)
    {
        AnsiConsole.MarkupLine($"[yellow]{EscapeMarkup(message)}[/]");
    }

    /// <summary>
    /// Writes an informational message in cyan.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteInfo(string message)
    {
        AnsiConsole.MarkupLine($"[cyan]{EscapeMarkup(message)}[/]");
    }

    /// <summary>
    /// Writes a debug message in gray.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public static void WriteDebug(string message)
    {
        AnsiConsole.MarkupLine($"[grey]{EscapeMarkup(message)}[/]");
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
        var timestamp = DateTime.Now.ToLongTimeString();
        AnsiConsole.Markup($"{timestamp}-Translated {counter}/{total}: ");
        AnsiConsole.Markup($"[yellow]{EscapeMarkup(sourceText)}[/]");
        AnsiConsole.Markup(" -> ");
        AnsiConsole.MarkupLine($"[green]{EscapeMarkup(targetText)}[/]");
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
            AnsiConsole.WriteLine(result);
        }
    }

    /// <summary>
    /// Converts System.ConsoleColor to Spectre.Console.Color
    /// </summary>
    private static Color ConvertToSpectreColor(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => Color.Black,
            ConsoleColor.DarkBlue => Color.Navy,
            ConsoleColor.DarkGreen => Color.Green,
            ConsoleColor.DarkCyan => Color.Teal,
            ConsoleColor.DarkRed => Color.Maroon,
            ConsoleColor.DarkMagenta => Color.Purple,
            ConsoleColor.DarkYellow => Color.Olive,
            ConsoleColor.Gray => Color.Grey,
            ConsoleColor.DarkGray => Color.Grey,
            ConsoleColor.Blue => Color.Blue,
            ConsoleColor.Green => Color.Lime,
            ConsoleColor.Cyan => Color.Aqua,
            ConsoleColor.Red => Color.Red,
            ConsoleColor.Magenta => Color.Fuchsia,
            ConsoleColor.Yellow => Color.Yellow,
            ConsoleColor.White => Color.White,
            _ => Color.White
        };
    }

    /// <summary>
    /// Escapes Spectre.Console markup characters in text
    /// </summary>
    private static string EscapeMarkup(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return text
            .Replace("[", "[[")
            .Replace("]", "]]");
    }
}