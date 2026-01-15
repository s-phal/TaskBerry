namespace TaskBerry;

class Program
{

    static void Main(string[] args)
    {
        TaskItem.CreateTableIfNotExists();

        Console.Clear();
        Console.WriteLine();

        if (args.Length == 0)
        {
            ShowAllTasks();
            return;
        }

        string command = args[0];
        string argument = args.Length > 1 ? args[1] : string.Empty;
        string flag = args.Length > 2 ? args[2] : string.Empty;
        switch (command)
        {
            case "add":
                if (string.IsNullOrWhiteSpace(argument))
                {
                    Console.WriteLine("Please enter a task description.");
                    Console.WriteLine();
                    return;
                }

                var taskItem = new TaskItem();
                taskItem.Title = argument;
                taskItem.Category = args.Length > 2 ? args[2] : string.Empty;
                taskItem.Save();

                Console.WriteLine("Task added.");
                Console.WriteLine();
                return;

            case "edit":
                if (!IsPositiveInterger(argument))
                {
                    Console.WriteLine("Please enter an ID.");
                    break;
                }
                Console.WriteLine($"parameters: {argument}");
                break;

            case "done":
                if (!IsPositiveInterger(argument))
                {
                    Console.WriteLine("Please enter an ID.");
                    break;
                }
                Console.WriteLine($"Task ID: {argument} completed.");
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

        Console.WriteLine();
    }


    static bool IsPositiveInterger(string input)
    {
        return int.TryParse(input, out var number) && number > 0;
    }

    static void ShowAllTasks()
    {
        Console.Clear();
        Console.WriteLine();

        var list = TaskItem.GetAll();

        if (list.Count == 0)
        {
            Console.WriteLine("0 tasks.");
            return;
        }

        int paddingCount = GetMaxTitleLength(list) + 5;

        Console.WriteLine(
                "ID".PadRight(5) +
                "Title".PadRight(paddingCount) +
                "Category");

        Console.WriteLine(
                "--".PadRight(5) +
                "-----".PadRight(paddingCount) +
                "--------");

        foreach (var item in list)
        {
            Console.WriteLine(
                    item.Id.ToString().PadRight(5) +
                    item.Title.PadRight(paddingCount) +
                    item.Category);
        }
    }

    static int GetMaxTitleLength(List<TaskItem> items)
    {
        int maxLength = 0;
        foreach (var item in items)
        {
            maxLength = item.Title.Length > maxLength ? item.Title.Length : maxLength;
        }
        return maxLength;

    }

    static void ShowHelpMenu()
    {
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  task add <title> []");

        // var grid = new Grid();
        // grid.AddColumn(new GridColumn().NoWrap());
        // grid.AddColumn(new GridColumn().PadLeft(2));
        // grid.AddRow("Options:");
        // grid.AddRow("  [blue]-h[/], [blue]--help[/]", "Show command line help.");
        // grid.AddRow("  [blue]-c[/], [blue]--configuration[/] <CONFIGURATION>", "The configuration to run for.");
        // grid.AddRow("  [blue]-v[/], [blue]--verbosity[/] <LEVEL>", "Set the [grey]MSBuild[/] verbosity level.");
        //
        // Console.Write(grid);

    }


}

