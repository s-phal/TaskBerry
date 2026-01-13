namespace TaskBerry;

using Spectre.Console;

class Program
{

    static void Main(string[] args)
    {
        TaskItem.CreateTableIfNotExists();

        AnsiConsole.Clear();
        AnsiConsole.WriteLine();

        if (args.Length == 0)
        {
            ShowAllTasks();
            return;
        }

        string command = args[0];
        string argument = args.Length > 1 ? args[1] : string.Empty;

        switch (command)
        {
            case "add":
                if (string.IsNullOrWhiteSpace(argument))
                {
                    AnsiConsole.WriteLine("Please enter a task description.");
                    AnsiConsole.WriteLine();
                    return;
                }

                var taskItem = new TaskItem();
                taskItem.Title = argument;
                taskItem.Save();

                AnsiConsole.WriteLine("Task added.");
                AnsiConsole.WriteLine();
                return;

            case "edit":
                if (!IsPositiveInterger(argument))
                {
                    AnsiConsole.WriteLine("Please enter an ID.");
                    break;
                }
                AnsiConsole.WriteLine($"parameters: {argument}");
                break;

            case "done":
                if (!IsPositiveInterger(argument))
                {
                    AnsiConsole.WriteLine("Please enter an ID.");
                    break;
                }
                AnsiConsole.WriteLine($"Task ID: {argument} completed.");
                break;

            case "list":
                ShowAllTasks();
                break;

            case "help":
                ShowHelpMenu();
                break;

            default:
                ShowHelpMenu();
                break;
        }

        AnsiConsole.WriteLine();
    }


    static bool IsPositiveInterger(string input)
    {
        return int.TryParse(input, out var number) && number > 0;
    }

    static void ShowAllTasks()
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine();

        var list = TaskItem.GetAll();

        var grid = new Grid();
        grid.AddColumn(new GridColumn());
        grid.AddColumn(new GridColumn());
        grid.AddColumn(new GridColumn().PadLeft(6).Alignment(Justify.Right));
        grid.AddRow("ID", "Title", "Category");
        grid.AddRow("--", "-----------", "--------");
        foreach (var item in list)
        {
            grid.AddRow(item.Id.ToString(), item.Title, item.Category);
        }
        grid.AddRow();
        grid.AddRow(list.Count.ToString() + " task(s).");
        grid.AddRow();

        AnsiConsole.Write(grid);
    }

    static void ShowHelpMenu()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Usage:");
        AnsiConsole.WriteLine("  task add <title> []");

        var grid = new Grid();
        grid.AddColumn(new GridColumn().NoWrap());
        grid.AddColumn(new GridColumn().PadLeft(2));
        grid.AddRow("Options:");
        grid.AddRow("  [blue]-h[/], [blue]--help[/]", "Show command line help.");
        grid.AddRow("  [blue]-c[/], [blue]--configuration[/] <CONFIGURATION>", "The configuration to run for.");
        grid.AddRow("  [blue]-v[/], [blue]--verbosity[/] <LEVEL>", "Set the [grey]MSBuild[/] verbosity level.");

        AnsiConsole.Write(grid);

    }


}

