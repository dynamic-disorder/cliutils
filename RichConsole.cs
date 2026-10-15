using Spectre.Console;
using Spectre.Console.Rendering;

namespace CliUtils;

/// <summary>
/// Enhanced console features using Spectre.Console
/// </summary>
public static class RichConsole
{
    /// <summary>
    /// Writes a table with the specified items and columns
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    /// <param name="items">Items to display</param>
    /// <param name="columns">Column definitions</param>
    public static void WriteTable<T>(IEnumerable<T> items, params string[] columns)
    {
        var table = new Table();
        
        foreach (var column in columns)
        {
            table.AddColumn(column);
        }
        
        foreach (var item in items)
        {
            var values = GetPropertyValues(item, columns);
            table.AddRow(values);
        }
        
        AnsiConsole.Write(table);
    }
    
    /// <summary>
    /// Executes an asynchronous operation with a progress bar
    /// </summary>
    /// <param name="title">Progress title</param>
    /// <param name="action">Action to execute with progress context</param>
    public static async Task WithProgressAsync(string title, Func<ProgressContext, Task> action)
    {
        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn()
            )
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask(title);
                await action(ctx);
            });
    }
    
    /// <summary>
    /// Executes an asynchronous operation with a progress display whose rows are added by the
    /// operation itself, for work whose item count is not known up front
    /// </summary>
    /// <param name="action">Action to execute with the progress context</param>
    public static async Task WithLiveProgressAsync(Func<ProgressContext, Task> action)
    {
        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn(),
                new SpinnerColumn()
            )
            .StartAsync(action);
    }

    /// <summary>
    /// Prompts the user for input with a default value
    /// </summary>
    /// <typeparam name="T">Type of input</typeparam>
    /// <param name="prompt">Prompt message</param>
    /// <param name="defaultValue">Default value</param>
    public static T? Ask<T>(string prompt, T? defaultValue = default)
    {
        return AnsiConsole.Prompt(
            new TextPrompt<T?>(prompt)
                .DefaultValue(defaultValue)
                .ShowDefaultValue()
        );
    }
    
    /// <summary>
    /// Prompts the user to select from multiple options
    /// </summary>
    /// <param name="title">Selection title</param>
    /// <param name="options">Available options</param>
    public static string Select(string title, params string[] options)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(options)
        );
    }
    
    /// <summary>
    /// Executes an asynchronous operation with a status spinner
    /// </summary>
    /// <param name="message">Status message</param>
    /// <param name="action">Action to execute with status context</param>
    public static async Task WithStatusAsync(string message, Func<StatusContext, Task> action)
    {
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Default)
            .SpinnerStyle(Style.Parse("green"))
            .StartAsync(message, action);
    }
    
    /// <summary>
    /// Writes content in a panel with optional header
    /// </summary>
    /// <param name="content">Panel content</param>
    /// <param name="header">Panel header (optional)</param>
    public static void WritePanel(string content, string? header = null)
    {
        var panel = new Panel(content)
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1)
        };
        
        if (!string.IsNullOrEmpty(header))
        {
            panel.Header = new PanelHeader(header);
        }
        
        AnsiConsole.Write(panel);
    }
    
    /// <summary>
    /// Creates a live display for dynamic content updates
    /// </summary>
    /// <param name="updateAction">Action to update the display</param>
    public static void CreateLiveDisplay(Action<LiveDisplayContext> updateAction)
    {
        var live = AnsiConsole.Live(new Text(""));
        live.Start(ctx =>
        {
            var context = new LiveDisplayContext(ctx);
            updateAction(context);
        });
    }
    
    /// <summary>
    /// Enhanced translation display with progress integration
    /// </summary>
    /// <param name="sourceText">Source text</param>
    /// <param name="targetText">Translated text</param>
    /// <param name="current">Current progress</param>
    /// <param name="total">Total items</param>
    public static void WriteTranslation(string sourceText, string targetText, int? current = null, int? total = null)
    {
        var message = new List<IRenderable>();
        
        if (current.HasValue && total.HasValue)
        {
            message.Add(new Text($"{DateTime.Now.ToLongTimeString()}-Translated {current}/{total}: "));
        }
        else
        {
            message.Add(new Text($"{DateTime.Now.ToLongTimeString()}: "));
        }
        
        message.Add(new Text($"[yellow]{EscapeMarkup(sourceText)}[/]"));
        message.Add(new Text(" -> "));
        message.Add(new Text($"[green]{EscapeMarkup(targetText)}[/]"));
        
        AnsiConsole.Write(new Rows(message));
    }
    
    /// <summary>
    /// Displays a confirmation prompt
    /// </summary>
    /// <param name="question">Question to ask</param>
    /// <param name="defaultValue">Default answer</param>
    public static bool Confirm(string question, bool defaultValue = true)
    {
        return AnsiConsole.Confirm(question, defaultValue);
    }
    
    /// <summary>
    /// Displays a multi-selection prompt
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    /// <param name="title">Selection title</param>
    /// <param name="choices">Available choices</param>
    public static IEnumerable<T> MultiSelect<T>(string title, params T[] choices) where T : notnull
    {
        return AnsiConsole.Prompt(
            new MultiSelectionPrompt<T>()
                .Title(title)
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to accept)[/]")
                .AddChoices(choices)
        );
    }
    
    /// <summary>
    /// Displays a tree structure
    /// </summary>
    /// <param name="title">Tree title</param>
    /// <param name="rootNode">Root node</param>
    /// <param name="nodeFormatter">Function to format nodes</param>
    public static void WriteTree(string title, SimpleTreeNode rootNode, Func<SimpleTreeNode, string> nodeFormatter)
    {
        var tree = new Tree(title);
        
        foreach (var child in rootNode.Children)
        {
            BuildTree(child, tree.AddNode(nodeFormatter(child)));
        }
        
        AnsiConsole.Write(tree);
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
    
    /// <summary>
    /// Gets property values from an object for table display
    /// </summary>
    private static string[] GetPropertyValues<T>(T item, string[] properties)
    {
        var values = new List<string>();
        
        foreach (var property in properties)
        {
            var propInfo = typeof(T).GetProperty(property);
            if (propInfo != null)
            {
                var value = propInfo.GetValue(item);
                values.Add(value?.ToString() ?? string.Empty);
            }
            else
            {
                values.Add(string.Empty);
            }
        }
        
        return values.ToArray();
    }
    
    /// <summary>
    /// Recursively builds a tree structure
    /// </summary>
    private static void BuildTree(SimpleTreeNode node, Spectre.Console.TreeNode treeNode)
    {
        foreach (var child in node.Children)
        {
            var childNode = treeNode.AddNode(child.Name);
            BuildTree(child, childNode);
        }
    }
}

/// <summary>
/// Context for live display updates
/// </summary>
public class LiveDisplayContext
{
    private readonly Spectre.Console.LiveDisplayContext _context;
    
    public LiveDisplayContext(Spectre.Console.LiveDisplayContext context)
    {
        _context = context;
    }
    
    public void Update(string content)
    {
        _context.UpdateTarget(new Text(content));
    }
}

/// <summary>
/// Simple tree node for tree displays
/// </summary>
public class SimpleTreeNode
{
    public string Name { get; set; }
    public List<SimpleTreeNode> Children { get; } = new();
    
    public SimpleTreeNode(string name)
    {
        Name = name;
    }
    
    public override string ToString() => Name;
}